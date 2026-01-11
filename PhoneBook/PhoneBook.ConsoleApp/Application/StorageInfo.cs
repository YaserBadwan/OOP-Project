namespace PhoneBook.ConsoleApp.Infrastructure;

public sealed class StorageInfo
{
    public string Provider { get; }
    public string? Path { get; }

    public StorageInfo(string provider, string? path)
    {
        Provider = provider;
        Path = path;
    }
}