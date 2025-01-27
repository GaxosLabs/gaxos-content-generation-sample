using System.Collections.Generic;
using System.Threading.Tasks;
using ContentGeneration.Models;
using ContentGeneration.Models.Stability;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.StabilityAI
{
    public class TextToImageCore : ParametersBasedGenerator<TextToImageCoreParameters, StabilityCoreTextToImageParameters>
    {
        public new class UxmlFactory : UxmlFactory<TextToImageCore, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        protected override string apiMethodName => nameof(ContentGenerationApi.RequestStabilityCoreTextToImageGeneration);
        protected override Task<string> RequestToApi(StabilityCoreTextToImageParameters parameters,
            GenerationOptions generationOptions, object data, bool estimate = false)
        {
            return ContentGenerationApi.Instance.RequestStabilityCoreTextToImageGeneration(
                parameters,
                generationOptions,
                data: data, estimate);
        }

        public override Generator generator => Generator.StabilityTextToImageCore;
    }
}