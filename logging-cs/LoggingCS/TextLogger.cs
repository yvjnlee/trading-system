using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Options;
using TradingEngineServer.Logging.LoggingConfiguration;

namespace TradingEngineServer.Logging
{
    public class TextLogger : AbstractLogger, ITextLogger
    {
        // private
        private readonly LoggerConfiguration _loggerConfiguration;
        
        public TextLogger(IOptions<LoggerConfiguration> loggingConfiguration) : base()
        {
            _loggerConfiguration = loggingConfiguration.Value ?? throw new ArgumentNullException(nameof(loggingConfiguration));
            if (_loggerConfiguration.LoggerType != LoggerType.Text)
            {
                throw new InvalidOperationException($"{nameof(TextLogger)} doesn't match LoggerType of {_loggerConfiguration.LoggerType}");
            }
            
            var now = DateTime.Now;
            string logDirectory = Path.Combine(_loggerConfiguration.TextLoggerConfiguration.Directory, $"{now:yyyy-MM-dd}");
            string uniqueLogName = $"{_loggerConfiguration.TextLoggerConfiguration.Filename}-{now:HH_mm_ss}";
            string baseLogName = Path.ChangeExtension(uniqueLogName, _loggerConfiguration.TextLoggerConfiguration.FileExtension);
            string filepath = Path.Combine(logDirectory, baseLogName);
            Directory.CreateDirectory(logDirectory);
            _ = Task.Run(() => LogAsync(filepath, _logQueue, _tokenSource.Token));
        }

        private static async void LogAsync(string filepath, BufferBlock<LogInformation> logQueue, CancellationToken token)
        {
            using var fs = new FileStream(filepath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
            using var sw = new StreamWriter(fs);
            try
            {
                while (true)
                {
                    var logItem = await logQueue.ReceiveAsync(token).ConfigureAwait(false);
                    string formattedMessage = FormatLogItem(logItem);
                    await sw.WriteLineAsync(formattedMessage).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private static string FormatLogItem(LogInformation logItem)
        {
            return $"[{logItem.Now:yyyy-MM-dd HH-mm-ss.fffffff}] [{logItem.ThreadName}:{logItem.ThreadId:000}] " + 
                   $"[{logItem.LogLevel}] {logItem.Message}";
        }

        protected override void Log(LogLevel logLevel, string module, string message)
        {
            _logQueue.Post(new LogInformation(logLevel, module, message, 
                DateTime.Now, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name));
        }
        
        // think of as destructor but not rly
        ~TextLogger()
        {
            Dispose(false);
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (_lock)
            {
                if (_disposed)
                    return;
                _disposed = true;
            }

            if (disposing)
            {
                // get rid of managed resources
                _tokenSource.Cancel();
                _tokenSource.Dispose();
            }
            
            // get rid of unmanaged resources
            
        }
        
        private readonly BufferBlock<LogInformation> _logQueue = new BufferBlock<LogInformation>();
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private readonly object _lock = new object();
        private bool _disposed = false;
    }
}