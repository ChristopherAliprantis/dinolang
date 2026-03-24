using System;
using System.Collections.Generic;
using System.Text;

namespace dinolang.Interpreter
{
    public class Globals
    {
        public static Dictionary<string, Variable> Vars = new();
        public static Dictionary<string, Function> Funcs = new();
    }

}

public class Variable
{
    dynamic? value;
    string? type;
}

public class Function
{
    List<dynamic>? parameters;
    string? code;
}
