using System;
using System.Threading.Tasks;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using ContentGeneration.Models.ElevenLabs;
using ContentGeneration.Models.Gaxos;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace ContentGeneration.Generators
{
    public class TextToSound : Generator
    {
        protected override Task<string> RequestGeneration(string prompt)
        {
            return ContentGenerationApi.Instance.RequestElevenLabsSoundGeneration(new ElevenLabsSoundParameters
            {
                Text = prompt
            });
        }

        [Serializable]
        class UnityEventUrl : UnityEvent<string>
        {
        }

        [SerializeField] UnityEventUrl _soundUrl;

        protected override Task ReportGeneration(PublishedAsset asset)
        {
            _soundUrl?.Invoke(asset.Request.GeneratorResult["url"]!.ToObject<string>());
            return Task.CompletedTask;
        }
    }
}