using System;
using System.Collections.Generic;
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
            if (!lines.Any(item => item.StartsWith("return")))
            {
                Console.WriteLine($"Function {name} doesn't return anything Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                Environment.Exit(1);
            }
            if (vals.Count != p.Count)
            {
                Console.WriteLine($"Function {name} expects {p.Count} arguments but got {vals.Count} arguments Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
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
            string varto = "";
            List<string> loopLines = new();
            decimal times = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line.StartsWith("#loop"))
                {
                    if (AfterChar(line, "#loop") != ";")
                    {
                        string thing = BeforeChar(AfterChar(line, "#loop"), ';');
                        string args = AfterChar(BeforeChar(thing, ')'), '(');
                        string[] argsl = args.Split(',');
                        if (argsl.Length != 2)
                        {
                            Console.WriteLine($"Invalid Loop declaration, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                            Environment.Exit(1);
                        }
                        try
                        {
                            times = GetValue(argsl[0], line);
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
                        varto = argsl[1];
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
                        if (l != 0) Interpret(new List<string> { $"{varto}=+({varto},1);" });
                        int st = ProcessLoop(loopLines, Convert.ToInt32(times), varto);
                        if (st == 0) break;
                    }
                    POL = false;
                    times = 0;
                    varto = "";
                    loopLines.Clear();
                }
                else if (POL == true) loopLines.Add(line);
                else if (line.StartsWith("print(") && line.EndsWith(");"))
                {
                    string arg = line.Substring(0, line.Length - 2);
                    arg = AfterChar(arg, '(');
                    Console.WriteLine(GetValue(arg, line));
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
                }
                else if (line.StartsWith("return(") && line.EndsWith(");"))
                {
                    string arg = BeforeChar(AfterChar(line, '('), ");");
                    var th = GetValue(arg, line);
                    RestoreDI(Nvsk, Nvs);

                    return th;
                }
                else if (line.Contains("(") && line.EndsWith(");"))
                {
                    GetValue(BeforeChar(line, ';'), line);
                }
                else
                {
                    Console.WriteLine($"Invalid Code, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
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
