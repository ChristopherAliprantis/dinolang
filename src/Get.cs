using System;
using CommandLine;
using System.Reflection;

namespace dinolang;


public partial class GetCode
{
    [Assembly: AssemblyCopyright("None")]

    [Option('m', "mode", Default = "debug", HelpText = "The mode to run in: debug or release.")]
    public string? Mode { get; set; }
    [Option('f', "file", Default = null, HelpText = "The file to run.")]
    public string? File { get; set; }

    [Option('c', "code", Default = "", HelpText = "The code to run if you dont have a file.")]
    public string? Code { get; set; }
    static void Main(string[] args)
    {
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
                    }

                }
                else if (opt.Code != null)
                {
                    code = opt.Code;
                }
            });
        List<string> Code = ToReadableLines(code);
        for (int i = 0;i < Code.Count; i++)
        {
            Console.WriteLine(Code[i]);
        }
    }
}
