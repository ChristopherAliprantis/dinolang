using System;
using System.Collections.Generic;
using System.Text;
namespace dinolang
{
    public partial class GetCode
    {
        public static List<string> Strip(string code)
        {
            List<char> charlist = new();
            List<string> New = new();
            for (int i = 0; i < code.Length; i++)
            {
                charlist.Add(code[i]);
                if (code[i] == ';')
                {
                    string line = "";
                    for (int j = 0; i < charlist.Count; i++)
                    {
                        line += charlist[i];
                    }
                    New.Add(line);
                    charlist.Clear();
                }
            }
            return New;
        }
    }
}
