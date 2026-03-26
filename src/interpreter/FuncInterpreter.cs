using System;
using System.Collections.Generic;
using System.Text;

namespace dinolang.interpreter
{
    public partial class Interpreter
    {
        public static dynamic? ProcessFunc(Function func, int Line, List<dynamic> vals)
        {
            var lines = func.code;
            var p = func.parameters;
            if (lines.Any(item => item.StartsWith("return") == false)) 
            {
                Console.WriteLine($"Function does not return anything, Line {Line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                Environment.Exit(1);
            }
            if (p.Count != 0)
            {
                for (int i = 0; i < p.Count; i++)
                {
                    {
                        lines.Insert(0, $"{p[i]}={vals[i]};");
                    }
                }
            }
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line.StartsWith("print(") && line.EndsWith(");"))
                {
                    string arg = BeforeChar(AfterChar(line, '('), ')');
                    Console.WriteLine(GetValue(arg, i + 1, null));
                }
                else if (Globals.Funcs.ContainsKey(BeforeChar(line, '(')))
                {
                    var f = Globals.Funcs[BeforeChar(line, '(')];
                    var P = BeforeChar(AfterChar(line, '('), ')');
                    var sP = P.Split(',').ToList();
                    List<dynamic> dP = new();
                    for (int h = 0; h < sP.Count; h++)
                    {
                        dP.Add(GetValue(sP[h], i+1, null));
                    }
                    GetValue(line, i+1, dP);
                }
                else if (line.StartsWith($"{BeforeChar(line, '=')}="))
                {
                    var b = BeforeChar(line, '=');
                    var a = BeforeChar(AfterChar(line, '='), ';');
                    dinolang.interpreter.Globals.Vars[b] = new Variable
                    {
                        value = GetValue(a, i + 1, null),
                    };
                    if (dinolang.interpreter.Globals.Vars[b].value is string) dinolang.interpreter.Globals.Vars[b].type = "string";
                    else if (dinolang.interpreter.Globals.Vars[b].value is decimal) dinolang.interpreter.Globals.Vars[b].type = "num";
                }
                else if (line.StartsWith("return(") && line.EndsWith(");"))
                {
                    string arg = BeforeChar(AfterChar(line, '('), ')');
                    return GetValue(arg, i + 1, null);
                }
                else
                {
                    Console.WriteLine($"Invalid Code, Line {i + 1} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
            }
            Console.WriteLine($"Invalid Value, Line {Line}.");
            Environment.Exit(1);
            return null;
        }
    }
}
