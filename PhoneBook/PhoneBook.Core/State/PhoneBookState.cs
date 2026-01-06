using PhoneBook.Core.Models;

namespace PhoneBook.Core.State;

public class PhoneBookState
{
    public List<Contact> Contacts { get; init; } = new();
}