using System.Net.Security;
using System.Runtime.InteropServices;
using Tachylite.Core.Models;
using Tachylite.Core.Services;
using Tachylite.Desktop.Services;

namespace Tachylite.Tests;

public class TestFileSystemService
{
    private FileSystemService? fileSystemService;

    private void DeleteUserConfigurationDirectory()
    {
        fileSystemService = new();
        if (Directory.Exists(fileSystemService.GetConfigurationDirectoryPath()))
        {
            Directory.Delete(fileSystemService.GetConfigurationDirectoryPath(), true);
        }
    }

    [Fact]
    public void GetFileType_FileSystemService_Desktop_CorrectlyIdentifiesMarkdownFile()
    {
        // Arrange
        fileSystemService = new FileSystemService();
        string markdownFilePath = "~/Desktop/secrets/awesome.md";
        FileType expected = FileType.MD;

        // Act
        FileType actual = fileSystemService.GetFileType(markdownFilePath);

        // Assert
        Assert.Equal(actual, expected);
    }
    
    // [Fact]
    // public void GetFileType_FileSystemService_Desktop_CorrectlyIdentifiesLastExtension()
    // {
    //     // Arrange
    //     fileSystemService = new FileSystemService();
    //     string markdownFilePath = "~/Desktop/secrets/awesome.pdf.md";
    //     FileType expected = FileType.MD;

    //     // Act
    //     FileType actual = fileSystemService.GetFileType(markdownFilePath);

    //     // Assert
    //     Assert.Equal(actual, expected);
    // }

    // [Fact]
    // public void GetFileType_FileSystemService_Desktop_CorrectlyIdentifiesPdfFile()
    // {
    //     // Arrange
    //     fileSystemService = new FileSystemService();
    //     string pdfFilePath = "C:\\Docs\\Docs2\\w.pdf";
    //     FileType expected = FileType.PDF;

    //     // Act
    //     FileType actual = fileSystemService.GetFileType(pdfFilePath);

    //     // Assert
    //     Assert.Equal(actual, expected);
    // }

    // [Fact]
    // public void GetFileType_FileSystemService_Desktop_CorrectlyIdentifiesRegardlessOfCasing()
    // {
    //     // Arrange
    //     fileSystemService = new FileSystemService();
    //     string pdfFilePath = "C:\\Docs\\Docs2\\W.PDF";
    //     FileType expected = FileType.PDF;

    //     // Act
    //     FileType actual = fileSystemService.GetFileType(pdfFilePath);

    //     // Assert
    //     Assert.Equal(actual, expected);
    // }

    // [Fact]
    // public void GetFileType_FileSystemService_Desktop_CorrectlyIdentifiesJsonFile()
    // {
    //     // Arrange
    //     fileSystemService = new FileSystemService();
    //     string jsonFilePath = ".config.json";
    //     FileType expected = FileType.JSON;

    //     // Act
    //     FileType actual = fileSystemService.GetFileType(jsonFilePath);

    //     // Assert
    //     Assert.Equal(actual, expected);
    // }

    // [Fact]
    // public void GetFileType_FileSystemService_Desktop_CorrectlyIdentifiesUnknownFile()
    // {
    //     // Arrange
    //     fileSystemService = new FileSystemService();
    //     string textFilePath = "/src/wheat.txt";
    //     FileType expected = FileType.UNKNOWN;

    //     // Act
    //     FileType actual = fileSystemService.GetFileType(textFilePath);

    //     // Assert
    //     Assert.Equal(actual, expected);
    // }

    // [Fact]
    // public void InitializeUserConfigurationDirectory_FileSystemService_Desktop_CorrectlyCreatesConfigurationDirectory()
    // {
    //     // Arrange
    //     fileSystemService = new FileSystemService();
    //     deleteUserConfigurationDirectory(fileSystemService);
    //     fileSystemService.InitializeUserConfigurationDirectory();

    //     // Act
    //     fileSystemService.InitializeUserConfigurationDirectory();
    //     bool exists = fileSystemService.UserConfigurationDirectoryExists();

    //     // Assert
    //     Assert.True(exists);
    // }

    // [Fact]
    // public void IsValidChest_FileSystemService_Desktop_CorrectlyIdentifiesValid()
    // {
    //     fileSystemService = new FileSystemService();

    //     // Assert
    //     Assert.True(fileSystemService.IsValidChest("mock\\chest"));
    // }

    // [Fact]
    // public void IsValidChest_FileSystemService_Desktop_CorrectlyIdentifiesInvalid()
    // {
    //     fileSystemService = new FileSystemService();

    //     // Assert
    //     Assert.True(fileSystemService.IsValidChest("mock\\notchest"));
    // }

    // [Fact]
    // public void CreateNewChest_FileSystemService_Desktop_CorrectlyCreatesChest()
    // {
    //     // Arrange
    //     fileSystemService = new FileSystemService();
    //     string path = ".";
    //     string name = "chungus";
    //     string chestPath = Path.Combine(path, name);
    //     bool exists = false;

