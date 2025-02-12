using System;

/* what are NuGet packages?
 * https://learn.microsoft.com/en-us/nuget/what-is-nuget
 * 
 * "...defines how packages for .NET are created, hosted, and consumed, and 
 * provides the tools for each of those roles"
 * 
 * "...a single ZIP file with the .nupkg extension that contains compiled code 
 * (DLLs), other files related to that code, and a descriptive manifest that 
 * includes information like the package's version number
 * 
 * pretty much just the package manager for .NET packages
 */
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using TradingEngineServer.Core;

/* 
 * builds entire dependency injection container and hosted service
 * 
 * notes:
 * - using allows for engine to be disposed of when the program terminates
 */
using var engine = TradingEngineServerHostBuilder.BuildTradingEngineServer();
TradingEngineServerServiceProvider.ServiceProvider = engine.Services;

// adding scope in case we need it later on
{
    using var scope = TradingEngineServerServiceProvider.ServiceProvider.CreateScope();
    await engine.RunAsync().ConfigureAwait(false);
}
