using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentGeneration.Helpers;
using ContentGeneration.Models.DallE;
using ContentGeneration.Models.Gaxos;
using ContentGeneration.Models.Stability;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Multi
{
    public class MultiTextToImage : VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<MultiTextToImage, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        PromptInput prompt => this.Q<PromptInput>("prompt");
        VisualElement promptRequired => this.Q<VisualElement>("promptRequired");
        VisualElement requestSent => this.Q<VisualElement>("requestSent");
        VisualElement requestFailed => this.Q<VisualElement>("requestFailed");
        VisualElement sendingRequest => this.Q<VisualElement>("sendingRequest");
        Button generateButton => this.Q<Button>("generateButton");

        public MultiTextToImage()
        {
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

                        ContentGenerationStore.Instance.RefreshRequestsAsync().Finally(() =>
                            ContentGenerationStore.Instance.RefreshStatsAsync().CatchAndLog());
                    });
            });
        }

        Gaxos.TextToImageParameters gaxosParameters => this.Q<Gaxos.TextToImageParameters>("gaxosParameters");
        DallE.TextToImageParameters dallEParameters => this.Q<DallE.TextToImageParameters>("dallEParameters");
        StabilityAI.TextToImageParameters stabilityParameters =>
            this.Q<StabilityAI.TextToImageParameters>("stabilityAiParameters");

        async Task SendRequests()
        {
            var gaxosApiParameters = new GaxosTextToImageParameters();
            gaxosParameters.ApplyParameters(gaxosApiParameters);
            gaxosApiParameters.Prompt = prompt.value;

            var dallEApiParameters = new DallETextToImageParameters();
            dallEParameters.ApplyParameters(dallEApiParameters);
            dallEApiParameters.Prompt = prompt.value;

            var stabilityApiParameters = new StabilityTextToImageParameters();
            stabilityParameters.ApplyParameters(stabilityApiParameters);
            stabilityApiParameters.TextPrompts =
                stabilityApiParameters.TextPrompts.Append(
                    new Prompt
                    {
                        Text = prompt.value,
                        Weight = 1
                    }).ToArray();

            await Task.WhenAll(
                ContentGenerationApi.Instance.RequestGaxosTextToImageGeneration(
                    gaxosApiParameters,
                    data: new
                    {
                        player_id = ContentGenerationStore.editorPlayerId
                    }),
                ContentGenerationApi.Instance.RequestDallETextToImageGeneration(
                    dallEApiParameters,
                    data: new
                    {
                        player_id = ContentGenerationStore.editorPlayerId
                    }),
                ContentGenerationApi.Instance.RequestStabilityTextToImageGeneration(
                    stabilityApiParameters,
                    data: new
                    {
                        player_id = ContentGenerationStore.editorPlayerId
                    })
            );
        }
    }
}