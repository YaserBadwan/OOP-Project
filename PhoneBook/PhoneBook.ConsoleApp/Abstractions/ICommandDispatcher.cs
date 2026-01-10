namespace PhoneBook.ConsoleApp.CLI;

public interface ICommandDispatcher
{
    bool TryDispatch(string inputLine);
}