using System;
using System.Threading.Tasks;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using ContentGeneration.Models.Gaxos;
using UnityEngine;
using UnityEngine.Events;

namespace ContentGeneration.Generators
{
    public class TextToImage : Generator
    {
        protected override Task<string> RequestGeneration(string prompt)
        {
            return ContentGenerationApi.Instance.RequestGaxosTextToImageGeneration(new GaxosTextToImageParameters()
            {
                Prompt = prompt,
                NSamples = 1
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