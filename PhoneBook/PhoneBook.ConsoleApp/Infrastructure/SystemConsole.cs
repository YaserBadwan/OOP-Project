using PhoneBook.ConsoleApp.Abstractions;

namespace PhoneBook.ConsoleApp.Infrastructure;

public class SystemConsole : IConsole
{
    public void WriteLine(string text)
    {
        Console.WriteLine(text);
    }

    public void Write(string text)
    {
        Console.Write(text);
    }
    public string? ReadLine()
    {
        return Console.ReadLine();
    }

    public void WriteError(string text)
    {
        var oldColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(text);
        Console.ForegroundColor = oldColor;
    }
    
    public void WriteWarning(string text)
    {
        var oldColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(text);
        Console.ForegroundColor = oldColor;
    }
    
    public void WriteSuccess(string text)
    {
        var old = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(text);
        Console.ForegroundColor = old;
    }
}