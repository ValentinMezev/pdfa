using System;
using System.IO;
using Aspose.Pdf;
using CommandLine;

namespace Aspose;

[Verb("convert", HelpText = "Take input PDF and convert to PDF/A-3a output.")]
internal class Convert : BaseCommand
{
    [Option("input", Required = true, HelpText = "Input file to convert.")]
    public string Input { get; set; } = string.Empty;

    [Option("output", Default = "output.pdf", HelpText = "Name of the output file.")]
    public string Output { get; set; } = string.Empty;

    public override int Run()
    {
        if (!File.Exists(Input))
        {
            Console.Error.WriteLine("Could not find input: {0}", Input);
            return 1;
        }

        var document = new Document(Input);
        Console.WriteLine("Loaded {0}", Input);
        Validate(document);
        ConvertDocument(document, Path.Combine(OutPath, Output));
        Validate(document);
        Console.WriteLine();

        return 0;
    }
}