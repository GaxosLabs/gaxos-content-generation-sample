using System.Threading.Tasks;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ContentGeneration.Generators
{
    public abstract class Generator : MonoBehaviour
    {
        [SerializeField] TMP_InputField InputField;
        [SerializeField] TMP_Text Label;
        [SerializeField] Button Button;

        string generationIdPlayerPrefKey => $"{gameObject.name}_{GetType().Name}_contentGeneratorKey";
        string assetUrlPlayerPrefKey => $"{gameObject.name}_{GetType().Name}_assetUrl";

        void Awake()
        {
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString(generationIdPlayerPrefKey)))
            {
                ShowStatus("Generating...");
            }

            var assetUrl = PlayerPrefs.GetString(assetUrlPlayerPrefKey);
            if (!string.IsNullOrEmpty(assetUrl))
            {
                Enable(false);
                enabled = false;
                ReportGeneration(assetUrl).Finally(() =>
                {
                    Enable(true);
                    enabled = true;
                });
            }

            Button.onClick.AddListener(() =>
            {
                Enable(false);
                enabled = false;
                ShowStatus("Requesting...");
                RequestGeneration(InputField.text).ContinueInMainThreadWith(t =>
                {
                    enabled = true;
                    if (t.IsFaulted)
                    {
                        ShowStatus($"Error: {t.Exception!.Message}");
                        Enable(true);
                    }
                    else
                    {
                        ShowStatus("Generating...");
                        PlayerPrefs.SetString(generationIdPlayerPrefKey, t.Result);
                    }
                });
            });
        }

        protected abstract Task<string> RequestGeneration(string prompt);

        void ShowStatus(string text)
        {
            Label.text = text;
            Label.gameObject.SetActive(true);
        }

        void Enable(bool enable)
        {
            InputField.interactable = enable;
            Button.interactable = enable;
        }

        bool refreshing;

        void Update()
        {
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString(generationIdPlayerPrefKey)))
            {
                Enable(false);
                if (!refreshing)
                {
                    refreshing = true;
                    Refresh().Finally(() => refreshing = false);
                }
            }
            else
            {
                Enable(true);
            }
        }

        async Task Refresh()
        {
            var id = PlayerPrefs.GetString(generationIdPlayerPrefKey);

            var result = await ContentGenerationApi.Instance.GetRequest(id);
            if (result.Status == RequestStatus.Generated)
            {
                ShowStatus("Generated!");
                await ContentGenerationApi.Instance.MakeAssetPublic(id, result.Assets[0].ID, true);
                await ContentGenerationApi.Instance.DeleteRequest(id);
                await ReportGeneration(result.Assets[0].URL);
                PlayerPrefs.SetString(assetUrlPlayerPrefKey, result.Assets[0].URL);
                PlayerPrefs.DeleteKey(generationIdPlayerPrefKey);
            }
            else if (result.Status == RequestStatus.Failed)
            {
                ShowStatus($"Error: {result.GeneratorError.Message}");
                await ContentGenerationApi.Instance.DeleteRequest(id);
                PlayerPrefs.DeleteKey(generationIdPlayerPrefKey);
            }

            await Task.Delay(3000);
        }

        protected abstract Task ReportGeneration(string url);
    }
}