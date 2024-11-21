using System;
using System.Linq;
using ContentGeneration.Editor.MainWindow.Components;
using ContentGeneration.Editor.MainWindow.Components.BasicExamples;
using ContentGeneration.Editor.MainWindow.Components.DallE;
using ContentGeneration.Editor.MainWindow.Components.ElevenLabs;
using ContentGeneration.Editor.MainWindow.Components.Gaxos;
using ContentGeneration.Editor.MainWindow.Components.Meshy;
using ContentGeneration.Editor.MainWindow.Components.Suno;
using ContentGeneration.Editor.MainWindow.Components.Multi;
using ContentGeneration.Editor.MainWindow.Components.RequestsList;
using ContentGeneration.Editor.MainWindow.Components.FavoritesList;
using ContentGeneration.Editor.MainWindow.Components.StabilityAI;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow
{
    public class MainWindow : EditorWindow
    {
        [SerializeField] VisualTreeAsset _root;
        SubWindowToggle[] _allToggles;

        GaxosTab _gaxosTab;
        DallETab _dallETab;
        StabilityTab _stabilityAITab;
        MeshyTab _meshyTab;
        SunoTab _sunoTab;
        ElevenLabsTab _elevenLabsTab;

        public static MainWindow instance { get; private set; }

        [MenuItem("AI Content Generation/Main window")]
        public static void ShowMainWindow()
        {
            instance = GetWindow<MainWindow>();
            instance.minSize = new Vector2(500, 300);
            instance.titleContent = new GUIContent("AI Content Generation");
        }

        public void CreateGUI()
        {
            var rootInstance = _root.Instantiate();
            rootInstance.style.height = new StyleLength(new Length(100, LengthUnit.Percent));

            rootInstance.RegisterCallback<AttachToPanelEvent>(_ =>
            {
                instance = this;
            });

            _gaxosTab = rootInstance.Q<GaxosTab>();
            _dallETab = rootInstance.Q<DallETab>();
            _stabilityAITab = rootInstance.Q<StabilityTab>();
            _meshyTab = rootInstance.Q<MeshyTab>();
            _sunoTab = rootInstance.Q<SunoTab>();
            _elevenLabsTab = rootInstance.Q<ElevenLabsTab>();
            var multiTextToImage = rootInstance.Q<MultiTextToImage>();
            var multiMasking = rootInstance.Q<MultiMasking>();
            var requestsList = rootInstance.Q<RequestsListTab>();
            var favoritesList = rootInstance.Q<FavoritesListTab>();
            var basicGeneration = rootInstance.Q<BasicExamplesTab>();
            var configuration = rootInstance.Q<Configuration>();

            var subWindowsContainer = rootInstance.Q<VisualElement>("subWindowsContainer");
            var subWindows = subWindowsContainer.Children().ToArray();
            foreach (var visualElement in subWindows)
            {
                subWindowsContainer.Remove(visualElement);
            }

            var sideMenuItemsContainer = rootInstance.Q<VisualElement>("sideMenuItemsContainer");
            var sideMenuGeneratorsContainer = rootInstance.Q<VisualElement>("sideMenuGeneratorsContainer");
            var sideMenuMultiGeneratorsContainer = rootInstance.Q<VisualElement>("sideMenuMultiGeneratorsContainer");
            _allToggles = sideMenuItemsContainer.Children().Concat(sideMenuGeneratorsContainer.Children())
                .Concat(sideMenuMultiGeneratorsContainer.Children()).Where(c => c is SubWindowToggle)
                .Cast<SubWindowToggle>().ToArray();

            rootInstance.Q<SubWindowToggleIcon>("subWindowToggleGaxos").OnToggled += (sender, v) =>
            {
                ToggleSubWindow(sender, v, subWindowsContainer, _gaxosTab);
            };
            rootInstance.Q<SubWindowToggleIcon>("subWindowToggleDallE").OnToggled += (sender, v) =>
            {
                ToggleSubWindow(sender, v, subWindowsContainer, _dallETab);
            };
            rootInstance.Q<SubWindowToggleIcon>("subWindowToggleStabilityAI").OnToggled += (sender, v) =>
            {
                ToggleSubWindow(sender, v, subWindowsContainer, _stabilityAITab);
            };
            rootInstance.Q<SubWindowToggleIcon>("subWindowToggleMeshy").OnToggled += (sender, v) =>
            {
                ToggleSubWindow(sender, v, subWindowsContainer, _meshyTab);
            };
            rootInstance.Q<SubWindowToggleIcon>("subWindowToggleSuno").OnToggled += (sender, v) =>
            {
                ToggleSubWindow(sender, v, subWindowsContainer, _sunoTab);
            };
            rootInstance.Q<SubWindowToggleIcon>("subWindowToggleElevenLabs").OnToggled += (sender, v) =>
            {
                ToggleSubWindow(sender, v, subWindowsContainer, _elevenLabsTab);
            };
            rootInstance.Q<SubWindowToggleIcon>("subWindowToggleMultiTextToImage").OnToggled += (sender, v) =>
            {
                ToggleSubWindow(sender, v, subWindowsContainer, multiTextToImage);
            };
            rootInstance.Q<SubWindowToggleIcon>("subWindowToggleMultiMasking").OnToggled += (sender, v) =>
            {
                ToggleSubWindow(sender, v, subWindowsContainer, multiMasking);
            };
            rootInstance.Q<SubWindowToggle>("subWindowToggleRequestsList").OnToggled += (sender, v) =>
            {
                ToggleSubWindow(sender, v, subWindowsContainer, requestsList);
            };
            rootInstance.Q<SubWindowToggle>("subWindowToggleFavoritesList").OnToggled += (sender, v) =>
            {
                ToggleSubWindow(sender, v, subWindowsContainer, favoritesList);
            };
            rootInstance.Q<SubWindowToggle>("subWindowToggleBasicExamples").OnToggled += (sender, v) =>
            {
                ToggleSubWindow(sender, v, subWindowsContainer, basicGeneration);
            };
            rootInstance.Q<SubWindowToggle>("subWindowToggleSettings").OnToggled += (sender, v) =>
            {
                ToggleSubWindow(sender, v, subWindowsContainer, configuration);
            };


            var credits = rootInstance.Q<TextField>("credits");
            var refreshCredits = rootInstance.Q<Button>("refreshCredits");

            void RefreshStats(Stats v)
            {
                if (v == null)
                {
                    credits.value = "";
                    return;
                }

                credits.value = $"{v.Credits.Total - v.Credits.Used} / {v.Credits.Total}";
            }

            ContentGenerationStore.Instance.OnStatsChanged += RefreshStats;
            RefreshStats(ContentGenerationStore.Instance.stats);
            refreshCredits.clicked += () =>
            {
                if (!refreshCredits.enabledSelf)
                    return;
                refreshCredits.SetEnabled(false);
                ContentGenerationStore.Instance.RefreshStatsAsync().Finally(() => { refreshCredits.SetEnabled(true); });
            };
            if (string.IsNullOrEmpty(Settings.instance.apiKey))
            {
                Debug.LogWarning("Please enter your api key on the Configuration section");
            }
            else
            {
                ContentGenerationStore.Instance.RefreshStatsAsync().CatchAndLog();
            }

            rootVisualElement.Add(rootInstance);
        }

        void ToggleSubWindow(SubWindowToggle sender, bool v, VisualElement subWindowsContainer,
            VisualElement subWindow)
        {
            if (v)
            {
                subWindowsContainer.Add(subWindow);
                subWindow.style.display = DisplayStyle.Flex;

                foreach (var subWindowToggle in _allToggles)
                {
                    if (sender != subWindowToggle)
                    {
                        subWindowToggle.ToggleOff();
                    }
                }
            }
            else
            {
                subWindowsContainer.Remove(subWindow);
            }
        }

        public Favorite ShowFavorite;
        public void GoTo(Favorite favorite)
        {
            string toggleName;
            switch (favorite.Generator)
            {
                case Generator.StabilityTextToImage:
                case Generator.StabilityTextToImageCore:
                case Generator.StabilityTextToImageUltra:
                case Generator.StabilityDiffusion3:
                case Generator.StabilityStableFast3d:
                case Generator.StabilityImageToImage:
                case Generator.StabilityMasking:
                    toggleName = "subWindowToggleStabilityAI";
                    break;
                case Generator.DallETextToImage:
                case Generator.DallEInpainting:
                    toggleName = "subWindowToggleDallE";
                    break;
                case Generator.MeshyTextToMesh:
                case Generator.MeshyTextToTexture:
                case Generator.MeshyTextToVoxel:
                case Generator.MeshyImageTo3d:
                    toggleName = "subWindowToggleMeshy";
                    break;
                case Generator.SunoClipWithPrompt:
                case Generator.SunoClipWithLyrics:
                case Generator.SunoLyrics:
                    toggleName = "subWindowToggleSuno";
                    break;
                case Generator.GaxosTextToImage:
                case Generator.GaxosMasking:
                    toggleName = "subWindowToggleGaxos";
                    break;
                case Generator.ElevenLabsSound:
                case Generator.ElevenLabsTextToSpeech:
                    toggleName = "subWindowToggleElevenLabs";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("generator", favorite.Generator.ToString());
            }

            _allToggles.First(i => i.name == toggleName).ToggleOn();
            ShowFavorite = favorite;
        }
    }
}