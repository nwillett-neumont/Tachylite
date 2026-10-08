using System;
using Tachylite.Core.Models;

namespace Tachylite.Core.Services;

public interface IFileSystemService
{
    public FileType GetFileType(string path);

    public bool UserConfigurationDirectoryExists();

    public void InitializeUserConfigurationDirectory();

    public bool IsValidChest(string path);

    public void CreateNewChest(string name, string path);

    public void AddRecent(string path);

    public string[] GetRecents();
}
