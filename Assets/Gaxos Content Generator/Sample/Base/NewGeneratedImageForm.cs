using System.Threading.Tasks;
using ContentGeneration.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Button = UnityEngine.UI.Button;

namespace Sample.Base
{
    [DisallowMultipleComponent]
    public abstract class NewGeneratedImageForm : MonoBehaviour
    {
        [SerializeField] TMP_InputField _promptTextInput;
        [SerializeField] GameObject _promptIsRequiredGameObject;
        [SerializeField] Button _generateButton;
        [SerializeField] Button _backButton;
        [SerializeField] TextMeshProUGUI _errorLabel;

        void Awake()
        {
            _generateButton.onClick.AddListener(GenerateButtonClicked);
        }

        void OnEnable()
        {
            _promptTextInput.text = "";
            _promptTextInput.interactable = true;
            
            _promptIsRequiredGameObject.SetActive(false);
            _errorLabel.gameObject.SetActive(false);

            _backButton.interactable = true;
            _generateButton.interactable = true;
        }

        [SerializeField] UnityEvent _onGenerationRequested;

        void GenerateButtonClicked()
        {
            if (string.IsNullOrEmpty(_promptTextInput.text))
            {
                _promptIsRequiredGameObject.SetActive(true);
                return;
            }

            _promptTextInput.interactable = false;
            _promptIsRequiredGameObject.SetActive(false);
            _errorLabel.gameObject.SetActive(false);
            _backButton.interactable = false;
            _generateButton.interactable = false;

            
            RequestGeneration(_promptTextInput.text).ContinueInMainThreadWith(
                t =>
                {
                    _promptTextInput.interactable = true;
                    _backButton.interactable = true;
                    _generateButton.interactable = true;

                    if (t.IsFaulted)
                    {
                        _errorLabel.gameObject.SetActive(true);
                        _errorLabel.text = t.Exception!.InnerException!.Message;
                        Debug.LogException(t.Exception!.InnerException!, this);
                    }
                    else
                    {
                        _onGenerationRequested?.Invoke();
                    }
                });
        }

        protected abstract Task RequestGeneration(string prompt);
    }
}