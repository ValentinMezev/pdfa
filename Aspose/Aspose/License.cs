using System.Reflection;
using System.Runtime.CompilerServices;

namespace Aspose;

internal static class License
{
    [ModuleInitializer]
    internal static void InitialiseLicense()
    {
        using var s = Assembly.GetExecutingAssembly().GetManifestResourceStream("Aspose.Aspose.PDF.NET.lic");
        var license = new Aspose.Pdf.License();
        license.SetLicense(s);
    }
}