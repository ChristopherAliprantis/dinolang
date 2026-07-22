using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace dinolang.interpreter;

public partial class Interpreter
{
    public static System.ValueTuple<int, dynamic?> ProcessLoop(List<string> lines, bool infunc)
    {
        dynamic? secondthing = ('n', 'r');
        int firstthing = 1;
        List<string> loopLines = new();
        string cond = "";
        List<string> IfLines = new();
        List<Variable> LVs = new();
        bool IF = false;
        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            if (line.StartsWith("L:"))
            {
                lines[i] = AfterChar(lines[i], "L:");
                Globals.dline = lines[i];
                line = lines[i];
            }
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
                bool COND = false;
                try
                {
                    COND = (bool)GetValue(cond, line);
                }
                catch
                {
                    Console.WriteLine($"Invalid Condition {cond}, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                dynamic? thing = (7, 7);
                if (COND == true) thing = ProcessIf(IfLines, infunc, true);
                if (thing is System.ValueTuple<int, int>)
                {

                }
                else
                {
                    if (thing == 0)
                    {
                        firstthing = 0;
                        break;
                    }
                    else
                    {
                        firstthing = 1;
                        break;
                    }
                }
                IfLines.Clear();
            }
            else if (IF) IfLines.Add(line);
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
            else if (line.StartsWith("WriteToFile(") && line.EndsWith(");"))
            {
                string arg = line.Substring(12, line.Length - 14);
                string[] ARGS = arg.Split(',');
                int h = 0;
                if (ARGS.Length != 2)
                {
                    Console.WriteLine($"Expected 2 parameters, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                foreach (string a in ARGS)
                {
                    try
                    {
                        ARGS[h] = GetValue(a, line);
                    }
                    catch
                    {
                        Console.WriteLine($"Expected string, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                    h++;
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
                string arg = line.Substring(11, line.Length - 13);
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
            else if (line.StartsWith("printnnl(") && line.EndsWith(");"))
            {
                string arg = line.Substring(0, line.Length - 2);
                arg = AfterChar(arg, "printnnl(");
                dynamic result = GetValue(arg, line);
                if (result is bool) result = result.ToString().ToUpper();
                else result = result?.ToString() ?? "NULL";
                if (Globals.TEXTbackgroundcolor == null) Console.Write(result);
                if (Globals.TEXTbackgroundcolor != null) Console.Write($"\x1b[48;2;{Globals.TEXTbackgroundcolor[0]};{Globals.TEXTbackgroundcolor[1]};{Globals.TEXTbackgroundcolor[2]}m{result}\x1b[0m");


            }
            else if (line.StartsWith("print(") && line.EndsWith(");"))
            {
                string arg = line.Substring(0, line.Length - 2);
                arg = AfterChar(arg, "print(");
                dynamic result = GetValue(arg, line);
                if (result is bool) result = result.ToString().ToUpper();
                else result = result?.ToString() ?? "NULL";
                if (Globals.TEXTbackgroundcolor == null) Console.WriteLine(result);
                if (Globals.TEXTbackgroundcolor != null) Console.WriteLine($"\x1b[48;2;{Globals.TEXTbackgroundcolor[0]};{Globals.TEXTbackgroundcolor[1]};{Globals.TEXTbackgroundcolor[2]}m{result}\x1b[0m");
            }
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
            else if (line.StartsWith("printnnlC(") && line.EndsWith(");"))
            {
                string arg = line.Substring(0, line.Length - 2);
                arg = AfterChar(arg, "printnnlC(");
                string[] argSS = arg.Split(',');
                dynamic result = GetValue(argSS[0], line);
                byte[] color = new byte[3];
                for (int I = 1; I < argSS.Length; I++)
                {
                    try
                    {
                        color[I - 1] = Convert.ToByte(GetValue(argSS[I], line));
                    }
                    catch
                    {
                        Console.WriteLine($"Expected positive num that is an integer and within the limits of 0-255, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                }
                if (result is bool) result = result.ToString().ToUpper();
                else result = result?.ToString() ?? "NULL";
                if (Globals.TEXTbackgroundcolor == null) Console.Write($"\x1b[38;2;{color[0]};{color[1]};{color[2]}m{result}\x1b[0m");
                else Console.Write($"\x1b[38;2;{color[0]};{color[1]};{color[2]};48;2;{Globals.TEXTbackgroundcolor[0]};{Globals.TEXTbackgroundcolor[1]};{Globals.TEXTbackgroundcolor[2]}m{result}\x1b[0m");

            }
            else if (line.StartsWith("printC(") && line.EndsWith(");"))
            {
                string arg = line.Substring(0, line.Length - 2);
                arg = AfterChar(arg, "printC(");
                string[] argSS = arg.Split(',');
                dynamic result = GetValue(argSS[0], line);
                byte[] color = new byte[3];
                for (int I = 1; I < argSS.Length; I++)
                {
                    try
                    {
                        color[I - 1] = Convert.ToByte(GetValue(argSS[I], line));
                    }
                    catch
                    {
                        Console.WriteLine($"Expected positive num that is an integer and within the limits of 0-255, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                }
                if (result is bool) result = result.ToString().ToUpper();
                else result = result?.ToString() ?? "NULL";
                if (Globals.TEXTbackgroundcolor == null) Console.WriteLine($"\x1b[38;2;{color[0]};{color[1]};{color[2]}m{result}\x1b[0m");
                else Console.WriteLine($"\x1b[38;2;{color[0]};{color[1]};{color[2]};48;2;{Globals.TEXTbackgroundcolor[0]};{Globals.TEXTbackgroundcolor[1]};{Globals.TEXTbackgroundcolor[2]}m{result}\x1b[0m");
            }
            else if (line.StartsWith("setBGColor(") && line.EndsWith(");"))
            {
                string arg = line.Substring(0, line.Length - 2);
                arg = AfterChar(arg, "setBGColor(");
                string[] argSS = arg.Split(',');
                if (argSS.Length != 3)
                {
                    Console.WriteLine($"Expected 3 parameters, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                sbyte[] color = new sbyte[3];

                for (int I = 0; I < argSS.Length; I++)
                {
                    try
                    {
                        color[I] = Convert.ToSByte(GetValue(argSS[I], line));
                    }
                    catch
                    {
                        Console.WriteLine($"Expected positive num that is an integer and within the limits of 0-255 or just -1, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                        Environment.Exit(1);
                    }
                }
                if (color is [-1, -1, -1]) Globals.TEXTbackgroundcolor = null;
                else if (Array.Exists(color, x => x < 0))
                {
                    Console.WriteLine($"Expected positive num that is an integer and within the limits of 0-255 or just -1, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                else Globals.TEXTbackgroundcolor = (byte[])(Array)color;
            }
            else if (line == "continue;")
            {
                firstthing = 1;
                break;
            }

            else if (line == "break;")
            {
                firstthing = 0;
                break;
            }
            else if ((line.Contains('=')) && (BeforeChar(line, '=').Length > 0) && (AfterChar(line, '=').Length > 1))
            {
                bool value = false;
                var b = BeforeChar(line, '=');
                var a = BeforeChar(AfterChar(line, $"{b}="), ';');
                if (a.StartsWith("(R!)"))
                {
                    value = true;
                }
                if (Globals.Vars.ContainsKey(b) && Globals.Vars[b].RO == true)
                {
                    Console.WriteLine($"Invalid Value, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                if (!Globals.Vars.ContainsKey(b))
                {
                    Console.WriteLine($"Invalid Value, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                dinolang.interpreter.Globals.Vars[b] = new Variable
                {
                    value = GetValue(a, line),
                    name = b,
                    RO = value
                };
                if (dinolang.interpreter.Globals.Vars[b].value is string) dinolang.interpreter.Globals.Vars[b].type = "string";
                else if (dinolang.interpreter.Globals.Vars[b].value is decimal) dinolang.interpreter.Globals.Vars[b].type = "num";
                else if (dinolang.interpreter.Globals.Vars[b].value is bool) dinolang.interpreter.Globals.Vars[b].type = "bool";
                else if (dinolang.interpreter.Globals.Vars[b].value is null) dinolang.interpreter.Globals.Vars[b].type = "null";
            }
            else if (infunc && (line.StartsWith("return(") && line.EndsWith(");")))
            {
                string arg = BeforeChar(AfterChar(line, '('), ");");
                var th = GetValue(arg, line);
                firstthing = 0;
                secondthing = th;
                break;
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
                    ProcessFunc(Globals.Funcs[fname], argsS, $"{fname}({string.Join(", ", Globals.Funcs[fname].parameters)})", line);
                }
                else
                {
                    Console.WriteLine($"Function {fname} not found Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
            }
            else
            {
                Console.WriteLine($"Invalid Code, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                Environment.Exit(1);
            }
        }
        foreach (Variable v in LVs)
        {
            dinolang.interpreter.Globals.Vars.Remove(v.name);
        }
        LVs.Clear();
        return new System.ValueTuple<int, dynamic?>
        {
            Item1 = firstthing,
            Item2 = secondthing
        };

    }
}