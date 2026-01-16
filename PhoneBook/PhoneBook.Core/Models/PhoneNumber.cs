namespace PhoneBook.Core.Models;

public sealed partial class PhoneNumber
{
    public string Raw { get; }
    public string E164 { get; }

    private PhoneNumber(string raw, string e164)
    {
        Raw = raw;
        E164 = e164;
    }
}