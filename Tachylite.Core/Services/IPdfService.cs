using System;
using Aspose.Pdf;
using Aspose.Pdf.Text;
using Tachylite.Core.Models;

namespace Tachylite.Core.Services;

public interface IPdfService
{
    public PdfDocumentModel ConstructPdfDocumentModel(byte[] document, string documentName, PdfPageBackgroundImageType backgroundImageType);
    public PdfPageModel ConstructPdfPageModel(Page page, PdfPageBackgroundImageType backgroundImageType);
    public PdfTextElementModel ConstructPdfTextElementModel(TextFragment textFragment);
    public PdfImageElementModel ConstructPdfImageElementModel(ImagePlacement imagePlacement);
}
