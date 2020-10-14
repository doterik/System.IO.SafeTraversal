using System.Collections.Generic;
using System.Linq;

namespace System.IO.SafeTraversal.Core
{
	public partial class SafeTraversal
	{
		#region Top Level Traversals
		private IEnumerable<FileInfo> TopLevelFilesTraversal(DirectoryInfo path, Func<FileInfo, bool>? filter = null)
		{
			FileInfo[]? files; // IEnumerable<FileInfo>? files = null;
			try
			{
				files = filter is null
					? path.GetFiles()
					: path.GetFiles().Where(x => x.Pass(filter)).ToArray();
			}
			catch (UnauthorizedAccessException ex)
			{
				files = null;
				OnLogError(new TraversalError(ex.Message));
			}
			catch (Exception ex)
			{
				files = null;
				OnLogError(new TraversalError(ex.Message));
			}

			for (var i = 0; i < files?.Length; i++) yield return files[i];
		}

		private IEnumerable<string> TopLevelFilesTraversal(string path, Func<FileInfo, bool>? filter = null)
		{
			string[]? files; // IEnumerable<string>? files = null;
			try
			{
				files = filter is null
					? Directory.GetFiles(path)
					: new DirectoryInfo(path).GetFiles().Where(x => x.Pass(filter)).Select(x => x.FullName).ToArray();
			}
			catch (UnauthorizedAccessException ex)
			{
				files = null;
				OnLogError(new TraversalError(ex.Message));
			}
			catch (Exception ex)
			{
				files = null;
				OnLogError(new TraversalError(ex.Message));
			}

			for (var i = 0; i < files?.Length; i++) yield return files[i];
		}

		private IEnumerable<DirectoryInfo> TopLevelDirectoriesTraversal(DirectoryInfo path, Func<DirectoryInfo, bool>? filter = null)
		{
			DirectoryInfo[]? dirs; // IEnumerable<DirectoryInfo>? dirs = null;
			try
			{
				dirs = filter is null
					? path.GetDirectories()
					: path.GetDirectories().Where(x => x.Pass(filter)).ToArray();
			}
			catch (UnauthorizedAccessException ex)
			{
				dirs = null;
				OnLogError(new TraversalError(ex.Message));
			}
			catch (Exception ex)
			{
				dirs = null;
				OnLogError(new TraversalError(ex.Message));
			}

			for (var i = 0; i < dirs?.Length; i++) yield return dirs[i];
		}

		private IEnumerable<string> TopLevelDirectoriesTraversal(string path, Func<DirectoryInfo, bool>? filter = null)
		{
			string[]? dirs; // IEnumerable<string>? dirs = null;
			try
			{
				dirs = filter is null
					? Directory.GetDirectories(path)
					: new DirectoryInfo(path).GetDirectories(path).Where(x => x.Pass(filter)).Select(x => x.FullName).ToArray();
			}
			catch (UnauthorizedAccessException ex)
			{
				dirs = null;
				OnLogError(new TraversalError(ex.Message));
			}
			catch (Exception ex)
			{
				dirs = null;
				OnLogError(new TraversalError(ex.Message));
			}

			for (var i = 0; i < dirs?.Length; i++) yield return dirs[i];
		}
		#endregion

		#region All Directories Level Traversals
		private IEnumerable<FileInfo> TraverseFilesCore(DirectoryInfo path, Func<FileInfo, bool>? filter = null)
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
				catch (UnauthorizedAccessException ex)
				{
					files = null;
					OnLogError(new TraversalError(ex.Message));
				}
				catch (Exception ex)
				{
					files = null;
					OnLogError(new TraversalError(ex.Message));
				}

				for (var i = 0; i < files?.Length; i++) yield return files[i];

				DirectoryInfo[]? dirs = null;
				try
				{
					dirs = currentDir.GetDirectories();
				}
				catch (UnauthorizedAccessException ex)
				{
					dirs = null;
					OnLogError(new TraversalError(ex.Message));
				}
				catch (Exception ex)
				{
					dirs = null;
					OnLogError(new TraversalError(ex.Message));
				}

				for (var i = 0; i < dirs?.Length; i++) directories.Enqueue(dirs[i]);
			}
		}

		private IEnumerable<string> TraverseFilesCore(string path, Func<FileInfo, bool>? filter = null)
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
				catch (UnauthorizedAccessException ex)
				{
					files = null;
					OnLogError(new TraversalError(ex.Message));
				}
				catch (Exception ex)
				{
					files = null;
					OnLogError(new TraversalError(ex.Message));
				}

				for (var i = 0; i < files?.Length; i++) yield return files[i];

				string[]? dirs = null;
				try
				{
					dirs = Directory.GetDirectories(currentDir);
				}
				catch (UnauthorizedAccessException ex)
				{
					dirs = null;
					OnLogError(new TraversalError(ex.Message));
				}
				catch (Exception ex)
				{
					dirs = null;
					OnLogError(new TraversalError(ex.Message));
				}

				for (var i = 0; i < dirs?.Length; i++) directories.Enqueue(dirs[i]);
			}
		}

		private IEnumerable<DirectoryInfo> TraverseDirectoriesCore(DirectoryInfo path, Func<DirectoryInfo, bool>? filter = null)
		{
			var directories = new Queue<DirectoryInfo>();
			directories.Enqueue(path);
			while (directories.Count > 0)
			{
				DirectoryInfo currentDir = directories.Dequeue();
				DirectoryInfo[]? dirs;
				try
				{
					dirs = currentDir.GetDirectories();
				}
				catch (UnauthorizedAccessException ex)
				{
					dirs = null;
					OnLogError(new TraversalError(ex.Message));
				}
				catch (Exception ex)
				{
					dirs = null;
					OnLogError(new TraversalError(ex.Message));
				}

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
						bool found = true; // TODO: true?
						try { found = filter(dirs[i]); } // To prevent malicious injection.
						catch { found = false; }

						if (found) yield return dirs[i]; // TODO Check order.
						directories.Enqueue(dirs[i]);
					}
				}
			}
		}

		private IEnumerable<string> TraverseDirectoriesCore(string path, Func<DirectoryInfo, bool>? filter = null)
		{
			var directories = new Queue<DirectoryInfo>();
			directories.Enqueue(new DirectoryInfo(path));
			while (directories.Count > 0)
			{
				DirectoryInfo currentDir = directories.Dequeue();
				DirectoryInfo[]? dirs;
				try
				{
					dirs = currentDir.GetDirectories();
				}
				catch (UnauthorizedAccessException ex)
				{
					dirs = null;
					OnLogError(new TraversalError(ex.Message));
				}
				catch (Exception ex)
				{
					dirs = null;
					OnLogError(new TraversalError(ex.Message));
				}

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
						bool found = true; // TODO: true ?
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
