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
        decimal times = 0.0m;
        string cond = "";
        List<string> IfLines = new();
        bool IF = false;
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
                if (COND) ProcessIf(IfLines, false);
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
            else if (line.StartsWith("printnnl(") && line.EndsWith(");"))
            {
                string arg = line.Substring(0, line.Length - 2);
                arg = AfterChar(arg, "printnnl(");
                dynamic result = GetValue(arg, line);
                if (result is bool) result = result.ToString().ToUpper();
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
                    value = GetValue(a, line),
                };
                if (dinolang.interpreter.Globals.Vars[b].value is string) dinolang.interpreter.Globals.Vars[b].type = "string";
                else if (dinolang.interpreter.Globals.Vars[b].value is decimal) dinolang.interpreter.Globals.Vars[b].type = "num";
                else if (dinolang.interpreter.Globals.Vars[b].value is bool) dinolang.interpreter.Globals.Vars[b].type = "bool";
                else if (dinolang.interpreter.Globals.Vars[b].value is null) dinolang.interpreter.Globals.Vars[b].type = "null";
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