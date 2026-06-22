using CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Markup;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
namespace dinolang.interpreter
{
    public partial class Interpreter
    {
        public static void Interpret(List<string> lines)
        {
            List<string> newlines = new();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//") || line == ";") continue;

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
            bool c = false;
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
                    else if (Ps.StartsWith('(') && Ps.EndsWith(")a"))
                    {
                        Ps = Ps.Substring(1, Ps.Length - 3);
                        c = true;
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
                        code = new List<string>(mfl),
                        command = c
                    };
                    //Console.WriteLine($"Function {name} created");
                    mf = false;
                    mfl.Clear();
                    mfp.Clear();
                    name = "";
                    c = false;
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
                            int st = ProcessLoop(loopLines, false).Item1;
                            if (st == 0) break;
                        }
                    }
                    else
                    {
                        while (true)
                        {
                            if ((bool)GetValue(args, line))
                            {
                                int st = ProcessLoop(loopLines, false).Item1;
                                if (st == 0) break;
                            }
                            else break;
                        }
                    }
                    POL = false;
                    times = 0;
                    loopLines.Clear();
                }
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
                        Console.WriteLine($"Invalid Condition {cond} {cond} {cond}, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }

                }
                else if (POL == true) loopLines.Add(line);
                else if (line.StartsWith("wait(") && line.EndsWith(");"))
                {
                    string arg = line.Substring(5, line.Length - 7);
                    decimal delay = 0.0m;
                    try
                    {
                        delay = (decimal)GetValue(arg, line);
                    }
                    catch
                    {
                        Console.WriteLine($"Expected num, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds((double)delay));
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
                else if (line.StartsWith("WriteToFile(") && line.EndsWith(");"))
                {
                    string arg = line.Substring(12, line.Length - 13);
                    string[] ARGS = arg.Split(',');

                    if (ARGS.Length != 2)
                    {
                        Console.WriteLine($"Expected 2 parameters, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                    if (File.Exists(ARGS[0]))
                    {
                        File.WriteAllText(ARGS[0], ARGS[1]);
                    }
                    else
                    {
                        Console.WriteLine($"File {ARGS[0]} not found, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                }
                else if (line.StartsWith("CreateFile(") && line.EndsWith(");"))
                {
                    string arg = line.Substring(11, line.Length - 12);
                    string[] ARGS = arg.Split(',');
                    if (ARGS.Length != 2)
                    {
                        Console.WriteLine($"Expected 2 parameters, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                    dynamic[] VALS = new dynamic[2];
                    for (int a = 0; a < 2; a++)
                    {
                        VALS[a] = GetValue(ARGS[a], line);
                        if (VALS[a] is not string)
                        {
                            Console.WriteLine($"Expected string, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                            Environment.Exit(1);
                        }
                    }
                    string[] vals = System.Linq.Enumerable.Cast<string>(VALS).ToArray();
                    if (Directory.Exists(vals[0]))
                    {
                        string fullpath = Path.Combine(vals[0], vals[1]);
                        File.WriteAllText(fullpath, "");
                    }
                    else
                    {
                        Console.WriteLine($"Directory {vals[0]} not found, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                }
                else if (line.StartsWith("CreateFolder(") && line.EndsWith(");"))
                {
                    string arg = line.Substring(13, line.Length - 15);
                    string[] ARGS = arg.Split(',');
                    if (ARGS.Length != 2)
                    {
                        Console.WriteLine($"Expected 2 parameters, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                    dynamic[] VALS = new dynamic[2];
                    for (int a = 0; a < 2; a++)
                    {
                        VALS[a] = GetValue(ARGS[a], line);
                        if (VALS[a] is not string)
                        {
                            Console.WriteLine($"Expected string, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                            Environment.Exit(1);
                        }
                    }
                    string[] vals = System.Linq.Enumerable.Cast<string>(VALS).ToArray();
                    if (Directory.Exists(vals[0]))
                    {
                        string fullpath = Path.Combine(vals[0], vals[1]);
                        Directory.CreateDirectory(fullpath);
                    }
                    else
                    {
                        Console.WriteLine($"Directory {vals[0]} not found, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                }
                else if (line.StartsWith("DeleteItem(") && line.EndsWith(");"))
                {
                    string arg = line.Substring(11, line.Length - 13);
                    try
                    {
                        arg = GetValue(arg, line);
                    }
                    catch
                    {
                        Console.WriteLine($"Expected a string as the path, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                    if (Directory.Exists(arg))
                    {
                        Directory.Delete(arg, true);
                    }
                    else if (File.Exists(arg))
                    {
                        File.Delete(arg);
                    }
                    else
                    {
                        Console.WriteLine($"Item not found: {arg}, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                }
                else if (line.StartsWith("clr(") && line.EndsWith(");"))
                {
                    Console.Clear();
                }
                else if (line.StartsWith("Exit(") && line.EndsWith(");"))
                {
                    string arg = line.Substring(5, line.Length - 7);
                    int code = 0;
                    try
                    {
                        code = Convert.ToInt32(GetValue(arg, line));

                    }
                    catch
                    {
                        Console.WriteLine($"Expected a num, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                    Environment.Exit(code);
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
                else if ((line.Contains('=')) && (BeforeChar(line, '=').Length > 0) && (AfterChar(line, '=').Length > 1))
                {
                    var b = BeforeChar(line, '=');
                    var a = BeforeChar(AfterChar(line, $"{b}="), ';');
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
                    var val = BeforeChar(line, ';');
                    string fname = BeforeChar(val, '(');
                    if (Globals.Funcs.ContainsKey(fname))
                    {
                        string inside = BeforeChar(AfterChar(val, $"{fname}("), ')');

                        List<dynamic> argsS = new List<dynamic>(0);

                        if (inside != "")
                        {
                            if (!string.IsNullOrWhiteSpace(inside))
                            {
                                string[] split = inside.Split(',');

                                foreach (var s in split)
                                {
                                    argsS.Add(s);
                                }
                            }
                        }
                        ProcessFunc(Globals.Funcs[fname], argsS, $"{fname}({string.Join(", ", Globals.Funcs[fname].parameters)})");
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid Code, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                Globals.ExecutedLines.Add(line);
            }
        }




        public static dynamic? GetValue(string val, string line)
        { 
            //Console.WriteLine($"[{val}] Length={val.Length}");

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
            if (val == "SC") return ";";
            if (dinolang.interpreter.Globals.Vars.ContainsKey(val))
            {
                return dinolang.interpreter.Globals.Vars[val].value;
            }
            string fname = BeforeChar(val, '(');
            if (Globals.Funcs.ContainsKey(fname))
            {
                if (Globals.Funcs[fname].command == true)
                {
                    Console.WriteLine($"Invalid Value {val}, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                string inside = BeforeChar(AfterChar(val, $"{fname}("), ')');

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
                return ProcessFunc(Globals.Funcs[fname], args, $"{fname}({string.Join(", ", Globals.Funcs[fname].parameters)})");
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
            if (val.StartsWith("GetType(") && val.EndsWith(")"))
            {
                string arg = val.Substring(8, val.Length - 9);
                var result = GetValue(arg, line);
                string type = "";
                if (result is string) type = "string";
                else if (result is decimal) type = "num";
                else if (result is bool) type = "bool";
                else if (result is null) type = "null";
                return type;
            }
            if (val.StartsWith("GetAppdataPath(") && val.EndsWith(")"))
            {
                string localappdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string appdata = Directory.GetParent(localappdata).FullName;
                return appdata;
            }
            if (val.StartsWith("GetContent(") && val.EndsWith(")"))
            {
                string arg = val.Substring(11, val.Length - 12);
                var path = GetValue(arg, line);
                if (path is not string)
                {
                    Console.WriteLine($"Expected a string, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                if (Directory.Exists(path))
                {
                    string[] allPaths = Directory.GetFileSystemEntries(path);
                    string formattedstrpaths = string.Join(Environment.NewLine, allPaths);
                    return formattedstrpaths;
                }
                else if (File.Exists(path))
                {
                    return File.ReadAllText(path);
                }
                else
                {
                    Console.WriteLine($"Expected a folder or file Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
            }
            if (val.StartsWith("ToString(") && val.EndsWith(")"))
            {
                string arg = val.Substring(9, val.Length - 10);
                var result = GetValue(arg, line);
                if (result is bool) result = result.ToString().ToUpper();
                return result?.ToString() ?? "NULL";
            }
            if (val.StartsWith("Line(") && val.EndsWith(")"))
            {
                string arg = val.Substring(5, val.Length - 6);
                int ar = 0;
                try
                {
                    ar = (int)GetValue(arg, line);
                }
                catch
                {
                    Console.WriteLine($"Expected a num that is an integer, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                return Globals.ExecutedLines[(Globals.ExecutedLines.Count - 1) - ar];
            }
            if (val == "ReadLine()")
            {
                string IN = Console.ReadLine();
                return IN;
            }
            if (val.StartsWith("ReadKey(") && val.EndsWith(")"))
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
                else if (CHAR  == "\n") CHAR = Environment.NewLine;
                return CHAR;
            }
            if (val.StartsWith("FD(") && val.EndsWith(")"))
            {
                string args = val.Substring(3, val.Length - 4);
                string[] ARGS = args.Split(',');
                if (ARGS.Length < 2)
                {
                    Console.WriteLine($"Expected 2 or more parameters, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                List<decimal> vals = new List<decimal>();
                for (int i = 0; i < ARGS.Length; i++)
                {
                    try
                    {
                        vals.Add(GetValue(ARGS[i], line));
                    }
                    catch
                    {
                        Console.WriteLine($"Expected num, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                }
                decimal result = vals[0];
                int I = 0;
                foreach (var VAL in vals)
                {
                    if (I == 0)
                    {
                        I++;
                        continue;
                    }
                    result = Math.Floor(result / VAL);
                    I++;
                }
                return result;
            }
            if (val.StartsWith("TimeToString(") && val.EndsWith(')'))
            {
                string arg = val.Substring(13, val.Length - 14);
                decimal t = 0.0m;
                try
                {
                    t = GetValue(arg, line);
                    if (t < 0)
                    {
                        Console.WriteLine($"Expected a positive num Line {line}");
                        Environment.Exit(1);
                    }
                }
                catch
                {
                    Console.WriteLine($"Expected a positive num Line {line}");
                    Environment.Exit(1);
                }
                const decimal usPerDay = 86_400_000_000m;
                const decimal usPerHour = 3_600_000_000m;
                const decimal usPerMinute = 60_000_000m;
                const decimal usPerSecond = 1_000_000m;
                const decimal usPerMillisecond = 1_000m;

                decimal d = Math.Floor(t / usPerDay);
                t %= usPerDay;

                decimal h = Math.Floor(t / usPerHour);
                t %= usPerHour;

                decimal m = Math.Floor(t / usPerMinute);
                t %= usPerMinute;

                decimal s = Math.Floor(t / usPerSecond);
                t %= usPerSecond;

                decimal ms = Math.Floor(t / usPerMillisecond);
                decimal us = t % usPerMillisecond;

                return $"{d}d {h:00}h {m:00}m {s:00}s {ms:000}ms {us:000}µs";
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
            if (val.StartsWith("RoundNum(") && val.EndsWith(")"))
            {
                string arg = val.Substring(9, val.Length - 10);
                string[] args = arg.Split(",");
                if (args.Length != 2)
                {
                    Console.WriteLine($"Expected 2 parameters, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                dynamic[] ARGS = new dynamic[2];
                int i = 0;
                foreach (string ARG in args)
                {
                    try
                    {
                        decimal NEW = 0.0m;
                        int NEW2;
                        if (i == 0) { NEW = GetValue(args[i], line); ARGS[0] = NEW; }
                        else { NEW2 = GetValue(args[i], line); ARGS[1] = NEW2; }
                    }
                    catch
                    {
                        if (i == 0) Console.WriteLine($"Expected num, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        else Console.WriteLine($"Expected integer, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                    i++;
                }
                Math.Round(ARGS[0], ARGS[1], MidpointRounding.AwayFromZero);
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
            if (val.StartsWith("LowerStr(") && val.EndsWith(')'))
            {
                string a = val.Substring(9, val.Length - 10);
                string arg = "";
                try
                {
                    arg = GetValue(a, line);
                }
                catch
                {
                    Console.WriteLine($"Expected string, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                return arg.ToLower();
            }
            if (val.StartsWith("UpperStr(") && val.EndsWith(')'))
            {
                string a = val.Substring(9, val.Length - 10);
                string arg = "";
                try
                {
                    arg = GetValue(a, line);
                }
                catch
                {
                    Console.WriteLine($"Expected string, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                return arg.ToUpper();
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
                int inde = 0;
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
                    inde = Convert.ToInt32(GetValue(VALS[1], line));
                }
                catch
                {
                    Console.WriteLine($"Expected a whole number for second parameter, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                if (inde < 0)
                {
                    Console.WriteLine($"Expected a whole number for second parameter, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                try
                {
                    return Val[inde].ToString();
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
                object?[] dynamics = new object?[2] { GetValue(VALS[0], line), GetValue(VALS[1], line) };
                if (dynamics[0] != null && dynamics[1] != null && dynamics[0].GetType() != dynamics[1].GetType())
                {
                    Console.WriteLine($"Both parameters need to be the same type except if one of the types is null, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                if (dynamics[0] is bool? || dynamics[1] is bool?)
                {
                    return (bool?)dynamics[0] == (bool?)dynamics[1];
                }
                else if (dynamics[0] is decimal? || dynamics[1] is decimal?)
                {
                    return (decimal?)dynamics[0] == (decimal?)dynamics[1];
                }
                else if (dynamics[0] is string or null || dynamics[1] is string or null)
                {
                    return (string?)dynamics[0] == (string?)dynamics[1];
                }
            }
            if (val.StartsWith("!=(") && val.EndsWith(")"))
            {
                string[] VALS = val.Substring(3, val.Length - 4).Split(',');
                if (VALS.Length != 2)
                {
                    Console.WriteLine($"Need 2 parameters to compare, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                object?[] dynamics = new object?[2] { GetValue(VALS[0], line), GetValue(VALS[1], line) };
                if (dynamics[0] != null && dynamics[1] != null && dynamics[0].GetType() != dynamics[1].GetType())
                {
                    Console.WriteLine($"Both parameters need to be the same type except if one of the types is null, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                if (dynamics[0] is bool? || dynamics[1] is bool?)
                {
                    return (bool?)dynamics[0] != (bool?)dynamics[1];
                }
                else if (dynamics[0] is decimal? || dynamics[1] is decimal?)
                {
                    return (decimal?)dynamics[0] != (decimal?)dynamics[1];
                }
                else if (dynamics[0] is string or null || dynamics[1] is string or null)
                {
                    return (string?)dynamics[0] != (string?)dynamics[1];
                }
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
            if (val == "GetN()")
            {
                string arg = val.Substring(4, val.Length - 5);
                long wintime = DateTime.Now.ToFileTimeUtc();
                return GetDinoTime(wintime);
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
            if (val.StartsWith("%(") && val.EndsWith(')'))
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
                        Console.WriteLine($"Invalid Value {val}, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                }
                decimal? result = Vals[0];
                for (int I = 0; I < Vals.Count; I++)
                {
                    if (I == 0) continue;
                    result %= Vals[I];
                }
                return result;
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
                        Console.WriteLine($"Invalid Value {val}, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
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
                        Console.WriteLine($"Invalid Value {val}, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
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
            if (val.StartsWith("RandomInt(") && val.EndsWith(')'))
            {
                string[] VALS = val.Substring(10, val.Length - 11).Split(',');
                if (VALS.Length != 2)
                {
                    Console.WriteLine($"Need 2 parameters to get random integer, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                decimal[] ranges = new decimal[VALS.Length];
                int i = 0;
                foreach (var v in VALS)
                {
                    try
                    {
                        decimal range = GetValue(v, line);
                        if (range % 1 != 0)
                        {
                            Console.WriteLine($"Expected a num that is an integer, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                            Environment.Exit(1);
                        }
                        ranges[i] = range;
                    }
                    catch
                    {
                        Console.WriteLine($"Invalid Value {val}, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                    i++;
                }
                return (decimal)Random.Shared.Next((int)ranges[0], (int)ranges[1]);
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
                        Console.WriteLine($"Invalid Value {val}, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
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
            Console.WriteLine($"Invalid Value {val}, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
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

        public static decimal GetDinoTime(long wintime)
        {
            const long EpochOffsetMicroseconds = 50491123200000000L;

            return (decimal)((wintime / 10L) + EpochOffsetMicroseconds);
        }
    }
}