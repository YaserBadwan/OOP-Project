using PhoneBook.ConsoleApp.Abstractions;
using PhoneBook.ConsoleApp.Presentation;
using PhoneBook.Core.Abstractions;
using PhoneBook.Core.Exceptions;
using PhoneBook.Core.Models;
using PhoneBook.Core.Services;

namespace PhoneBook.ConsoleApp.Application;

public sealed class ConsolePhoneBookApp
{
    private readonly IConsole _console;
    private readonly ICommandLoop _loop;

    public ConsolePhoneBookApp(IConsole console, ICommandLoop loop)
    {
        _console = console;
        _loop = loop;
        
    }

    public void Run()
    {
        ConsoleLayout.PrintTitle(_console, "PhoneBook CLI", "Type 'help' to see available commands.");
        _loop.Run();
        _console.WriteLine("");
        _console.WriteSuccess("✓ Bye.");
    }
}