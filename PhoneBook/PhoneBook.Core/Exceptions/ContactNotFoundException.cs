namespace PhoneBook.Core.Exceptions;

public sealed class ContactNotFoundException : RuleViolationException
{
    public ContactNotFoundException(string message) : base(message) { }
}