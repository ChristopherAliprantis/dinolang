using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace dinolang.interpreter;

public partial class Interpreter
{
    public static int ProcessLoop(List<string> lines)
    {
        List<string> loopLines = new();
        string?cond = "";
        List<string> IfLines = new();
        bool? IF = false;
        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            
            if (line.StartsWith("#if"))
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
                bool? COND = false;
                try
                {
                    COND = (bool?)GetValue(cond, lines, i);
                }
                catch
                {
                    Console.WriteLine($"Invalid Condition, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                dynamic? thing = (7, 7);
                if (COND.Value) thing = ProcessIf(IfLines, false, true);
                if (thing is System.ValueTuple<int, int>)
                {

                }
                else
                {
                    if (thing == 0) return 0;
                    else return 1;
                }
            }
            else if (IF.Value) IfLines.Add(line);
            else if (line.StartsWith("print(") && line.EndsWith(");"))
            {
                string?arg = line.Substring(0, line.Length - 2);
                arg = AfterChar(arg, "print(");
                dynamic result = GetValue(arg, lines, i);
                if (result is bool?) result = result.ToString().ToUpper();
                else result = result?.ToString() ?? "NULL";
                Console.WriteLine(result);
            }
            else if (line.StartsWith("clr(") && line.EndsWith(");"))
            {
                Console.Clear();
            }
            else if (line.StartsWith("Exit(") && line.EndsWith(");"))
            {
                string?arg = line.Substring(5, line.Length - 7);
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
            else if (line.StartsWith("WriteToFile(") && line.EndsWith(");"))
            {
                string?arg = line.Substring(12, line.Length - 13);
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
                string?arg = line.Substring(11, line.Length - 12);
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
                string[] vals = System.Linq.Enumerable.Cast<string>(VALS).ToArray();
                if (Directory.Exists(vals[0]))
                {
                    string?fullpath = Path.Combine(vals[0], vals[1]);
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
                string?arg = line.Substring(13, line.Length - 15);
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
                string[] vals = System.Linq.Enumerable.Cast<string>(VALS).ToArray();
                if (Directory.Exists(vals[0]))
                {
                    string?fullpath = Path.Combine(vals[0], vals[1]);
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
                string?arg = line.Substring(11, line.Length - 13);
                try
                {
                    arg = GetValue(arg, lines, i);
                }
                catch
                {
                    Console.WriteLine($"Expected a string?as the path, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
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
                string?arg = line.Substring(5, line.Length - 7);
                decimal? delay = 0.0m;
                try
                {
                    delay = (decimal?)GetValue(arg, lines, i);
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
                    string?output = process.StandardOutput.ReadToEnd();
                    string?errors = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(output)) Console.WriteLine(output);
                    if (!string.IsNullOrEmpty(errors)) Console.WriteLine(errors);
                }
            }
            else if (line.StartsWith("printnnl(") && line.EndsWith(");"))
            {
                string?arg = line.Substring(0, line.Length - 2);
                arg = AfterChar(arg, "printnnl(");
                dynamic result = GetValue(arg, lines, i);
                if (result is bool?) result = result.ToString().ToUpper();
                else result = result?.ToString() ?? "NULL";
                Console.Write(result);
            }
            else if (line == "continue;")
            {
                break;
            }

            else if (line == "break;") return 0;
            else if (line.StartsWith($"{BeforeChar(line, '=')}="))
            {
                var b = BeforeChar(line, '=');
                var a = BeforeChar(AfterChar(line, '='), ';');
                dinolang.interpreter.Globals.Vars[b] = new Variable
                {
                    value = GetValue(a, lines, i),
                };
                if (dinolang.interpreter.Globals.Vars[b].value is string) dinolang.interpreter.Globals.Vars[b].type = "string";
                else if (dinolang.interpreter.Globals.Vars[b].value is decimal?) dinolang.interpreter.Globals.Vars[b].type = "num";
                else if (dinolang.interpreter.Globals.Vars[b].value is bool?) dinolang.interpreter.Globals.Vars[b].type = "bool";
                else if (dinolang.interpreter.Globals.Vars[b].value is null) dinolang.interpreter.Globals.Vars[b].type = "null";
            }
            else if (line.Contains("(") && line.EndsWith(");"))
            {
                var val = BeforeChar(line, ';');
                string?fname = BeforeChar(val, '(');
                if (Globals.Funcs.ContainsKey(fname))
                {
                    string?inside = BeforeChar(AfterChar(val, $"{fname}("), ')');

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
        return 1;
    }

}