using CommandLine;
using System;
using System.Reflection;
using System.Text;

namespace dinolang;


public partial class GetCode
{

    [Option('f', "file", Default = null, HelpText = "The file to run like \"test.dno\" or multiple like inputting \"file1.dno, file2.dno\" must have quotes around the inputs.")]
    public string? File { get; set; }

    [Option('c', "code", Default = "", HelpText = "The code to run if you don't have a file.")]
    public string? Code { get; set; }

    [Option('h', Default = "false", HelpText = "The wiki link(which includes command line help). Use like -h true")]
    public string? Help { get; set; }

    public static List<string> codes = new();
    static int Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        string baseDir = AppContext.BaseDirectory;

        string SfilePath = Path.Combine(baseDir, "interpreter", "String.dno");
        string MfilePath = Path.Combine(baseDir, "interpreter", "Math.dno");
        string IfilePath = Path.Combine(baseDir, "interpreter", "Input.dno");
        string TfilePath = Path.Combine(baseDir, "interpreter", "Time.dno");
        string OfilePath = Path.Combine(baseDir, "interpreter", "Output.dno");
        string BfilePath = Path.Combine(baseDir, "interpreter", "Bool.dno");

        string code = System.IO.File.ReadAllText(SfilePath, Encoding.UTF8).Replace("\r", "").Replace("\n", "")
                    + System.IO.File.ReadAllText(MfilePath, Encoding.UTF8).Replace("\r", "").Replace("\n", "")
                    + System.IO.File.ReadAllText(BfilePath, Encoding.UTF8).Replace("\r", "").Replace("\n", "")
                    + System.IO.File.ReadAllText(IfilePath, Encoding.UTF8).Replace("\r", "").Replace("\n", "")
                    + System.IO.File.ReadAllText(OfilePath, Encoding.UTF8).Replace("\r", "").Replace("\n", "")
                    + System.IO.File.ReadAllText(TfilePath, Encoding.UTF8).Replace("\r", "").Replace("\n", "");
        if (args.Length == 1)
        {
            Console.WriteLine("Dino 1.5 Interactive Environment"+ Environment.NewLine);
            while (true)
            {
                Console.Write(">> ");
                string input = Console.ReadLine();
                if (input == "exit") Environment.Exit(0);
                code += input;
                dinolang.interpreter.Globals.Code = ToReadableLines(code);
                interpreter.Interpreter.Interpret(interpreter.Globals.Code);
                code = "";
            }
        }
        Parser.Default.ParseArguments<GetCode>(args)
            .WithParsed(opt =>
            {
                if (opt.File != null)
                {
                    int i = 0;
                    try
                    {
                        codes = opt.File.Split(',').ToList();
                        for (i = 0; i < codes.Count; i++) code += System.IO.File.ReadAllText(codes[i].Trim(), Encoding.UTF8);
                    }
                    catch
                    {
                        Console.WriteLine($"File {codes[i].Trim()} not found!");
                        Environment.Exit(1);
                    }

                }
                if (opt.Code != "")
                {
                    code += opt.Code;
                }
                if (opt.Help == "true") Console.WriteLine("https://github.com/ChristopherAliprantis/dinolang/wiki");
            });
        dinolang.interpreter.Globals.Code = ToReadableLines(code);
        /*foreach (string line in Code)
        {
            if (line.Trim() != "") Console.WriteLine(line);
        }*/
        interpreter.Interpreter.Interpret(interpreter.Globals.Code);
        return 0;
    }
}
