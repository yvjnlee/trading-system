using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using TradingEngineServer.Core.Configuration;

namespace TradingEngineServer.Core
{
	/*
	 * BackgroundService puts a contstraint on TradingEngineServer 
	 * - needs to overide background service's key method because it inherits
	 *   from BackgroundService
	 * - in this case it's ExecuteAsync
	 * 
	 * questions:
	 * - since its a background service does it mean that it can be a server?
	 * - is this polling, event based or something else?
	 */
	sealed class TradingEngineServer: BackgroundService, ITradingEngineServer
	{
		/*
		 * want to be able to log what we do, when we do something
		 * 
		 * notes:
		 * - we have the type of class we wanna log
		 * 
		 */
		private readonly ILogger<TradingEngineServer> _logger;
		private readonly TradingEngineServerConfiguration _tradingEngineServerConfig;

        /*
		 * we want to dependency inject the settings for the 
		 * trading engine server
		 * 
		 * notes:
		 * - we dont wanna write code to read from this file/format it 
		 * - we want Microsoft's library to take care of that for us
		 */
        public TradingEngineServer(ILogger<TradingEngineServer> logger,
			IOptions<TradingEngineServerConfiguration> config)
		{
			// we want to ensure the logger is not null (sanity check)
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));

			// the ?? (null-coalescing operator) returns the LHS opearand if it isn't null
			_tradingEngineServerConfig = config.Value ?? throw new ArgumentNullException(nameof(config));
		}

        /*
		 * this is how we are getting ExecuteAsync to be public
		 * 
		 * notes:
		 * - we don't necessarily need this here right now
		 * - just for completion
		 * 
		 * questions:
		 * - how does forwarding (the => operator) work?
		 *		- https://learn.microsoft.com/en-us/dotnet/standard/assembly/type-forwarding
		 *		- https://stackoverflow.com/questions/736968/how-do-you-explain-type-forwarding-in-simple-terms
		 *		- "... Type forwarding allows you to move a type to another assembly 
		 *		without having to recompile applications that use the original assembly"
		 */
        public Task Run(CancellationToken token) => ExecuteAsync(token);

        /*
         * notes: 
		 * - with services or game enginers there will always be a loop
		 * - server may not need a loop but good to have for completion
		 * - BackgroundService has a protected abstract method called ExecuteAsync
		 *		- we need to overide this to use the server
		 *		- we need to make this available in a public mehtod under TradningEngineServer
		 */
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
			_logger.LogInformation($"Starting {nameof(TradingEngineServer)}");

            while (!stoppingToken.IsCancellationRequested)
			{

			}

            _logger.LogInformation($"Stopped {nameof(TradingEngineServer)}");

            return Task.CompletedTask;
        }
    }
}

