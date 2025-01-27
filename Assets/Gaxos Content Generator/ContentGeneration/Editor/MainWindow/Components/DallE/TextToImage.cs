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

        protected override string apiMethodName => nameof(ContentGenerationApi.RequestDallETextToImageGeneration);
        protected override Task<string> RequestToApi(DallETextToImageParameters parameters, 
            GenerationOptions generationOptions, object data, bool estimate = false)
        {
            return ContentGenerationApi.Instance.RequestDallETextToImageGeneration(
                    parameters,
                    generationOptions, 
                    data: data, estimate);
        }

        public override Generator generator => Generator.DallETextToImage;
    }
}