using System.Collections.Generic;
using System.Threading.Tasks;
using ContentGeneration.Models;
using ContentGeneration.Models.Recraft;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Recraft
{
    // https://www.recraft.ai/docs#generate-image
    public class TextToImage : ParametersBasedGenerator<TextToImageParameters, RecraftTextToImageParameters>
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

        protected override string apiMethodName => nameof(ContentGenerationApi.RequestRecraftTextToImageGeneration);

        protected override Task<string> RequestToApi(RecraftTextToImageParameters parameters, GenerationOptions generationOptions, object data, bool estimate = false)
        {
            return ContentGenerationApi.Instance.RequestRecraftTextToImageGeneration(
                parameters,
                generationOptions, data: data, estimate);
        }

        public override Generator generator => Generator.RecraftTextToImage;
    }
}