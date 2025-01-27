using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Stability
{
    public enum Sampler
    {
        Ddim,
        Ddpm,
        KDpmpp2M,
        KDpmpp2SAncestral,
        KDpm2,
        KDpm2Ancestral,
        KEuler,
        KEulerAncestral,
        KHeun,
        KLms
    }

    internal class SamplerConverter : EnumJsonConverter<Sampler>
    {
        public override void WriteJson(JsonWriter writer, Sampler value, JsonSerializer serializer)
        {
            var str = value switch
            {
                Sampler.KDpmpp2M => "K_DPMPP_2M",
                Sampler.KDpmpp2SAncestral => "K_DPMPP_2S_ANCESTRAL",
                Sampler.KDpm2 => "K_DPM_2",
                Sampler.KDpm2Ancestral => "K_DPM_2_ANCESTRAL",
                Sampler.KEuler => "K_EULER",
                Sampler.KEulerAncestral => "K_EULER_ANCESTRAL",
                Sampler.KHeun => "K_HEUN",
                Sampler.KLms => "K_LMS",
                _ => value.ToString().ToUpperInvariant(),
            };
            writer.WriteValue(str);
        }

        protected override string AdaptString(string str)
        {
            return base.AdaptString(str).Replace("_", "");
        }
    }
}