using PhoneBook.Core.Exceptions;
using PhoneBook.Core.Models;
using PhoneBook.Core.Services;
using PhoneBook.Testing.TestDoubles;

namespace PhoneBook.Core.Tests.Services;

public sealed class PhoneBookServiceTests
{
    [Fact]
    public void Add_WhenPhoneAlreadyExists_ThrowsDuplicatePhoneNumberException()
    {
        var storage = new InMemoryPhoneBookStateStorage();
        var normalizer = new FakePhoneNumberNormalizer();
        var service = new PhoneBookService(storage, normalizer);

        var c1 = new Contact(
            phoneNumber: service.CreatePhoneNumber("+40123123123"),
            firstName: "Ana",
            lastName: null,
            email: null,
            pronouns: null,
            ringtone: Ringtone.Default,
            birthday: null,
            notes: null
        );

        var c2 = new Contact(
            phoneNumber: service.CreatePhoneNumber("+40123123123"),
            firstName: "Ana2",
            lastName: null,
            email: null,
            pronouns: null,
            ringtone: Ringtone.Default,
            birthday: null,
            notes: null
        );

        service.Add(c1);

        Assert.Throws<DuplicatePhoneNumberException>(() => service.Add(c2));
    }
    
    [Fact]
    public void CreatePhoneNumber_WhenInvalid_ThrowsValidationException()
    {
        var storage = new InMemoryPhoneBookStateStorage();
        var normalizer = new FakePhoneNumberNormalizer();
        var service = new PhoneBookService(storage, normalizer);

        Assert.Throws<ValidationException>(() => service.CreatePhoneNumber("0712345678"));
        Assert.Throws<ValidationException>(() => service.CreatePhoneNumber("+40 712 345 678"));
        Assert.Throws<ValidationException>(() => service.CreatePhoneNumber("+40abc"));
    }

}