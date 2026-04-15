using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;

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
            List<string> mfp = new List<string>(0);
            string? name = "";
            bool POL = false;
            List<string> loopLines = new();
            decimal times = 0;
            for (int i = 0; i < lines.Count; i++)
            {

                var line = lines[i];
                if (line.StartsWith("#func") && mf == false)
                {
                    mf = true;
                    name = BeforeChar(line.Substring(5), '(');
                    string Ps = AfterChar(line, "#func" + name);
                    Ps = BeforeChar(Ps, ';');
                    if (Ps.StartsWith('(') && Ps.EndsWith(')'))
                    {
                        Ps = Ps.Substring(1, Ps.Length - 2);

                        if (Ps != "") mfp = Ps.Split(',').ToList();
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
                        parameters = new List<string>(mfp),
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
                else if (line.StartsWith("#loop"))
                {
                    if (AfterChar(line, "#loop") != ";")
                    {
                        string thing = BeforeChar(AfterChar(line, "#loop"), ';');
                        string args = AfterChar(BeforeChar(thing, ')'), '(');
                        try
                        {
                            times = GetValue(args, line);
                            if (times % 1 == 0) ;
                            else
                            {
                                Console.WriteLine($"Invalid Loop declaration, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                                Environment.Exit(1);
                            }
                        }
                        catch
                        {
                            Console.WriteLine($"Invalid Loop declaration, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        }
                        POL = true;
                    }
                    else
                    {
                        Console.WriteLine($"Invalid Loop declaration, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                }
                else if (line == "#endloop;")
                {
                    if (POL == false)
                    {
                        Console.WriteLine($"No loop in making, Line {line} Try going on  https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                    for (int l = 0; l < Convert.ToInt32(times); l++)
                    {
                        int st = ProcessLoop(loopLines);
                        if (st == 0) break;
                    }
                    POL = false;
                    times = 0;
                    loopLines.Clear();
                }
                else if (POL == true) loopLines.Add(line);
                else if (line.StartsWith("print(") && line.EndsWith(");"))
                {
                    string arg = line.Substring(0, line.Length - 2);
                    arg = AfterChar(arg, '(');
                    Console.WriteLine(((string)GetValue(arg, line).ToString()));
                }
                else if (line.StartsWith("printnnl(") && line.EndsWith(");"))
                {
                    string arg = line.Substring(0, line.Length - 2);
                    arg = AfterChar(arg, '(');
                    Console.Write(((string)GetValue(arg, line).ToString()));
                }
                else if (line.StartsWith($"{BeforeChar(line, '=')}="))
                {
                    var b = BeforeChar(line, '=');
                    var a = BeforeChar(AfterChar(line, '='), ';');
                    dinolang.interpreter.Globals.Vars[b] = new Variable
                    {
                        value = GetValue(a, line),
                    };
                    if (dinolang.interpreter.Globals.Vars[b].value is string) dinolang.interpreter.Globals.Vars[b].type = "string";
                    else if (dinolang.interpreter.Globals.Vars[b].value is decimal) dinolang.interpreter.Globals.Vars[b].type = "num";
                    else if (dinolang.interpreter.Globals.Vars[b].value is bool) dinolang.interpreter.Globals.Vars[b].type = "bool";
                    else if (dinolang.interpreter.Globals.Vars[b].value is null) dinolang.interpreter.Globals.Vars[b].type = "null";
                }
                else if (line.Contains("(") && line.EndsWith(");"))
                {
                    var Var = BeforeChar(line, ';');
                    GetValue(Var, line);
                }
                else
                {
                    Console.WriteLine($"Invalid Code, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
            }
        }
        public static dynamic? GetValue(string val, string line)
        {
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
            if (val == "false") return false; 
            if (val == "true") return true;
            if (val == "null") return null;
            string? value2;
            if (val.StartsWith(':') && val.EndsWith(':'))
            {
                value2 = val.Substring(1, val.Length - 2);
                return value2;
            }
            if (val == "COLON")
            {
                return ":";
            }
            if (dinolang.interpreter.Globals.Vars.ContainsKey(val))
            {
                return dinolang.interpreter.Globals.Vars[val].value;
            }
            string fname = BeforeChar(val, '(');
            if (Globals.Funcs.ContainsKey(fname))
            {
                string inside = BeforeChar(AfterChar(val, '('), ')');

                List<dynamic> args = new List<dynamic>(0);

                if (inside != "") 
                {
                    if (!string.IsNullOrWhiteSpace(inside))
                    { 
                        string[] split = inside.Split(',');

                        foreach (var s in split)
                        {
                            args.Add(s);
                        }
                    }
                }
                return ProcessFunc(Globals.Funcs[fname], args, line);
            }
            if (val.StartsWith("==(") && val.EndsWith(")"))
            {
                string[] VALS = val.Substring(3, val.Length - 4).Split(',');
                if (VALS.Length != 2)
                {
                    Console.WriteLine($"Need 2 parameters to compare, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                dynamic[] dynamics = new dynamic[2] { GetValue(VALS[0], line), GetValue(VALS[1], line) };
                return dynamics[0] == dynamics[1];
            }
            if (val.StartsWith("!(") && val.EndsWith(")"))
            {
                string arg = val.Substring(2, val.Length - 3);
                var Out = GetValue(arg, line);
                if (Out is not bool)
                {
                    Console.WriteLine($"Cannot reverse non-boolean value, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                else
                {
                    return !(bool)Out;
                }
            }
            if (val.StartsWith("+(") && val.EndsWith(')'))
            {
                string[] VALS = val.Substring(2, val.Length - 3).Split(',');
                if (VALS.Length < 2)
                {
                    Console.WriteLine($"Need at least 2 parameters to add, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                List<dynamic> Vals = new();
                for (int i = 0; i < VALS.Length; i++)
                {
                    Vals.Add(GetValue(VALS[i], line));
                }
                bool allsame = Vals.Count == 0 ||
                    Vals.All(x => x.GetType() == Vals[0].GetType());
                if (!allsame)
                {
                    Console.WriteLine($"Not all values are the same, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                else
                {
                    dynamic? result = 0.0m;
                    if (Vals[0] is string)
                    {
                        result = "";
                    }
                    foreach (var v in Vals)
                    {
                        result += v;
                    }
                    return result;
                }

            }
            if (val.StartsWith("-(") && val.EndsWith(')'))
            {
                string[] VALS = val.Substring(2, val.Length - 3).Split(',');
                if (VALS.Length < 2)
                {
                    Console.WriteLine($"Need at least 2 parameters to subtract, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                List<decimal> Vals = new();
                for (int i = 0; i < VALS.Length; i++)
                {
                    try
                    {
                        Vals.Add(GetValue(VALS[i], line));
                    }
                    catch
                    {
                        Console.WriteLine($"Invalid Value, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                }
                decimal? result = Vals[0];
                for (int I = 0; I < Vals.Count; I++)
                {
                    if (I == 0) continue;
                    result -= Vals[I];
                }
                return result;
            }
            if (val.StartsWith("/(") && val.EndsWith(')'))
            {
                string[] VALS = val.Substring(2, val.Length - 3).Split(',');
                if (VALS.Length < 2)
                {
                    Console.WriteLine($"Need at least 2 parameters to divide, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                List<decimal> Vals = new();
                for (int i = 0; i < VALS.Length; i++)
                {
                    try
                    {
                        Vals.Add(GetValue(VALS[i], line));
                    }
                    catch
                    {
                        Console.WriteLine($"Invalid Value, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                }
                decimal? result = Vals[0];
                for (int I = 0; I < Vals.Count; I++)
                {
                    if (I == 0) continue;
                    try
                    {
                        result /= Vals[I];
                    }
                    catch
                    {
                        Console.WriteLine($"Cannot divide by {Vals[I]} Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                }
                return result;
            }
            if (val.StartsWith("*(") && val.EndsWith(')'))
            {
                string[] VALS = val.Substring(2, val.Length - 3).Split(',');
                if (VALS.Length < 2)
                {
                    Console.WriteLine($"Need at least 2 parameters to multiply, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                List<decimal> Vals = new();
                for (int i = 0; i < VALS.Length; i++)
                {
                    try
                    {
                        Vals.Add(GetValue(VALS[i], line));
                    }
                    catch
                    {
                        Console.WriteLine($"Invalid Value, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                }
                decimal? result = Vals[0];
                for (int I = 0; I < Vals.Count; I++)
                {
                    if (I == 0) continue;
                    result *= Vals[I];
                }
                return result;
            }
            
            Console.WriteLine($"Invalid Value, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
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
            if (!s.Contains(c)) return s;

            int i = s.IndexOf(c) + c.Length;
            return i >= 0 && i < s.Length ? s[i..] : "";
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
            if (!s.Contains(c)) return s;

            int i = s.IndexOf(c);
            return i >= 0 && i < s.Length - 1 ? s[(i + 1)..] : "";
        }
    }
}