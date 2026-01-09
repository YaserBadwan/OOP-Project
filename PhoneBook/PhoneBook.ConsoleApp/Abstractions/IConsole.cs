namespace PhoneBook.ConsoleApp.Abstractions;

public interface IConsole
{
    void WriteLine(string text);
    string? ReadLine();
}