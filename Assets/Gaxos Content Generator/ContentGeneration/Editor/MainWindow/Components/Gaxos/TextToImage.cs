using System.Collections.Generic;
using System.Threading.Tasks;
using ContentGeneration.Models;
using ContentGeneration.Models.Gaxos;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Gaxos
{
    public class TextToImage : ParametersBasedGenerator<TextToImageParameters, GaxosTextToImageParameters>
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

        protected override string apiMethodName => nameof(ContentGenerationApi.RequestGaxosTextToImageGeneration);

        protected override Task<string> RequestToApi(GaxosTextToImageParameters parameters, GenerationOptions generationOptions,
            object data, bool estimate = false)
        {
            return ContentGenerationApi.Instance.RequestGaxosTextToImageGeneration(
                parameters,
                generationOptions, data: data, estimate);
        }

        public override Generator generator => Generator.GaxosTextToImage;
    }
}