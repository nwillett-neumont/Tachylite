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

    public void PopRecent();

    public void CreateChestSubfolder(string chestPath, string? subfolder, string name);

    public void CreateMarkdownFile(string chestPath, string? subfolder, string name);

    public void ImportPdfFile(string chestPath, string? subfolder, string existingPdfPath);

    public string[] GetChestContents(string chestPath);

    public string[] GetFolderContents(string path);
    public bool UserDocumentsDirectoryExists();
    public void CreateUserDocumentsDirectory();
    public string GetUserDocumentsDirectory();

    public string GetMarkdownFileContents(string filePath);
    public void CreateUserFileBackup(string filePath);
    public void CreatePerChestFileBackup(string chestPath, string filePath);
}
