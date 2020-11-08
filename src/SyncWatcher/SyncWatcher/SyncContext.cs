using System;
using System.IO;

namespace SyncWatcher
{
	public class SyncContext
	{
		public string Source { get => m_source; set { m_source = NormalizePath(value); } }
		public string SourceFilter { get; set; } = "*.*";
		public string Destination { get => m_destination; set { m_destination = NormalizePath(value); } }

		private string m_source;
		private string m_destination;
		private FileSystemWatcher m_fileSystemWatcher = null;

		internal void CreateWatcher()
		{
			m_fileSystemWatcher = new FileSystemWatcher(Source, SourceFilter);
			m_fileSystemWatcher.IncludeSubdirectories = true;
			m_fileSystemWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Attributes | NotifyFilters.Size |
				NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.CreationTime | NotifyFilters.Security;
			m_fileSystemWatcher.Changed += FileSystemWatcher_Changed;
			m_fileSystemWatcher.Created += FileSystemWatcher_Created;
			m_fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;
			m_fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;
			m_fileSystemWatcher.EnableRaisingEvents = true;
		}

		private string NormalizePath(string i_path)
		{
			return i_path.Replace("\\", "/");
		}

		private string GetDestinationFile(string i_sourceFilePath)
		{
			string relativeFilePath = i_sourceFilePath.Substring(Source.Length).Trim("/".ToCharArray());
			return NormalizePath(Path.Combine(Destination, relativeFilePath));
		}

		private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
		{
			bool success = false;
			while (!success)
			{
				try
				{
					string sourceFile = NormalizePath(e.FullPath);
					string destinationFile = GetDestinationFile(sourceFile);
					File.Delete(destinationFile);

					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("  Deleted {0}", destinationFile);
					Console.ResetColor();

					success = true;
				}
				catch (IOException) { }
			}
		}

		private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
		{
			bool success = false;
			while (!success)
			{
				try
				{
					string destinationFile = GetDestinationFile(NormalizePath(e.FullPath));
					string oldDestinationFile = GetDestinationFile(NormalizePath(e.OldFullPath));
					File.Move(oldDestinationFile, destinationFile);

					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine("  Renamed {0} to {1}", oldDestinationFile, destinationFile);
					Console.ResetColor();

					success = true;
				}
				catch (IOException) { }
			}
		}

		private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
		{
			bool success = false;
			while (!success)
			{
				try
				{
					string sourceFile = NormalizePath(e.FullPath);
					string destinationFile = GetDestinationFile(sourceFile);
					File.Copy(sourceFile, destinationFile, true);

					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.WriteLine("  Created {0}", destinationFile);
					Console.ResetColor();

					success = true;
				}
				catch (IOException) { }
			}
		}

		private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
		{
			bool success = false;
			while (!success)
			{
				try
				{
					string sourceFile = NormalizePath(e.FullPath);
					if (!File.Exists(sourceFile))
					{
						return;
					}

					string destinationFile = GetDestinationFile(sourceFile);
					File.Copy(sourceFile, destinationFile, true);

					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("  Updated {0}", destinationFile);
					Console.ResetColor();

					success = true;
				}
				catch (IOException) { }
			}
		}
	}
}