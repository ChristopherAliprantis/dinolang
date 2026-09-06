using System;
using System.Collections.Generic;
using System.Text;

namespace dinolang.interpreter;


public struct StructBlueprint
{
    public string name;
    public List<string> fields;
}

public struct Struct
{
    public List<string> fields;
    public string instancevarname;
    public string typename;

    public Struct(StructBlueprint blueprint, string line)
    { 
        fields = blueprint.fields;
        typename = blueprint.name;
        foreach (var field in blueprint.fields)
        {
            string f = instancevarname + "." + field;
            if (!Globals.Vars.ContainsKey(f))
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