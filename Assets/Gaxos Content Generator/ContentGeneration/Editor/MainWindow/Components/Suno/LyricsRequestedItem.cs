using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContentGeneration.Editor.MainWindow.Components.RequestsList;
using ContentGeneration.Models;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Suno
{
    public class LyricsRequestedItem : VisualElementComponent, IRequestedItem
    {
        public new class UxmlFactory : UxmlFactory<LyricsRequestedItem, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        Button saveButton => this.Q<Button>("saveButton");

        RequestedItemCommon requestedItemCommon => this.Q<RequestedItemCommon>();

        public LyricsRequestedItem()
        {
            requestedItemCommon.OnDeleted += () =>
            {
                OnDeleted?.Invoke();
            };
            requestedItemCommon.OnRefreshed += v => value = v;

            saveButton.SetEnabled(false);
            saveButton.clicked += () =>
            {
                if (!saveButton.enabledSelf)
                    return;

                saveButton.SetEnabled(false);
                // MeshyModelHelper.Save(value.GeneratorResult).ContinueInMainThreadWith(t =>
                // {
                //     if (t.IsFaulted)
                //     {
                //         Debug.LogException(t.Exception!.InnerException);
                //     }
                //
                //     saveButton.SetEnabled(true);
                // });
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

                saveButton.SetEnabled(value.Status == RequestStatus.Generated);
                _cancellationTokenSource = new CancellationTokenSource();
            }
        }
    
        public Task Save(Request request)
        {
            throw new NotImplementedException();
        }
    }
}