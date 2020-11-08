namespace SyncWatcher
{
	class SyncWatcher
	{
		private readonly Configuration m_configuration;

		public SyncWatcher(Configuration i_configuration)
		{
			m_configuration = i_configuration;
		}

		public void StartWatching()
		{
			foreach (SyncContext syncContext in m_configuration.SyncContexts)
			{
				syncContext.CreateWatcher();
			}
		}
	}
}
