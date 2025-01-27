using System;
using System.Collections.Generic;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.FavoritesList
{
    public class FavoritedItemCommon : VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<FavoritedItemCommon, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield return new UxmlChildElementDescription(typeof(VisualElement)); }
            }
        }

        SubWindow favoritedItem => this.Q<SubWindow>("favoritedItem");
        Label generator => this.Q<Label>("generator");
        TextField generatorParameters => this.Q<TextField>("generatorParameters");
        Button deleteButton => this.Q<Button>("deleteButton");
        Button applyFavorite => this.Q<Button>("applyFavorite");

        public override VisualElement contentContainer => this.Q<VisualElement>("childrenContainer");

        public FavoritedItemCommon()
        {
            deleteButton.clicked += () =>
            {
                if (deleteButton.enabledSelf)
                {
                    deleteButton.SetEnabled(false);
                    ContentGenerationApi.Instance.DeleteFavorite(value.ID).ContinueInMainThreadWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            Debug.LogException(t.Exception!.InnerException);
                        }
                        else
                        {
                            OnDeleted?.Invoke();
                        }

                        deleteButton.SetEnabled(true);
                    });
                }
            };
            applyFavorite.clicked += () =>
            {
                if (applyFavorite.enabledSelf)
                {
                    MainWindow.instance.GoTo(value);
                }
            };
            generatorParameters.SetVerticalScrollerVisibility(ScrollerVisibility.Auto);
        }

        public event Action OnDeleted;
        Favorite _value;

        public Favorite value
        {
            get => _value;
            set
            {
                _value = value;
                if (value == null)
                    return;

                var generatorName = value.Generator.ToString();
                if (generatorName.StartsWith("Stability"))
                {
                    favoritedItem.subWindowIcon = "Stability AI";
                }
                else if (generatorName.StartsWith("Meshy"))
                {
                    favoritedItem.subWindowIcon = "Meshy";
                }
                else if (generatorName.StartsWith("Gaxos"))
                {
                    favoritedItem.subWindowIcon = "Gaxos Labs AI";
                }
                else
                {
                    favoritedItem.subWindowIcon = null;
                }

                favoritedItem.subWindowName = generatorName.CamelCaseToSpacesAndUpperCaseEachWord();

                generator.text = value.Generator.ToString();

                generatorParameters.value = value.GeneratorParameters?.ToString();
            }
        }
    }
}