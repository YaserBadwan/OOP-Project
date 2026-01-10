using PhoneBook.Core.Services;
using PhoneBook.Testing.Builders;
using PhoneBook.Testing.TestDoubles;

namespace PhoneBook.Core.Tests.Services;

public sealed class PhoneBookServiceWarningsTests
{
    [Fact]
    public void Add_WhenSameNameExists_ReturnsWarning()
    {
        var storage = new InMemoryPhoneBookStateStorage();
        var normalizer = new FakePhoneNumberNormalizer();
        var service = new PhoneBookService(storage, normalizer);

        var c1 = ContactBuilder.Create()
            .WithPhone(service.CreatePhoneNumber("+40333333333"))
            .WithFirstName("Ana")
            .Build();

        var c2 = ContactBuilder.Create()
            .WithPhone(service.CreatePhoneNumber("+40444444444"))
            .WithFirstName("Ana")
            .Build();

        service.Add(c1);
        var (_, warnings) = service.Add(c2);

        Assert.NotEmpty(warnings);
    }
}