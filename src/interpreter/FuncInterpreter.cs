using System;
using System.Collections.Generic;
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
                    if (times is decimal || times is int)
                    {
                        if (times is int) times = Convert.ToDecimal((int)times);
                        for (int l = 0; l < Convert.ToInt32((decimal)times); l++)
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
                    dynamic? thing = (7,7);
                    if (COND) thing = ProcessIf(IfLines, true);
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
                else if (line.StartsWith("print(") && line.EndsWith(");"))
                {
                    string arg = line.Substring(0, line.Length - 2);
                    arg = AfterChar(arg, '(');
                    string result = GetValue(arg, line)?.ToString() ?? "NULL";
                    Console.WriteLine(result);
                }
                else if (line.StartsWith("printnnl(") && line.EndsWith(");"))
                {
                    string arg = line.Substring(0, line.Length - 2);
                    arg = AfterChar(arg, '(');
                    string result = GetValue(arg, line)?.ToString() ?? "NULL";
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
