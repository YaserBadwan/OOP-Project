using PhoneBook.ConsoleApp.Abstractions;
using PhoneBook.ConsoleApp.Application;
using PhoneBook.ConsoleApp.CLI;
using PhoneBook.ConsoleApp.Cli.Commands;
using PhoneBook.ConsoleApp.CLI.Commands;
using PhoneBook.ConsoleApp.Infrastructure;
using PhoneBook.ConsoleApp.Presentation;
using PhoneBook.Core.Abstractions;
using PhoneBook.Core.Exceptions;
using PhoneBook.Core.Services;
using PhoneBook.Core.Storage;
using PhoneBook.Infrastructure.JsonStorage;
using PhoneBook.Infrastructure.Phone;

namespace PhoneBook.ConsoleApp;

internal static class Program
{
    private static void Main()
    {
        IConsole console = new SystemConsole();
        IPhoneBookStateStorage storage = new JsonFilePhoneBookStateStorage(new JsonFileStorageOptions());
        IPhoneNumberNormalizer normalizer = new LibPhoneNumberNormalizer();
        var presenter = new ConsoleContactPresenter(console);
        PhoneBookService service;
        

        try
        {
            service = new PhoneBookService(storage, normalizer);
        }
        catch (DomainException ex)
        {
            console.WriteLine("STARTUP ERROR: " + ex.Message);
            
            if (ex.InnerException is not null)
            {
                console.WriteLine("TECHNICAL: " + ex.InnerException.GetType().Name + " - " + ex.InnerException.Message);
            }
            
            return;
        }

        var commandContext = new CommandContext(console, service);
        
        ICommand[] commands =
        {
            new HelpCommand(),
            new ListCommand(presenter),
            new AddCommand(),
            new DeleteCommand(presenter),
            new SearchCommand(presenter),
            new EditCommand(),
            new ShowCommand(presenter),
        };
        
        ICommandDispatcher dispatcher = new CommandDispatcher(commandContext, commands);
        ICommandLoop loop = new CommandLoop(console, dispatcher);
        
        var app = new ConsolePhoneBookApp(console, loop);
        app.Run();
    }
}
