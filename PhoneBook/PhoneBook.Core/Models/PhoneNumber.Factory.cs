using PhoneBook.Core.Abstractions;
using PhoneBook.Core.Exceptions;

namespace PhoneBook.Core.Models;

public sealed partial class PhoneNumber
{
    private const string DefaultRegion = "RO";
    
    public static PhoneNumber Create(string raw, IPhoneNumberNormalizer normalizer, string? defaultRegion = null)
    {
        if (normalizer is null)
            throw new ArgumentNullException(nameof(normalizer));

        raw = (raw ?? string.Empty).Trim();
        if (raw.Length == 0)
            throw new ValidationException("Phone Number is required!");

        var region = string.IsNullOrWhiteSpace(defaultRegion)
            ? DefaultRegion
            : defaultRegion;

        var e164 = normalizer.ToE164(raw, region);

        return new PhoneNumber(raw, e164);
    }
}