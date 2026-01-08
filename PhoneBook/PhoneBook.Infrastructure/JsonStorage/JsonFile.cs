using System.Text.Json;

namespace PhoneBook.Infrastructure.JsonStorage;

internal static class JsonFile
{
    public static T? Read<T>(string path, JsonSerializerOptions options)
    {
        if (!File.Exists(path))
        {
            return default;
        }
        
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(json, options);
    }
    
    public static void WriteAtomic<T>(string path, T value, JsonSerializerOptions options)
    {
        EnsureDirectoryExists(path);
        
        var fullPath = Path.GetFullPath(path);
        var tmpPath = fullPath + ".tmp";
        
        var json = JsonSerializer.Serialize<T>(value, options);
        File.WriteAllText(tmpPath, json);

        if (File.Exists(fullPath))
        {
            var backupPath = fullPath + ".bak";
            File.Replace(tmpPath, fullPath, backupPath, ignoreMetadataErrors: true);
            TryDelete(backupPath);
        }
        else
        {
            File.Move(tmpPath, fullPath);
        }
    }

    private static void EnsureDirectoryExists(string filePath)
    {
        var dir =  Path.GetDirectoryName(Path.GetFullPath(filePath));
        if (!string.IsNullOrWhiteSpace(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            
        }
    }
}