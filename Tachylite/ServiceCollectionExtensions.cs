using System;
using Microsoft.Extensions.DependencyInjection;
using Tachylite.Core.Services;
using Tachylite.ViewModels;

namespace Tachylite;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddTransient<IValidationService, ValidationService>();
        collection.AddTransient<IDiceRollerService, DiceRollerService>();
        collection.AddTransient<DiceRollerViewModel>();
    }
}
