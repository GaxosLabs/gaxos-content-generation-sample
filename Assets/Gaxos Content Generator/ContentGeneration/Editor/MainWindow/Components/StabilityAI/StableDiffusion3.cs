using System.Collections.Generic;
using System.Threading.Tasks;
using ContentGeneration.Models;
using ContentGeneration.Models.Stability;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.StabilityAI
{
    public class StableDiffusion3 : ParametersBasedGenerator<StableDiffusion3Parameters, StabilityStableDiffusion3Parameters>
    {
        public new class UxmlFactory : UxmlFactory<StableDiffusion3, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        protected override string apiMethodName => nameof(ContentGenerationApi.RequestStabilityStableDiffusion3Generation);
        protected override Task<string> RequestToApi(StabilityStableDiffusion3Parameters parameters,
            GenerationOptions generationOptions, object data, bool estimate = false)
        {
            return ContentGenerationApi.Instance.RequestStabilityStableDiffusion3Generation(
                parameters,
                generationOptions,
                data: data, estimate);
        }

        public override Generator generator => Generator.StabilityDiffusion3;
    }
}