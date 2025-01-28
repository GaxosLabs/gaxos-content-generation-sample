using System;
using System.Threading.Tasks;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using ContentGeneration.Models.Gaxos;
using ContentGeneration.Models.Stability;
using UnityEngine;
using UnityEngine.Events;

namespace ContentGeneration.Generators
{
    public class ImageToImageMasking : Generator
    {
        [SerializeField] Texture2D _image;
        protected override Task<string> RequestGeneration(string prompt)
        {
            return ContentGenerationApi.Instance.RequestStabilityImageToImageGeneration(new StabilityImageToImageParameters
            {
                TextPrompts = new []
                {
                    new Prompt
                    {
                        Text = prompt,
                        Weight = 1
                    }
                },
                Samples = 1,
                InitImage = _image,
                ImageStrength = 0.01f,
            });
        }

        [Serializable]
        class UnityEventTexture : UnityEvent<Texture>
        {
        }

        [SerializeField] UnityEventTexture _textureGenerated;

        protected override async Task ReportGeneration(PublishedAsset asset)
        {
            _textureGenerated?.Invoke(await TextureHelper.DownloadImage(asset.URL));
        }
    }
}