using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text;

namespace PhoneBook.ConsoleApp.Infrastructure.Logging;

public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly FileLoggerOptions _options;
    private readonly BlockingCollection<string> _queue = new(new ConcurrentQueue<string>());
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _worker;

    public FileLoggerProvider(FileLoggerOptions options)
    {
        _options = options;

        Directory.CreateDirectory(_options.DirectoryPath);

        _worker = Task.Run(async () =>
        {
            var path = GetLogFilePath();
            await using var stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            await using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            try
            {
                foreach (var line in _queue.GetConsumingEnumerable(_cts.Token))
                {
                    await writer.WriteLineAsync(line);
                }
            }
            catch (OperationCanceledException)
            {
                
            }
        }, _cts.Token);
    }

    public ILogger CreateLogger(string categoryName) => new FileLogger(categoryName, Enqueue);

    public void Dispose()
    {
        _cts.Cancel();
        _queue.CompleteAdding();
        try
        {
            _worker.Wait(TimeSpan.FromSeconds(2));
        }
        catch
        {
            
        }
        _cts.Dispose();
        _queue.Dispose();
    }

    private void Enqueue(string line)
    {
        if (!_queue.IsAddingCompleted)
            _queue.Add(line);
    }

    private string GetLogFilePath()
    {
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var file = $"{_options.FileNamePrefix}-{date}.log";
        return Path.Combine(_options.DirectoryPath, file);
    }

    private sealed class FileLogger : ILogger
    {
        private readonly string _category;
        private readonly Action<string> _write;

        public FileLogger(string category, Action<string> write)
        {
            _category = category;
            _write = write;
        }

        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            var message = formatter(state, exception);
            var ts = DateTime.UtcNow.ToString("O");

            var line = exception is null
                ? $"{ts} [{logLevel}] {_category} {eventId.Id}: {message}"
                : $"{ts} [{logLevel}] {_category} {eventId.Id}: {message}{Environment.NewLine}{exception}";

            _write(line);
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();
            public void Dispose() { }
        }
    }
}
