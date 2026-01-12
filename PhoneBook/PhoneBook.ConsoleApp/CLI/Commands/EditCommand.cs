using PhoneBook.ConsoleApp.CLI;
using PhoneBook.ConsoleApp.Presentation;
using PhoneBook.Core.Exceptions;
using PhoneBook.Core.Models;

namespace PhoneBook.ConsoleApp.Cli.Commands;

public sealed class EditCommand : ICommand
{
    public string Verb => "edit";

    public void Execute(CommandContext context, string[] args)
    {
        var phone = args.Length >= 1 ? args[0] : ReadRequired(context, "Phone to edit: ");

        var original = context.Service.GetByPhone(phone);
        var edited = original.Clone();

        ConsoleLayout.PrintTitle(
            context.Console,
            "Edit contact",
            "Enter = keep current | '-' = clear optional field | 'cancel' = abort edit anytime");

        try
        {
            edited.UpdateDetails(
                firstName: ReadEditRequired(context, "First name", edited.FirstName),
                lastName: ReadEditOptional(context, "Last name", edited.LastName),
                email: ReadEditOptional(context, "Email", edited.Email),
                pronouns: ReadEditOptional(context, "Pronouns", edited.Pronouns),
                ringtone: ChooseRingtone(context, edited.Ringtone),
                birthday: ReadDateEdit(context, "Birthday (yyyy-mm-dd)", edited.Birthday),
                notes: ReadEditOptional(context, "Notes", edited.Notes)
            );
        }
        catch (DomainException ex)
        {
            context.Console.WriteError("✗ " + ex.Message);
            context.Console.WriteWarning("Please try again.\n");
        }
        
        context.Console.Write("Change phone number? (y/n): ");
        var changePhone = context.Console.ReadLine();

        if (IsYes(changePhone))
        {
            edited = TryChangePhone(context, edited);
        }

        context.Console.Write("Save changes? (y/n): ");
        var confirm = context.Console.ReadLine();

        if (!IsYes(confirm))
        {
            context.Console.WriteWarning("! Edit cancelled.");
            return;
        }

        var (updated, warnings) = context.Service.Update(
            originalPhone: original.PhoneNumber.E164,
            updated: edited
        );
        context.Console.WriteSuccess("✓ Contact updated.");
        
        if (warnings.Count > 0)
        {
            context.Console.WriteLine("");
            context.Console.WriteWarning("! Warnings:");
            foreach (var w in warnings)
                context.Console.WriteWarning("  - " + w.Message);
        }
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

            context.Console.WriteError("! This field is required.");
        }
    }

    private static string ReadEditRequired(CommandContext context, string label, string current)
    {
        while (true)
        {
            context.Console.Write($"{label} [{current}]: ");
            var input = context.Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                return current;
            }

            if (input == "-")
            {
                context.Console.WriteError("! This field is required and cannot be cleared.");
                continue;
            }

            return input;
        }
    }


    private static string? ReadEditOptional(CommandContext context, string label, string? current)
    {
        context.Console.Write($"{label} [{current ?? "-"}]: ");
        var input = context.Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(input))
        {
            return current;
        }

        if (input == "-")
        {
            return null;
        }

        return input;
    }


    private static DateOnly? ReadDateEdit(CommandContext context, string label, DateOnly? current)
    {
        while (true)
        {
            context.Console.Write($"{label} [{current?.ToString() ?? "-"}]: ");
            var input = context.Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                return current;
            }

            if (input == "-")
            {
                return null;
            }

            if (DateOnly.TryParseExact(input, "yyyy-MM-dd", out var d))
            {
                return d;
            }

            context.Console.WriteError("! Invalid date format.");
        }
    }

    private static Ringtone ChooseRingtone(CommandContext context, Ringtone current)
    {
        var values = Enum.GetValues<Ringtone>();

        context.Console.WriteLine("Choose ringtone:");
        for (int i = 0; i < values.Length; i++)
            context.Console.WriteLine($"  {i + 1}) {values[i]}");

        while (true)
        {
            context.Console.Write($"Selection (Enter = {current}): ");
            var input = context.Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                return current;
            }

            if (int.TryParse(input, out var idx) && idx >= 1 && idx <= values.Length)
            {
                return values[idx - 1];
            }

            context.Console.WriteError("! Invalid selection. Please try again.");
        }
    }
    
    private static Contact TryChangePhone(CommandContext context, Contact edited)
    {
        while (true)
        {
            context.Console.Write("New phone (or type 'cancel'): ");
            var raw = context.Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(raw))
            {
                context.Console.WriteWarning("Phone is required (or type 'cancel').");
                continue;
            }

            if (raw.Equals("cancel", StringComparison.OrdinalIgnoreCase))
            {
                context.Console.WriteSuccess("Phone change cancelled.");
                return edited;
            }

            try
            {
                var newPhone = context.Service.CreatePhoneNumber(raw);
                context.Console.WriteLine($"New phone accepted: {newPhone.E164}");
                return edited.WithPhoneNumber(newPhone);
            }
            catch (DomainException)
            {
                context.Console.WriteError("Invalid phone number. Try again.");
            }
        }
    }

    private static bool IsYes(string? input)
        => input != null &&
           (input.Equals("y", StringComparison.OrdinalIgnoreCase)
            || input.Equals("yes", StringComparison.OrdinalIgnoreCase));
}
