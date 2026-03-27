using System;
using System.Collections.Generic;
using System.Text;

namespace dinolang.interpreter
{
    public partial class Interpreter
    {
        public static dynamic? ProcessFunc(Function func, int Line, List<dynamic> vals)
        {
            var lines = new List<string>(func.code);
            var p = func.parameters;
            if (!lines.Any(item => item.StartsWith("return")))
            {
                Console.WriteLine($"Function does not return anything, Line {Line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                Environment.Exit(1);
            }
            var Nvs = new Dictionary<string, Variable>();
            var Nvsk = new List<string>();
            if (p.Count != 0)
            {
                for (int i = 0; i < p.Count; i++)
                {
                    {
                        string val = vals[i];
                        if (val is string) val = $":{val}:";
                        lines.Insert(0, $"{p[i]}={val};");
                        if (Globals.Vars.ContainsKey(p[i]))
                        {
                            Nvs[p[i]] = Globals.Vars[p[i]];
                            Nvsk.Add(p[i]);
                        }
                    }
                }
            }
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line.StartsWith("print(") && line.EndsWith(");"))
                {
                    string arg = BeforeChar(AfterChar(line, '('), ')');
                    if (string.IsNullOrWhiteSpace(arg)) arg = "::";
                    Console.WriteLine(GetValue(arg, Line, null));
                }
                else if (line.StartsWith($"{BeforeChar(line, '=')}="))
                {
                    var b = BeforeChar(line, '=');
                    var a = BeforeChar(AfterChar(line, '='), ';');
                    dinolang.interpreter.Globals.Vars[b] = new Variable
                    {
                        value = GetValue(a, Line, null),
                    };
                    if (dinolang.interpreter.Globals.Vars[b].value is string) dinolang.interpreter.Globals.Vars[b].type = "string";
                    else if (dinolang.interpreter.Globals.Vars[b].value is decimal) dinolang.interpreter.Globals.Vars[b].type = "num";
                }
                else if (line.StartsWith("return(") && line.EndsWith(");"))
                {
                    string arg = BeforeChar(AfterChar(line, '('), ')');
                    Console.WriteLine(arg);
                    var th = GetValue(arg, Line, null);
                    RestoreDI(Nvsk, Nvs);
                    
                    return th;
                }
                else if (line.Contains("(") && line.EndsWith(");"))
                {
                    GetValue(BeforeChar(line, ';'), i + 1, null);
                }
                else
                {
                    Console.WriteLine($"Invalid Code, Line {Line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
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
