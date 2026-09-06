using System;
using System.Collections.Generic;
using System.Text;

namespace dinolang.interpreter;

partial class Interpreter
{
    public struct StructBlueprint
    {
        public string name;
        public List<string> fields;
    }

    public struct Struct
    {
        public List<string> fields;
        public string instancevarname;

        public Struct(StructBlueprint blueprint, string line)
        {
            fields = blueprint.fields;
            foreach (var field in blueprint.fields)
            {
                string f = instancevarname + "." + field;
                if (f == instancevarname)
                {
                    Console.WriteLine($"Cannot assign field name to instance variable Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                else if (f == "TextBGColor" || f == "DLine" || f == "NL" || f == "BLANK" || f == "COMMA" || f == "COLON" || f == "SC")
                {
                    Console.WriteLine($"Invalid Value {f}, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                else if (!Globals.Vars.ContainsKey(f))
                {
                    Globals.Vars[f] = new Variable
                    {
                        name = f,
                        type = "null",
                        value = null,
                        RO = false
                    };
                }
                else
                {
                    Console.WriteLine($"Variable {f} already exists Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
            }
        }
    }
}