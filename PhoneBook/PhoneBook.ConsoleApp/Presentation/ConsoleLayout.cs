using PhoneBook.ConsoleApp.Abstractions;

namespace PhoneBook.ConsoleApp.Presentation;

public class ConsoleLayout
{
    public static void PrintTitle(IConsole c, string title, string? subtitle = null)
    {
        c.WriteLine("");
        c.WriteLine(title);
        c.WriteLine(new string('=', title.Length));

        if (!string.IsNullOrWhiteSpace(subtitle))
            c.WriteLine(subtitle);

        c.WriteLine("");
    }

    public static void PrintSection(IConsole c, string title)
    {
        c.WriteLine(title);
        c.WriteLine(new string('-', title.Length));
    }

    public static void PrintCommand(IConsole c, string command, string description, int width = 20)
    {
        c.WriteLine($"  {command.PadRight(width)} {description}");
    }

    public static void PrintKeyValue(IConsole c, string key, string value, int keyWidth = 13)
    {
        c.WriteLine($"{key.PadRight(keyWidth)} {value}");
    }
}