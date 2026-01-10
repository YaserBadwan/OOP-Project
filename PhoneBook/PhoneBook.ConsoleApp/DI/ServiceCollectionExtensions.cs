using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PhoneBook.ConsoleApp.CLI;

namespace PhoneBook.ConsoleApp.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAllCommandsFromConsoleApp(this IServiceCollection services)
    {
        var commandInterface = typeof(ICommand);

        var assembly = Assembly.GetExecutingAssembly();

        var commandTypes = assembly
            .GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .Where(t => commandInterface.IsAssignableFrom(t));

        foreach (var type in commandTypes)
        {
            services.AddSingleton(commandInterface, type);
        }

        return services;
    }
}