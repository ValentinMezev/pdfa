using System;
using System.IO;
using System.Net.Http;
using Aspose.Pdf;
using CommandLine;

namespace Aspose;

[Verb("convert-html", HelpText = "Take input HTML and create PDF/A-3a output.")]
internal class ConvertHtml : BaseCommand
{
    private static readonly HttpClient HttpClient = new();

    [Option(Required = true, HelpText = "Input HTML file to process.")]
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

        var document = ConvertHtmlToPdf(Input);
        var outputFileName = Path.Combine(OutPath, Output);
        document.Save(outputFileName);
        Console.WriteLine("Converted {0} to {1}, checking validity", Input, outputFileName);
        Console.WriteLine("Valid: {0}", Helper.ValidatePdf(document, Verbose));

        return 0;
    }

    private Document ConvertHtmlToPdf(string input)
    {
        var options = new HtmlLoadOptions
        {
            HtmlMediaType = HtmlMediaType.Print,
            PageInfo =
            {
                IsLandscape = false,
                Margin =
                {
                    Top = 3.7,
                    Bottom = 16,
                    Right = 0,
                    Left = 0
                }
            },
            CustomLoaderOfExternalResources = ResourceLoader,
            IsRenderToSinglePage = false,
            IsEmbedFonts = true,
            PageLayoutOption = HtmlPageLayoutOption.None,
            WarningHandler = new WarningHandler()
        };
        return new Document(input, options);
    }

    private class WarningHandler : IWarningCallback
    {
        public ReturnAction Warning(WarningInfo warning)
        {
            return ReturnAction.Continue;
        }
    }

    private LoadOptions.ResourceLoadingResult ResourceLoader(string url) =>
        new(GetContentFromUrl(url));

    private static byte[] GetContentFromUrl(string url) =>
        HttpClient.GetByteArrayAsync(url).GetAwaiter().GetResult();
}