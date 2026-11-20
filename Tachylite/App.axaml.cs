using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Tachylite.Core.Services;
using Tachylite.Factories;
using Tachylite.ViewModels;
using Tachylite.Views;

namespace Tachylite;

public partial class App : Application
{
    public static Action<IServiceCollection>? ConfigurePlatformSpecificServices { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
#if DEBUG
        this.AttachDeveloperTools();
#endif
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        collection.AddCommonServices();
        ConfigurePlatformSpecificServices!.Invoke(collection);

        var services = collection.BuildServiceProvider();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            IFileSystemService fileSystemService = services.GetRequiredService<IFileSystemService>();

            if (!fileSystemService.UserConfigurationDirectoryExists()) fileSystemService.InitializeUserConfigurationDirectory();

            string[] recents = fileSystemService.GetRecents();

            foreach (string path in recents)
            {
                if (fileSystemService.IsValidChest(path))
                {
                    desktop.MainWindow = new EditorWindow
                    {
                        DataContext = services.GetRequiredService<EditorViewModelFactory>().CreateEditorViewModel(path)
                    };
                    break;
                }
                else
                {
                    fileSystemService.PopRecent();
                }
            }

            recents = fileSystemService.GetRecents();

            if (recents.Length < 1)
            {
                desktop.MainWindow = new HomeWindow
                {
                    DataContext = services.GetRequiredService<HomeWindowViewModel>()
                };
            }

        }
        // else if (ApplicationLifetime is IActivityApplicationLifetime singleViewFactoryApplicationLifetime)
        // {
        //     singleViewFactoryApplicationLifetime.MainViewFactory = () => new MainView { DataContext = new MainViewModel() };
        // }
        // else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        // {
        //     singleViewPlatform.MainView = new MainView
        //     {
        //         DataContext = new MainViewModel()
        //     };
        // }

        base.OnFrameworkInitializationCompleted();
    }
}