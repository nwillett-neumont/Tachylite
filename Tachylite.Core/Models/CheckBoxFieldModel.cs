using System;

namespace Tachylite.Core.Models;

public class CheckBoxFieldModel : PdfFormFieldElementModel
{
    public required bool IsChecked { get; set; }
}
