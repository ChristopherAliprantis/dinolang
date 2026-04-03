using System;
using CommandLine;
using System.Reflection;

namespace dinolang;


public partial class GetCode
{
    [Option('f', "file", Default = null, HelpText = "The file to run.")]
    public string? File { get; set; }

    [Option('c', "code", Default = "", HelpText = "The code to run if you don't have a file.")]
    public string? Code { get; set; }

    [Option('h', Default = "false", HelpText = "The wiki link(which includes command line usage). Use like -h true")]
    public string? Help { get; set; }
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        string code = "";
        Parser.Default.ParseArguments<GetCode>(args)
            .WithParsed(opt =>
            {
                if (opt.File != null)
                {
                    try
                    {
                        code = System.IO.File.ReadAllText(opt.File);
                    } 
                    catch 
                    {
                        Console.WriteLine($"File {opt.File} not found!");
                        Environment.Exit(1);
                    }

                }
                if (opt.Code != "")
                {
                    code = opt.Code;
                }
                if (opt.Help == "true") Console.WriteLine("https://github.com/ChristopherAliprantis/dinolang/wiki");
            });
        List<string> Code = ToReadableLines(code);
        interpreter.Interpreter.Interpret(Code);
        Environment.Exit(1);
    }
}
