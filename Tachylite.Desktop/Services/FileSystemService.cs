using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using Aspose.Pdf.Operators;
using Avalonia.Markup.Xaml.Templates;
using Tachylite.Core.Models;
using Tachylite.Core.Services;

namespace Tachylite.Desktop.Services;

public class FileSystemService : IFileSystemService
{
    const string newFileMessage = 
    """
    # Hello!

    ### Welcome to Tachylite, the new home for all of your TTRPG stuff!
    """;

    public string GetConfigurationDirectoryPath()
    {
        string configurationDirectoryPath = "";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            configurationDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Tachylite");
        } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // I am currently unable to verify that this works, so I need to test it later.
            configurationDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "tachylite");
        } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // I am currently unable to verify that this works, so I need to test it later.
            configurationDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library/Application Support", "Tachylite");
        }

        return configurationDirectoryPath;
    }

    public FileType GetFileType(string path)
    {
        Regex validFileExtensions = new(@"^.*(\.md|\.pdf|\.json)$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        Match fileMatch = validFileExtensions.Match(path);

        return fileMatch.Groups[fileMatch.Groups.Count-1].Value.ToLower() switch
        {
            ".md" => FileType.MD,
            ".pdf" => FileType.PDF,
            ".json" => FileType.JSON,
            _ => FileType.UNKNOWN,
        };
    }

    public bool UserConfigurationDirectoryExists()
    {
        bool existsAndContainsItems = Path.Exists(GetConfigurationDirectoryPath()) && 
            Path.Exists(Path.Combine(GetConfigurationDirectoryPath(), "shared")) &&
            Path.Exists(Path.Combine(GetConfigurationDirectoryPath(), "backups")) &&
            File.Exists(Path.Combine(GetConfigurationDirectoryPath(), "recents.json")) &&
            File.Exists(Path.Combine(GetConfigurationDirectoryPath(), "templates.json")) &&
            File.Exists(Path.Combine(GetConfigurationDirectoryPath(), "settings.json"));

        return existsAndContainsItems;
    }

    public void InitializeUserConfigurationDirectory()
    {
        string configurationDirectoryPath = GetConfigurationDirectoryPath();
        
        Directory.CreateDirectory(configurationDirectoryPath);
        Directory.CreateDirectory(Path.Combine(configurationDirectoryPath, "shared"));
        Directory.CreateDirectory(Path.Combine(configurationDirectoryPath, "backups"));
        File.WriteAllText(Path.Combine(configurationDirectoryPath, "settings.json"), "{}");
        File.WriteAllText(Path.Combine(configurationDirectoryPath, "recents.json"), "{\"Recents\":[]}");
        File.WriteAllText(Path.Combine(configurationDirectoryPath, "templates.json"), "{\"Templates\":[]}");
    }

    public bool IsValidChest(string path)
    {
        return Path.Exists(path) && Path.Exists(Path.Combine(path, ".chest"));
    }

    public void CreateNewChest(string name, string path, TemplateModel? template)
    {
        if (string.IsNullOrWhiteSpace(path)) return;
        string chestPath = Path.Combine(path, name);
        Directory.CreateDirectory(chestPath);
        Directory.CreateDirectory(Path.Combine(chestPath, ".chest"));
        Directory.CreateDirectory(Path.Combine(chestPath, ".chest", "backups"));
        File.WriteAllText(Path.Combine(chestPath, "Hello.md"), newFileMessage);

        CopySharedFolderToChest(Path.Combine(GetConfigurationDirectoryPath(), "shared"), Path.Combine(chestPath, ".chest"));

        if (template != null) WriteFolderModelStructure(chestPath, null, template.Root);
    }

    private void CopySharedFolderToChest(string sharedDirectory, string chestPath)
    {
        string[] contents = GetFolderContents(sharedDirectory);
        foreach(var item in contents)
        {
            if (Directory.Exists(item)) Directory.CreateDirectory(Path.Combine(chestPath, Path.GetRelativePath(sharedDirectory, item)));
            else File.Copy(item, Path.Combine(chestPath, Path.Combine(chestPath, Path.GetRelativePath(sharedDirectory, item))));
        }
    }

    public void AddRecent(string path)
    {
        if (path.Contains('\\')) path = path.Replace('\\', Path.DirectorySeparatorChar);
        if (path.Contains('/')) path = path.Replace('/', Path.DirectorySeparatorChar);
        RecentFileList recents = new()
        {
            Recents = GetRecents()
        };

        if (recents.Recents.Contains(Path.GetFullPath(path))) return;

        if (recents.Recents.Length < 1)
        {
            recents.Recents = [path];
        } else
        {
            string[] newRecents = new string[recents.Recents.Length + 1];
            recents.Recents.CopyTo(newRecents);
            recents.Recents = newRecents;
        
            for (int i = recents.Recents.Length - 1; i > 0; i--)
            {
                recents.Recents[i] = recents.Recents[i - 1];
            } 
            recents.Recents[0] = path;
        }

        string newRecentsContent = JsonSerializer.Serialize(recents);
        File.WriteAllText(Path.Combine(GetConfigurationDirectoryPath(), "recents.json"), newRecentsContent);
    }

    public string[] GetRecents()
    {
        string recentsFileContent = File.ReadAllText(Path.Combine(GetConfigurationDirectoryPath(), "recents.json"));
        RecentFileList recents = JsonSerializer.Deserialize<RecentFileList>(recentsFileContent) ?? new();

        return recents.Recents;
    }

    public void PopRecent()
    {
        string[] recents = GetRecents();
        string[] newRecents = new string[recents.Length - 1];
        for (int i = 0; i < recents.Length - 1; i++)
        {
            newRecents[i] = recents[i + 1];
        }
        RecentFileList newRecentsModel = new() { Recents = newRecents };
        string newRecentsContent = JsonSerializer.Serialize(newRecentsModel);
        File.WriteAllText(Path.Combine(GetConfigurationDirectoryPath(), "recents.json"), newRecentsContent);
    }

    public void CreateChestSubfolder(string chestPath, string? subfolder, string name)
    {
        if (!IsValidChest(chestPath)) return;

        string createPath = chestPath;
        if (!string.IsNullOrWhiteSpace(subfolder) && Path.Exists(Path.Combine(chestPath, subfolder))) createPath = Path.Combine(chestPath, subfolder);

        if (Path.Exists(Path.Combine(createPath, name))) return;

        Directory.CreateDirectory(Path.Combine(createPath, name));
    }

    public void CreateMarkdownFile(string chestPath, string? subfolder, string name)
    {
        if (!IsValidChest(chestPath)) return;

        string createPath = chestPath;
        if (!string.IsNullOrWhiteSpace(subfolder) && Path.Exists(Path.Combine(chestPath, subfolder))) createPath = Path.Combine(chestPath, subfolder);

        if (!name.Contains(".md")) name += ".md";

        if (Path.Exists(Path.Combine(createPath, name))) return;

        File.Create(Path.Combine(createPath, name));
    }

    public void ImportPdfFile(string chestPath, string? subfolder, string existingPdfPath)
    {
        if (!IsValidChest(chestPath) || !Path.Exists(existingPdfPath)) return;
        if (GetFileType(existingPdfPath) != FileType.PDF) return;

        string destinationPath = chestPath;
        if (!string.IsNullOrWhiteSpace(subfolder) && Path.Exists(Path.Combine(chestPath, subfolder))) destinationPath = Path.Combine(chestPath, subfolder);
        destinationPath = Path.Combine(destinationPath, Path.GetFileName(existingPdfPath));

        if (Path.Exists(Path.Combine(destinationPath))) return;

        File.Copy(existingPdfPath, destinationPath);
    }

    public string[] GetChestContents(string chestPath)
    {
        if (!IsValidChest(chestPath)) return [];
        else return GetFolderContents(chestPath);
    }

    public string[] GetFolderContents(string path)
    {
        List<string> folders = [.. Directory.EnumerateDirectories(path)];
        List<string> files = [.. Directory.EnumerateFiles(path)];

        List<string> finalList = [];

        foreach(string folder in folders)
        {
            if (folder.Contains(".chest")) continue;
            finalList.Add(folder);
            string[] subContents = GetFolderContents(folder);
            if (subContents.Length > 0) finalList.AddRange(subContents);
        }

        finalList.AddRange(files);

        return [.. finalList];
    }

    public bool UserDocumentsDirectoryExists()
    {
        string documentsDirectoryPath = "";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            documentsDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Tachylite");
        } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // I am currently unable to verify that this works, so I need to test it later.
            documentsDirectoryPath = Path.Combine("~/Documents", "Tachylite");
        } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // I am currently unable to verify that this works, so I need to test it later.
            documentsDirectoryPath = Path.Combine("~/Documents", "Tachylite");
        }

        return Path.Exists(documentsDirectoryPath);
    }

    public void CreateUserDocumentsDirectory()
    {
        string documentsDirectoryPath = "";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            documentsDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Tachylite");
        } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // I am currently unable to verify that this works, so I need to test it later.
            documentsDirectoryPath = Path.Combine("~/Documents", "Tachylite");
        } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // I am currently unable to verify that this works, so I need to test it later.
            documentsDirectoryPath = Path.Combine("~/Documents", "Tachylite");
        }

        Directory.CreateDirectory(documentsDirectoryPath);
    }

    public string GetUserDocumentsDirectory()
    {
        string documentsDirectoryPath = "";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            documentsDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Tachylite");
        } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // I am currently unable to verify that this works, so I need to test it later.
            documentsDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Tachylite");
        } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // I am currently unable to verify that this works, so I need to test it later.
            documentsDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Tachylite");
        }

        return documentsDirectoryPath;
    }

    public string GetMarkdownFileContents(string filePath)
    {
        if (!File.Exists(filePath)) throw new IOException("filePath must point to an existing file");

        return File.ReadAllText(filePath);
    }

    public void CreateUserFileBackup(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath) || !UserConfigurationDirectoryExists()) return;

        string backupFileName = filePath.Replace(Path.DirectorySeparatorChar, '-').Replace(":", "");

        string[] files = [.. Directory.GetFiles(Path.Combine(GetConfigurationDirectoryPath(), "backups")).Where(file => file.Contains(backupFileName))];

        byte fileNumber = (byte)(files.Length + 1);

        for (int i = files.Length; i > 0; i--)
        {
            File.Copy(files[i-1], Path.Combine(GetConfigurationDirectoryPath(), "backups", $"{backupFileName}-{i+1}"), overwrite: true);
            File.Delete(files[i-1]);
        }

        if (fileNumber > 5) File.Delete(Path.Combine(GetConfigurationDirectoryPath(), "backups",$"{backupFileName}-6"));

        File.Copy(filePath, Path.Combine(GetConfigurationDirectoryPath(), "backups", $"{backupFileName}-1"));
    }

    public void CreatePerChestFileBackup(string chestPath, string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath) || !IsValidChest(chestPath)) return;

        string backupFileName = filePath.Replace(Path.DirectorySeparatorChar, '-').Replace(":", "");

        string[] files = [.. Directory.GetFiles(Path.Combine(chestPath, ".chest", "backups")).Where(file => file.Contains(backupFileName))];

        byte fileNumber = (byte)(files.Length + 1);

        for (int i = files.Length; i > 0; i--)
        {
            File.Copy(files[i-1], Path.Combine(chestPath, ".chest", "backups", $"{backupFileName}-{i+1}"), overwrite: true);
            File.Delete(files[i-1]);
        }

        if (fileNumber > 5) File.Delete(Path.Combine(chestPath, ".chest", "backups",$"{backupFileName}-6"));

        File.Copy(filePath, Path.Combine(chestPath, ".chest", "backups", $"{backupFileName}-1"));
    }

    public void SaveMarkdownFile(string filePath, string content)
    {
        if (!File.Exists(filePath)) throw new IOException($"File does not exist.");
        else if (GetFileType(filePath) != FileType.MD) throw new IOException($"File given is not a markdown file.");

        File.WriteAllTextAsync(filePath, content);
    }

    public void RenameFile(string filePath, string newName)
    {
        string? newFilePath = Path.GetDirectoryName(filePath);

        if (!File.Exists(filePath)) throw new IOException("File at given path does not exist.");
        else if (string.IsNullOrEmpty(newFilePath)) throw new IOException("The destination path is either null or inaccessible.");
        else if (File.Exists(newFilePath)) throw new IOException("File exists at destination path already.");

        File.Move(filePath, newFilePath);
    }
    public void RenameFolder(string folderPath, string newName)
    {
        string? newFolderPath = Path.GetDirectoryName(folderPath);

        if (!Directory.Exists(folderPath)) throw new IOException("Folder at the given path does not exist.");
        else if (string.IsNullOrEmpty(newFolderPath)) throw new IOException("The destination path is either null or inaccessible.");
        else if (Directory.Exists(newFolderPath)) throw new IOException("Folder exists at destination path already.");

        Directory.Move(folderPath, newFolderPath);
    }
    public Dictionary<string, TemplateModel> GetTemplates()
    {
        string templatePath = Path.Combine(GetConfigurationDirectoryPath(), "templates.json");
        if (!File.Exists(templatePath)) throw new IOException($"templates.json does not exist. Maybe ${GetConfigurationDirectoryPath()} was not initialized?");

        string templateJson = File.ReadAllText(templatePath);

        TemplateManagerModel templateModels = JsonSerializer.Deserialize<TemplateManagerModel>(templateJson) ?? new([]);

        return templateModels.Templates.ToDictionary(template => template.Name);
    }
    public void CreateTemplate(string chestPath, string templateName)
    {
        if (!Directory.Exists(chestPath) || !IsValidChest(chestPath)) throw new IOException("Chest is missing or invalid.");

        Dictionary<string,TemplateModel> existingTemplates = GetTemplates();

        if (existingTemplates.ContainsKey(templateName)) throw new ArgumentException("Given templateName cannot be the name of an existing template.");
        
        TemplateModel newTemplate = new(templateName, CreateFolderModel(chestPath, "."));

        existingTemplates.Add(templateName, newTemplate);

        string newTemplateJson = JsonSerializer.Serialize(new TemplateManagerModel([..existingTemplates.Values]));
        File.WriteAllText(Path.Combine(GetConfigurationDirectoryPath(), "templates.json"), newTemplateJson);
    }
    public void WriteFolderModelStructure(string chestPath, string? subPath, FolderModel folder)
    {
        string createPath = chestPath;
        if (!string.IsNullOrWhiteSpace(subPath)) createPath = Path.Combine(createPath, subPath);
        foreach (var folderModel in folder.SubFolders)
        {
            Directory.CreateDirectory(Path.Combine(createPath, folderModel.Name));
            WriteFolderModelStructure(chestPath, Path.Combine(subPath ?? "", createPath), folderModel);
        }

        foreach (var fileModel in folder.Files)
        {
            File.WriteAllBytes(Path.Combine(createPath, fileModel.Name), fileModel.Contents);
        }
    }
    public FolderModel CreateFolderModel(string path, string folderName)
    {
        List<FolderModel> subFolders = [];
        List<FileModel> files = [];
        foreach(var folder in Directory.EnumerateDirectories(path))
        {
            subFolders.Add(CreateFolderModel(folder, Path.GetFileName(folder)));
        }
        foreach(var file in Directory.EnumerateFiles(path)) files.Add(new FileModel(Path.GetFileName(file), File.ReadAllBytes(file)));

        return new FolderModel(folderName, [..subFolders], [..files]);
    }
    public byte[] ReadPdfData(string pdfPath)
    {
        if (!File.Exists(pdfPath)) throw new IOException("File does not exist.");
        else if (GetFileType(pdfPath) != FileType.PDF) throw new IOException("File is not a PDF.");

        return File.ReadAllBytes(pdfPath);
    }
    public void WritePdfData(string pdfPath, byte[] pdfData)
    {
        File.WriteAllBytes(pdfPath, pdfData);
    }
}
