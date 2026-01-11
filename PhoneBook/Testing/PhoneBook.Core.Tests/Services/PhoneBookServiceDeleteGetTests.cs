using PhoneBook.Core.Exceptions;
using PhoneBook.Core.Services;
using PhoneBook.Testing.Builders;
using PhoneBook.Testing.TestDoubles;

namespace PhoneBook.Core.Tests.Services;

public sealed class PhoneBookServiceDeleteGetTests
{
    private static PhoneBookService CreateService(InMemoryPhoneBookStateStorage storage)
        => new(storage, new FakePhoneNumberNormalizer());

    [Fact]
    public void GetByPhone_WhenInvalid_ThrowsValidationException()
    {
        var storage = new InMemoryPhoneBookStateStorage();
        var service = CreateService(storage);

        Assert.Throws<ValidationException>(() => service.GetByPhone("   "));
        Assert.Throws<ValidationException>(() => service.GetByPhone("0712345678")); // fake normalizer accepta doar +digits
    }

    [Fact]
    public void GetByPhone_WhenMissing_ThrowsContactNotFoundException()
    {
        var storage = new InMemoryPhoneBookStateStorage();
        var service = CreateService(storage);

        Assert.Throws<ContactNotFoundException>(() => service.GetByPhone("+40710000111"));
    }

    [Fact]
    public void DeleteByPhone_RemovesAndPersists()
    {
        var storage = new InMemoryPhoneBookStateStorage();
        var service1 = CreateService(storage);

        var c = ContactBuilder.Create()
            .WithPhone(service1.CreatePhoneNumber("+40710000222"))
            .WithFirstName("Ana")
            .Build();

        service1.Add(c);
        service1.DeleteByPhone("+40710000222");

        // nou service, trebuie sa fie sters
        var service2 = CreateService(storage);
        Assert.Throws<ContactNotFoundException>(() => service2.GetByPhone("+40710000222"));
    }
}