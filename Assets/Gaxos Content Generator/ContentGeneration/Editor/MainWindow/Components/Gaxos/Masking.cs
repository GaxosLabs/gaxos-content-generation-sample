using System.Collections.Generic;
using System.Threading.Tasks;
using ContentGeneration.Models;
using ContentGeneration.Models.Gaxos;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Gaxos
{
    public class Masking : ParametersBasedGenerator<MaskingParameters, GaxosMaskingParameters>
    {
        public new class UxmlFactory : UxmlFactory<Masking, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        protected override string apiMethodName => nameof(ContentGenerationApi.RequestGaxosMaskingGeneration);
        protected override Task<string> RequestToApi(GaxosMaskingParameters parameters, GenerationOptions generationOptions, object data, bool estimate = false)
        {
            return ContentGenerationApi.Instance.RequestGaxosMaskingGeneration(
                    parameters,
                    generationOptions, data: data, estimate);
        }

        public override Generator generator => Generator.GaxosMasking;
    }
}