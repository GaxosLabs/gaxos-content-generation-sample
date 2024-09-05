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

        protected override Task RequestToApi(GaxosTextToImageParameters parameters, GenerationOptions generationOptions,
            object data)
        {
            return ContentGenerationApi.Instance.RequestGaxosTextToImageGeneration(
                parameters,
                generationOptions, data: data);
        }
    }
}