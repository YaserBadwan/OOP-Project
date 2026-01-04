namespace PhoneBook.Core.Exceptions;

public class RuleViolationException : DomainException
{
    public RuleViolationException(string message) : base(message) { }
    public RuleViolationException(string message, Exception innerException) : base(message, innerException) { }
}