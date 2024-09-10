using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models
{
    public enum Generator
    {
        StabilityTextToImage, StabilityTextToImageCore, StabilityTextToImageUltra, StabilityDiffusion3,
        StabilityImageToImage, StabilityMasking,
        DallETextToImage, DallEInpainting,
        MeshyTextToMesh, MeshyTextToTexture,
        GaxosTextToImage, GaxosMasking
    }
    
    internal class GeneratorTypeConverter : EnumJsonConverter<Generator>
    {
        public static string ToString(Generator generator)
        {
            return generator switch
            {
                Generator.StabilityTextToImage => "stability-text-to-image",
                Generator.StabilityTextToImageCore => "stability-text-to-image-core",
                Generator.StabilityTextToImageUltra => "stability-text-to-image-ultra",
                Generator.StabilityDiffusion3 => "stability-diffusion-3",
                Generator.StabilityImageToImage => "stability-image-to-image",
                Generator.StabilityMasking => "stability-masking",
                Generator.DallETextToImage => "dall-e-text-to-image",
                Generator.DallEInpainting => "dall-e-inpainting",
                Generator.MeshyTextToMesh => "meshy-text-to-mesh",
                Generator.MeshyTextToTexture => "meshy-text-to-texture",
                Generator.GaxosTextToImage => "gaxos-text-to-image",
                Generator.GaxosMasking => "gaxos-masking",
                _ => generator.ToString()
            };
        }
        public override void WriteJson(JsonWriter writer, Generator value, JsonSerializer serializer)
        {
            writer.WriteValue(ToString(value));
        }

        protected override string AdaptString(string str)
        {
            return base.AdaptString(str).Replace("-", "");
        }
    }
}