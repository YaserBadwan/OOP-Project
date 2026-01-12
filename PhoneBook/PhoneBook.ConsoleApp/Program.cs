using Microsoft.Extensions.Configuration;
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
using PhoneBook.Infrastructure.MariaDbStorage;
using PhoneBook.Infrastructure.Phone;

// TERMINAL COMMANDS FOR STORAGE CHOICE
// dotnet run --project PhoneBook.ConsoleApp --launch-profile Json
// dotnet run --project PhoneBook.ConsoleApp --launch-profile InMemory
// dotnet run --project PhoneBook.ConsoleApp --launch-profile MariaDb

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
    
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(AppContext.BaseDirectory);

        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
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
        
        services.Configure<MariaDbStorageOptions>(
            context.Configuration.GetSection("PhoneBook:Storage:MariaDb"));
        
        var provider =
            Environment.GetEnvironmentVariable("PhoneBook__Storage__Provider")
            ?? context.Configuration["PhoneBook:Storage:Provider"]
            ?? "Json";

        var configuredPath = context.Configuration["PhoneBook:Storage:Path"];
        var jsonPath = string.IsNullOrWhiteSpace(configuredPath)
            ? AppPaths.ResolveDataFile("phonebook.json")
            : (Path.IsPathRooted(configuredPath)
                ? configuredPath
                : AppPaths.ResolveDataFile(configuredPath));

        if (string.Equals(provider, "InMemory", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton<IPhoneBookStateStorage, InMemoryPhoneBookStateStorage>();
        }
        else if (string.Equals(provider, "MariaDb", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton<IPhoneBookStateStorage, MariaDbPhoneBookStateStorage>();
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

        services.AddSingleton(sp =>
        {
            var storage = sp.GetRequiredService<IPhoneBookStateStorage>();

            return storage switch
            {
                InMemoryPhoneBookStateStorage =>
                    new StorageInfo("InMemory", "no persistence"),

                JsonFilePhoneBookStateStorage =>
                    new StorageInfo("Json", jsonPath),

                MariaDbPhoneBookStateStorage =>
                    new StorageInfo("MariaDb", "localhost:3307/phonebook"),

                _ =>
                    new StorageInfo(storage.GetType().Name, "Unknown")
            };
        });
        
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
