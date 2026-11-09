using System;
using System.Drawing;

namespace Tachylite.Core.Models;

public class PdfImageElementModel
{
    public required Rectangle Bounds { get; set; }
    public required byte[]? ImageData { get; set; }
}
