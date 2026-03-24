using System;
using System.Collections.Generic;
using System.Text;

namespace dinolang.interpreter
{
    public class Globals
    {
        public static Dictionary<string, Variable> Vars = new();
        public static Dictionary<string, Function> Funcs = new();
    }

}

public class Variable
{
    public dynamic? value;
    public string? type;
}

public class Function
{
    public List<dynamic>? parameters;
    public string? code;
}
