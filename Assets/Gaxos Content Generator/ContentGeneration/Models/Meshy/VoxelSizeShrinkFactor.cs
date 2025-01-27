using System;
using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Meshy
{
    public enum VoxelSizeShrinkFactor
    {
        _1, _2, _4, _8
    }

    internal class VoxelSizeShrinkFactorConverter : EnumJsonConverter<VoxelSizeShrinkFactor>
    {
        public override void WriteJson(JsonWriter writer, VoxelSizeShrinkFactor value, JsonSerializer serializer)
        {
            var v = value switch
            {
                VoxelSizeShrinkFactor._1 => 1,
                VoxelSizeShrinkFactor._2 => 2,
                VoxelSizeShrinkFactor._4 => 4,
                VoxelSizeShrinkFactor._8 => 8,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
            writer.WriteValue(v);
        }

        protected override string AdaptString(string str)
        {
            return "_" + str;
        }
    }
}