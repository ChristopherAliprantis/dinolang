using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
namespace dinolang
{
    public partial class GetCode
    {
        public static List<string> ToReadableLines(string?code)
        {
            
            var LINES = Regex.Split(code, "(?<=;)").Select(s => s.Trim()).Where(s => s != "").ToList();
            List<int> ItoRem = new(0);
            for (int i = 0; i < LINES.Count; i++)
            {
                bool? quote = false;
                string?copy = "";
                char[] chars = LINES[i].ToCharArray();
                for (int j = 0; j < LINES[i].Length; j++)
                {
                    char c = chars[j];
                    if (chars[j] == ':') quote = !quote;
                    if (chars[j] == ' ' && quote == false)
                    {
                        continue;
                    }
                    else copy += chars[j];
                }
                LINES[i] = copy;
            }
            return LINES;

        }
    }
}
