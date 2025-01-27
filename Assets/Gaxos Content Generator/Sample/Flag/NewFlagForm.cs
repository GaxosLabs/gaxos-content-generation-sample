using System.Threading.Tasks;
using ContentGeneration;
using ContentGeneration.Models.Gaxos;
using Sample.Base;
using Sample.Common;

namespace Sample.Flag
{
    public class NewFlagForm : NewGeneratedImageForm
    {
        public const string FlagSubject = "Flag";

        protected override Task RequestGeneration(string prompt)
        {
            return ContentGenerationApi.Instance.RequestGaxosTextToImageGeneration
            (new GaxosTextToImageParameters
            {
                Prompt = prompt,
                NegativePrompt = "Background, 3D, low quality, blurry",
                NSamples = 4,
            }, data: new
            {
                player_id = ProfileSettings.playerId,
                asset_type = FlagSubject
            });
        }
    }
}