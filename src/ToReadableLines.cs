using System;
using System.Collections.Generic;
using System.Text;
namespace dinolang
{
    public partial class GetCode
    {
        public static List<string> ToReadableLines(string code)
        {
            List<char> charlist = new();
            List<string> New = new();
            for (int i = 0; i < code.Length; i++)
            {
                charlist.Add(code[i]);
                if (code[i] == ';')
                {
                    string line = "";
                    for (int j = 0; j < charlist.Count; j++)
                    {
                        line += charlist[j];
                    }
                    line = line.Trim();
                    bool In = false;
                    List<char> line2 = new();
                    for (int h = 0; h < line.Length; h++)
                    {
                        if (line[h] == ':')
                        {
                            In = !In;
                        }
                        if (line[h] == ' ')
                        {
                            if (In == true)
                            {
                                line2.Add(line[h]);
                            }
                            else if (In == false)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            line2.Add(line[h]);
                        }
                    }
                    New.Add(string.Concat(line2));
                    charlist.Clear();
                }
            }
            return New;
        }
    }
}
