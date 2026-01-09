using PhoneBook.Core.Exceptions;
using System.Text.RegularExpressions;

namespace PhoneBook.Core.Models;

public sealed class Contact
{
    public PhoneNumber PhoneNumber { get; }
    public string FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? Email { get; private set; }
    public string? Pronouns { get; private set; }
    public Ringtone Ringtone { get; private set; }
    public DateOnly? Birthday { get; private set; }
    public string? Notes { get; private set; }

    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public Contact(PhoneNumber phoneNumber, 
        string? firstName, 
        string? lastName = null, 
        string? email = null, 
        string? pronouns = null,
        Ringtone ringtone = Ringtone.Default,
        DateOnly? birthday = null,
        string? notes = null)
    {
        PhoneNumber = phoneNumber ?? throw new ValidationException("Phone Number is required!");
        
        FirstName = NormalizeRequired(firstName, "First Name is required!");

        LastName = NormalizeOptional(lastName);
        Email = NormalizeOptional(email);
        Pronouns = NormalizeOptional(pronouns);
        Ringtone = ringtone;
        Birthday = birthday;
        Notes = NormalizeOptional(notes);

        Validate();
    }

    public void UpdateDetails(
        string? firstName,
        string? lastName,
        string? email,
        string? pronouns,
        Ringtone ringtone,
        DateOnly? birthday,
        string? notes)
    {
        FirstName = NormalizeRequired(firstName, "First name is required.");
        LastName = NormalizeOptional(lastName);
        Email = NormalizeOptional(email);
        Pronouns = NormalizeOptional(pronouns);
        Ringtone = ringtone;
        Birthday = birthday;
        Notes = NormalizeOptional(notes);

        Validate();
    }

    // Ca sa permitem user-ului sa dea "Cancel edit". Practic, user-ul editeaza un Draft.
    public Contact Clone() =>
        new(
            phoneNumber: PhoneNumber,
            firstName: FirstName,
            lastName: LastName,
            email: Email,
            pronouns: Pronouns,
            ringtone: Ringtone,
            birthday: Birthday,
            notes: Notes);

    private void Validate()
    {
        if (FirstName.Length == 0)
        {
            throw new ValidationException("First name is required!");
        }

        if (Email is not null && !EmailRegex.IsMatch(Email))
        {
            throw new ValidationException("Email is invalid!");
        }

        if (Birthday is not null && Birthday.Value > DateOnly.FromDateTime(DateTime.Today))
        {
            throw new ValidationException("Birthday cannot be in the future!");
        }
    }

    private static string NormalizeRequired(string? value, string errorMessage)
    {
        var normalized = NormalizeOptional(value);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ValidationException(errorMessage);
        }
        
        return normalized;
    }

    private static string NormalizeOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }
}