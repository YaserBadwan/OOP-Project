using PhoneBook.Core.Abstractions;
using PhoneBook.Core.Exceptions;

namespace PhoneBook.Testing.TestDoubles;

public sealed class FakePhoneNumberNormalizer : IPhoneNumberNormalizer
{
    public string ToE164(string raw, string defaultRegion)
    {
        if (raw is null)
        {
            throw new ValidationException("Phone number is required.");

        }
        var trimmed = raw.Trim();

        if (trimmed.Length < 2 || trimmed[0] != '+')
        {
            throw new ValidationException("Invalid phone number format.");
        }

        for (var i = 1; i < trimmed.Length; i++)
        {
            var ch = trimmed[i];
            if (ch < '0' || ch > '9')
            {
                throw new ValidationException("Invalid phone number format.");
            }
        }

        return trimmed;
    }
}