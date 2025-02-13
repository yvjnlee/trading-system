using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using TradingEngineServer.Logging;
using TradingEngineServer.Core.Configuration;
using TradingEngineServer.Logging.LoggingConfiguration;

/*
 * what does the host builder do?
 * 
 * notes:
 * - we want the TradingEngineServer to run asynchronously/be hosted
 * - we are building a host which will host and run the application as a background service
 * - we need a static method that builds a host for us
 *		- what is the importance of a static method?
 *			- https://stackoverflow.com/questions/202560/when-should-i-write-static-methods#:~:text=Static%20methods%20are%20usually%20useful,purpose%20solely%20using%20their%20arguments.
 * - adds the TradingEngineServer as its hosted service
 *		- can call the ExecuteSync operation
 * - 
 */

namespace TradingEngineServer.Core
{
	/*
	 * sealed class
	 * - nobody can override the content of the methods of this class
	 */
	public sealed class TradingEngineServerHostBuilder
	{
		public static IHost BuildTradingEngineServer()
			=> Host.CreateDefaultBuilder().ConfigureServices((hostContext, services) =>
			{
				// Start with config
				services.AddOptions();
				// configuring our services to show that TradingEngineServerConfiguration object exists
				services.Configure<TradingEngineServerConfiguration>(hostContext.Configuration.GetSection(nameof(TradingEngineServerConfiguration)));
				services.Configure<LoggerConfiguration>(hostContext.Configuration.GetSection(nameof(LoggerConfiguration)));

				/*
				 * Add singleton objects
				 * 
				 * questions:
				 * - what is a singleton object?
				 *		- 
				 */
				services.AddSingleton<ITradingEngineServer, TradingEngineServer>();
				services.AddSingleton<ITextLogger, TextLogger>();

				// this line will override line 45/previous singleton object associates with interfaces
				// services.AddSingleton<ITradingEngineServer, TradingEngineServer>();

				/*
				 * adding hosted service
				 */
				services.AddHostedService<TradingEngineServer>();
			}).Build();
	}
}

