using System.Collections.Generic;
using System.Threading.Tasks;
using ContentGeneration.Models;
using ContentGeneration.Models.Meshy;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Meshy
{
    public class ImageToMesh : ParametersBasedGenerator<ImageToMeshParameters, MeshyImageToMeshParameters>
    {
        public new class UxmlFactory : UxmlFactory<ImageToMesh, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        protected override string apiMethodName => nameof(ContentGenerationApi.RequestMeshyImageToMeshGeneration);

        protected override Task<string> RequestToApi(MeshyImageToMeshParameters parameters, GenerationOptions generationOptions,
            object data, bool estimate = false)
        {
            return ContentGenerationApi.Instance.RequestMeshyImageToMeshGeneration(
                parameters,
                generationOptions, data: data, estimate);
        }

        public override Generator generator => Generator.MeshyImageTo3d;
    }
}