using System.Collections.Generic;
using System.Threading.Tasks;
using ContentGeneration.Models;
using ContentGeneration.Models.DallE;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.DallE
{
    public class TextToImage : ParametersBasedGenerator<TextToImageParameters, DallETextToImageParameters>
    {
        public new class UxmlFactory : UxmlFactory<TextToImage, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        protected override Task RequestToApi(DallETextToImageParameters parameters, GenerationOptions generationOptions, object data)
        {
            return ContentGenerationApi.Instance.RequestDallETextToImageGeneration(
                    parameters,
                    generationOptions, 
                    data: data);
        }
    }
}