using System;

namespace TradingEngineServer.Logging
{
    public abstract class AbstractLogger : ILogger
    {
        protected AbstractLogger() {}
        public void Debug(string module, string message) => Log(LogLevel.Debug, module, message);
        public void Debug(string module, Exception exception) => Log(LogLevel.Debug, module, exception.ToString());
        public void Information(string module, string message) => Log(LogLevel.Information, module, message);
        public void Information(string module, Exception exception) => Log(LogLevel.Information, module, exception.ToString());
        public void Warning(string module, string message) => Log(LogLevel.Warning, module, message);
        public void Warning(string module, Exception exception) => Log(LogLevel.Warning, module, exception.ToString());
        public void Error(string module, string message) => Log(LogLevel.Error, module, message);
        public void Error(string module, Exception exception) => Log(LogLevel.Error, module, exception.ToString());
        protected abstract void Log(LogLevel logLevel, string module, string message);
    }
}