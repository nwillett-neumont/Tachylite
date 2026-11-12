using Tachylite.Core.Models;

namespace Tachylite.Core.Services;

public interface IFileSystemService
{
    public FileType GetFileType(string path);
    public bool UserConfigurationDirectoryExists();
    public void InitializeUserConfigurationDirectory();
    public bool IsValidChest(string path);
    public void CreateNewChest(string name, string path, TemplateModel? template);
    public void AddRecent(string path);
    public string[] GetRecents();
    public void PopRecent();
    public void CreateChestSubfolder(string chestPath, string? subfolder, string name);
    public void CreateMarkdownFile(string chestPath, string? subfolder, string name);
    public void ImportPdfFile(string chestPath, string? subfolder, string existingPdfPath);
    public (string[], string[]) GetChestContents(string chestPath); // First string represent unhidden stuff, second is hidden stuff
    public (string[], string[]) GetFolderContents(string path, bool isHidden); // First string represent unhidden stuff, second is hidden stuff
    public bool UserDocumentsDirectoryExists();
    public void CreateUserDocumentsDirectory();
    public string GetUserDocumentsDirectory();
    public string GetMarkdownFileContents(string filePath);
    public Task<string> GetMarkdownFileContentsAsync(string filePath);
    public void CreateUserFileBackup(string filePath);
    public void CreatePerChestFileBackup(string chestPath, string filePath);
    public void SaveMarkdownFile(string filePath, string content);
    public void RenameFile(string filePath, string newName);
    public void RenameFolder(string folderPath, string newName);
    public Dictionary<string, TemplateModel> GetTemplates();
    public void WriteFolderModelStructure(string chestPath, string? subPath, FolderModel folder);
    public void CreateTemplate(string chestPath, string templateName);
    public FolderModel CreateFolderModel(string path, string folderName);
    public byte[] ReadPdfData(string pdfPath);
    public void WritePdfData(string pdfPath, byte[] pdfData);
}
