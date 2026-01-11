using PhoneBook.Core.Services;
using PhoneBook.Testing.Builders;
using PhoneBook.Testing.TestDoubles;

namespace PhoneBook.Core.Tests.Services;

public sealed class PhoneBookServicePersistenceTests
{
    [Fact]
    public void Add_PersistsState_SoNewServiceInstanceSeesContact()
    {
        var storage = new InMemoryPhoneBookStateStorage();
        var normalizer = new FakePhoneNumberNormalizer();

        var service1 = new PhoneBookService(storage, normalizer);

        var phone = service1.CreatePhoneNumber("+40111111111");
        var contact = ContactBuilder.Create()
            .WithPhone(phone)
            .WithFirstName("Ana")
            .Build();

        service1.Add(contact);

        var service2 = new PhoneBookService(storage, normalizer);

        var all = service2.ListAll();
        Assert.Single(all);
        Assert.Equal("+40111111111", all[0].PhoneNumber.E164);
    }
}