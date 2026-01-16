using PhoneBook.ConsoleApp.Abstractions;
using PhoneBook.Core.Services;

namespace PhoneBook.ConsoleApp.CLI;

public sealed class CommandContext
{
    public IConsole Console { get; }
    public PhoneBookService Service { get; }
    public IPrompt Prompt { get; }


    public CommandContext(IConsole console, PhoneBookService service, IPrompt prompt)
    {
        Console = console;
        Service = service;
        Prompt = prompt;
    }
}