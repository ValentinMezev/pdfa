import org.apache.jempbox.xmp.XMPMetadata;
import org.apache.jempbox.xmp.XMPSchemaBasic;
import org.apache.jempbox.xmp.XMPSchemaDublinCore;
import org.apache.jempbox.xmp.XMPSchemaPDF;
import org.apache.jempbox.xmp.pdfa.XMPSchemaPDFAId;
import org.apache.pdfbox.cos.COSArray;
import org.apache.pdfbox.cos.COSDictionary;
import org.apache.pdfbox.pdmodel.*;
import org.apache.pdfbox.pdmodel.common.PDMetadata;
import org.apache.pdfbox.pdmodel.common.filespecification.PDComplexFileSpecification;
import org.apache.pdfbox.pdmodel.common.filespecification.PDEmbeddedFile;
import org.apache.pdfbox.pdmodel.documentinterchange.logicalstructure.PDMarkInfo;
import org.apache.pdfbox.pdmodel.documentinterchange.logicalstructure.PDStructureTreeRoot;
import org.apache.pdfbox.pdmodel.font.PDFont;
import org.apache.pdfbox.pdmodel.font.PDTrueTypeFont;
import org.apache.pdfbox.pdmodel.graphics.color.PDOutputIntent;

import javax.xml.transform.TransformerException;
import java.io.*;
import java.util.*;

public class Generator {
    public void generateDocument(String file, String outputFile) throws Exception {
        // the document
        PDDocument doc = null;
        try {
            //Add fonts
            File rawfile = new File(file);
            doc = PDDocument.load(rawfile);

            setPdFont(doc, "/OpenSans-Bold.ttf");
            setPdFont(doc, "/OpenSans-SemiBold.ttf");
            setPdFont(doc, "/OpenSans-Regular.ttf");

            PDDocumentCatalog cat = makeA3compliant(doc);

            // create output intent
            setColorProfile(doc, cat);

            doc.save(outputFile);

        } finally {
            if (doc != null) {
                doc.close();
            }
        }
    }

    private static void setColorProfile(PDDocument doc, PDDocumentCatalog cat) throws Exception {
        InputStream colorProfile = Generator.class.getResourceAsStream("/sRGB Color Space Profile.icm");
        PDOutputIntent oi = new PDOutputIntent(doc, colorProfile);
        oi.setInfo("sRGB IEC61966-2.1");
        oi.setOutputCondition("sRGB IEC61966-2.1");
        oi.setOutputConditionIdentifier("sRGB IEC61966-2.1");
        oi.setRegistryName("http://www.color.org");
        cat.addOutputIntent(oi);
    }

    private static PDFont setPdFont(PDDocument doc, String name) throws IOException {
        InputStream fontStream = Generator.class.getResourceAsStream(name);
        PDFont font = PDTrueTypeFont.loadTTF(doc, fontStream);
        return font;
    }

    private InputStream getFileFromResource(String fileName) throws IllegalArgumentException {

        InputStream stream = Generator.class.getResourceAsStream(fileName);
        if (stream == null) {
            throw new IllegalArgumentException("file not found! " + fileName);
        } else {
            return stream;
        }

    }

