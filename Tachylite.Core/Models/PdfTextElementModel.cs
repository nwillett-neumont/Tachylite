using System;
using System.Drawing;

namespace Tachylite.Core.Models;

public class PdfTextElementModel
{
    public required string? TextContent { get; set; }
    public required Rectangle? Bounds { get; set; }
    public required string? FontName { get; set; }
    public byte[]? FontData { get; set; }
    public required double? FontSize { get; set; }
    public required Color? ForegroundColor { get; set; }
    public required Color? BackgroundColor { get; set; }
}
