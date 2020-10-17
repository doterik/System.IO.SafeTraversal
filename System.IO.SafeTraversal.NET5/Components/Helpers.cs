﻿using System.Collections.Generic;
using System.Linq;

namespace System.IO.SafeTraversal.Core
{
	/// <summary>
	/// Helpers class for System.IO.SafeTraversal
	/// </summary>
	internal static class Helpers
	{
		#region Top Level Traversals
		private static IEnumerable<FileInfo> ExTraverseFiles3(DirectoryInfo path, SearchOption searchOption, Func<FileInfo, bool>? filter = null, Action<TraversalError>? onLogError = null)
		{
			return ExTraverseFiles2(path, searchOption, filter, onLogError);
		}

		private static IEnumerable<DirectoryInfo> ExTraverseDirectories3(DirectoryInfo path, SearchOption searchOption, Func<DirectoryInfo, bool>? filter = null, Action<TraversalError>? onLogError = null)
		{
			return ExTraverseDirectories2(path, searchOption, filter, onLogError);
		}

		internal static IEnumerable<FileInfo> ExTraverseFiles2(DirectoryInfo path, SearchOption searchOption, Func<FileInfo, bool>? filter = null, Action<TraversalError>? onLogError = null)
		{
			if (searchOption == SearchOption.TopDirectoryOnly)
			{
				FileInfo[]? files = GetFilesInPath(path, filter, onLogError); // IEnumerable<FileInfo>? files = null;
				for (var i = 0; i < files?.Length; i++) yield return files[i];
			}
			else
			{
				var directories = new Queue<DirectoryInfo>();
				directories.Enqueue(path);
				while (directories.Count > 0)
				{
					DirectoryInfo currentDir = directories.Dequeue();

					FileInfo[]? files = GetFilesInPath(currentDir, filter, onLogError); // IEnumerable<FileInfo>? files = null;
					for (var i = 0; i < files?.Length; i++) yield return files[i];

					DirectoryInfo[]? dirs = null;
					try
					{
						dirs = currentDir.GetDirectories();
					}
					catch (UnauthorizedAccessException ex)
					{
						dirs = null;
						if (onLogError is not null) onLogError(new TraversalError(ex.Message));
					}
					catch (Exception ex)
					{
						dirs = null;
						if (onLogError is not null) onLogError(new TraversalError(ex.Message));
					}

					for (var i = 0; i < dirs?.Length; i++) directories.Enqueue(dirs[i]);
				}
			}

			static FileInfo[]? GetFilesInPath(DirectoryInfo path, Func<FileInfo, bool>? filter, Action<TraversalError>? onLogError)
			{
				FileInfo[]? files;
				try
				{
					files = filter is null
						? path.GetFiles()
						: path.GetFiles().Where(x => x.Pass(filter)).ToArray();
				}
				catch (UnauthorizedAccessException ex)
				{
					files = null; if (onLogError is not null) onLogError(new TraversalError(ex.Message));
				}
				catch (Exception ex)
				{
					files = null; if (onLogError is not null) onLogError(new TraversalError(ex.Message));
				}
				return files;
			}
		}

		internal static IEnumerable<string> ExTraverseFiles2(string path, SearchOption searchOption, Func<FileInfo, bool>? filter = null, Action<TraversalError>? onLogError = null)
		{
			if (searchOption == SearchOption.TopDirectoryOnly)
			{
				string[]? files = GetFilesInPath(path, filter, onLogError); // IEnumerable<string>? files = null;
				for (var i = 0; i < files?.Length; i++) yield return files[i];
			}
			else
			{
				var directories = new Queue<string>();
				directories.Enqueue(path);
				while (directories.Count > 0)
				{
					string currentDir = directories.Dequeue();
					string[]? files = GetFilesInPath(currentDir, filter, onLogError); // IEnumerable<string>? files = null;

					for (var i = 0; i < files?.Length; i++) yield return files[i];

					string[]? dirs = null;
					try
					{
						dirs = Directory.GetDirectories(currentDir);
					}
					catch (UnauthorizedAccessException ex)
					{
						dirs = null;
						if (onLogError is not null) onLogError(new TraversalError(ex.Message));
					}
					catch (Exception ex)
					{
						dirs = null;
						if (onLogError is not null) onLogError(new TraversalError(ex.Message));
					}

					for (var i = 0; i < dirs?.Length; i++) directories.Enqueue(dirs[i]);
				}
			}

			static string[]? GetFilesInPath(string path, Func<FileInfo, bool>? filter, Action<TraversalError>? onLogError)
			{
				string[]? files;
				try
				{
					files = filter is null
						? Directory.GetFiles(path)
						: new DirectoryInfo(path).GetFiles().Where(x => x.Pass(filter)).Select(x => x.FullName).ToArray();
				}
				catch (UnauthorizedAccessException ex)
				{
					files = null;
					if (onLogError is not null) onLogError(new TraversalError(ex.Message));
				}
				catch (Exception ex)
				{
					files = null;
					if (onLogError is not null) onLogError(new TraversalError(ex.Message));
				}

				return files;
			}
		}

		internal static IEnumerable<DirectoryInfo> ExTraverseDirectories2(DirectoryInfo path, SearchOption searchOption, Func<DirectoryInfo, bool>? filter = null, Action<TraversalError>? onLogError = null)
		{
			if (searchOption == SearchOption.TopDirectoryOnly)
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
					if (onLogError is not null) onLogError(new TraversalError(ex.Message));
				}
				catch (Exception ex)
				{
					dirs = null;
					if (onLogError is not null) onLogError(new TraversalError(ex.Message));
				}

				for (var i = 0; i < dirs?.Length; i++) yield return dirs[i];
			}
			else
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
						if (onLogError is not null) onLogError(new TraversalError(ex.Message));
					}
					catch (Exception ex)
					{
						dirs = null;
						if (onLogError is not null) onLogError(new TraversalError(ex.Message));
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
		}

		internal static IEnumerable<string> ExTraverseDirectories2(string path, SearchOption searchOption, Func<DirectoryInfo, bool>? filter = null, Action<TraversalError>? onLogError = null)
		{
			if (searchOption == SearchOption.TopDirectoryOnly)
			{
				string[]? dirs; // IEnumerable<string>? dirs = null;
				try
				{
					dirs = filter is null
						? Directory.GetDirectories(path)
						: new DirectoryInfo(path).GetDirectories().Where(x => x.Pass(filter)).Select(x => x.FullName).ToArray();
				}
				catch (UnauthorizedAccessException ex)
				{
					dirs = null;
					if (onLogError is not null) onLogError(new TraversalError(ex.Message));
				}
				catch (Exception ex)
				{
					dirs = null;
					if (onLogError is not null) onLogError(new TraversalError(ex.Message));
				}

				for (var i = 0; i < dirs?.Length; i++) yield return dirs[i];
			}
			else
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
						if (onLogError is not null) onLogError(new TraversalError(ex.Message));
					}
					catch (Exception ex)
					{
						dirs = null;
						if (onLogError is not null) onLogError(new TraversalError(ex.Message));
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
		}

		#endregion

		#region All Directories Level Traversals
		#endregion
		internal static bool Pass<T>(this T x, Func<T, bool> filter) where T : FileSystemInfo
		{
			try { return filter(x); }
			catch { return false; }

			//var success = true; // Why? It prevents exception being thrown inside filter.
			//try { success = filter(x); }
			//catch { success = false; }
			//return success;
		}
	}
}