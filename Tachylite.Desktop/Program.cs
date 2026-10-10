using System;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Tachylite.Core.Services;
using Tachylite.Desktop.Services;

namespace Tachylite.Desktop;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        App.ConfigurePlatformSpecificServices = services =>
        {
            services.AddSingleton<IFileSystemService, FileSystemService>();
        };

        BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
