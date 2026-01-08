using PhoneBook.Core.Abstractions;
using PhoneBook.Core.Exceptions;
using System.Text.RegularExpressions;

namespace PhoneBook.Core.Models;

public sealed partial class PhoneNumber
{
    private const string DefaultRegion = "RO";
    
    private static readonly Regex E164Regex =
        new(@"^\+[1-9]\d{1,14}$", RegexOptions.Compiled);
    
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

    // Folosita DOAR la Load() din storage :) 
    public static PhoneNumber FromE164(string e164, string? raw = null)
    {
        e164 = (e164 ?? string.Empty).Trim();
        if (!E164Regex.IsMatch(e164))
        {
            throw new ValidationException("Saved phone number is corrupted (invalid E.164 format).");
        }

        raw = (raw ?? e164).Trim();
        return new PhoneNumber(raw, e164);
    }
}