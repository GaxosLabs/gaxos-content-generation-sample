using System.Collections.Generic;
using System.Threading.Tasks;
using ContentGeneration.Models;
using ContentGeneration.Models.Stability;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.StabilityAI
{
    public class StableFast3d : ParametersBasedGenerator<StableFast3dParameters, StabilityStableFast3d>
    {
        public new class UxmlFactory : UxmlFactory<StableFast3d, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        protected override string apiMethodName => nameof(ContentGenerationApi.RequestStabilityStableFast3dGeneration);
        protected override Task<string> RequestToApi(StabilityStableFast3d parameters,
            GenerationOptions generationOptions, object data, bool estimate = false)
        {
            return ContentGenerationApi.Instance.RequestStabilityStableFast3dGeneration(
                parameters,
                generationOptions,
                data: data, estimate);
        }

        public override Generator generator => Generator.StabilityStableFast3d;
    }
}