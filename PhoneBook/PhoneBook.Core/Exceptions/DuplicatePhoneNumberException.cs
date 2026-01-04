namespace PhoneBook.Core.Exceptions;

public sealed class DuplicatePhoneNumberException : RuleViolationException
{
    public string E164 { get; }
    
    public DuplicatePhoneNumberException(string e164) : base("Phone Number is already in use.")
    {
        E164 = e164;
    }
    
}