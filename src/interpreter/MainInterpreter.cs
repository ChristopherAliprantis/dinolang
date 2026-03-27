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
            List<string> newlines = new();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith("//")) continue;

                newlines.Add(line);
            }
            
            lines = newlines;
            lines = newlines;
  
            for (int i = 0; i < lines.Count; i++) 
            {
                bool mf = false;
                List<string> mfl = new();
                List<string> mfp = new List<string>();
                var line = lines[i];
                string? name = "";
                string? Ps = "";
                if (line.StartsWith("#func") && mf == false)
                {
                    mf = true;
                    name = BeforeChar(AfterChar(line, 'c'), '(');
                    Ps = BeforeChar(AfterChar(line, name[^1]), ';');
                    if (Ps.StartsWith('(') && Ps.EndsWith(')'))
                    {
                        Ps.Remove(0);
                        Ps.Remove(Ps.Length -1);
                        mfp = Ps.Split(',').ToList();
                    }
                    else
                    {
                        Console.WriteLine($"Invalid Function declaration, Line {i + 1} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                }
                else if (line == "#endfunc;")
                {
                    dinolang.interpreter.Globals.Funcs[name] = new Function
                    {
                        parameters = mfp,
                        code = mfl,
                    };
                    
                }
                else if (mf)
                {
                    mfl.Add(line);
                    continue;
                }
                else if (line.StartsWith("print(") && line.EndsWith(");"))
                {
                    string arg = BeforeChar(AfterChar(line, '('), ')');
                    Console.WriteLine(GetValue(arg, i + 1, null));
                }
                else if (Globals.Funcs.ContainsKey(BeforeChar(line, '(')))
                {
                    var f = Globals.Funcs[BeforeChar(line, '(')];
                    var p = BeforeChar(AfterChar(line, '('), ')');
                    var sP = p.Split(',').ToList();
                    List<dynamic> dP = new();
                    for (int h = 0; h < sP.Count; h++)
                    {
                        dP.Add(GetValue(sP[h], i+1, dP));
                    }
                    GetValue(line, i+1, dP);
                }
                else if (line.StartsWith($"{BeforeChar(line, '=')}="))
                {
                    var b = BeforeChar(line, '=');
                    var a = BeforeChar(AfterChar(line, '='), ';');
                    dinolang.interpreter.Globals.Vars[b] = new Variable
                    {
                        value = GetValue(a, i+1, null),
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

        public static dynamic? GetValue(string val, int line, List<dynamic>? vals)
        {
            if (vals == null) vals = new();
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
                            return ProcessFunc(dinolang.interpreter.Globals.Funcs[val], line, vals);
                        }
                    }
                }
                
            }
            if (dinolang.interpreter.Globals.Vars.ContainsKey(val))
            {
                return dinolang.interpreter.Globals.Vars[val].value;
            }
            Console.WriteLine($"Invalid Value, Line {line}");
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