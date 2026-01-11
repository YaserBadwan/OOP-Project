using PhoneBook.Core.Models;

namespace PhoneBook.Testing.Builders;

public sealed class ContactBuilder
{
    private PhoneNumber _phoneNumber = default!;
    private string _firstName = "Ana";
    private string? _lastName;
    private string? _email;
    private string? _pronouns;
    private Ringtone _ringtone = Ringtone.Default;
    private DateOnly? _birthday;
    private string? _notes;

    public static ContactBuilder Create() => new();

    public ContactBuilder WithPhone(PhoneNumber phoneNumber)
    {
        _phoneNumber = phoneNumber;
        return this;
    }

    public ContactBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public ContactBuilder WithLastName(string? lastName)
    {
        _lastName = lastName;
        return this;
    }

    public ContactBuilder WithEmail(string? email)
    {
        _email = email;
        return this;
    }

    public ContactBuilder WithPronouns(string? pronouns)
    {
        _pronouns = pronouns;
        return this;
    }

    public ContactBuilder WithRingtone(Ringtone ringtone)
    {
        _ringtone = ringtone;
        return this;
    }

    public ContactBuilder WithBirthday(DateOnly? birthday)
    {
        _birthday = birthday;
        return this;
    }

    public ContactBuilder WithNotes(string? notes)
    {
        _notes = notes;
        return this;
    }

    public Contact Build()
    {
        if (_phoneNumber is null)
            throw new InvalidOperationException("PhoneNumber must be set. Use WithPhone().");

        return new Contact(
            phoneNumber: _phoneNumber,
            firstName: _firstName,
            lastName: _lastName,
            email: _email,
            pronouns: _pronouns,
            ringtone: _ringtone,
            birthday: _birthday,
            notes: _notes
        );
    }
}