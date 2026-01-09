using System.Text.Json;
using PhoneBook.Core.Exceptions;

namespace PhoneBook.Infrastructure.JsonStorage;

internal sealed class PhoneBookDtoFileStore
{
    private readonly JsonFileStorageOptions _options;
    private readonly JsonSerializerOptions _json;

    public PhoneBookDtoFileStore(JsonFileStorageOptions options)
    {
        _options = options;
        _json = new JsonSerializerOptions
        {
            WriteIndented = options.PrettyPrint,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public PhoneBookStateDto? TryLoad()
    {
        try
        {
            return JsonFile.Read<PhoneBookStateDto>(_options.FilePath, _json);
        }
        catch (JsonException ex)
        {
            throw new StorageException("Could not load phonebook data: the storage file is corrupted.", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new StorageException("Could not load phonebook data: permission denied.", ex);
        }
        catch (IOException ex)
        {
            throw new StorageException("Could not load phonebook data: I/O error while reading storage file.", ex);
        }
    }
    
    public void Save(PhoneBookStateDto dto)
        => JsonFile.WriteAtomic(_options.FilePath, dto, _json);
}