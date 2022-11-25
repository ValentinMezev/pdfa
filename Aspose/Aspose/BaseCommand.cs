using System;
using System.IO;
using Aspose.Pdf;
using CommandLine;

namespace Aspose;

internal abstract class BaseCommand
{
    private string _outPath = string.Empty;

    [Option("output-path", HelpText = "Output path.", Default = "$PWD/Examples")]
    public string OutPath
    {
        get => _outPath;
        set
        {
            if (value.StartsWith("$PWD"))
                value = Path.Combine(Directory.GetCurrentDirectory(), "Examples");
            _outPath = value;
        }
    }

    [Option(Default = false)]
    public bool Verbose { get; set; }

    public abstract int Run();

    protected void Validate(Document document)
    {
        Console.WriteLine("Checking validity");
        Console.WriteLine("Valid: {0}", Helper.ValidatePdf(document, Verbose));
    }

    protected void ConvertDocument(Document document, string saveAs)
    {
        Console.WriteLine("Converting...");
        Console.WriteLine("Converted: {0}", Helper.ConvertToPdfA3A(document, Verbose));
        document.Save(saveAs);
        Console.WriteLine("Converted to PDF/A-3 as {0}", saveAs);
    }
}