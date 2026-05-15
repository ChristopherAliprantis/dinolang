using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Markup;

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
            dynamic times = 0.0m;
            string args = "";
            string cond = "";
            List<string> IfLines = new();
            bool IF = false;
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
                        args = AfterChar(BeforeChar(line, ");"), "#loop(");
                        try
                        {
                            times = GetValue(args, line);
                            if (times is not bool)
                            {
                                if (times % 1 == 0) ;
                                else
                                {
                                    Console.WriteLine($"Invalid Loop declaration, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                                    Environment.Exit(1);
                                }
                            }
                        }
                        catch
                        {
                            Console.WriteLine($"Invalid Loop declaration, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                            Environment.Exit(1);
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
                    if (times is not bool)
                    {
                        times = Convert.ToDecimal(times);
                        for (long l = 0; l < Convert.ToInt64((decimal)times); l++)
                        {
                            int st = ProcessLoop(loopLines);
                            if (st == 0) break;
                        }
                    }
                    else
                    {
                        while (true)
                        {
                            if ((bool)GetValue(args, line))
                            {
                                int st = ProcessLoop(loopLines);
                                if (st == 0) break;
                            }
                            else break;
                        }
                    }
                    POL = false;
                    times = 0;
                    loopLines.Clear();
                }
                else if (POL == true) loopLines.Add(line);
                else if (line.StartsWith("#if"))
                {
                    cond = AfterChar(BeforeChar(line, ");"), "#if(");
                    IF = true;
                }
                else if (line == "#endif;")
                {
                    if (IF == false)
                    {
                        Console.WriteLine($"No if statement in making, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                    IF = false;
                    bool COND = false;
                    try
                    {
                        COND = (bool)GetValue(cond, line);
                    }
                    catch 
                    {
                        Console.WriteLine($"Invalid Value, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                    if (COND) ProcessIf(IfLines, false, false);
                }
                else if (IF) IfLines.Add(line);
                else if (line.StartsWith("print(") && line.EndsWith(");"))
                {
                    string arg = line.Substring(0, line.Length - 2);
                    arg = AfterChar(arg, "print(");
                    dynamic result = GetValue(arg, line);
                    if (result is bool) result = result.ToString().ToUpper();
                    else result = result?.ToString() ?? "NULL";
                    Console.WriteLine(result);
                }
                else if (line.StartsWith("PowershellCall(") && line.EndsWith(");"))
                {
                    var arg = line.Substring(15, line.Length - 17);
                    dynamic arg2 = GetValue(arg, line);
                    if (arg2 is not string)
                    {
                        Console.WriteLine($"Expected String, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(0);
                    }
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-ExecutionPolicy Bypass -Command \"{arg2}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };

                    using (var process = Process.Start(startInfo))
                    {
                        string output = process.StandardOutput.ReadToEnd();
                        string errors = process.StandardError.ReadToEnd();

                        process.WaitForExit();

                        if (!string.IsNullOrEmpty(output)) Console.WriteLine(output);
                        if (!string.IsNullOrEmpty(errors)) Console.WriteLine(errors);
                    }
                }
                else if (line.StartsWith("printnnl(") && line.EndsWith(");"))
                {
                    string arg = line.Substring(0, line.Length - 2);
                    arg = AfterChar(arg, "printnnl(");
                    dynamic result = GetValue(arg, line);
                    if (result is bool) result = result.ToString().ToUpper();
                    else result = result?.ToString() ?? "NULL";
                    Console.Write(result);
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
            if (val == "NL") return Environment.NewLine;
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
            if (val.StartsWith("ToNum(") && val.EndsWith(")"))
            {
                string arg = "";
                arg = val.Substring(6, val.Length - 7);
                dynamic result = 0.0m;
                result = Convert.ToDecimal(GetValue(arg, line));
                if (result is not decimal)
                {
                    Console.WriteLine($"Cannot convert to number, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                return result;
            }
            if (val.StartsWith("ToString(") && val.EndsWith(")"))
            {
                string arg = val.Substring(9, val.Length - 10);
                var result = GetValue(arg, line);
                if (result is bool) result = result.ToString().ToUpper();
                return result?.ToString() ?? "NULL";
            }
            if (val.StartsWith("ReadLine(") && val.EndsWith(")"))
            {
                string IN = Console.ReadLine();
                return IN;
            }
            if (val.StartsWith("ReadKey(") && val.EndsWith(')'))
            {
                string arg = val.Substring(8, val.Length - 9);
                bool ARG = false;
                try
                {
                    ARG = (bool)GetValue(arg, line);
                }
                catch
                {
                    Console.WriteLine($"Expected a bool Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                ConsoleKeyInfo IN = new();
                if (ARG == true) IN = Console.ReadKey(true);
                else IN = Console.ReadKey(false);
                string CHAR = IN.KeyChar.ToString();
                if (CHAR == "\r") CHAR = Environment.NewLine;
                return CHAR;
            }
            if (val.StartsWith(">(") && val.EndsWith(")"))
            {
                string[] VALS = val.Substring(2, val.Length - 3).Split(',');
                if (VALS.Length != 2)
                {
                    Console.WriteLine($"Need 2 parameters to compare, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                decimal[] decimals = new decimal[2] { 0, 0 };
                for (int i = 0; i < 2; i++)
                {
                    var Var = GetValue(VALS[i], line);
                    if (Var is not decimal)
                    {
                        Console.WriteLine($"Cannot compare non-numeric value, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                    else
                    {
                        decimals[i] = (decimal)Var;
                    }
                }
                return decimals[0] > decimals[1];
            }
            if (val.StartsWith("and(") && val.EndsWith(")"))
            {
                string[] VALS = val.Substring(4, val.Length - 5).Split(',');
                if (VALS.Length != 2)
                {
                    Console.WriteLine($"Need 2 parameters to evaluate, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                bool[] bools = new bool[2] { false, false };

                for (int i = 0; i < 2; i++)
                {
                    var Var = GetValue(VALS[i], line);
                    if (Var is not bool)
                    {
                        Console.WriteLine($"Cannot evaluate non-boolean value, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                    else
                    {
                        bools[i] = (bool)Var;
                    }
                }
                return bools[0] && bools[1];
            }
            if (val.StartsWith("or(") && val.EndsWith(")"))
            {
                string[] VALS = val.Substring(3, val.Length - 4).Split(',');
                if (VALS.Length != 2)
                {
                    Console.WriteLine($"Need 2 parameters to evaluate, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                bool[] bools = new bool[2] { false, false };

                for (int i = 0; i < 2; i++)
                {
                    var Var = GetValue(VALS[i], line);
                    if (Var is not bool)
                    {
                        Console.WriteLine($"Cannot evaluate non-boolean value, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                    else
                    {
                        bools[i] = (bool)Var;
                    }
                }
                return bools[0] || bools[1];
            }
            if (val.StartsWith("<(") && val.EndsWith(")"))
            {
                string[] VALS = val.Substring(2, val.Length - 3).Split(',');
                if (VALS.Length != 2)
                {
                    Console.WriteLine($"Need 2 parameters to compare, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                decimal[] decimals = new decimal[2] { 0, 0};
                for (int i = 0; i < 2; i++)
                {
                    var Var = GetValue(VALS[i], line);
                    if (Var is not decimal)
                    {
                        Console.WriteLine($"Cannot compare non-numeric value, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                    else
                    {
                        decimals[i] = (decimal)Var;
                    }
                }
                return decimals[0] < decimals[1];
            }
            if (val.StartsWith("Slen(") && val.EndsWith(")"))
            {
                string arg = val.Substring(5, val.Length - 6);
                string? Str = " ";
                try
                {
                    Str = GetValue(arg, line);
                }
                catch
                {
                    Console.WriteLine($"Expected a string, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                return Str.Length;
            }
            if (val.StartsWith("charat(") && val.EndsWith(")"))
            {
                string[] VALS = val.Substring(7, val.Length - 8).Split(',');
                if (VALS.Length != 2)
                {
                    Console.WriteLine($"Need 2 parameters to get character, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                string? Val = " ";
                int index = 0;
                try
                {
                    Val = GetValue(VALS[0], line);
                }
                catch
                {
                    Console.WriteLine($"Expected string for first parameter, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                try
                {
                    index = Convert.ToInt32(GetValue(VALS[1], line));
                }
                catch
                {
                    Console.WriteLine($"Expected a whole number for second parameter, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                if (index <0)
                {
                    Console.WriteLine($"Expected a whole number for second parameter, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                try
                {
                    return Val[index].ToString();
                }
                catch
                {
                    Console.WriteLine($"Index out of range, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
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
                return (dynamic?)dynamics[0] == (dynamic?)dynamics[1];
            }
            if (val.StartsWith("!=(") && val.EndsWith(")"))
            {
                string[] VALS = val.Substring(3, val.Length - 4).Split(',');
                if (VALS.Length != 2)
                {
                    Console.WriteLine($"Need 2 parameters to compare, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                dynamic[] dynamics = new dynamic[2] { GetValue(VALS[0], line), GetValue(VALS[1], line) };
                return (dynamic?)dynamics[0] != (dynamic?)dynamics[1];
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