using System;
using System.Threading;
using System.Threading.Tasks;

namespace TradingEngineServer.Core
{
	interface ITradingEngineServer
	{
        /*
		 *  what is a task?
		 *  https://learn.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/task-based-asynchronous-pattern-tap
		 *  
		 *  notes: 
		 *  - "...resembles a thread or ThreadPool work item but at a higher level of abstraction"
		 *  - provides two main benefits
		 *		- more efficient
		 *		- more scalable use of system resources
		 *	- allows for concurrency but with more progtammatic control with a thread or work item
		 */
        Task Run(CancellationToken token);
	}
}

