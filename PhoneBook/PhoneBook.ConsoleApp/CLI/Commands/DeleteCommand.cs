using PhoneBook.ConsoleApp.Abstractions;
using PhoneBook.ConsoleApp.Presentation;

namespace PhoneBook.ConsoleApp.CLI.Commands;

public sealed class DeleteCommand : ICommand
{
    private readonly IContactPresenter _presenter;

    public DeleteCommand(IContactPresenter presenter)
    {
        _presenter = presenter;
    }
    public string Verb => "delete";

    public void Execute(CommandContext context, string[] args)
    {
        var phone = args.Length >= 1 ? args[0] : ReadRequired(context, "Phone to delete: ");

        var contact = context.Service.GetByPhone(phone);

        ConsoleLayout.PrintTitle(context.Console, "Delete contact");
        _presenter.ShowListItem(contact);

        context.Console.WriteWarning("! This action cannot be undone.");
        if (!Confirm(context, "Continue? (y/n): "))
        {
            context.Console.WriteWarning("! Operation cancelled.");
            return;
        }

        context.Service.DeleteByPhone(phone);
        context.Console.WriteSuccess("✓ Contact deleted.");
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

    private static bool Confirm(CommandContext context, string label)
    {
        context.Console.Write(label);
        var input = context.Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        return input.Equals("y", StringComparison.OrdinalIgnoreCase)
               || input.Equals("yes", StringComparison.OrdinalIgnoreCase);
    }
}