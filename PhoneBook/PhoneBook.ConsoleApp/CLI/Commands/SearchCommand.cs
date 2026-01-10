using PhoneBook.ConsoleApp.Abstractions;
using PhoneBook.ConsoleApp.Presentation;

namespace PhoneBook.ConsoleApp.CLI.Commands;

public sealed class SearchCommand : ICommand
{
    public string Verb => "search";
    private readonly IContactPresenter _presenter;

    public SearchCommand(IContactPresenter presenter)
    {
        _presenter = presenter;
    }
    
    public void Execute(CommandContext context, string[] args)
    {
        var query = args.Length >= 1
            ? string.Join(' ', args) 
            : ReadRequired(context, "Search: ").Trim();

        var results = context.Service.SearchExact(query);

        if (results.Count == 0)
        {
            context.Console.WriteWarning("! No results.");
            return;
        }

        if (results.Count == 1)
        {
            context.Console.WriteSuccess("✓ 1 match found. Showing details.");
            _presenter.ShowDetails(results[0]);
            return;
        }

        ConsoleLayout.PrintTitle(context.Console, $"Search results ({results.Count})");

        foreach (var c in results)
        {
            _presenter.ShowListItem(c);
        }

        context.Console.WriteLine("");
    }
    
    private static string ReadRequired(CommandContext context, string label)
    {
        while (true)
        {
            context.Console.Write(label);
            var input = context.Console.ReadLine()?.Trim();

            if (!string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            context.Console.WriteWarning("! This field is required.");
        }
    }
}