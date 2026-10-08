using System;
using Microsoft.Extensions.DependencyInjection;
using Tachylite.Core.Services;

namespace Tachylite;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddTransient<IValidationService, ValidationService>();
    }
}
