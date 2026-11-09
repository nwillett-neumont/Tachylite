using System;

namespace Tachylite.Core.Models;

public class PdfDocumentModel
{
    public required string Name { get; set; }
    public required PdfPageModel[] Pages { get; set; }
}
