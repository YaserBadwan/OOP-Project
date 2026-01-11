using System;

namespace PhoneBook.ConsoleApp.Infrastructure;

public static class AppPaths
{
    private const string AppFolderName = "PhoneBook";

    public static string DataDir
    {
        get
        {
            var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(baseDir, AppFolderName);
        }
    }

    public static string LogsDir => Path.Combine(DataDir, "logs");

    public static string ResolveDataFile(string fileName)
        => Path.Combine(DataDir, fileName);
}