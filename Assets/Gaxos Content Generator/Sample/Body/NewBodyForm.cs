using System.Threading.Tasks;
using ContentGeneration;
using ContentGeneration.Models.Meshy;
using Sample.Base;
using Sample.Common;
using UnityEngine;

namespace Sample.Body
{
    public class NewBodyForm : NewGeneratedImageForm
    {
        public const string BodySubject = "Body";
        
        [SerializeField] string _modelExtension = "fbx";
        [SerializeField] TextAsset _model;

        protected override Task RequestGeneration(string prompt)
        {
            return ContentGenerationApi.Instance.RequestMeshyTextToTextureGeneration
            (new MeshyTextToTextureParameters
            {
                Model = _model.bytes,
                ModelExtension = _modelExtension,
                ObjectPrompt = "Robot",
                StylePrompt = prompt,
                NegativePrompt = "low quality, low resolution, low poly, ugly",
                ArtStyle = TextToTextureArtStyle.Fake3dCartoon,
            }, data: new
            {
                player_id = ProfileSettings.playerId,
                asset_type = BodySubject
            });
        }
    }
}