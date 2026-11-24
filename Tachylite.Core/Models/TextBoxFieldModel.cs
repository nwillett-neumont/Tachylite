using System;
using System.Drawing;

namespace Tachylite.Core.Models;

public class TextBoxFieldModel : PdfFormFieldElementModel
{
    public required bool IsMultiline { get; set; }
    public required bool IsScrollable { get; set; }
    public int? MaxLength { get; set; }
    public required string Value { get; set; }
    public string? DefaultValue { get; set; }
    public double? FontSize { get; set; }
    public string? FontName { get; set; }
    public Color Color { get; set; }
}