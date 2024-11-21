using System.Collections.Generic;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using ContentGeneration.Models.Meshy;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Meshy
{
    public class TextToMesh : VisualElementComponent, IGeneratorVisualElement
    {
        public new class UxmlFactory : UxmlFactory<TextToMesh, UxmlTraits>
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
        PromptInput negativePrompt => this.Q<PromptInput>("negativePrompt");
        EnumField artStyle => this.Q<EnumField>("artStyle");

        Button improvePrompt => this.Q<Button>("improvePromptButton");
        
        public TextToMesh()
        {
            generationOptionsElement.OnCodeHasChanged = RefreshCode;
            prompt.OnChanged += _ => RefreshCode();
            negativePrompt.OnChanged += _ => RefreshCode();
            artStyle.RegisterValueChangedCallback(_ => RefreshCode());

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

                var parameters = new MeshyTextToMeshParameters
                {
                    Prompt = prompt.value,
                    NegativePrompt = string.IsNullOrEmpty(negativePrompt.value) ? null : negativePrompt.value,
                    ArtStyle =(TextToMeshArtStyle)artStyle.value
                };
                ContentGenerationApi.Instance.RequestMeshyTextToMeshGeneration(
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
                "var requestId = await ContentGenerationApi.Instance.RequestMeshyTextToMeshGeneration\n" +
                "\t(new MeshyTextToMeshParameters\n" +
                "\t{\n" +
                $"\t\tPrompt = \"{prompt.value}\",\n" +
                (string.IsNullOrEmpty(negativePrompt.value) ? "" : $"\t\tNegativePrompt = \"{negativePrompt.value}\",\n") +
                $"\t\tArtStyle = ArtStyle.{artStyle.value}\n" +
                "\t},\n" +
                $"{generationOptionsElement?.GetCode()}" +
                ")";
        }

        public Generator generator => Generator.MeshyTextToMesh;
        public void Show(Favorite favorite)
        {
            var parameters = favorite.GeneratorParameters.ToObject<MeshyTextToMeshParameters>();
            generationOptionsElement.Show(favorite.GenerationOptions);

            prompt.value = parameters.Prompt;
            negativePrompt.value = parameters.NegativePrompt;
            artStyle.value = parameters.ArtStyle;
            
            RefreshCode();
        }
    }
}