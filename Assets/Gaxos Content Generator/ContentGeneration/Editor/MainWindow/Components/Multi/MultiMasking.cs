using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentGeneration.Helpers;
using ContentGeneration.Models.Gaxos;
using ContentGeneration.Models.Stability;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Multi
{
    public class MultiMasking : VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<MultiMasking, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        ImageSelection mask => this.Q<ImageSelection>("mask");
        Label maskRequired => this.Q<Label>("maskRequiredLabel");
        PromptInput prompt => this.Q<PromptInput>("prompt");
        VisualElement promptRequired => this.Q<VisualElement>("promptRequired");
        VisualElement requestSent => this.Q<VisualElement>("requestSent");
        VisualElement requestFailed => this.Q<VisualElement>("requestFailed");
        VisualElement sendingRequest => this.Q<VisualElement>("sendingRequest");
        Button generateButton => this.Q<Button>("generateButton");

        public MultiMasking()
        {
            maskRequired.style.visibility = Visibility.Hidden;

            requestSent.style.display = DisplayStyle.None;
            requestFailed.style.display = DisplayStyle.None;
            sendingRequest.style.display = DisplayStyle.None;
            
            promptRequired.style.visibility = Visibility.Hidden;
            prompt.OnChanged += _ => promptRequired.style.visibility = Visibility.Hidden;

            var improvePromptButton = this.Q<Button>("improvePromptButton");
            improvePromptButton.clicked += () =>
            {
                if (!improvePromptButton.enabledSelf)
                    return;

                if (string.IsNullOrEmpty(prompt.value))
                {
                    promptRequired.style.visibility = Visibility.Visible;
                    return;
                }

                improvePromptButton.SetEnabled(false);
                prompt.SetEnabled(false);
                ContentGenerationApi.Instance.ImprovePrompt(prompt.value, "midjourney").ContinueInMainThreadWith(
                    t =>
                    {
                        improvePromptButton.SetEnabled(true);
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

                if (mask.image == null)
                {
                    maskRequired.style.visibility = Visibility.Visible;
                    return;
                }
                maskRequired.style.visibility = Visibility.Hidden;

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

                SendRequests().ContinueInMainThreadWith(
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
        }

        Gaxos.MaskingParameters gaxosParameters => this.Q<Gaxos.MaskingParameters>("gaxosParameters");
        StabilityAI.MaskingParameters stabilityParameters =>
            this.Q<StabilityAI.MaskingParameters>("stabilityAiParameters");
        
        async Task SendRequests()
        {
            var gaxosApiParameters = new GaxosMaskingParameters();
            gaxosParameters.ApplyParameters(gaxosApiParameters);
            gaxosApiParameters.Prompt = prompt.value;
            gaxosApiParameters.Mask = (Texture2D)mask.image;

            var stabilityApiParameters = new StabilityMaskedImageParameters();
            stabilityParameters.ApplyParameters(stabilityApiParameters);
            stabilityApiParameters.TextPrompts =
                stabilityApiParameters.TextPrompts.Append(
                    new Prompt
                    {
                        Text = prompt.value,
                        Weight = 1
                    }).ToArray();
            stabilityApiParameters.InitImage = (Texture2D)mask.image;
            stabilityApiParameters.MaskSource = MaskSource.InitImageAlpha;
            
            await Task.WhenAll(
                ContentGenerationApi.Instance.RequestGaxosMaskingGeneration(
                    gaxosApiParameters,
                    data: new
                    {
                        player_id = ContentGenerationStore.editorPlayerId
                    }),
                ContentGenerationApi.Instance.RequestStabilityMaskedImageGeneration(
                    stabilityApiParameters,
                    data: new
                    {
                        player_id = ContentGenerationStore.editorPlayerId
                    })
            );
        }
    }
}

