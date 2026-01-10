using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhoneBook.ConsoleApp.Abstractions;
using PhoneBook.ConsoleApp.Application;
using PhoneBook.ConsoleApp.CLI;
using PhoneBook.ConsoleApp.DI;
using PhoneBook.ConsoleApp.Infrastructure;
using PhoneBook.ConsoleApp.Presentation;
using PhoneBook.Core.Abstractions;
using PhoneBook.Core.Services;
using PhoneBook.Core.Storage;
using PhoneBook.Infrastructure.JsonStorage;
using PhoneBook.Infrastructure.Phone;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
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

        services.AddSingleton(new JsonFileStorageOptions
        {
            FilePath = context.Configuration["PhoneBook:Storage:Path"] ?? "phonebook.json",
            PrettyPrint = true
        });

        services.AddSingleton<IPhoneBookStateStorage, JsonFilePhoneBookStateStorage>();

        // ========= CLI =========
        services.AddSingleton<CommandContext>();
        services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
        services.AddSingleton<ICommandLoop, CommandLoop>();

        // AUTO-REGISTER ALL ICommand
        services.AddAllCommandsFromConsoleApp();
    })
    .Build();

// START LOOP
host.Services.GetRequiredService<ConsolePhoneBookApp>().Run();
;