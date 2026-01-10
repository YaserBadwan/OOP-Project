namespace PhoneBook.ConsoleApp.Abstractions;

public interface IConsole
{
    void WriteLine(string text);
    void Write(string text);
    string? ReadLine();
    
    void WriteError(string text);
    void WriteWarning(string text);
    void WriteSuccess(string text);
}