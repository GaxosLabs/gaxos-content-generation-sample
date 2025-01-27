using System.Threading.Tasks;
using ContentGeneration;
using ContentGeneration.Models.Meshy;
using Sample.Base;
using Sample.Common;

namespace Sample.Sword
{
    public class NewSwordForm : NewGeneratedImageForm
    {
        public const string SwordSubject = "Sword";

        protected override Task RequestGeneration(string prompt)
        {
            return ContentGenerationApi.Instance.RequestMeshyTextToMeshGeneration
            (new MeshyTextToMeshParameters
            {
                Prompt = prompt,
                ArtStyle = TextToMeshArtStyle.Pbr,
            }, data: new
            {
                player_id = ProfileSettings.playerId,
                asset_type = SwordSubject
            });
        }
    }
}