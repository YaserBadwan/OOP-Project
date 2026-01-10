using PhoneBook.Core.Exceptions;
using PhoneBook.Core.Services;
using PhoneBook.Testing.TestDoubles;

namespace PhoneBook.Core.Tests.Services;

public sealed class PhoneBookServiceDeleteTests
{
    [Fact]
    public void DeleteByPhone_WhenMissing_ThrowsContactNotFoundException()
    {
        var storage = new InMemoryPhoneBookStateStorage();
        var normalizer = new FakePhoneNumberNormalizer();
        var service = new PhoneBookService(storage, normalizer);

        Assert.Throws<ContactNotFoundException>(() => service.DeleteByPhone("+40123456789"));
    }
}