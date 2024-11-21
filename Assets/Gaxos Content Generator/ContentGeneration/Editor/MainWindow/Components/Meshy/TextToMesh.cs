using System;
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
        DropdownField artStyle => this.Q<DropdownField>("artStyle");
        Toggle sendSeed => this.Q<Toggle>("sendSeed");
        SliderInt seed => this.Q<SliderInt>("seed");
        EnumField aiModel => this.Q<EnumField>("aiModel");
        EnumField topology => this.Q<EnumField>("topology");
        SliderInt targetPolyCount => this.Q<SliderInt>("targetPolyCount");

        Button improvePrompt => this.Q<Button>("improvePromptButton");
        
        public TextToMesh()
        {
            generationOptionsElement.OnCodeHasChanged = RefreshCode;
            prompt.OnChanged += _ => RefreshCode();
            negativePrompt.OnChanged += _ => RefreshCode();
            artStyle.RegisterValueChangedCallback(_ => RefreshCode());

            sendSeed.RegisterValueChangedCallback(v => SendSeedChanged(v.newValue));
            seed.RegisterValueChangedCallback(_ => RefreshCode());
            aiModel.RegisterValueChangedCallback(v => AiModelChanged(v.newValue));
            topology.RegisterValueChangedCallback(_ => RefreshCode());
            targetPolyCount.RegisterValueChangedCallback(_ => RefreshCode());

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
                    ArtStyle =Enum.Parse<TextToMeshArtStyle>(artStyle.value, true),
                    Seed = sendSeed.value ? seed.value : null,
                    AIModel = (AiModel)aiModel.value,
                    Topology = (Topology)topology.value,
                    TargetPolyCount = targetPolyCount.value,
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

            SendSeedChanged(sendSeed.value);
            AiModelChanged(aiModel.value);
        }

        void AiModelChanged(Enum value)
        {
            var aiModelValue = (AiModel)value;
            var selectedArtStyle = Enum.Parse<TextToMeshArtStyle>(
                artStyle.value ?? TextToMeshArtStyle.Realistic.ToString(), 
                true);
            artStyle.choices.Clear();
            if (aiModelValue == AiModel.Meshy4)
            {
                artStyle.choices.Add(TextToMeshArtStyle.Realistic.ToString());
                artStyle.choices.Add(TextToMeshArtStyle.Sculpture.ToString());
                artStyle.choices.Add(TextToMeshArtStyle.Pbr.ToString());
                if (selectedArtStyle is TextToMeshArtStyle.Cartoon or TextToMeshArtStyle.LowPoly)
                {
                    selectedArtStyle = TextToMeshArtStyle.Realistic;
                }
            }
            else
            {
                artStyle.choices.Add(TextToMeshArtStyle.Realistic.ToString());
                artStyle.choices.Add(TextToMeshArtStyle.Cartoon.ToString());
                artStyle.choices.Add(TextToMeshArtStyle.LowPoly.ToString());
                artStyle.choices.Add(TextToMeshArtStyle.Sculpture.ToString());
                artStyle.choices.Add(TextToMeshArtStyle.Pbr.ToString());
            }

            artStyle.value = selectedArtStyle.ToString();
            RefreshCode();
        }

        void SendSeedChanged(bool sendSeed)
        {
            seed.style.display = sendSeed ? DisplayStyle.Flex : DisplayStyle.None;
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
                $"\t\tArtStyle = ArtStyle.{artStyle.value},\n" +
                (sendSeed.value ? $"\t\tSeed = {seed.value},\n": "") +
                $"\t\tAIModel = AiModel.{aiModel.value},\n" +
                $"\t\tTopology = Topology.{topology.value},\n" +
                $"\t\tTargetPolyCount = {targetPolyCount.value},\n" +
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
            artStyle.value = parameters.ArtStyle.ToString();
            sendSeed.value = parameters.Seed != null;
            if (parameters.Seed.HasValue)
            {
                seed.value = parameters.Seed.Value;
            }
            aiModel.value = parameters.AIModel;
            topology.value = parameters.Topology;
            targetPolyCount.value = parameters.TargetPolyCount;
            
            SendSeedChanged(sendSeed.value);
            AiModelChanged(aiModel.value);
        }
    }
}