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
        var newFirstName = NormalizeRequired(firstName, "First name is required.");
        var newLastName = NormalizeOptional(lastName);
        var newEmail = NormalizeOptional(email);
        var newPronouns = NormalizeOptional(pronouns);
        var newNotes = NormalizeOptional(notes);
        
        ValidateDraft(newFirstName, newEmail, birthday);
        
        FirstName = newFirstName;
        LastName = newLastName;
        Email = newEmail;
        Pronouns = newPronouns;
        Ringtone = ringtone;
        Birthday = birthday;
        Notes = newNotes;
    }
    
    public Contact WithPhoneNumber(PhoneNumber newPhoneNumber)
    {
        if (newPhoneNumber is null)
        {
            throw new ValidationException(nameof(newPhoneNumber));
        }

        return new Contact(
            phoneNumber: newPhoneNumber,
            firstName: FirstName,
            lastName: LastName,
            email: Email,
            pronouns: Pronouns,
            ringtone: Ringtone,
            birthday: Birthday,
            notes: Notes
        );
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

    private static void ValidateDraft(string firstName, string? email, DateOnly? birthday)
    {
        if (firstName.Length == 0)
            throw new ValidationException("First name is required!");

        if (email is not null && !EmailRegex.IsMatch(email))
            throw new ValidationException("Email is invalid!");

        if (birthday is not null && birthday.Value > DateOnly.FromDateTime(DateTime.Today))
            throw new ValidationException("Birthday cannot be in the future!");
    }
    
    private void Validate()
    {
        ValidateDraft(FirstName, Email, Birthday);
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

    private static string? NormalizeOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }
}