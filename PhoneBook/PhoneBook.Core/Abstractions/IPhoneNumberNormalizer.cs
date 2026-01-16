using System.Security.AccessControl;

namespace PhoneBook.Core.Abstractions;

public interface IPhoneNumberNormalizer
{
    string ToE164(string raw, string defaultRegion);
}
