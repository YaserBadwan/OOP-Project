using PhoneBook.Core.Exceptions;
using PhoneBook.Core.Models;
using PhoneBook.Core.State;
using PhoneBook.Core.Storage;

namespace PhoneBook.Infrastructure.JsonStorage;

public sealed class JsonFilePhoneBookStateStorage : IPhoneBookStateStorage
{
    private readonly PhoneBookDtoFileStore _fileStore;

    public JsonFilePhoneBookStateStorage(JsonFileStorageOptions options)
    {
        _fileStore = new PhoneBookDtoFileStore(options);
    }

    public PhoneBookState Load()
    {
        try
        {
            var dto = _fileStore.TryLoad();
            if (dto is null)
            {
                return new PhoneBookState();
            }

            return MapToDomain(dto);
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new StorageException("Could not load phonebook data from storage.", ex);
        }
    }

    public void Save(PhoneBookState state)
    {
        try
        {
            var dto = MapToDto(state);
            _fileStore.Save(dto);
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new StorageException("Could not save phonebook data to storage.", ex);
        }
    }

    private static PhoneBookState MapToDomain(PhoneBookStateDto dto)
    {
        return new PhoneBookState
        {
            Contacts = dto.Contacts.Select(c =>
            {
                var phone = PhoneNumber.FromE164(c.PhoneE164, c.PhoneRaw);

                return new Contact(
                    phoneNumber: phone,
                    firstName: c.FirstName,
                    lastName: c.LastName,
                    email: c.Email,
                    pronouns: c.Pronouns,
                    ringtone: c.Ringtone is null ? Ringtone.Default : (Ringtone)c.Ringtone.Value,
                    birthday: c.Birthday,
                    notes: c.Notes
                );
            }).ToList()
        };
    }

    private static PhoneBookStateDto MapToDto(PhoneBookState state)
    {
        return new PhoneBookStateDto
        {
            Contacts = state.Contacts.Select(c => new ContactDto
            {
                PhoneE164 = c.PhoneNumber.E164,
                PhoneRaw = c.PhoneNumber.Raw,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Pronouns = c.Pronouns,
                Ringtone = (int)c.Ringtone,
                Birthday = c.Birthday,
                Notes = c.Notes
            }).ToList()
        };
    }
}
