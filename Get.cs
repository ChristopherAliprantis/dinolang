using System;
using CommandLine;

namespace dinolang;

public class Getfl
{
    [Option('m', "mode", Default = "debug", HelpText = "The mode to run in: debug or release.")]
    public static string? Mode { get; set; }
    [Option('f', "file", Default = null, HelpText = "The file to run.")]
    public static string? File { get; set; }

    [Option('c', "code", Default = "", HelpText = "The code to run if you dont have a file.")]
    public static string? Code { get; set; }
    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Getfl>(args)
            .WithParsed(opt =>
            {
                if (opt.File != null)
                {
                    string code = System.IO.File.ReadAllText(opt.File);
                }
                else if (opt.Code != null)
                {
                    Console.WriteLine($"Running direct code: {opt.Code}");
                }
            });
    }
}
