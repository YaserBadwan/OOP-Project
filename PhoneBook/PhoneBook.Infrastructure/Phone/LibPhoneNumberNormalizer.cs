using PhoneBook.Core.Abstractions;
using PhoneNumbers; // biblioteca pt normalizarea numerelor de telefon

namespace PhoneBook.Infrastructure.Phone;

public sealed class LibPhoneNumberNormalizer : IPhoneNumberNormalizer
{
    private readonly PhoneNumberUtil _util = PhoneNumberUtil.GetInstance();

    public string ToE164(string raw, string defaultRegion)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new ArgumentException("Phone number is required.", nameof(raw));

        var parsed = _util.Parse(raw, defaultRegion);

        if (!_util.IsValidNumber(parsed))
            throw new ArgumentException("Phone number is invalid.", nameof(raw));

        return _util.Format(parsed, PhoneNumberFormat.E164);
    }
}