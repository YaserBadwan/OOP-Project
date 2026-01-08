namespace PhoneBook.Infrastructure.JsonStorage;

public sealed class JsonFileStorageOptions
{
    public string FilePath { get; init; } = "phonebook.json";
    public bool PrettyPrint { get; init; } = true;
}