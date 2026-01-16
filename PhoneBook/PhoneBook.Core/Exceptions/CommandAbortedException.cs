namespace PhoneBook.Core.Exceptions;

public sealed class CommandAbortedException : Exception
{
    public CommandAbortedException() : base("Command aborted by user.") { }
}