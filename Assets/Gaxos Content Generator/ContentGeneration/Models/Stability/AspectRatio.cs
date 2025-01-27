using System;
using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Stability
{
    public enum AspectRatio
    {
        _1_1, _16_9, _21_9, _2_3, _3_2, _4_5, _5_4, _9_16, _9_21
    }

    internal class AspectRatioConverter : EnumJsonConverter<AspectRatio>
    {
        public override void WriteJson(JsonWriter writer, AspectRatio value, JsonSerializer serializer)
        {
            var str = value switch
            {
                AspectRatio._1_1 => "1:1",
                AspectRatio._16_9 => "16:9",
                AspectRatio._21_9 => "21:9",
                AspectRatio._2_3 => "2:3",
                AspectRatio._3_2 => "3:2",
                AspectRatio._4_5 => "4:5",
                AspectRatio._5_4 => "5:4",
                AspectRatio._9_16 => "9:26",
                AspectRatio._9_21 => "9:21",
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
            writer.WriteValue(str);
        }

        protected override string AdaptString(string str)
        {
            return "_" + str.Replace(":", "_");
        }
    }
}