using System;

namespace TradingEngineServer.Logging
{
    public record LogInformation(LogLevel LogLevel, string Module, string Message, DateTime Now, int ThreadId, string ThreadName);
}

// look into this more later
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit
    {
    };
}