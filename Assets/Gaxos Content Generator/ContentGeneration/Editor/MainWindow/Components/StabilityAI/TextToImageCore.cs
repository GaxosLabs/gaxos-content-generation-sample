using System.Collections.Generic;
using System.Threading.Tasks;
using ContentGeneration.Models;
using ContentGeneration.Models.Stability;
using Newtonsoft.Json.Linq;
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
        protected override Task RequestToApi(StabilityCoreTextToImageParameters parameters,
            GenerationOptions generationOptions, object data)
        {
            return ContentGenerationApi.Instance.RequestStabilityCoreTextToImageGeneration(
                parameters,
                generationOptions,
                data: data);
        }

        public override Generator generator => Generator.StabilityTextToImageCore;
        public override void Show(JObject generatorParameters)
        {
        }
    }
}