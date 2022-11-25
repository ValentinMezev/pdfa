using System;
using System.IO;
using Aspose.Pdf;
using CommandLine;

namespace Aspose;

[Verb("generate", HelpText = "Create HelloWorld PDF and PDF/A-3a outputs.")]
internal class Generate : BaseCommand
{
    private string HelloWorld => Path.Combine(OutPath, "HelloWorld.pdf");
    private string HelloWorldPdfA3 => Path.Combine(OutPath, "HelloWorld_PDF-A3a.pdf");

    public override int Run()
    {
        Directory.CreateDirectory(OutPath);

        var document = CreateDocument();
        Console.WriteLine("Created {0}", HelloWorld);
        Validate(document);
        ConvertDocument(document, HelloWorldPdfA3);
        Validate(document);
        Console.WriteLine();

        return 0;
    }

    private Document CreateDocument()
    {
        var document = new Document();
        document.Info.Producer = "Aspose";
        document.Info.Author = "Test";
        document.Info.CreationDate = document.Info.ModDate = DateTime.Now;
        var page = document.Pages.Add();
        page.Paragraphs.Add(new Pdf.Text.TextFragment("Hello World!"));
        document.Save(HelloWorld);
        return document;
    }
}