    private PDDocumentCatalog makeA3compliant(PDDocument doc) throws IOException, TransformerException {
        {
            PDDocumentCatalog cat = doc.getDocumentCatalog();

            //required, if signatures on page won't be null
            if(cat.getStructureTreeRoot() == null) {
                PDStructureTreeRoot root = new PDStructureTreeRoot();
                cat.setStructureTreeRoot(root);
            }

            PDMetadata metadata = new PDMetadata(doc);

            //Add file
            embedFile(doc, cat);

            cat.setMetadata(metadata);

            // meta
            XMPMetadata xmp = new XMPMetadata();
            XMPSchemaPDFAId pdfaid = new XMPSchemaPDFAId(xmp);
            xmp.addSchema(pdfaid);

            XMPSchemaDublinCore dc = xmp.addDublinCoreSchema();
            String creator = System.getProperty("test");
            String producer = "test";
            dc.addCreator(creator);
            dc.setAbout("");

            XMPSchemaBasic xsb = xmp.addBasicSchema();
            xsb.setAbout("");

            xsb.setCreatorTool(creator);
            xsb.setCreateDate(GregorianCalendar.getInstance());
            // PDDocumentInformation pdi=doc.getDocumentInformation();
            PDDocumentInformation pdi = new PDDocumentInformation();
            pdi.setProducer(producer);
            pdi.setAuthor(creator);
            doc.setDocumentInformation(pdi);

            XMPSchemaPDF pdf = xmp.addPDFSchema();
            pdf.setProducer(producer);
            pdf.setAbout("");

            // Mandatory: PDF-A3 is tagged PDF which has to be expressed using a
            // MarkInfo dictionary (PDF A/3 Standard sec. 6.7.2.2)
            PDMarkInfo markinfo = new PDMarkInfo();
            markinfo.setMarked(true);
            doc.getDocumentCatalog().setMarkInfo(markinfo);

            doc.getDocumentCatalog().getCOSDictionary().setDate("ModDate", new GregorianCalendar());

            pdfaid.setPart(3);
            pdfaid.setConformance("A");/*
             * All files are PDF/A-3, setConformance refers
             * to the level conformance, e.g. PDF/A-3-B where
             * B means only visually preservable, U means
             * visually and unicode preservable and A -like
             * in this case- means full compliance, i.e.
             * visually, unicode and structurally preservable
             */
            pdfaid.setAbout("");
            metadata.importXMPMetadata(xmp);
            return cat;
        }
    }

    private void embedFile(PDDocument doc, PDDocumentCatalog cat) {
        //https://www.loc.gov/preservation/digital/formats/fdd/fdd000360.shtml
        try {
            String filename = "sample.xml";
            InputStream xml = getFileFromResource("/sample.xml");
            int length = getFileFromResource("/sample.xml").readAllBytes().length;

            PDEmbeddedFilesNameTreeNode efTree = new PDEmbeddedFilesNameTreeNode();

            //first create the file specification, which holds the embedded file
            PDComplexFileSpecification fs = new PDComplexFileSpecification();
            fs.setFile(filename);

            //create metadata for the attachments (see spec)
            COSDictionary embeddedMetaFs = fs.getCOSDictionary();
            // Relation "Source" for linking with eg. catalog
            //https://api.itextpdf.com/iText5/java/5.5.11/com/itextpdf/text/pdf/AFRelationshipValue.html
            embeddedMetaFs.setName("AFRelationship", "Alternative");
            //https://api.itextpdf.com/iText5/java/5.5.10/com/itextpdf/text/pdf/PdfFileSpecification.html#setUnicodeFileName-java.lang.String-boolean-
            embeddedMetaFs.setString("UF", filename);

            PDEmbeddedFile ef = new PDEmbeddedFile(doc, xml);
            //set some of the attributes of the embedded file
            ef.setSubtype( "text/xml" );
            ef.setSize( length );
            ef.setCreationDate( new GregorianCalendar());
            //required by a3
            ef.setModDate(GregorianCalendar.getInstance());

            fs.setEmbeddedFile(ef);
            //add a3 metadata
            fs.setFileDescription("some description");

            //now add the entry to the embedded file tree and set in the document.
            Map efMap = new HashMap();
            efMap.put( "My first attachment", fs );
            efTree.setNames( efMap );
            //attachments are stored as part of the "names" dictionary in the document catalog
            PDDocumentNameDictionary names = new PDDocumentNameDictionary(cat);
            names.setEmbeddedFiles( efTree );
            doc.getDocumentCatalog().setNames( names );

            // AF entry (Array) in catalog with the FileSpec
            //Relationship links are established from the document or parts of the document by use of the AF key, which contains an array of file specification dictionaries
            //see the spec.
            COSArray cosArray = new COSArray();
            cosArray.add(fs);
            cat.getCOSDictionary().setItem("AF", cosArray);
        } catch (Exception ex) {
            System.out.println(ex.getMessage());
        }
    }
}
