namespace PhoneBook.ConsoleApp.Infrastructure;

public sealed class StorageInfo
{
    public string Provider { get; }
    public string? Details { get; }

    public StorageInfo(string provider, string? details)
    {
        Provider = provider;
        Details = details;
    }
    
    public string ToDisplayString() => $"Storage: {Provider} ({Details})";
}