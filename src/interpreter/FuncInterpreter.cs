using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace dinolang.interpreter
{
    public partial class Interpreter
    {
        public static dynamic? ProcessFunc(Function func, List<dynamic> vals, string name)
        {
            var lines = new List<string>(func.code);
            var p = func.parameters;
            Console.WriteLine($"param count: {p.Count}, vals count: {vals.Count}");
            if (!lines.Any(item => item.StartsWith("return")))
            {
                Console.WriteLine($"Function {name} doesn't return anything Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                Environment.Exit(1);
            }
            var Nvs = new Dictionary<string, Variable>();
            var Nvsk = new List<string>();                
            for (int i = 0; i < p.Count; i++)
            {
                {
                    dynamic val = vals[i];
                    Insert(lines ,0, $"{p[i]}={val};");
                    if (Globals.Vars.ContainsKey(p[i]))
                    {
                        Nvs[p[i]] = Globals.Vars[p[i]];
                        Nvsk.Add(p[i]);
                    }
                }
            }
            Console.WriteLine(string.Join(", ", lines));
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line.StartsWith("print(") && line.EndsWith(");"))
                {
                    string arg = BeforeChar(AfterChar(line, '('), ");");
                    Console.WriteLine(GetValue(arg, line, null));
                }
                else if (line.StartsWith($"{BeforeChar(line, '=')}="))
                {
                    var b = BeforeChar(line, '=');
                    var a = BeforeChar(AfterChar(line, '='), ';');
                    dinolang.interpreter.Globals.Vars[b] = new Variable
                    {
                        value = GetValue(a, line, null),
                    };
                    if (dinolang.interpreter.Globals.Vars[b].value is string) dinolang.interpreter.Globals.Vars[b].type = "string";
                    else if (dinolang.interpreter.Globals.Vars[b].value is decimal) dinolang.interpreter.Globals.Vars[b].type = "num";
                }
                else if (line.StartsWith("return(") && line.EndsWith(");"))
                {
                    string arg = BeforeChar(AfterChar(line, '('), ");");
                    var th = GetValue(arg, line, null);
                    RestoreDI(Nvsk, Nvs);

                    return th;
                }
                else if (line.Contains("(") && line.EndsWith(");"))
                {
                    GetValue(BeforeChar(line, ';'), line, null);
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

        public static List<string> Insert(List<string> input, int pos, string value)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (pos < 0 || pos > input.Count)
                throw new ArgumentOutOfRangeException(nameof(pos));

            input.Add(default);

            for (int i = input.Count - 1; i > pos; i--)
            {
                input[i] = input[i - 1];
            }
            input[pos] = value;

            return input;
        }
    }
}
