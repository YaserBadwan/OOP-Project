namespace PhoneBook.Infrastructure.JsonStorage;

internal sealed class PhoneBookStateDto
{
    public int SchemaVersion { get; set; } = 1;
    public List<ContactDto> Contacts { get; set; } = new();
}

internal sealed class ContactDto
{
    public string PhoneE164 { get; set; } = "";
    public string? PhoneRaw { get; set; }
    
    public string FirstName { get; set; } = "";
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Pronouns { get; set; }
    public int? Ringtone { get; set; }
    public DateOnly? Birthday { get; set; }
    public string? Notes { get; set; }
}