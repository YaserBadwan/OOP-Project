using PhoneBook.Core.Models;

namespace PhoneBook.ConsoleApp.Abstractions;

public interface IContactPresenter
{
    void ShowListItem(Contact contact);
    void ShowDetails(Contact contact);
}