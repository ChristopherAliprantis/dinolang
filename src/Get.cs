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

    public static List<string> codes;
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        string baseDir = AppContext.BaseDirectory;

        string SSfilePath = Path.Combine(baseDir, "interpreter", "StringStuff.dno");
        string MfilePath = Path.Combine(baseDir, "interpreter", "Math.dno");

        string code = System.IO.File.ReadAllText(SSfilePath, Encoding.UTF8) + System.IO.File.ReadAllText(MfilePath, Encoding.UTF8);
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
        List<string> Code = ToReadableLines(code);
        interpreter.Interpreter.Interpret(Code);
        Environment.Exit(1);
    }
}
