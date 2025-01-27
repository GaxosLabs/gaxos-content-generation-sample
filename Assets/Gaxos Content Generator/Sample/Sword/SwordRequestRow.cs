using ContentGeneration.Models;
using Sample.Base;

namespace Sample.Sword
{
    public class SwordRequestRow : RequestRow
    {
        protected override string GetPrompt(Request request)
        {
            return request.GeneratorParameters["prompt"]!.ToObject<string>();
        }
    }
}