namespace PhoneBook.ConsoleApp.Abstractions;

public interface IPrompt
{
    string? ReadLine(string? label = null);

    string Required(string label);
    string? Optional(string label);

    string EditRequired(string label, string current);
    string? EditOptional(string label, string? current);

    bool Confirm(string label);

    int ChooseIndex(string title, IReadOnlyList<string> options, int? defaultIndex = null);
    TEnum ChooseEnum<TEnum>(string title, TEnum current) where TEnum : struct, Enum;
    
    string? Email(string label);
    string? EditEmail(string label, string? current);

    DateOnly? DateOptional(string label, string format = "yyyy-MM-dd");
    DateOnly? EditDate(string label, DateOnly? current, string format = "yyyy-MM-dd");
}