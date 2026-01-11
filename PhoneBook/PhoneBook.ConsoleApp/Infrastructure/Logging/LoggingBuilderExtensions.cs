using Microsoft.Extensions.Logging;

namespace PhoneBook.ConsoleApp.Infrastructure.Logging;

public static class LoggingBuilderExtensions
{
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder, FileLoggerOptions options)
    {
        builder.AddProvider(new FileLoggerProvider(options));
        return builder;
    }
}