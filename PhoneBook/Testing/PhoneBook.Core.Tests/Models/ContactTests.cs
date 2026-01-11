using PhoneBook.Core.Exceptions;
using PhoneBook.Core.Models;
using PhoneBook.Core.Services;
using PhoneBook.Testing.Builders;
using PhoneBook.Testing.TestDoubles;

namespace PhoneBook.Core.Tests.Models;

public sealed class ContactTests
{
    private static PhoneBookService CreateService()
    {
        var storage = new InMemoryPhoneBookStateStorage();
        var normalizer = new FakePhoneNumberNormalizer();
        return new PhoneBookService(storage, normalizer);
    }

    [Fact]
    public void Constructor_TrimsFirstAndLastName()
    {
        var service = CreateService();
        var phone = service.CreatePhoneNumber("+40711111111");

        var c = new Contact(
            phoneNumber: phone,
            firstName: "  Ana  ",
            lastName: "  Pop  ",
            email: null,
            pronouns: null,
            ringtone: Ringtone.Default,
            birthday: null,
            notes: null
        );

        Assert.Equal("Ana", c.FirstName);
        Assert.Equal("Pop", c.LastName);
    }

    [Fact]
    public void Constructor_InvalidEmail_ThrowsValidationException()
    {
        var service = CreateService();
        var phone = service.CreatePhoneNumber("+40722222222");

        Assert.Throws<ValidationException>(() => new Contact(
            phoneNumber: phone,
            firstName: "Ana",
            lastName: null,
            email: "not-an-email",
            pronouns: null,
            ringtone: Ringtone.Default,
            birthday: null,
            notes: null
        ));
    }
    
    [Fact]
    public void UpdateDetails_ChangesOptionalFields()
    {
        var service = CreateService();
        var phone = service.CreatePhoneNumber("+40733333333");

        var c = ContactBuilder.Create()
            .WithPhone(phone)
            .WithFirstName("Ana")
            .WithLastName("Pop")
            .WithEmail("ana@ex.com")
            .WithPronouns("she/her")
            .WithRingtone(Ringtone.Classic)
            .WithBirthday(new DateOnly(2000, 1, 2))
            .WithNotes("note")
            .Build();

        c.UpdateDetails(
            firstName: "Ana Maria",
            lastName: null,
            email: "ana2@ex.com",
            pronouns: null,
            ringtone: Ringtone.Silent,
            birthday: null,
            notes: "updated"
        );

        Assert.Equal("Ana Maria", c.FirstName);
        Assert.Null(c.LastName);
        Assert.Equal("ana2@ex.com", c.Email);
        Assert.Null(c.Pronouns);
        Assert.Equal(Ringtone.Silent, c.Ringtone);
        Assert.Null(c.Birthday);
        Assert.Equal("updated", c.Notes);
    }

    [Fact]
    public void UpdateDetails_InvalidEmail_ThrowsValidationException_AndDoesNotCorruptContact()
    {
        var service = CreateService();
        var phone = service.CreatePhoneNumber("+40744444444");

        var c = ContactBuilder.Create()
            .WithPhone(phone)
            .WithFirstName("Ana")
            .WithEmail("ana@ex.com")
            .Build();

        // update invalid
        Assert.Throws<ValidationException>(() => c.UpdateDetails(
            firstName: "Ana",
            lastName: null,
            email: "bad-email",
            pronouns: null,
            ringtone: Ringtone.Default,
            birthday: null,
            notes: null
        ));

        // verificare stare initiala
        Assert.Equal("ana@ex.com", c.Email);
    }

    [Fact]
    public void Clone_ReturnsIndependentCopy()
    {
        var service = CreateService();
        var phone = service.CreatePhoneNumber("+40755555555");

        var original = ContactBuilder.Create()
            .WithPhone(phone)
            .WithFirstName("Ana")
            .WithNotes("n1")
            .Build();

        var clone = original.Clone();

        clone.UpdateDetails(
            firstName: "Ana2",
            lastName: null,
            email: null,
            pronouns: null,
            ringtone: Ringtone.Default,
            birthday: null,
            notes: "n2"
        );

        // original neschimbat
        Assert.Equal("Ana", original.FirstName);
        Assert.Equal("n1", original.Notes);

        // clone schimbat
        Assert.Equal("Ana2", clone.FirstName);
        Assert.Equal("n2", clone.Notes);
    }

    [Fact]
    public void WithPhoneNumber_CreatesNewContactWithDifferentPhone_ButSameOtherFields()
    {
        var service = CreateService();
        var phone1 = service.CreatePhoneNumber("+40766666666");
        var phone2 = service.CreatePhoneNumber("+40777777777");

        var c1 = ContactBuilder.Create()
            .WithPhone(phone1)
            .WithFirstName("Ana")
            .WithLastName("Pop")
            .WithEmail("ana@ex.com")
            .WithRingtone(Ringtone.Marimba)
            .Build();

        var c2 = c1.WithPhoneNumber(phone2);

        Assert.Equal(phone1.E164, c1.PhoneNumber.E164);
        Assert.Equal(phone2.E164, c2.PhoneNumber.E164);

        Assert.Equal(c1.FirstName, c2.FirstName);
        Assert.Equal(c1.LastName, c2.LastName);
        Assert.Equal(c1.Email, c2.Email);
        Assert.Equal(c1.Ringtone, c2.Ringtone);
    }

}