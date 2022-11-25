using System;
using System.IO;
using Aspose.Pdf;

namespace Aspose;

public static class Helper
{
    public static bool ConvertToPdfA3A(Document document, bool log) =>
        ExecuteWithStreamLogging(s =>
            document.Convert(s, PdfFormat.PDF_A_3A, ConvertErrorAction.Delete, ConvertTransparencyAction.Default), log);

    public static bool ValidatePdf(Document document, bool log) =>
        ExecuteWithStreamLogging(s => document.Validate(s, PdfFormat.PDF_A_3A), log);

    private static T ExecuteWithStreamLogging<T>(Func<Stream, T> operation, bool log)
    {
        using var ms = new MemoryStream();
        var result = operation.Invoke(ms);
        if (!log) return result;
        ms.Seek(0, SeekOrigin.Begin);
        var xml = System.Xml.Linq.XDocument.Load(ms);
        xml.Save(Console.Out, System.Xml.Linq.SaveOptions.None);
        Console.WriteLine();
        return result;
    }
}