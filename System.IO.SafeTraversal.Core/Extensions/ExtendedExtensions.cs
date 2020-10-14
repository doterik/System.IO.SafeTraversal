using System.Collections.Generic;
using System.Linq;
namespace System.IO.SafeTraversal.Core
{
	public partial class SafeTraversal
	{
		#region Top Level Traversals
		private static IEnumerable<FileInfo> ExTopLevelFilesTraversal(DirectoryInfo path, Func<FileInfo, bool>? filter = null)
		{
			FileInfo[]? files; // IEnumerable<FileInfo>? files = null;
			try
			{
				files = filter is null
					? path.GetFiles()
					: path.GetFiles().Where(x => x.Pass(filter)).ToArray();
			}
			catch { files = null; }

			for (var i = 0; i < files?.Length; i++) yield return files[i];
		}

		private static IEnumerable<string> ExTopLevelFilesTraversal(string path, Func<FileInfo, bool>? filter = null)
		{
			string[]? files; // IEnumerable<string>? files = null;
			try
			{
				files = filter is null
					? Directory.GetFiles(path)
					: new DirectoryInfo(path).GetFiles().Where(x => x.Pass(filter)).Select(x => x.FullName).ToArray();
			}
			catch { files = null; }

			for (var i = 0; i < files?.Length; i++) yield return files[i];
		}

		private static IEnumerable<DirectoryInfo> ExTopLevelDirectoriesTraversal(DirectoryInfo path, Func<DirectoryInfo, bool>? filter = null)
		{
			DirectoryInfo[]? dirs; // IEnumerable<DirectoryInfo>? dirs = null;
			try
			{
				dirs = filter is null
					? path.GetDirectories()
					: path.GetDirectories().Where(x => x.Pass(filter)).ToArray();
			}
			catch { dirs = null; }

			for (var i = 0; i < dirs?.Length; i++) yield return dirs[i];
		}

		private static IEnumerable<string> ExTopLevelDirectoriesTraversal(string path, Func<DirectoryInfo, bool>? filter = null)
		{
			string[]? dirs; // IEnumerable<string>? dirs = null;
			try
			{
				dirs = filter is null
					? Directory.GetDirectories(path)
					: new DirectoryInfo(path).GetDirectories(path).Where(x => x.Pass(filter)).Select(x => x.FullName).ToArray();
			}
			catch { dirs = null; }

			for (var i = 0; i < dirs?.Length; i++) yield return dirs[i];
		}
		#endregion

		#region All Directories Level Traversals
		private static IEnumerable<FileInfo> ExTraverseFilesCore(DirectoryInfo path, Func<FileInfo, bool>? filter = null)
		{
			var directories = new Queue<DirectoryInfo>();
			directories.Enqueue(path);
			while (directories.Count > 0)
			{
				DirectoryInfo currentDir = directories.Dequeue();
				FileInfo[]? files; // IEnumerable<FileInfo>? files = null;
				try
				{
					files = filter is null
						? currentDir.GetFiles()
						: currentDir.GetFiles().Where(x => x.Pass(filter)).ToArray();
				}
				catch { files = null; }

				for (var i = 0; i < files?.Length; i++) yield return files[i];

				DirectoryInfo[]? dirs = null;
				try { dirs = currentDir.GetDirectories(); }
				catch { dirs = null; }

				for (var i = 0; i < dirs?.Length; i++) directories.Enqueue(dirs[i]);
			}
		}

		private static IEnumerable<string> ExTraverseFilesCore(string path, Func<FileInfo, bool>? filter = null)
		{
			var directories = new Queue<string>();
			directories.Enqueue(path);
			while (directories.Count > 0)
			{
				string currentDir = directories.Dequeue();
				string[]? files; // IEnumerable<string>? files = null;
				try
				{
					files = filter is null
						? Directory.GetFiles(currentDir)
						: new DirectoryInfo(currentDir).GetFiles().Where(x => x.Pass(filter)).Select(x => x.FullName).ToArray();
				}
				catch { files = null; }

				for (var i = 0; i < files?.Length; i++) yield return files[i];

				string[]? dirs = null;
				try { dirs = Directory.GetDirectories(currentDir); }
				catch { dirs = null; }

				for (var i = 0; i < dirs?.Length; i++) directories.Enqueue(dirs[i]);
			}
		}

		private static IEnumerable<DirectoryInfo> ExTraverseDirectoriesCore(DirectoryInfo path, Func<DirectoryInfo, bool>? filter = null)
		{
			var directories = new Queue<DirectoryInfo>();
			directories.Enqueue(path);
			while (directories.Count > 0)
			{
				DirectoryInfo currentDir = directories.Dequeue();
				DirectoryInfo[]? dirs;
				try { dirs = currentDir.GetDirectories(); }
				catch { dirs = null; }

				if (filter is null)
				{
					for (var i = 0; i < dirs?.Length; i++)
					{
						directories.Enqueue(dirs[i]); // TODO Check order.
						yield return dirs[i];
					}
				}
				else
				{
					for (var i = 0; i < dirs?.Length; i++)
					{
						bool found;
						try { found = filter(dirs[i]); } // To prevent malicious injection.
						catch { found = false; }

						if (found) yield return dirs[i]; // TODO Check order.
						directories.Enqueue(dirs[i]);
					}
				}
			}
		}

		private static IEnumerable<string> ExTraverseDirectoriesCore(string path, Func<DirectoryInfo, bool>? filter = null)
		{
			var directories = new Queue<DirectoryInfo>();
			directories.Enqueue(new DirectoryInfo(path));
			while (directories.Count > 0)
			{
				DirectoryInfo currentDir = directories.Dequeue();
				DirectoryInfo[]? dirs;
				try { dirs = currentDir.GetDirectories(); }
				catch { dirs = null; }

				if (filter is null)
				{
					for (var i = 0; i < dirs?.Length; i++)
					{
						directories.Enqueue(dirs[i]); // TODO Check order.
						yield return dirs[i].FullName;
					}
				}
				else
				{
					for (var i = 0; i < dirs?.Length; i++)
					{
						bool found;
						try { found = filter(dirs[i]); } // To prevent malicious injection.
						catch { found = false; }

						if (found) yield return dirs[i].FullName; // TODO Check order.
						directories.Enqueue(dirs[i]);
					}
				}
			}
		}
		#endregion
	}
}
