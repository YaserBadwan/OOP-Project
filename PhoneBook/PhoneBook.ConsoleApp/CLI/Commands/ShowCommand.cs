using PhoneBook.ConsoleApp.Abstractions;

namespace PhoneBook.ConsoleApp.CLI.Commands;

public sealed class ShowCommand : ICommand
{
    private readonly IContactPresenter _presenter;

    public ShowCommand(IContactPresenter presenter)
    {
        _presenter = presenter;
    }

    public string Verb => "show";

    public void Execute(CommandContext context, string[] args)
    {
        var phone = args.Length >= 1
            ? args[0]
            : ReadRequired(context, "Phone to show: ");

        var contact = context.Service.GetByPhone(phone);
        _presenter.ShowDetails(contact);
    }

    private static string ReadRequired(CommandContext context, string label)
    {
        while (true)
        {
            context.Console.Write(label);
            var input = context.Console.ReadLine()?.Trim();

            if (!string.IsNullOrWhiteSpace(input))
                return input;

            context.Console.WriteWarning("! This field is required.");
        }
    }
}