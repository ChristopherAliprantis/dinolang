using System;
using System.Collections.Generic;
using System.Text;

namespace dinolang.interpreter;

partial class Interpreter
{
    public struct Struct
    {
        public string typename { get; set; }
        public List<string> Lines;
        public List<StructVar> Vars;

        public dynamic AccessVar(string varName, string instancename, string line)
        {
            StructVar VAR = Vars.Find(v => v.name == varName);
            if (VAR == null)
            {
                Console.WriteLine($"Invalid Value {instancename}.{varName} Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                Environment.Exit(1);
            } 

            /* if ((line.Contains('=')) && (BeforeChar(line, '=').Length > 0) && (AfterChar(line, '=').Length > 1))
            {
                bool value = false;
                var b = BeforeChar(line, '=');
                var a = BeforeChar(AfterChar(line, $"{b}="), ';');
                if (a.StartsWith("(R!)"))
                {
                    a = a.Substring(4);
                    value = true;
                }
                if (Globals.Vars.ContainsKey(b) && Globals.Vars[b].RO == true)
                {
                    Console.WriteLine($"Invalid Value {b}, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                if (b == "TextBGColor" || b == "DLine" || b == "NL" || b == "BLANK" || b == "COMMA" || b == "COLON" || b == "SC")
                {
                    Console.WriteLine($"Invalid Value {b}, Line {line} Try going on https://github.com/ChristopherAliprantis/dinolang/wiki/ for help");
                    Environment.Exit(1);
                }
                var v = GetValue(a, line);
                if (Globals.Vars.ContainsKey(b) && v is List<dynamic>)
                {
                    v = new List<dynamic>(v);
                }
                else if (v is Dictionary<dynamic, dynamic> && Globals.Vars.ContainsKey(b))
                {
                    v = new Dictionary<dynamic, dynamic>(v);
                }
                else if (v is Struct && Globals.Vars.ContainsKey(b))
                {
                    v = Struct.CopyStruct(v);
                }
                dinolang.interpreter.Globals.Vars[b] = new Variable
                {
                    value = v,
                    name = b,
                    RO = value
                };
                if (dinolang.interpreter.Globals.Vars[b].value is string) dinolang.interpreter.Globals.Vars[b].type = "string";
                else if (dinolang.interpreter.Globals.Vars[b].value is decimal) dinolang.interpreter.Globals.Vars[b].type = "num";
                else if (dinolang.interpreter.Globals.Vars[b].value is bool) dinolang.interpreter.Globals.Vars[b].type = "bool";
                else if (dinolang.interpreter.Globals.Vars[b].value is null) dinolang.interpreter.Globals.Vars[b].type = "null";
                else if (dinolang.interpreter.Globals.Vars[b].value is List<dynamic>) dinolang.interpreter.Globals.Vars[b].type = "list";
                else if (dinolang.interpreter.Globals.Vars[b].value is Dictionary<dynamic, dynamic>) dinolang.interpreter.Globals.Vars[b].type = "dictionary";
            } */
            return "";
        }

        public Struct(StructBluePrint blueprint)
        {
            Lines = blueprint.Lines;
            typename = blueprint.Name;
        }

        public static Struct CopyStruct(Struct original)
        {
            Struct copy = new Struct();
            copy.Lines = new List<string>(original.Lines);
            copy.Vars = new List<StructVar>(original.Vars);
            return copy;
        }
    }

    public struct StructBluePrint
    {
        public List<string> Lines;
        public string Name;
        public List<StructVar> Vars;
        public StructBluePrint(string name, List<string> lines)
        {
            Name = name;
            Lines = lines;
        }
    }

    public class StructVar
    {
        public string name { get; set; }
        public int index { get; set; }
        public StructVar(string name, int index)
        {
            this.name = name;
            this.index = index;
        }
    }

}