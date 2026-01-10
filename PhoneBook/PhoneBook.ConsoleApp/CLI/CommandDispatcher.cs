namespace PhoneBook.ConsoleApp.CLI;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly CommandContext _context;
    private readonly IReadOnlyDictionary<string, ICommand> _commands;
    
    public CommandDispatcher(CommandContext context, IEnumerable<ICommand> commands)
    {
        _context = context;

        _commands = commands.ToDictionary(
            c => c.Verb,
            c => c,
            StringComparer.OrdinalIgnoreCase);
    }
    
    public bool TryDispatch(string inputLine)
    {
        var tokens = Tokenize(inputLine);
        if (tokens.Length == 0)
        {
            return true;
        }

        var verb = tokens[0];
        var args = tokens.Skip(1).ToArray();

        if (verb.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
            verb.Equals("quit", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        
        if (!_commands.TryGetValue(verb, out var cmd))
        {
            _context.Console.WriteWarning("! Unknown command. Type 'help'.");
            return true;
        }

        cmd.Execute(_context, args);
        return true;
    }
    
    private static string[] Tokenize(string input)
        => input.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}