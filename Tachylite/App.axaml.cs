using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Tachylite.Core.Services;
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
<<<<<<< Updated upstream
=======

        var services = collection.BuildServiceProvider();
>>>>>>> Stashed changes

        var services = collection.BuildServiceProvider();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            IFileSystemService fileSystemService = services.GetRequiredService<IFileSystemService>();

            string[] recents = fileSystemService.GetRecents();

            foreach (string path in recents)
            {
                if (fileSystemService.IsValidChest(path))
                {
                    desktop.MainWindow = new EditorWindow
                    {
                        DataContext = new EditorViewModel(path, fileSystemService, services.GetRequiredService<IValidationService>())
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
                    DataContext = new HomeWindowViewModel(recents, fileSystemService, services.GetRequiredService<IValidationService>())
                };
            }

        }
        else if (ApplicationLifetime is IActivityApplicationLifetime singleViewFactoryApplicationLifetime)
        {
            singleViewFactoryApplicationLifetime.MainViewFactory = () => new MainView { DataContext = new MainViewModel() };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}