using System;
using Microsoft.Extensions.DependencyInjection;
using Tachylite.Core.Models;
using Tachylite.Core.Services;
using Tachylite.Factories;
using Tachylite.ViewModels;

namespace Tachylite;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddTransient<IValidationService, ValidationService>();
        collection.AddTransient<IUndoService, UndoService>();
        collection.AddTransient<IDiceRollerService, DiceRollerService>();
        collection.AddSingleton<IPdfService, PdfService>();
        collection.AddTransient<DiceRollerViewModel>();
        collection.AddTransient<HomeViewModel>();
        collection.AddTransient<NewChestViewModel>();

        collection.AddSingleton<Func<string, EditorViewModel>>(serviceProvider =>
            path => ActivatorUtilities.CreateInstance<EditorViewModel>(serviceProvider, path));

        collection.AddSingleton<Func<string, MarkdownEditorViewModel>>(serviceProvider =>
            path => ActivatorUtilities.CreateInstance<MarkdownEditorViewModel>(serviceProvider, path));

        collection.AddSingleton<Func<string, PdfEditorViewModel>>(serviceProvider =>
            path => ActivatorUtilities.CreateInstance<PdfEditorViewModel>(
            serviceProvider, path, PdfPageBackgroundImageType.Bitmap));

        collection.AddSingleton<Func<HomeWindowViewModel>>(serviceProvider =>
            () => serviceProvider.GetRequiredService<HomeWindowViewModel>());

        collection.AddSingleton<EditorViewModelFactory>();
        collection.AddTransient<HomeWindowViewModel>();
    }
}
