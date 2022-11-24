using System;
using System.IO;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.XMP;
using iText.Pdfa;

namespace iText
{
    public class Program
    {
        private static readonly string InputPdf = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.pdf");
        private static string OutputPdf = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.pdf");

        // resources
        private static string Intent = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources/color/sRGB Color Space Profile.icm");
        private static string AttachmentFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources/data/example.xml");
        private static string Font = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources/font/FreeSans.ttf");
        private static string BoldFont = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources/font/FreeSansBold.ttf");


        public static void Main(string[] args)
        {
            var source = new PdfDocument(new PdfReader(InputPdf));
            var intent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1",
                new FileStream(Intent, FileMode.Open, FileAccess.Read));
            var target = new PdfADocument(new PdfWriter(OutputPdf), PdfAConformanceLevel.PDF_A_3B, intent);
            target.SetTagged();
            target.GetCatalog().SetLang(new PdfString("en-US"));
            target.GetCatalog().SetViewerPreferences(new PdfViewerPreferences().SetDisplayDocTitle(true));

            var meta = XMPMetaFactory.Create();
            var info = target.GetDocumentInfo();

            info.AddCreationDate();
            meta.SetProperty(XMPConst.NS_XMP, PdfConst.CreateDate, info.GetMoreInfo("CreationDate"));

            info.AddModDate();
            meta.SetProperty(XMPConst.NS_XMP, PdfConst.ModifyDate, info.GetMoreInfo("ModDate"));

            info.SetTitle("Test PDF/A");

            target.SetXmpMetadata(meta);


            AddInputFileToDoc(source, target);
            AttachFile(target);


            PdfFont font = PdfFontFactory.CreateFont(Font, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
            PdfFont bold = PdfFontFactory.CreateFont(BoldFont, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
            target.AddFont(font);
            target.AddFont(bold);

            target.Close();
        }

        private static void AddInputFileToDoc(PdfDocument source, PdfADocument target)
        {
            source.CopyPagesTo(1, source.GetNumberOfPages(), target);
        }

        private static void AttachFile(PdfADocument target)
        {
            PdfDictionary parameters = new PdfDictionary();
            parameters.Put(PdfName.ModDate, new PdfDate().GetPdfObject());
            PdfFileSpec fileSpec = PdfFileSpec.CreateEmbeddedFileSpec(target, File.ReadAllBytes(Path.Combine(
                AttachmentFile
            )), "example.xml", "example.xml", new PdfName("application/xml"), parameters, PdfName.Data);
            fileSpec.Put(new PdfName("AFRelationship"), new PdfName("Data"));
            target.AddFileAttachment("example.xml", fileSpec);
            PdfArray array = new PdfArray();
            array.Add(fileSpec.GetPdfObject().GetIndirectReference());
            target.GetCatalog().Put(new PdfName("AF"), array);
        }
    }
}
