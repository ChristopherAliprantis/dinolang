using System;
using System.Collections.Generic;
using System.Text;

namespace dinolang.interpreter
{
    public partial class Interpreter
    {
        public static dynamic? ProcessFunc(Function func, int line)
        {
            Console.WriteLine($"Invalid Value, Line {line}.");
            Environment.Exit(0);
            if (1 + 1 == 2) return null;
        }
    }
}
