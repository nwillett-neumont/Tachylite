using System;

namespace Tachylite.Core.Models;

public class FolderModel(string name, FolderModel[] subFolders, FileModel[] files)
{
    public string Name { get; set; } = name;
    public FolderModel[] SubFolders { get; set; } = subFolders;
    public FileModel[] Files { get; set; } = files;
}
