namespace PhoneBook.ConsoleApp.Infrastructure.Logging;

public sealed class FileLoggerOptions
{
    public string DirectoryPath { get; set; } = "logs";
    public string FileNamePrefix { get; set; } = "phonebook";
}