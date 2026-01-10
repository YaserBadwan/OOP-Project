using PhoneBook.ConsoleApp.Abstractions;
using PhoneBook.ConsoleApp.Presentation;

namespace PhoneBook.ConsoleApp.CLI.Commands;

public sealed class ListCommand : ICommand
{
    private readonly IContactPresenter _presenter;

    public ListCommand(IContactPresenter presenter)
    {
        _presenter = presenter;
    }
    
    public string Verb => "list";

    public void Execute(CommandContext context, string[] args)
    {
        var contacts = context.Service.ListAll();

        if (contacts.Count == 0)
        {
            context.Console.WriteWarning("! No contacts found.");
            context.Console.WriteLine("Use 'add' to create a new contact.");
            return;
        }

        ConsoleLayout.PrintTitle(context.Console, $"Contacts ({contacts.Count})");

        foreach (var c in contacts)
        {
            _presenter.ShowListItem(c);
        }

        context.Console.WriteLine("");
    }
}