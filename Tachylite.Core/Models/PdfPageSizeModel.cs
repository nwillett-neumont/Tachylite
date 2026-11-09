using System;

namespace Tachylite.Core.Models;

public struct PdfPageSizeModel
{
    public double Width { get; set; }
    public double Height { get; set; }

    public PdfPageSizeModel()
    {
        Width = 0;
        Height = 0;
    }

    public PdfPageSizeModel(double width, double height)
    {
        Width = width;
        Height = height;
    }
}
