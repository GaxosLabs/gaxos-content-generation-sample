using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContentGeneration.Editor.MainWindow.Components.RequestsList;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Meshy
{
    public class MeshyTextToMeshRequestedItem : VisualElementComponent, IRequestedItem
    {
        public new class UxmlFactory : UxmlFactory<MeshyTextToMeshRequestedItem, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        Label refineStatus => this.Q<Label>("refineStatus");
        VisualElement refineErrorDetails => this.Q<VisualElement>("refineErrorDetails");
        Label refineError => this.Q<Label>("refineError");
        Button videoButton => this.Q<Button>("videoButton");
        Button refineButton => this.Q<Button>("refineButton");
        Button saveButton => this.Q<Button>("saveButton");

        RequestedItemCommon requestedItemCommon => this.Q<RequestedItemCommon>();

        public MeshyTextToMeshRequestedItem()
        {
            requestedItemCommon.OnDeleted += () =>
            {
                OnDeleted?.Invoke();
            };
            requestedItemCommon.OnRefreshed += v => value = v;

            refineButton.SetEnabled(false);
            refineButton.clicked += () =>
            {
                if (!refineButton.enabledSelf)
                    return;

                refineButton.SetEnabled(false);
                ContentGenerationApi.Instance.RefineMeshyTextToMesh(value.ID).ContinueInMainThreadWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        Debug.LogException(t.Exception!.InnerException);
                    }
                    else
                    {
                        requestedItemCommon.Refresh();
                    }

                    refineButton.SetEnabled(true);
                });
            };
            videoButton.SetEnabled(false);
            videoButton.clicked += () =>
            {
                Application.OpenURL(
                    value.GeneratorResult["refine_result"]?["video_url"]!.ToObject<string>() ??
                    value.GeneratorResult["video_url"]!.ToObject<string>());
            };
            saveButton.SetEnabled(false);
            saveButton.clicked += () =>
            {
                if (!saveButton.enabledSelf)
                    return;

                saveButton.SetEnabled(false);
                Save(value).ContinueInMainThreadWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        Debug.LogException(t.Exception!.InnerException);
                    }

                    saveButton.SetEnabled(true);
                });
            };
        }

        CancellationTokenSource _cancellationTokenSource;
        public event Action OnDeleted;

        public Request value
        {
            get => requestedItemCommon.value;
            set
            {
                requestedItemCommon.value = value;

                _cancellationTokenSource?.Cancel();

                if (value == null)
                    return;

                videoButton.SetEnabled(value.GeneratorResult != null);
                refineButton.SetEnabled(
                    value.GeneratorResult != null && !value.GeneratorResult.ContainsKey("refine_status"));
                saveButton.SetEnabled(value.GeneratorResult != null);

                refineStatus.text = "Not requested (In order to get textures you need to Refine your model)";
                refineErrorDetails.style.display = DisplayStyle.None;

                if (value.GeneratorResult != null && 
                    value.GeneratorResult.ContainsKey("refine_status"))
                {
                    var refineStatusText = value.GeneratorResult["refine_status"]!.ToObject<string>();
                    if(!string.IsNullOrEmpty(refineStatusText))
                    {
                        var refineStatusValue = Enum.Parse<RequestStatus>(refineStatusText, true);

                        refineStatus.text = refineStatusValue.ToString();
                        refineStatus.ClearClassList();
                        refineStatus.AddToClassList(refineStatus.text.ToLower());

                        refineErrorDetails.style.display =
                            refineStatusValue == RequestStatus.Failed ? DisplayStyle.Flex : DisplayStyle.None;
                        refineError.text = value.GeneratorError?.Message +
                                           (value.GeneratorError?.Error == null
                                               ? ""
                                               : $" [{value.GeneratorError?.Error}]");
                    }
                }

                _cancellationTokenSource = new CancellationTokenSource();
            }
        }

        public Task Save(Request request)
        {
            return MeshyModelHelper.Save(
                request.GeneratorResult["refine_result"] ?? request.GeneratorResult);
        }
    }
}