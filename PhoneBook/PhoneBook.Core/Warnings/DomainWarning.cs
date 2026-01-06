namespace PhoneBook.Core.Warnings;

public sealed record class DomainWarning(string Code, string Message);

// Record este un tip de clasa care imi genereaza automat un constructor
// Imi declara Code si Message
// Imi defineste Equals, GetHashCode si ToString