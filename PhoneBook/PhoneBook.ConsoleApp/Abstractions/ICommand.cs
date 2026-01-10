namespace PhoneBook.ConsoleApp.CLI;

public interface ICommand
{
    string Verb { get; }
    void Execute(CommandContext context, string[] args);
}