    //     //Act
    //     fileSystemService.CreateNewChest(path, name);
    //     exists = Path.Exists(chestPath) && 
    //         Path.Exists(Path.Combine(chestPath, ".chest")) &&
    //         File.Exists(Path.Combine(chestPath, "Hello.md"));

    //     // Assert
    //     Assert.True(exists);
    // }

    // [Fact]
    // public void GetRecents_FileSystemService_Desktop_CorrectlyReadsRecents()
    // {
    //     // Arrange
    //     fileSystemService = new FileSystemService();
    //     deleteUserConfigurationDirectory(fileSystemService);
    //     fileSystemService.InitializeUserConfigurationDirectory();
        
    //     string[] expected = [];

    //     //Act
    //     string[] actual = fileSystemService.GetRecents();

    //     // Assert
    //     Assert.Equal(expected, actual);
    // }

    // [Fact]
    // public void AddRecent_FileSystemService_Desktop_CorrectlyAddsRecent()
    // {
    //     // Arrange
    //     fileSystemService = new FileSystemService();
    //     deleteUserConfigurationDirectory(fileSystemService);
    //     fileSystemService.InitializeUserConfigurationDirectory();
        
    //     string pathToAdd = "~/testpath/chest";
    //     string[] expected = ["~/testpath/chest"];

    //     //Act
    //     fileSystemService.AddRecent(pathToAdd);
    //     string[] actual = fileSystemService.GetRecents();

    //     // Assert
    //     Assert.Equal(expected, actual);
    // }

    // [Fact]
    // public void AddRecent_FileSystemService_Desktop_CorrectlyAddsRecentWhenMultipleExist()
    // {
    //     // Arrange
    //     fileSystemService = new FileSystemService();
    //     deleteUserConfigurationDirectory(fileSystemService);
    //     fileSystemService.InitializeUserConfigurationDirectory();

    //     string pathToAdd = "~/testpath/chest";
    //     string[] expected = ["~/testpath/chest", "~/testpath/chest"];

    //     //Act
    //     fileSystemService.AddRecent(pathToAdd);
    //     fileSystemService.AddRecent(pathToAdd);
    //     string[] actual = fileSystemService.GetRecents();

    //     // Assert
    //     Assert.Equal(expected, actual);
    // }

    // [Fact]
    // public void AddRecent_FileSystemService_Desktop_CorrectlyHandlesMaximumOfTen()
    // {
    //     // Arrange
    //     fileSystemService = new FileSystemService();
    //     deleteUserConfigurationDirectory(fileSystemService);
    //     fileSystemService.InitializeUserConfigurationDirectory();

    //     string pathToAdd = "~/testpath/chests";
    //     string fillerPath = "~/testpath/chest";
    //     string lastPath = "~/testpath/chests";
    //     string[] expected = [
    //         "~/testpath/chests", 
    //         "~/testpath/chest", 
    //         "~/testpath/chest", 
    //         "~/testpath/chest", 
    //         "~/testpath/chest", 
    //         "~/testpath/chest", 
    //         "~/testpath/chest", 
    //         "~/testpath/chest", 
    //         "~/testpath/chest", 
    //         "~/testpath/chest"];

    //     //Act
    //     fileSystemService.AddRecent(lastPath);
    //     for (int i = 0; i < 9; i++)
    //     {
    //         fileSystemService.AddRecent(fillerPath);
    //     }
    //     fileSystemService.AddRecent(pathToAdd);
    //     string[] actual = fileSystemService.GetRecents();

    //     // Assert
    //     Assert.Equal(expected, actual);
    // }

    // [Fact]
    // public void CreateChestSubfolder_FileSystemService_Desktop_CorrectlyCreatesSubfolder()
    // {
        
    //     //Arrange
    //     fileSystemService = new FileSystemService();

    //     string pathToChest = "C:\\Users\\nwillett\\Downloads\\che";

    //     //Act
    //     fileSystemService.CreateChestSubfolder(pathToChest, null, "Subfolder");

    //     // Assert
    //     Assert.True(Path.Exists("C:\\Users\\nwillett\\Downloads\\che\\Subfolder"));
    // }

    // [Fact]
    // public void CreateChestSubfolder_FileSystemService_Desktop_CorrectlyCreatesSubfolderInSubfolder()
    // {
        
    //     //Arrange
    //     fileSystemService = new FileSystemService();

    //     string pathToChest = "C:\\Users\\nwillett\\Downloads\\che";

    //     //Act
    //     fileSystemService.CreateChestSubfolder(pathToChest, "Subfolder", "Subfolder2");

    //     // Assert
    //     Assert.True(Path.Exists("C:\\Users\\nwillett\\Downloads\\che\\Subfolder\\Subfolder2"));
    // }

    // [Fact]
    // public void CreateMarkdownFile_FileSystemService_Desktop_CorrectlyCreatesMarkdownFile()
    // {
        
    //     //Arrange
    //     fileSystemService = new FileSystemService();

