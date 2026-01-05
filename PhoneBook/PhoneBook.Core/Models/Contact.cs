using PhoneBook.Core.Exceptions;

namespace PhoneBook.Core.Models;

public sealed class Contact
{
    public PhoneNumber PhoneNumber { get; }
    public string FirstName { get; }

    public Contact(PhoneNumber phoneNumber, string? firstName)
    {
        PhoneNumber = phoneNumber ?? throw new ValidationException("Phone Number is required!");

        FirstName = (firstName ?? string.Empty).Trim();
        if (FirstName.Length == 0)
        {
            throw new ValidationException("First Name is required!");
        }
    }
}