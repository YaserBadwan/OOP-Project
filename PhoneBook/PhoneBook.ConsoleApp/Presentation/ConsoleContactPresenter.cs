using PhoneBook.ConsoleApp.Abstractions;
using PhoneBook.Core.Models;

namespace PhoneBook.ConsoleApp.Presentation;

public sealed class ConsoleContactPresenter : IContactPresenter
{
    private readonly IConsole _console;

    public ConsoleContactPresenter(IConsole console)
    {
        _console = console;
    }

    public void ShowListItem(Contact c)
    {
        var name = $"{c.FirstName} {c.LastName}".Trim();
        _console.WriteLine($"{name} | {c.PhoneNumber.E164}");
    }

    public void ShowDetails(Contact c)
    {
        ConsoleLayout.PrintTitle(_console, "Contact details");

        ConsoleLayout.PrintKeyValue(_console, "Phone (E164):", c.PhoneNumber.E164);
        ConsoleLayout.PrintKeyValue(_console, "Phone (Raw):", c.PhoneNumber.Raw ?? "-");
        ConsoleLayout.PrintKeyValue(_console, "First name:", c.FirstName);
        ConsoleLayout.PrintKeyValue(_console, "Last name:", c.LastName ?? "-");
        ConsoleLayout.PrintKeyValue(_console, "Email:", c.Email ?? "-");
        ConsoleLayout.PrintKeyValue(_console, "Pronouns:", c.Pronouns ?? "-");
        ConsoleLayout.PrintKeyValue(_console, "Ringtone:", c.Ringtone.ToString());
        ConsoleLayout.PrintKeyValue(_console, "Birthday:", c.Birthday?.ToString("yyyy-MM-dd") ?? "-");
        ConsoleLayout.PrintKeyValue(_console, "Notes:", c.Notes ?? "-");
        
        _console.WriteLine("");
        _console.WriteLine($"Tip: edit {c.PhoneNumber.E164} | delete {c.PhoneNumber.E164}");
        _console.WriteLine("");
    }
}