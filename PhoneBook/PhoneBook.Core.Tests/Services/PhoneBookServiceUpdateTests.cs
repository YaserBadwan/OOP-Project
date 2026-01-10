using PhoneBook.Core.Exceptions;
using PhoneBook.Core.Services;
using PhoneBook.Testing.Builders;
using PhoneBook.Testing.TestDoubles;

namespace PhoneBook.Core.Tests.Services;

public sealed class PhoneBookServiceUpdateTests
{
    private static PhoneBookService CreateService(InMemoryPhoneBookStateStorage storage)
        => new(storage, new FakePhoneNumberNormalizer());

    [Fact]
    public void Update_WhenOriginalNotFound_ThrowsContactNotFoundException()
    {
        var storage = new InMemoryPhoneBookStateStorage();
        var service = CreateService(storage);

        var updated = ContactBuilder.Create()
            .WithPhone(service.CreatePhoneNumber("+40710000001"))
            .WithFirstName("Ana")
            .Build();

        Assert.Throws<ContactNotFoundException>(() =>
            service.Update(originalPhone: "+40719999999", updated: updated));
    }

    [Fact]
    public void Update_WhenUpdatedPhoneConflicts_ThrowsDuplicatePhoneNumberException()
    {
        var storage = new InMemoryPhoneBookStateStorage();
        var service = CreateService(storage);

        var c1 = ContactBuilder.Create()
            .WithPhone(service.CreatePhoneNumber("+40710000001"))
            .WithFirstName("Ana")
            .Build();

        var c2 = ContactBuilder.Create()
            .WithPhone(service.CreatePhoneNumber("+40710000002"))
            .WithFirstName("Maria")
            .Build();

        service.Add(c1);
        service.Add(c2);

        // Numar de telefon duplicat
        var updatedC1 = c1.WithPhoneNumber(service.CreatePhoneNumber("+40710000002"));

        Assert.Throws<DuplicatePhoneNumberException>(() =>
            service.Update(originalPhone: "+40710000001", updated: updatedC1));
    }

    [Fact]
    public void Update_WhenDuplicateNameExists_ReturnsWarning_AndExcludesSelf()
    {
        var storage = new InMemoryPhoneBookStateStorage();
        var service = CreateService(storage);

        var c1 = ContactBuilder.Create()
            .WithPhone(service.CreatePhoneNumber("+40710000001"))
            .WithFirstName("Ana")
            .WithLastName("Pop")
            .Build();

        var c2 = ContactBuilder.Create()
            .WithPhone(service.CreatePhoneNumber("+40710000002"))
            .WithFirstName("Maria")
            .WithLastName("Ionescu")
            .Build();

        service.Add(c1);
        service.Add(c2);

        var (updated1, warnings1) = service.Update("+40710000001", c1.Clone());

        Assert.Equal(c1.PhoneNumber.E164, updated1.PhoneNumber.E164);
        Assert.Empty(warnings1);

        var updatedC1 = c1.Clone();
        updatedC1.UpdateDetails(
            firstName: "Maria",
            lastName: "Ionescu",
            email: updatedC1.Email,
            pronouns: updatedC1.Pronouns,
            ringtone: updatedC1.Ringtone,
            birthday: updatedC1.Birthday,
            notes: updatedC1.Notes
        );

        var (_, warnings2) = service.Update("+40710000001", updatedC1);

        Assert.NotEmpty(warnings2);
        Assert.Contains(warnings2, w => w.Code == "DUPLICATE_NAME");
    }


    [Fact]
    public void Update_PersistsChanges()
    {
        var storage = new InMemoryPhoneBookStateStorage();
        var service1 = CreateService(storage);

        var c = ContactBuilder.Create()
            .WithPhone(service1.CreatePhoneNumber("+40710000010"))
            .WithFirstName("Ana")
            .Build();

        service1.Add(c);

        var updated = c.Clone();
        updated.UpdateDetails(
            firstName: "Ana Maria",
            lastName: null,
            email: null,
            pronouns: null,
            ringtone: updated.Ringtone,
            birthday: updated.Birthday,
            notes: null
        );

        service1.Update("+40710000010", updated);

        // nou service pe acelasi storage, trebuie sa vada update-ul
        var service2 = CreateService(storage);
        var reloaded = service2.GetByPhone("+40710000010");

        Assert.Equal("Ana Maria", reloaded.FirstName);
    }
}
