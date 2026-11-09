using System;
using System.Drawing;

namespace Tachylite.Core.Models;

public class PdfPageModel
{
    public required int PageNumber { get; set; }
    public required PdfPageSizeModel Dimensions { get; set; }
    public required int Rotation { get; set; }
    public required byte[] BackgroundImage { get; set; }
    public required PdfPageBackgroundImageType BackgroundImageType { get; set; }
    public required PdfTextElementModel[] TextElements { get; set; }
    public required PdfImageElementModel[] ImageElements { get; set; }
}
