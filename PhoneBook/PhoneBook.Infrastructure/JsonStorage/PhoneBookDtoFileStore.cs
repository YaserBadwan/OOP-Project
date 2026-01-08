using System.Text.Json;
using PhoneBook.Core.State;

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
        => JsonFile.Read<PhoneBookStateDto>(_options.FilePath, _json);
    
    public void Save(PhoneBookStateDto dto)
        => JsonFile.WriteAtomic(_options.FilePath, dto, _json);
}