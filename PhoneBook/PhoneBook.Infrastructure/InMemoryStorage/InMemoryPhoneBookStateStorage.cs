using PhoneBook.Core.State;
using PhoneBook.Core.Storage;

namespace PhoneBook.Infrastructure.InMemory;

public sealed class InMemoryPhoneBookStateStorage : IPhoneBookStateStorage
{
    private PhoneBookState _state;

    public InMemoryPhoneBookStateStorage(PhoneBookState? initialState = null)
    {
        _state = initialState ?? new PhoneBookState();
    }

    public PhoneBookState Load() => Clone(_state);

    public void Save(PhoneBookState state) => _state = Clone(state);

    private static PhoneBookState Clone(PhoneBookState source)
        => new()
        {
            Contacts = source.Contacts.Select(c => c.Clone()).ToList()
        };
}