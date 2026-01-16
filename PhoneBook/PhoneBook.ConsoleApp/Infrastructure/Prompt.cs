using System.Text.RegularExpressions;
using PhoneBook.ConsoleApp.Abstractions;
using PhoneBook.Core.Exceptions;

namespace PhoneBook.ConsoleApp.Infrastructure;

public sealed class Prompt : IPrompt
{
    private readonly IConsole _console;
    
    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public Prompt(IConsole console)
    {
        _console = console;
    }

    public string? ReadLine(string? label = null)
    {
        if (!string.IsNullOrWhiteSpace(label))
            _console.Write(label);

        var input = _console.ReadLine();
        if (input is null)
        {
            return null;
        }

        if (input.Trim().Equals("cancel", StringComparison.OrdinalIgnoreCase))
        {
            throw new CommandAbortedException();
        }
            

        return input.Trim();
    }

    public string Required(string label)
    {
        while (true)
        {
            var input = ReadLine(label)?.Trim();

            if (!string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            _console.WriteError("! This field is required.");
        }
    }

    public string? Optional(string label)
    {
        var input = ReadLine(label)?.Trim();
        return string.IsNullOrWhiteSpace(input) ? null : input;
    }

    public string EditRequired(string label, string current)
    {
        while (true)
        {
            var input = ReadLine($"{label} [{current}]: ")?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                return current;
            }

            if (input == "-")
            {
                _console.WriteError("! This field is required and cannot be cleared.");
                continue;
            }

            return input;
        }
    }

    public string? EditOptional(string label, string? current)
    {
        var display = current ?? "-";
        var input = ReadLine($"{label} [{display}]: ")?.Trim();

        if (string.IsNullOrWhiteSpace(input))
        {
            return current;
        }

        if (input == "-")
        {
            return null;
        }

        return input;
    }

    public bool Confirm(string label)
    {
        var suffix = " (y/n) ";
        while (true)
        {
            var input = ReadLine(label + suffix)?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            if (input.Equals("y", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (input.Equals("n", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("no", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            _console.WriteError("! Please answer y/n (or type 'cancel').");
        }
    }

    public int ChooseIndex(string title, IReadOnlyList<string> options, int? defaultIndex = null)
    {
        _console.WriteLine(title);
        for (int i = 0; i < options.Count; i++)
        {
            _console.WriteLine($"  {i + 1}) {options[i]}");
        }

        while (true)
        {
            var defText = defaultIndex is null ? "" : $"Enter = {defaultIndex.Value + 1}";
            var input = ReadLine($"Selection ({defText}): ")?.Trim();

            if (string.IsNullOrWhiteSpace(input) && defaultIndex is not null)
            {
                return defaultIndex.Value;
            }

            if (int.TryParse(input, out var idx) && idx >= 1 && idx <= options.Count)
            {
                return idx - 1;
            }

            _console.WriteError("! Invalid selection. Try again.");
        }
    }

    public TEnum ChooseEnum<TEnum>(string title, TEnum current) where TEnum : struct, Enum
    {
        var values = Enum.GetValues<TEnum>();
        var names = values.Select(v => v.ToString()).ToList();

        var currentIndex = Array.IndexOf(values, current);
        var chosenIndex = ChooseIndex(title, names, currentIndex >= 0 ? currentIndex : null);

        return values[chosenIndex];
    }
    
    public string? Email(string label)
    {
        while (true)
        {
            var input = ReadLine(label)?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            if (IsValidEmail(input))
            {
                return input;
            }

            _console.WriteWarning("! Invalid email format.");
        }
    }

    public string? EditEmail(string label, string? current)
    {
        while (true)
        {
            var display = current ?? "-";
            var input = ReadLine($"{label} [{display}]: ")?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                return current;
            }

            if (input == "-")
            {
                return null;
            }

            if (IsValidEmail(input))
            {
                return input;
            }

            _console.WriteWarning("! Invalid email format.");
        }
    }

    private static bool IsValidEmail(string? email)
    {
        return email is null || EmailRegex.IsMatch(email);
    }
    
    public DateOnly? DateOptional(string label, string format = "yyyy-MM-dd")
    {
        while (true)
        {
            var input = ReadLine($"{label} [{format}]: ")?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            if (DateOnly.TryParseExact(input, format, out var d))
            {
                return d;
            }

            _console.WriteError("! Invalid date format. Try again.");
        }
    }

    public DateOnly? EditDate(string label, DateOnly? current, string format = "yyyy-MM-dd")
    {
        while (true)
        {
            var currentText = current is null ? "-" : current.Value.ToString(format);
            var input = ReadLine($"{label} [{currentText}]: ")?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                return current;
            }

            if (input == "-")
            {
                return null;
            }

            if (DateOnly.TryParseExact(input, format, out var d))
            {
                return d;
            }

            _console.WriteError("! Invalid date format (or type 'cancel').");
        }
    }
}
