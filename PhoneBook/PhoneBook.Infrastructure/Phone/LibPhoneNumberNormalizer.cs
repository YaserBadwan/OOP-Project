using PhoneBook.Core.Abstractions;
using PhoneBook.Core.Exceptions;
using PhoneNumbers; // biblioteca pt normalizarea numerelor de telefon

namespace PhoneBook.Infrastructure.Phone;

public sealed class LibPhoneNumberNormalizer : IPhoneNumberNormalizer
{
    // PhoneNumberUtil este o clasa care contine metadata despre toate numerele de telefon
    // este readonly, fiindca este FOARTE mare. Se declara o singura data in tot proiectul.
    private readonly PhoneNumberUtil _util = PhoneNumberUtil.GetInstance();

    public string ToE164(string raw, string defaultRegion)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new ValidationException("Phone number is required.");

        try
        {
            var parsed = _util.Parse(raw, defaultRegion);

            if (!_util.IsValidNumber(parsed))
                throw new ValidationException("Phone number is invalid.");

            return _util.Format(parsed, PhoneNumberFormat.E164);
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ValidationException("Phone number is invalid.", ex);
        }

    }
}