using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SyncWatcher
{
	class Program
	{
		static bool s_wait = true;

		static async Task Main(string[] args)
		{
			try
			{
				Console.WriteLine("Loading configuration...");
				string configFilePath = args.Length > 0 ? args[0] : "config.swconf";
				string configFileContents = await File.ReadAllTextAsync(configFilePath);
				Configuration configuration = JsonConvert.DeserializeObject<Configuration>(configFileContents);
				Console.WriteLine("Configuration loaded");

				Console.WriteLine("Starting up...");
				SyncWatcher syncWatcher = new SyncWatcher(configuration);

				Console.WriteLine("Watching for file changes...");
				syncWatcher.StartWatching();

				Console.CancelKeyPress += Console_CancelKeyPress;

				while (s_wait)
				{
				}
			}
			catch (ArgumentException) { }
		}

		private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			s_wait = false;
			e.Cancel = true;
		}
	}
}
