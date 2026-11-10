using System;

namespace Tachylite.Core.Models;

public class TemplateModel(string name, FolderModel root)
{
    public string Name { get; set; } = name;
    public FolderModel Root { get; set; } = root;
}
