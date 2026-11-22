using Aspose.Pdf;
using Aspose.Pdf.Devices;
using Aspose.Pdf.Text;
using Tachylite.Core.Models;

namespace Tachylite.Core.Services;

public class PdfService : IPdfService
{
    public PdfDocumentModel ConstructPdfDocumentModel(byte[] documentData, string documentName, PdfPageBackgroundImageType backgroundImageType)
    {
        using Document document = Document.Open(documentData);

        List<PdfPageModel> pages = [];

        foreach(var page in document.Pages)
        {
            pages.Add(ConstructPdfPageModel(page, backgroundImageType));
        }

        return new PdfDocumentModel
        {
            Name = documentName,
            Pages = [..pages]
        };
    }
    
    public PdfPageModel ConstructPdfPageModel(Page page, PdfPageBackgroundImageType backgroundImageType)
    {
        List<PdfTextElementModel> textElements = [];
        List<PdfImageElementModel> imageElements = [];

        TextFragmentAbsorber textAbsorber = new();
        page.Accept(textAbsorber);

        foreach(var textFragment in textAbsorber.TextFragments)
        {
            textElements.Add(ConstructPdfTextElementModel(textFragment));
        }

        for (var i = textAbsorber.TextFragments.Count; i >= 1; i--)
        {
            textAbsorber.TextFragments.Remove(textAbsorber.TextFragments[i]);
        }

        ImagePlacementAbsorber imagePlacementAbsorber = new();
        page.Accept(imagePlacementAbsorber);

        foreach(var imagePlacement in imagePlacementAbsorber.ImagePlacements)
        {
            // Changed by OpenCode Big Pickle
            var bounds = imagePlacement.Rectangle.ToRect();
            if (bounds.Width <= 0 || bounds.Height <= 0) continue;

            imageElements.Add(new PdfImageElementModel
            {
                Bounds = bounds,
                ImageData = null // images live in the background; nothing to decode
            });
        }

        byte[] backgroundImage;
        
        // Redone by OpenCode Big Pickle.
        // Use a raster device. SvgDevice does not faithfully render this PDF's
        // CMYK images with soft masks, which leaves large blank areas.
        PngDevice imageDevice = new(new Resolution(150));
        backgroundImage = imageDevice.Process(page);

        return new PdfPageModel()
        {
            PageNumber = page.Number,
            Rotation = page.RotateDegrees,
            Dimensions = new PdfPageSizeModel(page.Width, page.Height),
            BackgroundImage = backgroundImage,
            BackgroundImageType = backgroundImageType,
            TextElements = [..textElements],
            ImageElements = [..imageElements]
        };
    }

    public PdfTextElementModel ConstructPdfTextElementModel(TextFragment textFragment)
    {
        return new PdfTextElementModel()
        {
            TextContent = textFragment.Text,
            Bounds = textFragment.Rectangle?.ToRect(),
            FontName = textFragment.TextState.FontName,
            FontSize = textFragment.TextState.FontSize,
            ForegroundColor = textFragment.TextState.ForegroundColor?.ToRgb(),
            BackgroundColor = textFragment.TextState.BackgroundColor?.ToRgb()
        };
    }
}
