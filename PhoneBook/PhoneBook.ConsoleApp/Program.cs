using PhoneBook.ConsoleApp.Abstractions;
using PhoneBook.ConsoleApp.Infrastructure;
using PhoneBook.Core.Abstractions;
using PhoneBook.Core.Exceptions;
using PhoneBook.Core.Models;
using PhoneBook.Core.Services;
using PhoneBook.Core.Storage;
using PhoneBook.Infrastructure.JsonStorage;
using PhoneBook.Infrastructure.Phone;

namespace PhoneBook.ConsoleApp;

internal static class Program
{
    private static void Main()
    {
        IConsole console = new SystemConsole();

        var path = Path.Combine(AppContext.BaseDirectory, "phonebook.json");

        var storageOptions = new JsonFileStorageOptions();

        IPhoneBookStateStorage storage =
            new JsonFilePhoneBookStateStorage(storageOptions);

        IPhoneNumberNormalizer normalizer =
            new LibPhoneNumberNormalizer();

        PhoneBookService service;

        try
        {
            service = new PhoneBookService(storage, normalizer);
        }
        catch (DomainException ex)
        {
            console.WriteLine("STARTUP ERROR: " + ex.Message);

            if (ex.InnerException is not null)
                console.WriteLine(
                    "TECHNICAL: " +
                    ex.InnerException.GetType().Name +
                    " - " +
                    ex.InnerException.Message);

            return;
        }

        console.WriteLine($"Storage file: {path}");
        console.WriteLine($"Loaded contacts: {service.ListAll().Count}");

        var phone = PhoneNumber.Create(
            raw: "+19123126089",
            normalizer: normalizer,
            defaultRegion: null
        );

        var contact = new Contact(
            phoneNumber: phone,
            firstName: "Eu",
            lastName: "Eulescu",
            email: "eu.tu-niciunu@gmama.com",
            pronouns: null,
            ringtone: Ringtone.Default,
            birthday: null,
            notes: "storage test"
        );

        try
        {
            var (added, warnings) = service.Add(contact);

            console.WriteLine($"Added: {added.PhoneNumber.E164}");
            foreach (var w in warnings)
                console.WriteLine("WARNING: " + w.Message);
        }
        catch (Exception ex)
        {
            console.WriteLine("ERROR: " + ex.Message);
        }

        console.WriteLine($"Now contacts: {service.ListAll().Count}");
        console.WriteLine("Close app and run again to verify reload.");
    }
}
