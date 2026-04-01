using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
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
            bool mf = false;
            List<string> mfl = new();
            List<string> mfp = new List<string>();
            string? name = "";
            for (int i = 0; i < lines.Count; i++) 
            {
                
                var line = lines[i];
                if (line.StartsWith("#func") && mf == false)
                {
                    mf = true;
                    name = BeforeChar(line.Substring(5), '(');
                    string Ps = AfterChar(line, name[^1]);
                    Ps = BeforeChar(Ps, ';');
                    if (Ps.StartsWith('(') && Ps.EndsWith(')'))
                    {
                        Ps = Ps.Substring(1, Ps.Length - 2);
                        
                        mfp = Ps.Split(',').ToList();
                    }
                    else
                    {
                        Console.WriteLine($"Invalid Function declaration, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                }
                else if (line == "#endfunc;")
                {
                    if (mf == false)
                    {
                        Console.WriteLine($"No function in making, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                    dinolang.interpreter.Globals.Funcs[name] = new Function
                    {
                        parameters = mfp,
                        code = new List<string>(mfl)
                    };
                    mf = false;
                    mfl.Clear();
                    mfp.Clear();
                    name = "";
                }
                else if (mf)
                {
                    mfl.Add(line);
                    continue;
                }
                else if (line.StartsWith("print(") && line.EndsWith(");"))
                {
                    string arg = BeforeChar(AfterChar(line, '('), ");");
                    Console.WriteLine(GetValue(arg, line, null));
                }
                else if (line.StartsWith($"{BeforeChar(line, '=')}="))
                {
                    var b = BeforeChar(line, '=');
                    var a = BeforeChar(AfterChar(line, '='), ';');
                    dinolang.interpreter.Globals.Vars[b] = new Variable
                    {
                        value = GetValue(a, line, null),
                    };
                    if (dinolang.interpreter.Globals.Vars[b].value is string) dinolang.interpreter.Globals.Vars[b].type = "string";
                    else if (dinolang.interpreter.Globals.Vars[b].value is decimal) dinolang.interpreter.Globals.Vars[b].type = "num";
                }
                else if (line.Contains("(") && line.EndsWith(");"))
                {
                    var Var = BeforeChar(line, ';');
                    GetValue(Var, line, null);
                }
                else
                {
                    Console.WriteLine($"Invalid Code, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
            }
        }

        public static dynamic? GetValue(string val, string line, List<dynamic>? vals)
        {
            if (vals == null) vals = new();
            if (string.IsNullOrWhiteSpace(val))
            {
                val = "::";
            }
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
            string fname = BeforeChar(val, '(');
            if (Globals.Funcs.ContainsKey(fname))
            {
                string inside = BeforeChar(AfterChar(val, '('), ')');

                List<dynamic> args = new List<dynamic>();

                if (!string.IsNullOrWhiteSpace(inside))
                {
                    string[] split = inside.Split(',');

                    foreach (var s in split)
                    {
                        args.Add(s);
                    }
                }
                return ProcessFunc(Globals.Funcs[fname], args, line);
            }

            if (dinolang.interpreter.Globals.Vars.ContainsKey(val))
            {
                return dinolang.interpreter.Globals.Vars[val].value;
            } 
            Console.WriteLine($"Invalid Value, Line {line}");
            Environment.Exit(1); 
            if (1 + 1 == 2) return " "; 
        } 

        public static string BeforeChar(string s, string c)
        {
            if (!s.Contains(c))
            {
                return s;
            }
            int i = s.IndexOf(c);
            return i >= 0 ? s[..i] : s;
        }

        public static string AfterChar(string s, string c)
        {
            if (!s.Contains(c))
            {
                return s;
            }
            int i = s.IndexOf(c);
            return i >= 0 && i < s.Length - 1 ? s[(i + 1)..] : "";
        }
        public static string BeforeChar(string s, char c)
        {
            if (!s.Contains(c))
            {
                return s;
            }
            int i = s.IndexOf(c);
            return i >= 0 ? s[..i] : s;
        }

        public static string AfterChar(string s, char c)
        {
            if (!s.Contains(c))
            {
                return s;
            }
            int i = s.IndexOf(c);
            return i >= 0 && i < s.Length - 1 ? s[(i + 1)..] : "";
        }
    }
}