using PhoneBook.Core.Abstractions;
using PhoneBook.Core.Exceptions;
using PhoneBook.Core.Models;
using PhoneBook.Core.State;
using PhoneBook.Core.Storage;
using PhoneBook.Core.Warnings;

namespace PhoneBook.Core.Services;

public sealed class PhoneBookService
{
    private readonly IPhoneBookStateStorage _storage; 
    private readonly IPhoneNumberNormalizer _phoneNumberNormalizer;
    
    private PhoneBookState _state;
    
    private const string DefaultRegion = "RO";

    public PhoneBookService(
        IPhoneBookStateStorage storage,
        IPhoneNumberNormalizer phoneNumberNormalizer)
    {
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        _phoneNumberNormalizer = phoneNumberNormalizer ?? throw new ArgumentNullException(nameof(phoneNumberNormalizer));
        
        _state = _storage.Load();
    }

    
    // Lista -> implicit sortata alfabetic
    public IReadOnlyList<Contact> ListAll()
    {
        return _state.Contacts
            .OrderBy(c => c.FirstName)
            .ThenBy(c => c.LastName)
            .ThenBy(c => c.PhoneNumber.E164)
            .ToList();
    }
    
    // Add cu autosave 
    public (Contact Added, IReadOnlyList<DomainWarning> Warnings) Add(Contact contact)
    {
        if (contact is null)
        {
            throw new ArgumentNullException(nameof(contact));
        }

        EnsureUniquePhoneOrThrow(contact.PhoneNumber.E164, exceptE164: null);

        var warnings = BuildDuplicateNameWarnings(contact, exceptE164: null);
        
        _state.Contacts.Add(contact);
        Persist();
        
        return (contact, warnings);
    }
    
    // GET
    public Contact GetByPhone(string phone)
    {
        var e164 = NormalizePhoneKeyOrThrow(phone);

        var existing = FindByE164OrNull(e164);
        if (existing is null)
        {
            throw new ContactNotFoundException($"Contact not found for phone: {e164}");
        }
        
        return existing;
    }
    
    // DELETE cu confirmare in UI
    public void DeleteByPhone(string phone)
    {
        var e164 = NormalizePhoneKeyOrThrow(phone);
        
        var existing = FindByE164OrNull(e164);
        if (existing is null)
        {
            throw new ContactNotFoundException($"Contact not found for phone: {e164}");
        }
        
        _state.Contacts.Remove(existing);
        Persist();
    }
    
    // Update
    public (Contact Updated, IReadOnlyList<DomainWarning> Warnings)
        Update(string originalPhone, Contact updated)
    {
        if (updated is null)
        {
            throw new ArgumentNullException(nameof(updated));
        }

        var originalE164 = NormalizePhoneKeyOrThrow(originalPhone);

        var index = _state.Contacts.FindIndex(c => c.PhoneNumber.E164 == originalE164);
        if (index < 0)
        {
            throw new ContactNotFoundException($"Contact not found for phone: {originalE164}");
        }
        
        // Daca user-ul a schimbat numarul, verificam unicitatea, excluzand contactul curent
        EnsureUniquePhoneOrThrow(updated.PhoneNumber.E164, exceptE164: originalE164);

        var warnings = BuildDuplicateNameWarnings(updated, exceptE164: originalE164);
        
        // Replace snapshot (simplu si predictibil)
        _state.Contacts[index] = updated;
        Persist();
        
        return (updated, warnings);
    }
    
    // SEARCH cu exact match, case-insensitive, OR
    // Daca sunt multiple rezultate, UI le afiseaza si user alege
    public IReadOnlyList<Contact> SearchExact(string? query)
    {
        query = (query ?? string.Empty).Trim();
        if (query.Length == 0)
        {
            return Array.Empty<Contact>();
        }
        
        // Incercam sa interpretam query-ul si ca telefon (E164)
        var queryE164 = TryNormalizeToE164OrNull(query);
        
        return _state.Contacts
            .Where(c =>
                EqualsCi(c.FirstName, query) ||
                (c.LastName is not null && EqualsCi(c.LastName, query)) ||
                EqualsCi(c.PhoneNumber.Raw, query) ||
                (queryE164 is not null && c.PhoneNumber.E164 == queryE164))
            .OrderBy(c => c.FirstName)
            .ThenBy(c => c.LastName)
            .ThenBy(c => c.PhoneNumber.E164)
            .ToList();
    }
    
    // HELPERS
    private void Persist()
    {
        _storage.Save(_state);
    }
    
    private Contact? FindByE164OrNull(string e164)
        => _state.Contacts.FirstOrDefault(c => c.PhoneNumber.E164 == e164);

    private void EnsureUniquePhoneOrThrow(string newE164, string? exceptE164)
    {
        var exists = _state.Contacts.Any(c =>
            c.PhoneNumber.E164 == newE164 && (exceptE164 is null || c.PhoneNumber.E164 != exceptE164));

        if (exists)
        {
            throw new DuplicatePhoneNumberException($"Phone number already exists for phone: {newE164}");
        }
    }

    private IReadOnlyList<DomainWarning> BuildDuplicateNameWarnings(Contact candidate, string? exceptE164)
        {
            bool duplicate = _state.Contacts.Any(c =>
            {
                if (exceptE164 is not null && c.PhoneNumber.E164 == exceptE164)
                {
                    return false;
                }

                bool sameFirst = EqualsCi(c.FirstName, candidate.FirstName);

                bool sameLast =
                    (c.LastName is null && candidate.LastName is null) ||
                    (c.LastName is not null && candidate.LastName is not null &&
                     EqualsCi(c.LastName, candidate.LastName));

                return sameFirst && sameLast;
            });

            if (!duplicate)
            {
                return Array.Empty<DomainWarning>();
            }

            var last = candidate.LastName is null ? "" : $" {candidate.LastName}";
            return new[]
            {
                new DomainWarning(
                    Code: "DUPLICATE_NAME",
                    Message: $"Warning: another contact with the name '{candidate.FirstName}{last}' already exists.")
            };
        }

        private static bool EqualsCi(string? a, string? b)
            => string.Equals(a?.Trim(), b?.Trim(), StringComparison.OrdinalIgnoreCase);

        private string NormalizePhoneKeyOrThrow(string? phone)
        {
            phone = (phone ?? string.Empty).Trim();
            if (phone.Length == 0)
            {
                throw new ValidationException("Phone Number is required!");
            }
            
            var e164 = TryNormalizeToE164OrNull(phone);
            if (e164 is null)
            {
                throw new ValidationException("Phone Number is not valid!");
            }

            return e164;
    }

        private string? TryNormalizeToE164OrNull(string? raw)
        {
            raw = (raw ?? string.Empty).Trim();
            if (raw.Length == 0)
            {
                return null;
            }

            try
            {
                return _phoneNumberNormalizer.ToE164(raw, DefaultRegion);
            }
            catch (DomainException)
            {
                return null;
            }
        }
}