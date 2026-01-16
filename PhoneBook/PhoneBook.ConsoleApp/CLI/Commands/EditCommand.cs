using PhoneBook.ConsoleApp.Presentation;
using PhoneBook.Core.Exceptions;
using PhoneBook.Core.Models;

namespace PhoneBook.ConsoleApp.CLI.Commands;

public sealed class EditCommand : ICommand
{
    public string Verb => "edit";

    public void Execute(CommandContext context, string[] args)
    {
        var console = context.Console;
        var prompt = context.Prompt;

        var phone = args.Length >= 1
            ? args[0]
            : prompt.Required("Phone to edit: ");

        var original = context.Service.GetByPhone(phone);
        var edited = original.Clone();

        ConsoleLayout.PrintTitle(
            console,
            "Edit contact",
            "Enter = keep current | '-' = clear optional field | 'cancel' = abort edit anytime"
        );

        try
        {
            edited.UpdateDetails(
                firstName: prompt.EditRequired("First name", edited.FirstName),
                lastName: prompt.EditOptional("Last name", edited.LastName),
                email: prompt.EditEmail("Email", edited.Email),
                pronouns: prompt.EditOptional("Pronouns", edited.Pronouns),
                ringtone: prompt.ChooseEnum("Choose ringtone:", edited.Ringtone),
                birthday: prompt.EditDate("Birthday", edited.Birthday, "yyyy-MM-dd"),
                notes: prompt.EditOptional("Notes", edited.Notes)
            );
        }
        catch (DomainException ex)
        {
            console.WriteError("✗ " + ex.Message);
            console.WriteWarning("Please try again.\n");
            return;
        }

        if (prompt.Confirm("Change phone number?"))
        {
            edited = ChangePhone(context, edited);
        }

        if (!prompt.Confirm("Save changes?"))
        {
            console.WriteWarning("! Edit cancelled.");
            return;
        }

        var (_, warnings) = context.Service.Update(
            originalPhone: original.PhoneNumber.E164,
            updated: edited
        );

        console.WriteSuccess("✓ Contact updated.");

        if (warnings.Count > 0)
        {
            console.WriteLine("");
            console.WriteWarning("! Warnings:");
            foreach (var w in warnings)
                console.WriteWarning("  - " + w.Message);
        }
    }

    private static Contact ChangePhone(CommandContext context, Contact edited)
    {
        var prompt = context.Prompt;
        var console = context.Console;

        while (true)
        {
            var raw = prompt.Required("New phone: ");

            try
            {
                var newPhone = context.Service.CreatePhoneNumber(raw);
                console.WriteLine($"New phone accepted: {newPhone.E164}");
                return edited.WithPhoneNumber(newPhone);
            }
            catch (DomainException ex)
            {
                console.WriteError("! Invalid phone: " + ex.Message);
            }
        }
    }
}
