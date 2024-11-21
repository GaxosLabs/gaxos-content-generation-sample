using System.Collections.Generic;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using ContentGeneration.Models.Suno;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Suno
{
    public class SunoClipWithLyrics : VisualElementComponent, IGeneratorVisualElement
    {
        public new class UxmlFactory : UxmlFactory<SunoClipWithLyrics, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        TextField code => this.Q<TextField>("code");
        VisualElement requestSent => this.Q<VisualElement>("requestSent");
        VisualElement requestFailed => this.Q<VisualElement>("requestFailed");
        VisualElement sendingRequest => this.Q<VisualElement>("sendingRequest");
        Button generateButton => this.Q<Button>("generateButton");

        TextField lyrics => this.Q<TextField>("lyrics");
        VisualElement promptRequired => this.Q<VisualElement>("promptRequired");
        TextField tags => this.Q<TextField>("tags");
        TextField title => this.Q<TextField>("title");
        
        public SunoClipWithLyrics()
        {
            lyrics.RegisterValueChangedCallback(_ => RefreshCode());
            tags.RegisterValueChangedCallback(_ => RefreshCode());
            title.RegisterValueChangedCallback(_ => RefreshCode());

            requestSent.style.display = DisplayStyle.None;
            requestFailed.style.display = DisplayStyle.None;
            sendingRequest.style.display = DisplayStyle.None;
            
            generateButton.RegisterCallback<ClickEvent>(_ =>
            {
                if (!generateButton.enabledSelf) return;

                if (string.IsNullOrEmpty(lyrics.value))
                {
                    promptRequired.style.visibility = Visibility.Visible;
                    return;
                }

                promptRequired.style.visibility = Visibility.Hidden;

                requestSent.style.display = DisplayStyle.None;
                requestFailed.style.display = DisplayStyle.None;

                generateButton.SetEnabled(false);
                sendingRequest.style.display = DisplayStyle.Flex;

                var parameters = new SunoClipWithLyricsParameters
                {
                    Prompt = lyrics.value,
                    Tags = tags.value,
                    Title = title.value,
                };
                ContentGenerationApi.Instance.RequestSunoClipWithLyricsGeneration(
                    parameters,
                    data: new
                    {
                        player_id = ContentGenerationStore.editorPlayerId
                    }).ContinueInMainThreadWith(
                    t =>
                    {
                        generateButton.SetEnabled(true);
                        sendingRequest.style.display = DisplayStyle.None;
                        if (t.IsFaulted)
                        {
                            requestFailed.style.display = DisplayStyle.Flex;
                            Debug.LogException(t.Exception);
                        }
                        else
                        {
                            requestSent.style.display = DisplayStyle.Flex;
                        }
                        ContentGenerationStore.Instance.RefreshRequestsAsync().Finally(() => ContentGenerationStore.Instance.RefreshStatsAsync().CatchAndLog());
                    });
            });

            RefreshCode();
        }

        void RefreshCode()
        {
            code.value =
                "var requestId = await ContentGenerationApi.Instance.RequestSunoClipWithLyricsGeneration\n" +
                "\t(new SunoClipWithLyricsParameters\n" +
                "\t{\n" +
                $"\t\tPrompt = \"{lyrics.value}\",\n" +
                $"\t\tTags = \"{tags.value}\",\n" +
                $"\t\tTitle = \"{title.value}\",\n" +
                "\t}\n" +
                ")";
        }

        public Generator generator => Generator.MeshyTextToMesh;
        public void Show(Favorite favorite)
        {
            var parameters = favorite.GeneratorParameters.ToObject<SunoClipWithLyricsParameters>();

            lyrics.value = parameters.Prompt;
            tags.value = parameters.Tags;
            title.value = parameters.Title;
            
            RefreshCode();
        }
    }
}