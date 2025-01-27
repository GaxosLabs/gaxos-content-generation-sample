using System;

namespace ContentGeneration.Helpers
{
    public static class StringExtension
    {
        static string CamelCaseTo(this string str, string target, Func<char, char> extraProcessing = null)
        {
            extraProcessing ??= c => c;
            
            var lowerStr = str.ToLowerInvariant();
            var ret = lowerStr[0].ToString();

            for (var i = 1; i < str.Length; i++)
            {
                if (char.IsUpper(str[i]) || char.IsNumber(str[i]))
                {
                    ret += target;
                    ret += extraProcessing(lowerStr[i]);
                }
                else
                {
                    ret += lowerStr[i];
                }
            }

            return ret;
        }

        public static string MakeFirstLetterUppercase(this string str)
        {
            return str[0].ToString().ToUpper() + str[1..];
        }
        public static string CamelCaseToSpacesAndUpperCaseEachWord(this string str)
        {
            return str.CamelCaseTo(" ", c=> c.ToString().ToUpper()[0]).MakeFirstLetterUppercase();
        }
        public static string CamelCaseToDashes(this string str)
        {
            return str.CamelCaseTo("-");
        }
        public static string CamelCaseToUnderscores(this string str)
        {
            return str.CamelCaseTo("_");
        }

        public static string DashesToCamelCase(this string str)
        {
            var ret = "";

            var nextIsUpperCase = true;
            for (var i = 0; i < str.Length; i++)
            {
                if (str[i] == '-')
                {
                    nextIsUpperCase = true;
                }
                else
                {
                    var nextChar = str[i];
                    if (nextIsUpperCase)
                    {
                        nextChar = char.ToUpperInvariant(nextChar);
                    }

                    ret += nextChar;
                    
                    nextIsUpperCase = false;
                }
            }

            return ret;
        }
    }
}