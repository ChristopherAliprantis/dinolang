using System;
using System.Collections.Generic;
using System.Text;

namespace dinolang.interpreter
{
    public partial class Interpreter
    {
        public static void Interpret(List<string> lines)
        {
            for (int i = 0; i < lines.Count; i++) 
            {
                
            }
        }

        public static dynamic? GetValue(string val, int line)
        {
            decimal? value1;
            try
            {
                value1 = decimal.Parse(val);
                return value1;
            }
            catch { }
            string? value2;
            if (val.StartsWith(':') && val.EndsWith(':'))
            {
                value2 = val.Substring(1, val.Length - 1);
                return value2;
            }
            
            if (dinolang.interpreter.Globals.Funcs.ContainsKey(val))
            {
                var poses = (0, 0); 
                for (int i = 0; i <= val.Length; i++)
                {
                    if (val[i] == '(')
                    {
                        poses.Item1 = i;
                    }
                    if (val[i] == ')')
                    {
                        poses.Item2 = i;
                        string brackets = val.Substring(poses.Item1, poses.Item2).Trim();
                        int j;
                        int paramS = 0;
                        for (j = 0; j <= brackets.Length; j++)
                        {
                            if (brackets[j] != ',') paramS++;
                        }
                        if (j == 0) break;
                        Function func = dinolang.interpreter.Globals.Funcs[val];
                        if (func.parameters.Count == paramS)
                        {
                            return dinolang.interpreter.Globals.Funcs[val];
                        }
                    }
                }
                
            }
            if (dinolang.interpreter.Globals.Vars.ContainsKey(val))
            {
                return dinolang.interpreter.Globals.Vars[val];
            }
            Environment.Exit(0);
            if (1 + 1 == 2) return null;
        }
    }
}
