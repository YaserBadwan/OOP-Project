using PhoneBook.Core.Abstractions;
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
        
        var phone = ReadPhone(context);
        var firstName = ReadRequired(context, "First name: ");
        
        var lastName = ReadOptional(context, "Last name: ");
        var email = ReadOptional(context, "Email: ");
        var pronouns = ReadOptional(context, "Pronouns: ");

        var ringtone = ChooseRingtone(context, defaultValue: Ringtone.Default);
        var birthday = ReadDateOptional(context, "Birthday (yyyy-mm-dd): ");
        var notes = ReadOptional(context, "Notes: ");
        
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
        
        context.Console.WriteSuccess($"✓ Contact added: {added.PhoneNumber.E164}");

        if (warnings.Count > 0)
        {
            context.Console.WriteLine("");
            context.Console.WriteWarning("! Warnings:");
            foreach (var w in warnings)
                context.Console.WriteWarning("  - " + w.Message);
        }
    }
    
    private PhoneNumber ReadPhone(CommandContext context)
    {
        while (true)
        {
            var raw = ReadRequired(context, "Phone number: ");

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
    
    private static string ReadRequired(CommandContext context, string label)
    {
        while (true)
        {
            context.Console.Write(label);
            var input = context.Console.ReadLine();

            input = input?.Trim();
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            context.Console.WriteWarning("! This field is required.");
        }
    }
    
    private static string? ReadOptional(CommandContext context, string label)
    {
        context.Console.Write(label);
        var input = context.Console.ReadLine();
        input = input?.Trim();

        return string.IsNullOrWhiteSpace(input) ? null : input;
    }
    
    private static Ringtone ChooseRingtone(CommandContext context, Ringtone defaultValue)
    {
        var values = Enum.GetValues<Ringtone>();

        context.Console.WriteLine("Choose a ringtone:");
        for (int i = 0; i < values.Length; i++)
        {
            context.Console.WriteLine($"  {i + 1}) {values[i]}");
        }

        while (true)
        {
            context.Console.Write($"Selection (1-{values.Length}, Enter = {defaultValue}): ");
            var input = context.Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                return defaultValue;
            }

            if (int.TryParse(input, out var index) && index >= 1 && index <= values.Length)
            {
                return values[index - 1];
            }

            context.Console.WriteWarning("! Invalid selection. Please enter a number from the list.");
        }
    }
    
    private static DateOnly? ReadDateOptional(CommandContext context, string label)
    {
        while (true)
        {
            var input = ReadOptional(context, label);
            if (input is null)
            {
                return null;
            }

            if (DateOnly.TryParseExact(input, "yyyy-MM-dd", out var date))
            {
                return date;
            }

            context.Console.WriteWarning("! Invalid date format. Use yyyy-mm-dd (e.g. 2002-10-31).");
        }
    }
}