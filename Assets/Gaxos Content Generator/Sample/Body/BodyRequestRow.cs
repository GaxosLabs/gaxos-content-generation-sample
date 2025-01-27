using ContentGeneration.Models;
using Sample.Base;

namespace Sample.Body
{
    public class BodyRequestRow : RequestRow
    {
        protected override string GetPrompt(Request request)
        {
            return request.GeneratorParameters["style_prompt"]!.ToObject<string>();
        }
    }
}