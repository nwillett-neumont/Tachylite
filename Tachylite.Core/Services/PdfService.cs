using System;
using System.Drawing;
using System.Drawing.Imaging;
using Aspose.Pdf;
using Aspose.Pdf.Annotations;
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

        ImagePlacementAbsorber imagePlacementAbsorber = new();
        page.Accept(imagePlacementAbsorber);

        foreach(var imagePlacement in imagePlacementAbsorber.ImagePlacements)
        {
            imageElements.Add(ConstructPdfImageElementModel(imagePlacement));
        }

        byte[] backgroundImage;

        if (backgroundImageType == PdfPageBackgroundImageType.Bitmap)
        {
            BmpDevice imageDevice = new();
            backgroundImage = imageDevice.Process(page);
        } else
        {
            MemoryStream stream = new();
            SvgDevice imageDevice = new();
            imageDevice.Process(page, stream);
            backgroundImage = stream.ToArray();
            stream.Dispose();
        }

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

    public PdfImageElementModel ConstructPdfImageElementModel(ImagePlacement imagePlacement)
    {
        return new PdfImageElementModel()
        {
            Bounds = imagePlacement.Rectangle.ToRect(),
            ImageData = imagePlacement.Image?.GetDecodedData()
        };
    }
}
