using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;
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
            configurationDirectoryPath = Path.Combine("~/.config", "tachylite");
        } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // I am currently unable to verify that this works, so I need to test it later.
            configurationDirectoryPath = Path.Combine("~/Library/Application Support", "Tachylite");
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
    }

    public bool IsValidChest(string path)
    {
        return Path.Exists(path) && Path.Exists(Path.Combine(path, ".chest"));
    }

    public void CreateNewChest(string name, string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return;
        string chestPath = Path.Combine(path, name);
        Directory.CreateDirectory(chestPath);
        Directory.CreateDirectory(Path.Combine(chestPath, ".chest"));
        File.WriteAllText(Path.Combine(chestPath, "Hello.md"), newFileMessage);
    }

    public void AddRecent(string path)
    {
        RecentFileList recents = new()
        {
            Recents = GetRecents()
        };

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
            documentsDirectoryPath = Path.Combine("~/Documents", "Tachylite");
        } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // I am currently unable to verify that this works, so I need to test it later.
            documentsDirectoryPath = Path.Combine("~/Documents", "Tachylite");
        }

        return documentsDirectoryPath;
    }
}
