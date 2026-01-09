using PhoneBook.ConsoleApp.Abstractions;

namespace PhoneBook.ConsoleApp.Infrastructure;

public class SystemConsole : IConsole
{
    public void WriteLine(string text)
    {
        Console.WriteLine(text);
    }

    public string? ReadLine()
    {
        return Console.ReadLine();
    }
}