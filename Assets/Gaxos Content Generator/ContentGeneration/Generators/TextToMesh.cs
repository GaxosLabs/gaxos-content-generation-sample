using System.Threading.Tasks;
using ContentGeneration.Models.Meshy;

namespace ContentGeneration.Generators
{
    public class TextToMesh : Generator
    {
        protected override Task<string> RequestGeneration(string prompt)
        {
            return ContentGenerationApi.Instance.RequestMeshyTextToMeshGeneration(new MeshyTextToMeshParameters
            {
                Prompt = prompt,
            });
        }

        protected override Task ReportGeneration(string url)
        {
            throw new System.NotImplementedException();
        }
    }
}