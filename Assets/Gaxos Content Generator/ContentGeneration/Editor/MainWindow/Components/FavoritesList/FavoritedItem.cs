using System;
using System.Collections.Generic;
using ContentGeneration.Models;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.FavoritesList
{
    public class FavoritedItem : VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<FavoritedItem, UxmlTraits>
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

        public Favorite value
        {
            get => favoritedItemCommon.value;
            set => favoritedItemCommon.value = value;
        }

        FavoritedItemCommon favoritedItemCommon => this.Q<FavoritedItemCommon>();

        public FavoritedItem()
        {
            favoritedItemCommon.OnDeleted += () =>
            {
                OnDeleted?.Invoke();
            };
        }
    }
}