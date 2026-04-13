using System;
using System.Collections.Generic;
using System.Text;

namespace dinolang.interpreter;

public partial class Interpreter
{
    public static int ProcessLoop(List<string> lines)
    {
        bool POL = false;
        List<string> loopLines = new();
        decimal times = 0;

        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];

            if (line.StartsWith("print(") && line.EndsWith(");"))
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
                    value = GetValue(a, line),
                };
                if (dinolang.interpreter.Globals.Vars[b].value is string) dinolang.interpreter.Globals.Vars[b].type = "string";
                else if (dinolang.interpreter.Globals.Vars[b].value is decimal) dinolang.interpreter.Globals.Vars[b].type = "num";
                else if (dinolang.interpreter.Globals.Vars[b].value is bool) dinolang.interpreter.Globals.Vars[b].type = "bool";
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
        return 1;
    }

}