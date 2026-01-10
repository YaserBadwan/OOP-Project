using PhoneBook.Core.Services;
using PhoneBook.Testing.Builders;
using PhoneBook.Testing.TestDoubles;

namespace PhoneBook.Core.Tests.Services;

public sealed class PhoneBookServiceListAllTests
{
    private static PhoneBookService CreateService(InMemoryPhoneBookStateStorage storage)
        => new(storage, new FakePhoneNumberNormalizer());

    [Fact]
    public void ListAll_ReturnsSortedByFirstNameThenLastNameThenE164()
    {
        var storage = new InMemoryPhoneBookStateStorage();
        var service = CreateService(storage);

        var aNull = ContactBuilder.Create()
            .WithPhone(service.CreatePhoneNumber("+40710200002"))
            .WithFirstName("Ana")
            .WithLastName(null)
            .Build();

        var aPop2 = ContactBuilder.Create()
            .WithPhone(service.CreatePhoneNumber("+40710200003"))
            .WithFirstName("Ana")
            .WithLastName("Pop")
            .Build();

        var aPop1 = ContactBuilder.Create()
            .WithPhone(service.CreatePhoneNumber("+40710200001"))
            .WithFirstName("Ana")
            .WithLastName("Pop")
            .Build();

        var b = ContactBuilder.Create()
            .WithPhone(service.CreatePhoneNumber("+40710200004"))
            .WithFirstName("Bogdan")
            .WithLastName(null)
            .Build();

        service.Add(b);
        service.Add(aPop2);
        service.Add(aNull);
        service.Add(aPop1);

        var list = service.ListAll();

        Assert.Equal(4, list.Count);

        Assert.Equal("Ana", list[0].FirstName);
        Assert.Equal("Ana", list[1].FirstName);
        Assert.Equal("Ana", list[2].FirstName);
        Assert.Equal("Bogdan", list[3].FirstName);

        var ana = list.Take(3).ToList();

        Assert.Null(ana[0].LastName);
        Assert.Equal("Pop", ana[1].LastName);
        Assert.Equal("Pop", ana[2].LastName);

        Assert.True(string.CompareOrdinal(ana[1].PhoneNumber.E164, ana[2].PhoneNumber.E164) < 0);
    }
}
