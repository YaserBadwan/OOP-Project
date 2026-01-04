using System.Text;

namespace PhoneBook.Core.Models;

public sealed class PhoneNumber : IEquatable<PhoneNumber>
{
    public string Raw { get; }
    public string E164 { get; }

    private PhoneNumber(string raw, string e164)
    {
        Raw = raw;
        E164 = e164;
    }
    
    public override string ToString() => Raw;
    
    public bool Equals(PhoneNumber? other)
        => other is not null && 
           string.Equals(E164, other.E164, StringComparison.Ordinal);

    public override bool Equals(object? obj) => obj is PhoneNumber other && Equals(other);
    
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(E164);
}