namespace PhoneBook.Core.Exceptions;

public sealed class StorageException : DomainException
{
    public StorageException(string message) : base(message) { }
    public StorageException(string message, Exception innerException) : base(message, innerException) { }
}