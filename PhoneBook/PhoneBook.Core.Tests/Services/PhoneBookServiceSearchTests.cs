using PhoneBook.Core.Services;
using PhoneBook.Testing.Builders;
using PhoneBook.Testing.TestDoubles;

namespace PhoneBook.Core.Tests.Services;

public sealed class PhoneBookServiceSearchTests
{
    private static PhoneBookService CreateService(InMemoryPhoneBookStateStorage storage)
        => new(storage, new FakePhoneNumberNormalizer());

    [Fact]
    public void SearchExact_WhenEmpty_ReturnsEmpty()
    {
        var storage = new InMemoryPhoneBookStateStorage();
        var service = CreateService(storage);

        Assert.Empty(service.SearchExact(null));
        Assert.Empty(service.SearchExact(""));
        Assert.Empty(service.SearchExact("   "));
    }

    [Fact]
    public void SearchExact_MatchesByFirstLastAndFullName_CaseInsensitive()
    {
        var storage = new InMemoryPhoneBookStateStorage();
        var service = CreateService(storage);

        var c = ContactBuilder.Create()
            .WithPhone(service.CreatePhoneNumber("+40710000333"))
            .WithFirstName("Ana")
            .WithLastName("Pop")
            .Build();

        service.Add(c);

        Assert.Single(service.SearchExact("ana"));
        Assert.Single(service.SearchExact("POP"));
        Assert.Single(service.SearchExact("ana pop"));
    }

    [Fact]
    public void SearchExact_MatchesByRawOrE164()
    {
        var storage = new InMemoryPhoneBookStateStorage();
        var service = CreateService(storage);

        var phone = service.CreatePhoneNumber("+40710000444");
        var c = ContactBuilder.Create()
            .WithPhone(phone)
            .WithFirstName("Ana")
            .Build();

        service.Add(c);

        Assert.Single(service.SearchExact("+40710000444"));

        Assert.Single(service.SearchExact("   +40710000444   "));
    }

    [Fact]
    public void SearchExact_ReturnsSortedResults()
    {
        var storage = new InMemoryPhoneBookStateStorage();
        var service = CreateService(storage);

        var a2 = ContactBuilder.Create()
            .WithPhone(service.CreatePhoneNumber("+40710000002"))
            .WithFirstName("Ana")
            .WithLastName("Zed")
            .Build();

        var a1 = ContactBuilder.Create()
            .WithPhone(service.CreatePhoneNumber("+40710000001"))
            .WithFirstName("Ana")
            .WithLastName("Pop")
            .Build();

        var b = ContactBuilder.Create()
            .WithPhone(service.CreatePhoneNumber("+40710000003"))
            .WithFirstName("Bogdan")
            .Build();

        service.Add(b);
        service.Add(a2);
        service.Add(a1);

        var results = service.SearchExact("ana");

        Assert.Equal(2, results.Count);
        Assert.Equal("Pop", results[0].LastName);
        Assert.Equal("Zed", results[1].LastName);
    }
}
