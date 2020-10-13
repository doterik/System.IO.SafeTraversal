using System.Collections.Generic;
using System.Linq;
namespace System.IO.SafeTraversal.Core
{
	public partial class SafeTraversal
	{
		#region Top Level Traversals
		private IEnumerable<FileInfo> TopLevelFilesTraversal(DirectoryInfo path)
		{
			FileInfo[]? files;
			try
			{
				files = path.GetFiles();
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
			if (files != null)
			{
				for (int i = 0; i < files.Length; i++)
				{
					yield return files[i];
				}
			}
		}
		private IEnumerable<FileInfo> TopLevelFilesTraversal(DirectoryInfo path, Func<FileInfo, bool> filter)
		{
			IEnumerable<FileInfo>? files = null;
			try
			{
				files = path.GetFiles().Where(x =>
				{
					var success = true; // Why? It prevents exception being thrown inside filter.
					try
					{
						success = filter(x);
					}
					catch { success = false; }
					return success;
				});
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
			if (files != null)
			{
				foreach (FileInfo file in files)
				{
					yield return file;
				}
			}
		}


		private IEnumerable<string> TopLevelFilesTraversal(string path)
		{
			string[]? files;
			try
			{
				files = Directory.GetFiles(path);
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
			if (files != null)
			{
				for (int i = 0; i < files.Length; i++)
				{
					yield return files[i];
				}
			}
		}
		private IEnumerable<string> TopLevelFilesTraversal(string path, Func<FileInfo, bool> filter)
		{
			IEnumerable<string>? files = null;
			try
			{
				files = new DirectoryInfo(path)
					.GetFiles()
					.Where(x =>
					{
						var success = true; // Why? It prevents exception being thrown inside filter.
						try
						{
							success = filter(x);
						}
						catch { success = false; }
						return success;
					})
					.Select(x => x.FullName);
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

			if (files != null)
			{
				foreach (string file in files)
				{
					yield return file;
				}
			}
		}

		private IEnumerable<DirectoryInfo> TopLevelDirectoriesTraversal(DirectoryInfo path)
		{
			DirectoryInfo[]? dirs;
			try
			{
				dirs = path.GetDirectories();
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
			if (dirs != null)
			{
				for (int i = 0; i < dirs.Length; i++)
				{
					yield return dirs[i];
				}
			}
		}
		private IEnumerable<DirectoryInfo> TopLevelDirectoriesTraversal(DirectoryInfo path, Func<DirectoryInfo, bool> filter)
		{
			IEnumerable<DirectoryInfo>? dirs = null;
			try
			{
				dirs = path.GetDirectories()
					.Where(x =>
					{
						var success = true; // Why? It prevents exception being thrown inside filter.
						try
						{
							success = filter(x);
						}
						catch { success = false; }
						return success;
					});
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
			if (dirs != null)
			{
				foreach (DirectoryInfo dir in dirs)
				{
					yield return dir;
				}
			}
		}
		private IEnumerable<string> TopLevelDirectoriesTraversal(string path)
		{
			string[]? dirs;
			try
			{
				dirs = Directory.GetDirectories(path);
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
			if (dirs != null)
			{
				for (int i = 0; i < dirs.Length; i++)
				{
					yield return dirs[i];
				}
			}
		}
		private IEnumerable<string> TopLevelDirectoriesTraversal(string path, Func<DirectoryInfo, bool> filter)
		{
			IEnumerable<string>? dirs = null;
			try
			{
				dirs = new DirectoryInfo(path)
					.GetDirectories(path)
					.Where(x =>
					{
						var success = true; // Why? It prevents exception being thrown inside filter.
						try
						{
							success = filter(x);
						}
						catch { success = false; }
						return success;
					})
					.Select(x => x.FullName);
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
			if (dirs != null)
			{
				foreach (string dir in dirs)
				{
					yield return dir;
				}
			}
		}
		#endregion

		#region All Directories Level Traversals
		private IEnumerable<FileInfo> TraverseFilesCore(DirectoryInfo path)
		{
			var directories = new Queue<DirectoryInfo>();
			directories.Enqueue(path);
			while (directories.Count > 0)
			{
				DirectoryInfo currentDir = directories.Dequeue();
				FileInfo[]? files;
				try
				{
					files = currentDir.GetFiles();
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
				if (files != null)
				{
					for (int i = 0; i < files.Length; i++)
					{
						yield return files[i];
					}
				}
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
				if (dirs != null)
				{
					for (int i = 0; i < dirs.Length; i++)
					{
						directories.Enqueue(dirs[i]);
					}
				}
			}
		}
		private IEnumerable<FileInfo> TraverseFilesCore(DirectoryInfo path, Func<FileInfo, bool> filter)
		{
			var directories = new Queue<DirectoryInfo>();
			directories.Enqueue(path);
			while (directories.Count > 0)
			{
				DirectoryInfo currentDir = directories.Dequeue();
				IEnumerable<FileInfo>? files = null;
				try
				{
					files = currentDir
						.GetFiles()
						.Where(x =>
						{
							var success = true; // Why? It prevents exception being thrown inside filter.
							try
							{
								success = filter(x);
							}
							catch { success = false; }
							return success;
						});
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
				if (files != null)
				{
					foreach (FileInfo file in files)
					{
						yield return file;
					}
				}
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
				if (dirs != null)
				{
					for (int i = 0; i < dirs.Length; i++)
					{
						directories.Enqueue(dirs[i]);
					}
				}
			}
		}

		private IEnumerable<string> TraverseFilesCore(string path)
		{
			var directories = new Queue<string>();
			directories.Enqueue(path);
			while (directories.Count > 0)
			{
				string currentDir = directories.Dequeue();
				string[]? files;
				try
				{
					files = Directory.GetFiles(currentDir);
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
				if (files != null)
				{
					for (int i = 0; i < files.Length; i++)
					{
						yield return files[i];
					}
				}
				string[]? dirs;
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
				if (dirs != null)
				{
					for (int i = 0; i < dirs.Length; i++)
					{
						directories.Enqueue(dirs[i]);
					}
				}
			}
		}
		private IEnumerable<string> TraverseFilesCore(string path, Func<FileInfo, bool> filter)
		{
			var directories = new Queue<string>();
			directories.Enqueue(path);
			while (directories.Count > 0)
			{
				string currentDir = directories.Dequeue();
				IEnumerable<string>? files = null;
				try
				{
					files = new DirectoryInfo(currentDir)
						.GetFiles()
						.Where(x =>
						{
							var success = true; // Why? It prevents exception being thrown inside filter.
							try
							{
								success = filter(x);
							}
							catch { success = false; }
							return success;
						})
						.Select(x => x.FullName);
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
				if (files != null)
				{
					foreach (string file in files)
					{
						yield return file;
					}
				}
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
				if (dirs != null)
				{
					for (int i = 0; i < dirs.Length; i++)
					{
						directories.Enqueue(dirs[i]);
					}
				}
			}
		}

		private IEnumerable<DirectoryInfo> TraverseDirectoriesCore(DirectoryInfo path)
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
				if (dirs != null)
				{
					for (int i = 0; i < dirs.Length; i++)
					{
						directories.Enqueue(dirs[i]);
						yield return dirs[i];
					}
				}
			}
		}
		private IEnumerable<DirectoryInfo> TraverseDirectoriesCore(DirectoryInfo path, Func<DirectoryInfo, bool> filter)
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
				if (dirs != null)
				{
					for (int i = 0; i < dirs.Length; i++)
					{
						bool found = true; // TODO: true?
						try
						{
							found = filter(dirs[i]); // To prevent malicious injection.
						}
						catch { found = false; }

						if (found) yield return dirs[i];

						directories.Enqueue(dirs[i]);
					}
				}
			}
		}
		private IEnumerable<string> TraverseDirectoriesCore(string path)
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
				if (dirs != null)
				{
					for (int i = 0; i < dirs.Length; i++)
					{
						directories.Enqueue(dirs[i]);
						yield return dirs[i].FullName;
					}
				}
			}
		}
		private IEnumerable<string> TraverseDirectoriesCore(string path, Func<DirectoryInfo, bool> filter)
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
				if (dirs != null)
				{

					for (int i = 0; i < dirs.Length; i++)
					{
						bool found = true; // TODO: true ?
						try
						{
							found = filter(dirs[i]); // To prevent malicious injection.
						}
						catch { found = false; }

						if (found) yield return dirs[i].FullName;

						directories.Enqueue(dirs[i]);
					}
				}
			}
		}

		#endregion

	}
}
