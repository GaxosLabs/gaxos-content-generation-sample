using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.FavoritesList
{
    public class FavoritesListTab : VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<FavoritesListTab, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        Button refreshButton => this.Q<Button>("refreshButton");
        MultiColumnListView listView => this.Q<MultiColumnListView>();
        FavoritedItem favoritedItem => this.Q<FavoritedItem>("defaultFavoritedItem");

        string _selectedId;

        public FavoritesListTab()
        {
            refreshButton.RegisterCallback<ClickEvent>(_ => { Refresh(); });

            ContentGenerationStore.Instance.OnFavoritesChanged += _ =>
            {
                var previousSelectId = _selectedId;
                listView.RefreshItems();
                listView.selectedIndex = -1;
                if (previousSelectId != null)
                {
                    for (var i = 0; i < ContentGenerationStore.Instance.Favorites.Count; i++)
                    {
                        if (ContentGenerationStore.Instance.Favorites[i].ID == previousSelectId)
                        {
                            listView.selectedIndex = i;
                            break;
                        }
                    }
                }
            };
            listView.itemsSource = ContentGenerationStore.Instance.Favorites;
            if (!string.IsNullOrEmpty(Settings.instance.apiKey))
            {
                Refresh();
            }

            Func<VisualElement> CreateCell(int i1)
            {
                return () =>
                {
                    var ret = new Label
                    {
                        name = $"p{i1}"
                    };
                    ret.AddToClassList(ret.name);

                    return ret;
                };
            }

            for (var i = 0; i < listView.columns.Count; i++)
            {
                var listViewColumn = listView.columns[i];
                listViewColumn.makeCell = CreateCell(i);
            }

            listView.columnSortingChanged += Refresh;
            listView.columns["id"].bindCell = (element, index) =>
                (element as Label)!.text = ContentGenerationStore.Instance.Favorites[index].ID.ToString();
            listView.columns["generator"].bindCell = (element, index) =>
                (element as Label)!.text = ContentGenerationStore.Instance.Favorites[index].Generator.ToString();
            listView.columns["creditsCost"].bindCell = (element, index) =>
            {
                var label = (element as Label)!;
                label.text = ContentGenerationStore.Instance.Favorites[index].DeductedCredits
                    .ToString(CultureInfo.InvariantCulture);
            };
            listView.columns["delete"].bindCell = (element, index) =>
            {
                var button = element.Children().FirstOrDefault(c => c is Button) as Button;
                if (button == null)
                {
                    button = new Button();
                    button.AddToClassList("deleteButton");
                    button.clicked += DeleteButtonClicked;
                    element.Add(button);
                }

                void DeleteButtonClicked()
                {
                    if (!button.enabledSelf)
                        return;

                    button.SetEnabled(false);
                    ContentGenerationApi.Instance.DeleteFavorite(ContentGenerationStore.Instance.Favorites[index].ID)
                        .ContinueInMainThreadWith(t =>
                        {
                            button.SetEnabled(true);
                            if (t.IsFaulted)
                            {
                                Debug.LogException(t.Exception);
                            }

                            ContentGenerationStore.Instance.RefreshFavoritesAsync().CatchAndLog();
                        });
                }
            };

            favoritedItem.OnDeleted += () =>
            {
                listView.selectedIndex = -1;
                Refresh();
            };
            favoritedItem.style.display = DisplayStyle.None;

            listView.selectionChanged += objects =>
            {
                _selectedId = null;
                var objectsArray = objects.ToArray();
                favoritedItem.value = null;

                if (objectsArray.Length > 0)
                {
                    var favorite = (objectsArray[0] as Favorite)!;
                    _selectedId = favorite.ID;
                    favoritedItem.value = favorite;
                }

                favoritedItem.style.display =
                    favoritedItem.value == null ? DisplayStyle.None : DisplayStyle.Flex;
            };
        }

        void Refresh()
        {
            ContentGenerationStore.Instance.RefreshFavoritesAsync().CatchAndLog();
        }
    }
}