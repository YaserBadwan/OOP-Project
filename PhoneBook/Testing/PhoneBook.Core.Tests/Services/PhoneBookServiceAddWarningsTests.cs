using PhoneBook.Core.Services;
using PhoneBook.Testing.Builders;
using PhoneBook.Testing.TestDoubles;

namespace PhoneBook.Core.Tests.Services;

public sealed class PhoneBookServiceAddWarningsTests
{
    private static PhoneBookService CreateService(InMemoryPhoneBookStateStorage storage)
        => new(storage, new FakePhoneNumberNormalizer());

    [Fact]
    public void Add_WhenSameFirstNameAndBothLastNameNull_ReturnsDuplicateNameWarning()
    {
        var storage = new InMemoryPhoneBookStateStorage();
        var service = CreateService(storage);

        var c1 = ContactBuilder.Create()
            .WithPhone(service.CreatePhoneNumber("+40710100001"))
            .WithFirstName("Ana")
            .WithLastName(null)
            .Build();

        var c2 = ContactBuilder.Create()
            .WithPhone(service.CreatePhoneNumber("+40710100002"))
            .WithFirstName("ana")
            .WithLastName(null)
            .Build();

        service.Add(c1);
        var (_, warnings) = service.Add(c2);

        Assert.Contains(warnings, w => w.Code == "DUPLICATE_NAME");
    }

    [Fact]
    public void Add_WhenSameFirstNameAndSameLastName_ReturnsDuplicateNameWarning()
    {
        var storage = new InMemoryPhoneBookStateStorage();
        var service = CreateService(storage);

        var c1 = ContactBuilder.Create()
            .WithPhone(service.CreatePhoneNumber("+40710100003"))
            .WithFirstName("Ana")
            .WithLastName("Pop")
            .Build();

        var c2 = ContactBuilder.Create()
            .WithPhone(service.CreatePhoneNumber("+40710100004"))
            .WithFirstName("ANA")
            .WithLastName(" pop ")
            .Build();

        service.Add(c1);
        var (_, warnings) = service.Add(c2);

        Assert.Contains(warnings, w => w.Code == "DUPLICATE_NAME");
    }

    [Fact]
    public void Add_WhenSameFirstNameButOneHasLastNameAndOtherNot_DoesNotWarn()
    {
        var storage = new InMemoryPhoneBookStateStorage();
        var service = CreateService(storage);

        var c1 = ContactBuilder.Create()
            .WithPhone(service.CreatePhoneNumber("+40710100005"))
            .WithFirstName("Ana")
            .WithLastName(null)
            .Build();

        var c2 = ContactBuilder.Create()
            .WithPhone(service.CreatePhoneNumber("+40710100006"))
            .WithFirstName("Ana")
            .WithLastName("Pop")
            .Build();

        service.Add(c1);
        var (_, warnings) = service.Add(c2);

        Assert.Empty(warnings);
    }
}
