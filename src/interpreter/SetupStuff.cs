using System;
using System.Collections.Generic;
using System.Text;

namespace dinolang.interpreter
{
    public class Globals
    {
        public static Dictionary<string, Variable> Vars = new();
        public static Dictionary<string, Function> Funcs = new();
        public static List<string> Code = new();
        public static string dline = "PLACEHOLDER";
        public static byte[]? TEXTbackgroundcolor = null;
        public static Dictionary<string, StructBluePrint> Structs = new();
    }
}

public class Variable
{
    public dynamic? value = null;
    public string? type = "";
    public string? name = "";
    public bool RO  =false;
}

public class Function
{
    public List<string>? parameters = new();
    public bool command { get; set; }
    public List<string>? code = new();
    public bool addcalllineasdebugline { get; set; }
}

public struct Struct
{
    public string typename { get; set; }
    public List<string> Lines;

    public Struct(StructBluePrint blueprint)
    {
        Lines = blueprint.Lines;
        typename = blueprint.Name;
    }

    public static Struct CopyStruct(Struct original)
    {
        Struct copy = new Struct();
        copy.Lines = new List<string>(original.Lines);
        return copy;
    }
}

public struct StructBluePrint
{
    public List<string> Lines;
    public string Name;
    public StructBluePrint(string name, List<string> lines)
    {
        Name = name;
        Lines = lines;
    }
}
