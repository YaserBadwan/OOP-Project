using PhoneBook.ConsoleApp.Abstractions;
using PhoneBook.ConsoleApp.Presentation;

namespace PhoneBook.ConsoleApp.CLI.Commands;

public sealed class HelpCommand : ICommand
{
    public string Verb => "help";

    public void Execute(CommandContext context, string[] args)
    {
        var c = context.Console;

        ConsoleLayout.PrintTitle(
            c,
            "PhoneBook CLI — Help",
            "Commands are case-insensitive. Examples below.");

        ConsoleLayout.PrintSection(c, "General");
        ConsoleLayout.PrintCommand(c, "help", "Show this help screen");
        ConsoleLayout.PrintCommand(c, "exit | quit", "Exit the application");

        c.WriteLine("");
        ConsoleLayout.PrintSection(c, "Contacts");
        ConsoleLayout.PrintCommand(c, "list", "List all contacts");
        ConsoleLayout.PrintCommand(c, "show <phone>", "Show full contact details");
        ConsoleLayout.PrintCommand(c, "search <query>", "Search contacts (exact match)");
        ConsoleLayout.PrintCommand(c, "add", "Add a new contact (interactive)");
        ConsoleLayout.PrintCommand(c, "edit <phone>", "Edit a contact (Save/Cancel)");
        ConsoleLayout.PrintCommand(c, "delete <phone>", "Delete a contact (confirmation)");

        c.WriteLine("");
        ConsoleLayout.PrintSection(c, "Examples");
        c.WriteLine("  list");
        c.WriteLine("  show +40xxxxxxxxx");
        c.WriteLine("  search Ana");
        c.WriteLine("  edit +40xxxxxxxxx");
        c.WriteLine("  delete +40xxxxxxxxx");

        c.WriteLine("");
        ConsoleLayout.PrintSection(c, "Notes");
        c.WriteLine("  • Phone numbers are normalized to E.164 internally (default region: RO).");
        c.WriteLine("  • In edit mode, press Enter to keep the current value.");
        c.WriteLine("");
    }
}
