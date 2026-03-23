using System;
using CommandLine;

namespace dinolang;

public class GetCode
{
    [Option('m', "mode", Default = "debug", HelpText = "The mode to run in: debug or release.")]
    public string? Mode { get; set; }
    [Option('f', "file", Default = null, HelpText = "The file to run.")]
    public string? File { get; set; }

    [Option('c', "code", Default = "", HelpText = "The code to run if you dont have a file.")]
    public string? Code { get; set; }
    static void Main(string[] args)
    {
        string code;
        Parser.Default.ParseArguments<Getfl>(args)
            .WithParsed(opt =>
            {
                if (opt.File != null)
                {
                    code = System.IO.File.ReadAllText(opt.File);
                }
                else if (opt.Code != null)
                {
                    code = opt.Code;
                }
            });
    }
}
