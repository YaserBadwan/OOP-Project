using PhoneBook.Core.State;

namespace PhoneBook.Core.Storage;

public interface IPhoneBookStateStorage
{
    PhoneBookState Load();
    void Save(PhoneBookState state);
}