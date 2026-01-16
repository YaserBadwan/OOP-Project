using Microsoft.Extensions.Logging;
using PhoneBook.ConsoleApp.Abstractions;
using PhoneBook.Core.Exceptions;

namespace PhoneBook.ConsoleApp.CLI;

public sealed class CommandLoop : ICommandLoop
{
    private readonly ILogger<CommandLoop> _logger;
    private readonly IConsole _console;
    private readonly ICommandDispatcher _dispatcher;
    
    public CommandLoop(ILogger<CommandLoop> logger, IConsole console, ICommandDispatcher dispatcher)
    {
        _logger = logger;
        _console = console;
        _dispatcher = dispatcher;
    }

    public void Run()
    {
        _logger.LogInformation("CommandLoop started.");
        
        while (true)
        {
            _console.Write("PhoneBook> ");
            var input =  _console.ReadLine();

            if (input is null)
            {
                return;
            }
            
            input = input.Trim();
            if (input.Length == 0)
            {
                continue;
            }
            
            try
            {
                var keepRunning = _dispatcher.TryDispatch(input);
                if (!keepRunning)
                {
                    return;
                }
            }
            catch (CommandAbortedException)
            {
                _console.WriteWarning("! Cancelled.");
            }
            catch (DomainException ex)
            {
                _console.WriteError("✗ " + ex.Message);
                if (ex.InnerException is not null)
                {
                    _logger.LogWarning(
                        ex.InnerException,
                        "Domain exception occurred: {Message}",
                        ex.Message);
                }
                else
                {
                    _logger.LogWarning(
                        ex,
                        "Domain exception occurred without inner exception: {Message}",
                        ex.Message);
                }
            }
            catch (Exception ex)
            {
                _console.WriteError("✗ Unexpected error. Please try again.");
                _logger.LogError(
                    ex,
                    "Unhandled non-domain exception in CommandLoop");
            }
        }
    }
}