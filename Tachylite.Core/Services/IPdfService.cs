using System;
using Aspose.Pdf;
using Aspose.Pdf.Forms;
using Aspose.Pdf.Text;
using Tachylite.Core.Models;

namespace Tachylite.Core.Services;

public interface IPdfService
{
    // Changed by OpenCode's Big Pickle: optional callbacks allow the UI to show the document and each
    // page as soon as they are ready instead of waiting for every background image to render.
    public PdfDocumentModel ConstructPdfDocumentModel(
        byte[] document,
        string documentName,
        PdfPageBackgroundImageType backgroundImageType,
        Action<PdfDocumentModel>? onDocumentReady = null,
        Action<PdfPageModel>? onPageReady = null);
    // Changed by OpenCode's Big Pickle: ConstructPdfPageModel now receives the already-rendered,
    // field-free background image so form field values do not get rendered into the page background.
    public PdfPageModel ConstructPdfPageModel(Page page, Field[] fields, PdfPageBackgroundImageType backgroundImageType, byte[] backgroundImage);
    public PdfTextElementModel ConstructPdfTextElementModel(TextFragment textFragment);
    // Changed by OpenCode's Big Pickle: ConstructPdfFormFieldElementModel now accepts explicit widget bounds
    // and a per-widget key so text box fields with null/empty field-level rectangles can still be extracted correctly.
    public PdfFormFieldElementModel ConstructPdfFormFieldElementModel(Field field, Rectangle? bounds = null, string? widgetKey = null, int pageNumber = 0);
    public byte[] WritePdfBytes(byte[] documentData, PdfDocumentModel documentModel);
}
