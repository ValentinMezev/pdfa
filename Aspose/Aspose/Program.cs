using CommandLine;

return Parser.Default.ParseArguments<Aspose.Generate, Aspose.Convert, Aspose.ConvertHtml>(args)
    .MapResult(
            (Aspose.Generate command) => command.Run(),
            (Aspose.Convert command) => command.Run(),
            (Aspose.ConvertHtml command) => command.Run(),
            errs => 1);
