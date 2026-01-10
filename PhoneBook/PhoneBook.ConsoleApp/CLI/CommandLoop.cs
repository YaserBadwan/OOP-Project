using PhoneBook.ConsoleApp.Abstractions;
using PhoneBook.Core.Exceptions;

namespace PhoneBook.ConsoleApp.CLI;

public sealed class CommandLoop : ICommandLoop
{
    private readonly IConsole _console;
    private readonly ICommandDispatcher _dispatcher;
    
    public CommandLoop(IConsole console, ICommandDispatcher dispatcher)
    {
        _console = console;
        _dispatcher = dispatcher;
    }

    public void Run()
    {

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
            catch (DomainException ex)
            {
                _console.WriteError("✗ " + ex.Message);
            }
            catch (Exception)
            {
                _console.WriteError("✗ Unexpected error. Please try again.");
            }
        }
    }
}