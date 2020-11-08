using System.Collections.Generic;

namespace SyncWatcher
{
	public class Configuration
	{
		public List<SyncContext> SyncContexts { get; set; } = new List<SyncContext>();
	}
}
