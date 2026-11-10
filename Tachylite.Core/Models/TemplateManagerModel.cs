using System;

namespace Tachylite.Core.Models;

public class TemplateManagerModel(TemplateModel[] templates)
{
    public TemplateModel[] Templates { get; set; } = templates;
}
