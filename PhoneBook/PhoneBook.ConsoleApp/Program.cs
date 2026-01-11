using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PhoneBook.ConsoleApp.Abstractions;
using PhoneBook.ConsoleApp.Application;
using PhoneBook.ConsoleApp.CLI;
using PhoneBook.ConsoleApp.DI;
using PhoneBook.ConsoleApp.Infrastructure;
using PhoneBook.ConsoleApp.Infrastructure.Logging;
using PhoneBook.ConsoleApp.Presentation;
using PhoneBook.Core.Abstractions;
using PhoneBook.Core.Services;
using PhoneBook.Core.Storage;
using PhoneBook.Infrastructure.InMemory;
using PhoneBook.Infrastructure.JsonStorage;
using PhoneBook.Infrastructure.Phone;

// TERMINAL COMMANDS FOR STORAGE CHOICE
// dotnet run --project PhoneBook.ConsoleApp --launch-profile Json
// dotnet run --project PhoneBook.ConsoleApp --launch-profile InMemory

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((context, logging) =>
    {
        logging.ClearProviders();

        var logsDir = AppPaths.LogsDir;
        Directory.CreateDirectory(logsDir);

        logging.AddFile(new FileLoggerOptions
        {
            DirectoryPath = logsDir,
            FileNamePrefix = context.Configuration["Logging:File:Prefix"] ?? "phonebook"
        });

        logging.AddDebug();
    })
    .ConfigureServices((context, services) =>
    {
        // ========= UI =========
        services.AddSingleton<IConsole, SystemConsole>();
        services.AddSingleton<IContactPresenter, ConsoleContactPresenter>();
        services.AddSingleton<ConsoleLayout>();
        services.AddSingleton<ConsolePhoneBookApp>();

        // ========= Core =========
        services.AddSingleton<PhoneBookService>();

        // ========= Infrastructure =========
        services.AddSingleton<IPhoneNumberNormalizer, LibPhoneNumberNormalizer>();

        var provider = context.Configuration["PhoneBook:Storage:Provider"] ?? "Json";

        var configuredPath = context.Configuration["PhoneBook:Storage:Path"]; 
        var jsonPath = string.IsNullOrWhiteSpace(configuredPath)
            ? AppPaths.ResolveDataFile("phonebook.json")
            : (Path.IsPathRooted(configuredPath)
                ? configuredPath
                : AppPaths.ResolveDataFile(configuredPath));

        services.AddSingleton(new StorageInfo(provider, jsonPath));

        if (string.Equals(provider, "InMemory", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton<IPhoneBookStateStorage, InMemoryPhoneBookStateStorage>();
        }
        else
        {
            Directory.CreateDirectory(AppPaths.DataDir);

            services.AddSingleton(new JsonFileStorageOptions
            {
                FilePath = jsonPath,
                PrettyPrint = true
            });

            services.AddSingleton<IPhoneBookStateStorage, JsonFilePhoneBookStateStorage>();
        }

        // ========= CLI =========
        services.AddSingleton<CommandContext>();
        services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
        services.AddSingleton<ICommandLoop, CommandLoop>();

        services.AddAllCommandsFromConsoleApp();
    })
    .Build();

host.Services.GetRequiredService<ILoggerFactory>()
    .CreateLogger("Startup")
    .LogInformation("App started.");

host.Services.GetRequiredService<ConsolePhoneBookApp>().Run();
