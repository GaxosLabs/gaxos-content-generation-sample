using System.Collections.Generic;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using ContentGeneration.Models.Suno;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Suno
{
    public class SunoClip : VisualElementComponent, IGeneratorVisualElement
    {
        public new class UxmlFactory : UxmlFactory<SunoClip, UxmlTraits>
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
        GenerationOptionsElement generationOptionsElement => this.Q<GenerationOptionsElement>("generationOptions");
        VisualElement requestSent => this.Q<VisualElement>("requestSent");
        VisualElement requestFailed => this.Q<VisualElement>("requestFailed");
        VisualElement sendingRequest => this.Q<VisualElement>("sendingRequest");
        Button generateButton => this.Q<Button>("generateButton");

        PromptInput prompt => this.Q<PromptInput>("prompt");
        VisualElement promptRequired => this.Q<VisualElement>("promptRequired");
        EnumField model => this.Q<EnumField>("model");
        Toggle makeInstrumental => this.Q<Toggle>("makeInstrumental");

        Button improvePrompt => this.Q<Button>("improvePromptButton");
        
        public SunoClip()
        {
            generationOptionsElement.OnCodeHasChanged = RefreshCode;
            prompt.OnChanged += _ => RefreshCode();
            model.RegisterValueChangedCallback(_ => RefreshCode());
            makeInstrumental.RegisterValueChangedCallback(_ => RefreshCode());

            requestSent.style.display = DisplayStyle.None;
            requestFailed.style.display = DisplayStyle.None;
            sendingRequest.style.display = DisplayStyle.None;
            
            improvePrompt.clicked += () =>
            {
                if (string.IsNullOrEmpty(prompt.value))
                    return;
                
                if(!improvePrompt.enabledSelf)
                    return;

                improvePrompt.SetEnabled(false);
                prompt.SetEnabled(false);
                ContentGenerationApi.Instance.ImprovePrompt(prompt.value, "dalle-3").ContinueInMainThreadWith(
                    t =>
                    {
                        improvePrompt.SetEnabled(true);
                        prompt.SetEnabled(true);
                        if (t.IsFaulted)
                        {
                            Debug.LogException(t.Exception!.InnerException!);
                            return;
                        }

                        prompt.value = t.Result;
                    });
            };

            generateButton.RegisterCallback<ClickEvent>(_ =>
            {
                if (!generateButton.enabledSelf) return;

                if (string.IsNullOrEmpty(prompt.value))
                {
                    promptRequired.style.visibility = Visibility.Visible;
                    return;
                }

                promptRequired.style.visibility = Visibility.Hidden;

                requestSent.style.display = DisplayStyle.None;
                requestFailed.style.display = DisplayStyle.None;

                generateButton.SetEnabled(false);
                sendingRequest.style.display = DisplayStyle.Flex;

                var parameters = new SunoClipParameters
                {
                    GptDescriptionPrompt = prompt.value,
                    Mv = (Model)model.value,
                    MakeInstrumental = makeInstrumental.value,
                };
                ContentGenerationApi.Instance.RequestSunoClipWithPromptGeneration(
                    parameters,
                    generationOptionsElement.GetGenerationOptions(), data: new
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
                "var requestId = await ContentGenerationApi.Instance.RequestSunoClipGeneration\n" +
                "\t(new SunoClipParameters\n" +
                "\t{\n" +
                $"\t\tGptDescriptionPrompt = \"{prompt.value}\",\n" +
                $"\t\tMv = Model.{model.value},\n" +
                $"\t\tMakeInstrumental = {makeInstrumental.value},\n" +
                "\t},\n" +
                $"{generationOptionsElement?.GetCode()}" +
                ")";
        }

        public Generator generator => Generator.MeshyTextToMesh;
        public void Show(Favorite favorite)
        {
            var parameters = favorite.GeneratorParameters.ToObject<SunoClipParameters>();
            generationOptionsElement.Show(favorite.GenerationOptions);

            prompt.value = parameters.GptDescriptionPrompt;
            model.value = parameters.Mv;
            makeInstrumental.value = parameters.MakeInstrumental;
            
            RefreshCode();
        }
    }
}