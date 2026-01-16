using PhoneBook.Core.Exceptions;
using PhoneBook.Core.Models;

namespace PhoneBook.ConsoleApp.CLI.Commands;

public sealed class AddCommand : ICommand
{
    public string Verb => "add";

    public void Execute(CommandContext context, string[] args)
    {
        var c = context.Console;

        c.WriteLine("Add contact (leave optional fields empty and press Enter).");
        c.WriteLine("Type 'cancel' anytime to abort.\n");

        var phone = ReadPhone(context);
        var firstName = context.Prompt.Required("First name: ");

        var lastName = context.Prompt.Optional("Last name: ");
        var email = context.Prompt.Email("Email: ");
        var pronouns = context.Prompt.Optional("Pronouns: ");

        var ringtone = context.Prompt.ChooseEnum("Choose a ringtone:", Ringtone.Default);
        var birthday = context.Prompt.DateOptional("Birthday", format: "yyyy-MM-dd");
        var notes = context.Prompt.Optional("Notes: ");

        var contact = new Contact(
            phoneNumber: phone,
            firstName: firstName,
            lastName: lastName,
            email: email,
            pronouns: pronouns,
            ringtone: ringtone,
            birthday: birthday,
            notes: notes
        );

        var (added, warnings) = context.Service.Add(contact);

        c.WriteSuccess($"✓ Contact added: {added.PhoneNumber.E164}");

        if (warnings.Count > 0)
        {
            c.WriteLine("");
            c.WriteWarning("! Warnings:");
            foreach (var w in warnings)
                c.WriteWarning("  - " + w.Message);
        }
    }

    private static PhoneNumber ReadPhone(CommandContext context)
    {
        while (true)
        {
            var raw = context.Prompt.Required("Phone number: ");

            try
            {
                return context.Service.CreatePhoneNumber(raw);
            }
            catch (DomainException ex)
            {
                context.Console.WriteWarning("! Invalid phone: " + ex.Message);
            }
        }
    }
}
