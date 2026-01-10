using Microsoft.Extensions.Logging;
using PhoneBook.ConsoleApp.Abstractions;
using PhoneBook.ConsoleApp.Presentation;
using PhoneBook.Core.Abstractions;
using PhoneBook.Core.Exceptions;
using PhoneBook.Core.Models;
using PhoneBook.Core.Services;

namespace PhoneBook.ConsoleApp.Application;

public sealed class ConsolePhoneBookApp
{
    private readonly ILogger<ConsolePhoneBookApp> _logger;
    private readonly IConsole _console;
    private readonly ICommandLoop _loop;

    public ConsolePhoneBookApp(ILogger<ConsolePhoneBookApp> logger, IConsole console, ICommandLoop loop)
    {
        _logger = logger;
        _console = console;
        _loop = loop;
        
    }

    public void Run()
    {
        try
        {
            ConsoleLayout.PrintTitle(_console, "PhoneBook CLI", "Type 'help' to see available commands.");
            _loop.Run();
            _console.WriteLine("");
            _console.WriteSuccess("✓ Bye.");
        }
        catch (DomainException ex)
        {
            _console.WriteError(ex.Message);
            _logger.LogError(ex, "Unhandled DomainException: {Message}", ex.Message);
        }
        catch (Exception ex)
        {
            _console.WriteError("Unexpected error. Please check logs.");
            _logger.LogError(ex, "Unhandled non-domain exception.");
        }
    }
}