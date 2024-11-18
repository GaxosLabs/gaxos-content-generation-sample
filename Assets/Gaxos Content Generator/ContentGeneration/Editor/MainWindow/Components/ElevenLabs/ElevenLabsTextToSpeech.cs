using System.Collections.Generic;
using ContentGeneration.Models;
using ContentGeneration.Models.ElevenLabs;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.ElevenLabs
{
    // https://elevenlabs.io/docs/api-reference/text-to-speech
    public class ElevenLabsTextToSpeech : VisualElementComponent, IGeneratorVisualElement
    {
        public new class UxmlFactory : UxmlFactory<ElevenLabsTextToSpeech, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        VisualElement requestSent => this.Q<VisualElement>("requestSent");
        VisualElement requestFailed => this.Q<VisualElement>("requestFailed");
        VisualElement sendingRequest => this.Q<VisualElement>("sendingRequest");
        Button generateButton => this.Q<Button>("generateButton");
        TextField code => this.Q<TextField>("code");

        public ElevenLabsTextToSpeech()
        {
            requestSent.style.display = DisplayStyle.None;
            requestFailed.style.display = DisplayStyle.None;
            sendingRequest.style.display = DisplayStyle.None;

            generateButton.RegisterCallback<ClickEvent>(_ =>
            {
                if (!generateButton.enabledSelf) return;

                requestSent.style.display = DisplayStyle.None;
                requestFailed.style.display = DisplayStyle.None;
                sendingRequest.style.display = DisplayStyle.None;

                // if (string.IsNullOrEmpty(text.value))
                // {
                //     promptRequired.style.visibility = Visibility.Visible;
                //     return;
                // }
                // promptRequired.style.visibility = Visibility.Hidden;

                generateButton.SetEnabled(false);
                sendingRequest.style.display = DisplayStyle.Flex;

                // var parameters = new ElevenLabsTextToSpeechParameters()
                // {
                // };
                // ContentGenerationApi.Instance.RequestElevenLabsTextToSpeechGeneration(
                //     parameters,
                //     generationOptionsElement.GetGenerationOptions(), data: new
                //     {
                //         player_id = ContentGenerationStore.editorPlayerId
                //     }).ContinueInMainThreadWith(
                //     t =>
                //     {
                //         generateButton.SetEnabled(true);
                //         sendingRequest.style.display = DisplayStyle.None;
                //         if (t.IsFaulted)
                //         {
                //             requestFailed.style.display = DisplayStyle.Flex;
                //             Debug.LogException(t.Exception);
                //         }
                //         else
                //         {
                //             requestSent.style.display = DisplayStyle.Flex;
                //         }
                //         ContentGenerationStore.Instance.RefreshRequestsAsync().Finally(() => ContentGenerationStore.Instance.RefreshStatsAsync().CatchAndLog());
                //     });
            });

            RefreshCode();
        }

        void RefreshCode()
        {
            code.value =
                "var requestId = await ContentGenerationApi.Instance.RequestElevenLabsTextToSpeechGeneration\n" +
                "\t(new ElevenLabsTextToSpeechParameters\n" +
                "\t{\n" +
                "\t},\n" +
                // $"{generationOptionsElement?.GetCode()}" +
                ")";
        }

        public Generator generator => Generator.MeshyTextToTexture;
        public void Show(Favorite favorite)
        {
            var parameters = favorite.GeneratorParameters.ToObject<ElevenLabsTextToSpeechParameters>();
            // generationOptionsElement.Show(favorite.GenerationOptions);

            RefreshCode();
        }
    }
}