using System;

namespace Tachylite.Core.Models;

public class ListBoxFieldModel : PdfFormFieldElementModel
{
    public required bool IsMultiselect { get; set; }

    public string? DefaultValue { get; set; }
    public required string?[] Options { get; set; }

    public required int[] SelectedIndices { get; set; }
}
