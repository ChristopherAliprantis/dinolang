using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Timers;

namespace dinolang.interpreter
{
    public partial class Interpreter
    {
        public static dynamic? ProcessFunc(Function func, List<dynamic> vals, string name)
        {
            var lines = new List<string>(func.code);
            var p = func.parameters;
            if (vals.Count != p.Count)
            {
                Console.WriteLine($"Function {name} expects {p.Count} argument(s) but got {vals.Count} argument(s) Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                Environment.Exit(1);
            }

            var Nvs = new Dictionary<string, Variable>();
            var Nvsk = new List<string>();
            for (int i = 0; i < p.Count; i++)
            {
                {
                    string val = vals[i];
                    lines.Insert(0, $"{p[i]}={val};");
                    if (Globals.Vars.ContainsKey(p[i]))
                    {
                        Nvs[p[i]] = Globals.Vars[p[i]];
                        Nvsk.Add(p[i]);
                    }
                }
            }
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
                if (i == lines.Count - 1 && !line.StartsWith("return"))
                {
                    Console.WriteLine($"Function {name} doesn't return anything Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                else if (line.StartsWith("#loop"))
                {
                    if (AfterChar(line, "#loop") != ";")
                    {
                        args = AfterChar(BeforeChar(line, ");"), "#loop(");
                        try
                        {
                            times = GetValue(args, lines, i);
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
                            (int, dynamic?) st = ProcessLoop(loopLines, true);
                            if (st.Item2 is System.ValueTuple<char, char>)
                            {

                            }
                            else
                            {
                                return st.Item2;
                            }
                            if (st.Item1 == 0) break;
                        }
                    }
                    else
                    {
                        while (true)
                        {
                            if ((bool)GetValue(args, lines, i))
                            {
                                (int, dynamic?) st = ProcessLoop(loopLines, true);
                                if (st.Item2 == (System.ValueTuple<char, char>)('n', 'r'))
                                {
                                    
                                }
                                else
                                {
                                    return st.Item2;
                                }
                                if (st.Item1 == 0) break;
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
                        COND = (bool)GetValue(cond, lines, i);
                    }
                    catch
                    {
                        Console.WriteLine($"Invalid Condition {cond} {cond}, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                    dynamic? thing = (7, 7);
                    if (COND) thing = ProcessIf(IfLines, true, false);
                    if (thing is System.ValueTuple<int, int>)
                    {

                    }
                    else
                    {
                        RestoreDI(Nvsk, Nvs);
                        return thing;
                    }
                }
                else if (IF) IfLines.Add(line);
                else if (line.StartsWith("Exit(") && line.EndsWith(");"))
                {
                    string arg = line.Substring(5, line.Length - 7);
                    int code = 0;
                    try
                    {
                        code = Convert.ToInt32(GetValue(arg, lines, i));

                    }
                    catch
                    {
                        Console.WriteLine($"Expected a num, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                    Environment.Exit(code);
                }
                else if (line.StartsWith("clr(") && line.EndsWith(");"))
                {
                    Console.Clear();
                }
                else if (line.StartsWith("print(") && line.EndsWith(");"))
                {
                    string arg = line.Substring(0, line.Length - 2);
                    arg = AfterChar(arg, "print(");
                    dynamic result = GetValue(arg, lines, i);
                    if (result is bool) result = result.ToString().ToUpper();
                    else result = result?.ToString() ?? "NULL";
                    Console.WriteLine(result);
                }
                else if (line.StartsWith("printnnl(") && line.EndsWith(");"))
                {
                    string arg = line.Substring(0, line.Length - 2);
                    arg = AfterChar(arg, "printnnl(");
                    dynamic result = GetValue(arg, lines, i);
                    if (result is bool) result = result.ToString().ToUpper();
                    else result = result?.ToString() ?? "NULL";
                    Console.Write(result);
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
                        VALS[a] = GetValue(ARGS[a], lines, i);
                        if (VALS[a] is not string)
                        {
                            Console.WriteLine($"Expected string, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                            Environment.Exit(1);
                        }
                    }
                    string[] val = System.Linq.Enumerable.Cast<string>(VALS).ToArray();
                    if (Directory.Exists(vals[0]))
                    {
                        string fullpath = Path.Combine(val[0], val[1]);
                        File.WriteAllText(fullpath, "");
                    }
                    else
                    {
                        Console.WriteLine($"Directory {val[0]} not found, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
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
                        VALS[a] = GetValue(ARGS[a], lines, i);
                        if (VALS[a] is not string)
                        {
                            Console.WriteLine($"Expected string, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                            Environment.Exit(1);
                        }
                    }
                    string[] val = System.Linq.Enumerable.Cast<string>(VALS).ToArray();
                    if (Directory.Exists(vals[0]))
                    {
                        string fullpath = Path.Combine(val[0], val[1]);
                        Directory.CreateDirectory(fullpath);
                    }
                    else
                    {
                        Console.WriteLine($"Directory {val[0]} not found, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                }
                else if (line.StartsWith("DeleteItem(") && line.EndsWith(");"))
                {
                    string arg = line.Substring(11, line.Length - 13);
                    try
                    {
                        arg = GetValue(arg, lines, i);
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
                else if (line.StartsWith("wait(") && line.EndsWith(");"))
                {
                    string arg = line.Substring(5, line.Length - 7);
                    decimal delay = 0.0m;
                    try
                    {
                        delay = (decimal)GetValue(arg, lines, i);
                    }
                    catch
                    {
                        Console.WriteLine($"Expected num, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds((double)delay));
                }
                else if (line.StartsWith("PowershellCall(") && line.EndsWith(");"))
                {
                    var arg = line.Substring(15, line.Length - 17);
                    dynamic arg2 = GetValue(arg, lines, i);
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
                else if (line.Contains('=') && BeforeChar(line, '=').Length > 0 && AfterChar(line, '=').Length > 1)
                {
                    var b = BeforeChar(line, '=');
                    var a = BeforeChar(AfterChar(line, '='), ';');
                    dinolang.interpreter.Globals.Vars[b] = new Variable
                    {
                        value = GetValue(a, lines, i),
                    };
                    if (dinolang.interpreter.Globals.Vars[b].value is string) dinolang.interpreter.Globals.Vars[b].type = "string";
                    else if (dinolang.interpreter.Globals.Vars[b].value is decimal) dinolang.interpreter.Globals.Vars[b].type = "num";
                    else if (dinolang.interpreter.Globals.Vars[b].value is bool) dinolang.interpreter.Globals.Vars[b].type = "bool";
                    else if (dinolang.interpreter.Globals.Vars[b].value is null) dinolang.interpreter.Globals.Vars[b].type = "null";
                }
                else if (line.StartsWith("return(") && line.EndsWith(");"))
                {
                    string arg = BeforeChar(AfterChar(line, '('), ");");
                    var th = GetValue(arg, lines, i);
                    RestoreDI(Nvsk, Nvs);

                    return th;
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
                        ProcessFunc(Globals.Funcs[fname], argsS, line);
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid Code, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                Globals.ExecutedLines.Add(line);
            }


            return null;
        }

        public static void RestoreDI(List<string> it, Dictionary<string, Variable> from)
        {
            for (int i = 0; i < it.Count; i++)
            {
                Globals.Vars[it[i]] = from[it[i]];
            }
        }
    }
}
