using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContentGeneration.Models;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.RequestsList
{
    public class RequestedItem : VisualElementComponent, IRequestedItem
    {
        public new class UxmlFactory : UxmlFactory<RequestedItem, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        public event Action OnDeleted;

        public Request value
        {
            get => requestedItemCommon.value;
            set => requestedItemCommon.value = value;
        }

        RequestedItemCommon requestedItemCommon => this.Q<RequestedItemCommon>();

        public RequestedItem()
        {
            requestedItemCommon.OnDeleted += () =>
            {
                OnDeleted?.Invoke();
            };
        }
    
        public Task Save(Request request)
        {
            throw new NotImplementedException();
        }
    }
}