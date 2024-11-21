using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContentGeneration.Editor.MainWindow.Components.RequestsList;
using ContentGeneration.Models;
using UnityEditor;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Suno
{
    public class SunoLyricsRequestedItem : VisualElementComponent, IRequestedItem
    {
        public new class UxmlFactory : UxmlFactory<SunoLyricsRequestedItem, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        RequestedItemCommon requestedItemCommon => this.Q<RequestedItemCommon>();
        TextField title => this.Q<TextField>("title");
        TextField lyrics => this.Q<TextField>("lyrics");
        Button copyToClipboard => this.Q<Button>("copyToClipboard");

        public SunoLyricsRequestedItem()
        {
            requestedItemCommon.OnDeleted += () =>
            {
                OnDeleted?.Invoke();
            };
            requestedItemCommon.OnRefreshed += v => value = v;
            copyToClipboard.clicked += () =>
            {
                EditorGUIUtility.systemCopyBuffer = lyrics.text;
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

                title.value = value?.GeneratorResult?["title"]?.ToObject<string>();
                lyrics.value = value?.GeneratorResult?["text"]?.ToObject<string>();
                
                if (value == null)
                    return;

                _cancellationTokenSource = new CancellationTokenSource();
            }
        }
    
        public Task Save(Request request)
        {
            throw new NotImplementedException();
        }
    }
}