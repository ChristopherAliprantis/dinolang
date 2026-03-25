using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace dinolang.interpreter
{
    public partial class Interpreter
    {
        public static void Interpret(List<string> lines)
        {
            var newlines = lines;
            int back = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    newlines.RemoveAt(i - back);
                    back++;
                }
                if (lines[i].StartsWith("//"))
                {
                    newlines.RemoveAt(i - back);
                    back++;
                }
                if (string.IsNullOrWhiteSpace(lines[i]) && i == lines.Count - 1)
                {
                    return;
                }
            }
            lines = newlines;
  
            for (int i = 0; i < lines.Count; i++) 
            {
                var line = lines[i];
                if (line.StartsWith("print(") && line.EndsWith(");"))
                {
                    string arg = line.Substring(6, line.Length - 8);
                    Console.WriteLine(GetValue(arg, i + 1));
                }
                else if (line.StartsWith($"{BeforeChar(line, '=')}="))
                {
                    var b = BeforeChar(line, '=');
                    var a = BeforeChar(AfterChar(line, '='), ';');
                    dinolang.interpreter.Globals.Vars[b] = new Variable
                    {
                        value = GetValue(a, i+1),
                    };
                    if (dinolang.interpreter.Globals.Vars[b].value is string) dinolang.interpreter.Globals.Vars[b].type = "string";
                    else if (dinolang.interpreter.Globals.Vars[b].value is decimal) dinolang.interpreter.Globals.Vars[b].type = "num";
                }
                else
                {
                    Console.WriteLine($"Invalid Code, Line {i+1} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
            }
        }

        public static dynamic? GetValue(string val, int line)
        {
            decimal? value1;
            try
            {
                value1 = decimal.Parse(val);
                return value1;
            }
            catch { }
            string? value2;
            if (val.StartsWith(':') && val.EndsWith(':'))
            {
                value2 = val.Substring(1, val.Length - 2);
                return value2;
            }
            
            if (dinolang.interpreter.Globals.Funcs.ContainsKey(val))
            {
                var poses = (0, 0); 
                for (int i = 0; i < val.Length; i++)
                {
                    if (val[i] == '(')
                    {
                        poses.Item1 = i;
                    }
                    if (val[i] == ')')
                    {
                        poses.Item2 = i;
                        string brackets = val.Substring(poses.Item1, poses.Item2).Trim();
                        int j;
                        int paramS = 0;
                        for (j = 0; j <= brackets.Length; j++)
                        {
                            if (brackets[j] != ',') paramS++;
                        }
                        if (j == 0) break;
                        Function func = dinolang.interpreter.Globals.Funcs[val];
                        if (func.parameters.Count == paramS)
                        {
                            return ProcessFunc(dinolang.interpreter.Globals.Funcs[val], line);
                        }
                    }
                }
                
            }
            if (dinolang.interpreter.Globals.Vars.ContainsKey(val))
            {
                return dinolang.interpreter.Globals.Vars[val].value;
            }
            Console.WriteLine($"Invalid Value, Line {line}.");
            Environment.Exit(1);
            if (1 + 1 == 2) return null;
        }

        public static string BeforeChar(string s, char c)
        {
            int i = s.IndexOf(c);
            return i >= 0 ? s[..i] : s;
        }

        public static string AfterChar(string s, char c)
        {
            int i = s.IndexOf(c);
            return i >= 0 && i < s.Length - 1 ? s[(i + 1)..] : "";
        }
    }
}