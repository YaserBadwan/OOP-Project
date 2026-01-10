using PhoneBook.ConsoleApp.Abstractions;
using PhoneBook.Core.Services;

namespace PhoneBook.ConsoleApp.CLI;

public sealed class CommandContext
{
    public IConsole Console { get; }
    public PhoneBookService Service { get; }


    public CommandContext(IConsole console, PhoneBookService service)
    {
        Console = console;
        Service = service;
    }
}