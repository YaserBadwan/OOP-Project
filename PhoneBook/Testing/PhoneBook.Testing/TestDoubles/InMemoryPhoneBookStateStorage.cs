using PhoneBook.Core.Abstractions;
using PhoneBook.Core.State;
using PhoneBook.Core.Storage;

namespace PhoneBook.Testing.TestDoubles;

public sealed class InMemoryPhoneBookStateStorage : IPhoneBookStateStorage
{
    private PhoneBookState _state;

    public InMemoryPhoneBookStateStorage(PhoneBookState? initialState = null)
    {
        _state = initialState ?? new PhoneBookState();
    }

    public PhoneBookState Load()
    {
        return Clone(_state);
    }

    public void Save(PhoneBookState state)
    {
        _state = Clone(state);
    }

    public void Reset(PhoneBookState? newState = null)
    {
        _state = newState ?? new PhoneBookState();
    }

    private static PhoneBookState Clone(PhoneBookState source)
    {
        return new PhoneBookState
        {
            Contacts = source.Contacts.Select(c => c.Clone()).ToList()
        };
    }
}