    //     string pathToChest = "C:\\Users\\nwillett\\Downloads\\che";
    //     string fileName = "Markdo";

    //     //Act
    //     fileSystemService.CreateMarkdownFile(pathToChest, null, fileName);

    //     // Assert
    //     Assert.True(File.Exists("C:\\Users\\nwillett\\Downloads\\che\\Markdo.md"));
    // }

    // [Fact]
    // public void CreateMarkdownFile_FileSystemService_Desktop_CorrectlyHandlesFileWithMDExtension()
    // {
        
    //     //Arrange
    //     fileSystemService = new FileSystemService();

    //     string pathToChest = "C:\\Users\\nwillett\\Downloads\\che";
    //     string fileName = "Markdot.md";
    //     //Act
    //     fileSystemService.CreateMarkdownFile(pathToChest, null, fileName);

    //     // Assert
    //     Assert.True(File.Exists("C:\\Users\\nwillett\\Downloads\\che\\Markdot.md"));
    // }

    // [Fact]
    // public void ImportPdfFile_FileSystemService_Desktop_CorrectlyCopiesPdfFile()
    // {
        
    //     //Arrange
    //     fileSystemService = new FileSystemService();

    //     string pathToChest = "C:\\Users\\nwillett\\Downloads\\che";
    //     string pdfPath = "C:\\Users\\nwillett\\Downloads\\Interactive_DnD_3.5_Character_Sheet.pdf";
    //     //Act
    //     fileSystemService.ImportPdfFile(pathToChest, null, pdfPath);

    //     // Assert
    //     Assert.True(File.Exists("C:\\Users\\nwillett\\Downloads\\che\\Interactive_DnD_3.5_Character_Sheet.pdf") && File.Exists(pdfPath));
    // }

    // [Fact]
    // public void ImportPdfFile_FileSystemService_Desktop_IgnoresNonPdfFiles()
    // {
        
    //     //Arrange
    //     fileSystemService = new FileSystemService();

    //     string pathToChest = "C:\\Users\\nwillett\\Downloads\\che";
    //     string pdfPath = "C:\\Users\\nwillett\\Downloads\\logisim-generic-2.7.1.jar";

    //     //Act
    //     fileSystemService.ImportPdfFile(pathToChest, null, pdfPath);

    //     // Assert
    //     Assert.True(!File.Exists("C:\\Users\\nwillett\\Downloads\\che\\logisim-generic-2.7.1.jar") && File.Exists(pdfPath));
    // }

    // [Fact]
    // public void GetChestContents_FileSystemService_Desktop_GetsContentsCorrectly()
    // {
        
    //     //Arrange
    //     fileSystemService = new FileSystemService();

    //     string pathToChest = "C:\\Users\\nwillett\\Desktop\\Test";
    //     string[] expected = [
    //         "C:\\Users\\nwillett\\Desktop\\Test\\balding_pics",
    //         "C:\\Users\\nwillett\\Desktop\\Test\\balding_pics\\my_wiener.md",
    //         "C:\\Users\\nwillett\\Desktop\\Test\\Balls",
    //         "C:\\Users\\nwillett\\Desktop\\Test\\shud",
    //         "C:\\Users\\nwillett\\Desktop\\Test\\shud\\Interactive_DnD_3.5_Character_Sheet.pdf",
    //         "C:\\Users\\nwillett\\Desktop\\Test\\Cock.md",
    //         "C:\\Users\\nwillett\\Desktop\\Test\\Hello.md",
    //     ];

    //     //Act
    //     string[] actual = fileSystemService.GetChestContents(pathToChest);

    //     // Assert
    //     Assert.Equal(expected, actual);
    // }

    // [Fact]
    // public void CreateUserDocumentsDirectory_FileSystemService_Desktop_CreatesDirectory()
    // {
        
    //     //Arrange
    //     fileSystemService = new FileSystemService();

    //     //Act
    //     fileSystemService.CreateUserDocumentsDirectory();
    //     bool exists = fileSystemService.UserDocumentsDirectoryExists();

    //     // Assert
    //     Assert.True(exists);
    // }

    // [Fact]
    // public void CreateUserFileBackup_FileSystemService_Desktop_CreatesFile()
    // {
        
    //     //Arrange
    //     fileSystemService = new FileSystemService();

    //     //Act
    //     fileSystemService.CreateUserFileBackup("C:\\Users\\nwillett\\Documents\\Tachylite\\Balls2\\Hello.md");

    //     // Assert
    // }

    // [Fact]
    // public void CreatePerChestFileBackup_FileSystemService_Desktop_CreatesFile()
    // {
        
    //     //Arrange
    //     fileSystemService = new FileSystemService();

    //     //Act
    //     fileSystemService.CreatePerChestFileBackup("C:\\Users\\nwillett\\Documents\\Tachylite\\Balls2\\", "C:\\Users\\nwillett\\Documents\\Tachylite\\Balls2\\Hello.md");

    //     // Assert
    // }
}
