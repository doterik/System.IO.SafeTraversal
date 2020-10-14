using System.Collections.Generic;

namespace System.IO.SafeTraversal.Core
{
	public partial class SafeTraversal
	{
		#region FileInfo and DirectoryInfo
		/// <summary>
		/// Iterates files within top level directories.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <returns>An IEnumerable of FileInfo.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		public static IEnumerable<FileInfo> GetFiles(DirectoryInfo path) => GetFiles(path, SearchOption.TopDirectoryOnly);

		/// <summary>
		/// Iterates files using search option.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption"> Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <returns>An IEnumerable of FileInfo.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		public static IEnumerable<FileInfo> GetFiles(DirectoryInfo path, SearchOption searchOption)
		{
			if (path is null) throw new ArgumentNullException(nameof(path), "`path` cannot be null");
			if (!path.Exists) throw new DirectoryNotFoundException($"{path.FullName} doesn't exist");

			return searchOption == SearchOption.TopDirectoryOnly ? ExTopLevelFilesTraversal(path) : ExTraverseFilesCore(path);
		}

		/// <summary>
		/// Iterates files using search option and custom filter.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption"> Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="filter">Custom filter to filter files based on condition.</param>
		/// <returns>An IEnumerable of FileInfo.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`filter` cannot be null.</exception>
		public static IEnumerable<FileInfo> GetFiles(DirectoryInfo path, SearchOption searchOption, Func<FileInfo, bool> filter)
		{
			if (path is null) throw new ArgumentNullException(nameof(path), "`path` cannot be null");
			if (!path.Exists) throw new DirectoryNotFoundException($"{path.FullName} doesn't exist");
			if (filter is null) throw new ArgumentNullException(nameof(filter), "`filter` cannot be null");

			return searchOption == SearchOption.TopDirectoryOnly ? ExTopLevelFilesTraversal(path, filter) : ExTraverseFilesCore(path, filter);
		}

		/// <summary>
		/// Iteratess files using search option and filters based on the common size
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption"> Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="commonSize">Windows's explorer-like size filtering option.</param>
		/// <returns>An IEnumerable of FileInfo.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		public static IEnumerable<FileInfo> GetFiles(DirectoryInfo path, SearchOption searchOption, CommonSize commonSize)
		{
			return GetFiles(path, searchOption, (fileInfo) => MatchByCommonSize(fileInfo, commonSize));
		}

		/// <summary>
		/// Iterates files using search option and filters based on search file by name option.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption"> Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="searchFileByName">Specified option to filter files based on the name.</param>
		/// <returns>An IEnumerable of FileInfo.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`searchFileByName` cannot be null.</exception>
		public static IEnumerable<FileInfo> GetFiles(DirectoryInfo path, SearchOption searchOption, SearchFileByNameOption searchFileByName)
		{
			if (searchFileByName is null) throw new ArgumentNullException(nameof(searchFileByName), "`searchFileByName` cannot be null");

			var stringComparison = searchFileByName.CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;

			return searchFileByName.IncludeExtension
				? GetFiles(path, searchOption, (fileInfo) => MatchByNameWithExtension(fileInfo, searchFileByName.Name, stringComparison))
				: GetFiles(path, searchOption, (fileInfo) => MatchByName(fileInfo, searchFileByName.Name, stringComparison));
		}

		/// <summary>
		/// Iterates files using search option and filters based on size.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="searchFileBySize">Specifies size option (B, KB, MB.. PB)</param>
		/// <returns>An IEnumerable of FileInfo.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`searchFileBySize` cannot be null.</exception>
		public static IEnumerable<FileInfo> GetFiles(DirectoryInfo path, SearchOption searchOption, SearchFileBySizeOption searchFileBySize)
		{
			if (searchFileBySize is null) throw new ArgumentNullException(nameof(searchFileBySize), "`searchFileBySize` cannot be null");

			return GetFiles(path, searchOption, (fileInfo) => MatchBySize(fileInfo, searchFileBySize.Size, searchFileBySize.SizeType));
		}

		/// <summary>
		/// Iterates files using search option and filters based on size range.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="searchFileBySizeRange">Specifies size range option (B, KB, MB.. PB).</param>
		/// <returns>An IEnumerable of FileInfo.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`searchFileBySizeRange` cannot be null.</exception>
		public static IEnumerable<FileInfo> GetFiles(DirectoryInfo path, SearchOption searchOption, SearchFileBySizeRangeOption searchFileBySizeRange)
		{
			if (searchFileBySizeRange is null) throw new ArgumentNullException(nameof(searchFileBySizeRange), "`searchFileBySizeRange` cannot be null");

			return GetFiles(path, searchOption, (fileInfo) => MatchBySizeRange(fileInfo, searchFileBySizeRange.LowerBoundSize, searchFileBySizeRange.UpperBoundSize, searchFileBySizeRange.SizeType));
		}

		/// <summary>
		/// Iterates files using search option and filters based on date (creation, last access, last modified).
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="searchFileByDate">Date option used for filtering.</param>
		/// <returns>An IEnumerable of FileInfo.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`searchFileByDate` cannot be null.</exception>
		public static IEnumerable<FileInfo> GetFiles(DirectoryInfo path, SearchOption searchOption, SearchFileByDateOption searchFileByDate)
		{
			if (searchFileByDate is null) throw new ArgumentNullException(nameof(searchFileByDate), "`searchFileByDate` cannot be null");

			return GetFiles(path, searchOption, (fileInfo) => MatchByDate(fileInfo, searchFileByDate.Date, searchFileByDate.DateComparisonType));
		}

		/// <summary>
		/// Iterates files using search option and filters based on date range (creation, last access, last modified).
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="searchFileByDateRange">Date range option used for filtering.</param>
		/// <returns>An IEnumerable of FileInfo.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`searchFileByDateRange` cannot be null.</exception>
		public static IEnumerable<FileInfo> GetFiles(DirectoryInfo path, SearchOption searchOption, SearchFileByDateRangeOption searchFileByDateRange)
		{
			if (searchFileByDateRange is null) throw new ArgumentNullException(nameof(searchFileByDateRange), "`searchFileByDateRange` cannot be null");

			return GetFiles(path, searchOption, (fileInfo) => MatchByDateRange(fileInfo, searchFileByDateRange.LowerBoundDate, searchFileByDateRange.UpperBoundDate, searchFileByDateRange.DateComparisonType));
		}

		/// <summary>
		/// Iterates files using search option and filters based on regular expression pattern.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="searchFileByRegularExpressionPattern">Regular expression pattern.</param>
		/// <returns>An IEnumerable of FileInfo.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`searchFileByRegularExpressionPattern` cannot be null.</exception>
		public static IEnumerable<FileInfo> GetFiles(DirectoryInfo path, SearchOption searchOption, SearchFileByRegularExpressionOption searchFileByRegularExpressionPattern)
		{
			if (searchFileByRegularExpressionPattern is null) throw new ArgumentNullException(nameof(searchFileByRegularExpressionPattern), "`searchFileByRegularExpressionPattern` cannot be null");

			return searchFileByRegularExpressionPattern.IncludeExtension
				? GetFiles(path, searchOption, (fileInfo) => MatchByPatternWithExtension(fileInfo, searchFileByRegularExpressionPattern.Pattern))
				: GetFiles(path, searchOption, (fileInfo) => MatchByPattern(fileInfo, searchFileByRegularExpressionPattern.Pattern));
		}

		/// <summary>
		/// Iterates files using search option and filters based on composite option.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="fileSearchOptions">Composite option that holds many options for filtering.</param>
		/// <returns>An IEnumerable of FileInfo.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`fileSearchOptions` cannot be null.</exception>
		public static IEnumerable<FileInfo> GetFiles(DirectoryInfo path, SearchOption searchOption, SafeTraversalFileSearchOptions fileSearchOptions)
		{
			if (fileSearchOptions is null) throw new ArgumentNullException(nameof(fileSearchOptions), "`fileSearchOptions` cannot be null");

			return GetFiles(path, searchOption, (fileInfo) => TranslateFileOptions(fileInfo, fileSearchOptions));
		}

		/// <summary>
		/// Iterate top level directories.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <returns>IEnumerable of DirectoryInfo.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		public static IEnumerable<DirectoryInfo> GetDirectories(DirectoryInfo path) => GetDirectories(path, SearchOption.TopDirectoryOnly);

		/// <summary>
		/// Iterate directories using search option.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <returns>IEnumerable of DirectoryInfo.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		public static IEnumerable<DirectoryInfo> GetDirectories(DirectoryInfo path, SearchOption searchOption)
		{
			if (path is null) throw new ArgumentNullException(nameof(path), "`path` cannot be null");
			if (!path.Exists) throw new DirectoryNotFoundException($"{path.FullName} doesn't exist");

			return searchOption == SearchOption.TopDirectoryOnly ? ExTopLevelDirectoriesTraversal(path) : ExTraverseDirectoriesCore(path);
		}

		/// <summary>
		/// Iterate directories using search option and custom filter.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="filter">Custom filter to filter files based on condition.</param>
		/// <returns>IEnumerable of DirectoryInfo.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`filter` cannot be null.</exception> 
		public static IEnumerable<DirectoryInfo> GetDirectories(DirectoryInfo path, SearchOption searchOption, Func<DirectoryInfo, bool> filter)
		{
			if (path is null) throw new ArgumentNullException(nameof(path), "`path` cannot be null");
			if (!path.Exists) throw new DirectoryNotFoundException($"{path.FullName} doesn't exist");
			if (filter is null) throw new ArgumentNullException(nameof(filter), "`filter` cannot be null");

			return searchOption == SearchOption.TopDirectoryOnly ? ExTopLevelDirectoriesTraversal(path, filter) : ExTraverseDirectoriesCore(path, filter);
		}

		/// <summary>
		/// Iterate directories using search option and directory attributes.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="attributes">Directory attributes.</param>
		/// <returns>IEnumerable of DirectoryInfo.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		public static IEnumerable<DirectoryInfo> GetDirectories(DirectoryInfo path, SearchOption searchOption, FileAttributes attributes)
		{
			return GetDirectories(path, searchOption, (dirInfo) => MatchDirByAttributes(dirInfo, attributes));
		}

		/// <summary>
		/// Iterate directories using search option and filters based on date (creation, last access, last modified).
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="searchDirectoryByDateOption">Date option used for filtering.</param>
		/// <returns>IEnumerable of DirectoryInfo.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`searchDirectoryByDateOption` cannot be null.</exception> 
		public static IEnumerable<DirectoryInfo> GetDirectories(DirectoryInfo path, SearchOption searchOption, SearchDirectoryByDateOption searchDirectoryByDateOption)
		{
			if (searchDirectoryByDateOption is null) throw new ArgumentNullException(nameof(searchDirectoryByDateOption), "`searchDirectoryByDateOption` cannot be null");

			return GetDirectories(path, searchOption, (dirInfo) => MatchDirByDate(dirInfo, searchDirectoryByDateOption.Date, searchDirectoryByDateOption.DateComparisonType));
		}

		/// <summary>
		/// Iterate directories using search option and filters based on name.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="searchDirectoryByName">Specified option to filter files based on the name.</param>
		/// <returns>IEnumerable of DirectoryInfo.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`searchDirectoryByName` cannot be null.</exception> 

		public static IEnumerable<DirectoryInfo> GetDirectories(DirectoryInfo path, SearchOption searchOption, SearchDirectoryByNameOption searchDirectoryByName)
		{
			if (searchDirectoryByName is null) throw new ArgumentNullException(nameof(searchDirectoryByName), "`searchDirectoryByName` cannot be null");

			var stringComparison = searchDirectoryByName.CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;

			return GetDirectories(path, searchOption, (dirInfo) => MatchDirByName(dirInfo, searchDirectoryByName.Name, stringComparison));
		}

		/// <summary>
		/// Iterate directories using search option and filters based on regular expression pattern.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="searchDirectoryByRegularExpressionPattern">Regular expression pattern.</param>
		/// <returns>IEnumerable of DirectoryInfo.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`searchDirectoryByRegularExpressionPattern` cannot be null.</exception> 
		public static IEnumerable<DirectoryInfo> GetDirectories(DirectoryInfo path, SearchOption searchOption, SearchDirectoryByRegularExpressionOption searchDirectoryByRegularExpressionPattern)
		{
			if (searchDirectoryByRegularExpressionPattern is null) throw new ArgumentNullException(nameof(searchDirectoryByRegularExpressionPattern), "`searchDirectoryByRegularExpressionPattern` cannot be null");

			return GetDirectories(path, searchOption, (dirInfo) => MatchDirByPattern(dirInfo, searchDirectoryByRegularExpressionPattern.Pattern));
		}

		/// <summary>
		/// Iterate directories using search option and filters based on composite option.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="directorySearchOptions">Composite option that holds many options for filtering.</param>
		/// <returns>IEnumerable of DirectoryInfo.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`searchDirectoryByRegularExpressionPattern` cannot be null.</exception>
		public static IEnumerable<DirectoryInfo> GetDirectories(DirectoryInfo path, SearchOption searchOption, SafeTraversalDirectorySearchOptions directorySearchOptions)
		{
			if (directorySearchOptions is null) throw new ArgumentNullException(nameof(directorySearchOptions), "`searchDirectoryByRegularExpressionPattern` cannot be null");

			return GetDirectories(path, searchOption, (dirInfo) => TranslateDirOptions(dirInfo, directorySearchOptions));
		}
		#endregion

		#region String
		/// <summary>
		/// Iterates files within top level directories.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <returns>An IEnumerable of String.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		public static IEnumerable<string> GetFiles(string path) => GetFiles(path, SearchOption.TopDirectoryOnly);

		/// <summary>
		/// Iterates files using search option.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption"> Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <returns>An IEnumerable of String.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		public static IEnumerable<string> GetFiles(string path, SearchOption searchOption)
		{
			if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path), "`path` cannot be null");
			if (!Directory.Exists(path)) throw new DirectoryNotFoundException($"{path} doesn't exist");

			return searchOption == SearchOption.TopDirectoryOnly ? ExTopLevelFilesTraversal(path) : ExTraverseFilesCore(path);
		}

		/// <summary>
		/// Iterates files using search option and custom filter.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption"> Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="filter">Custom filter to filter files based on condition.</param>
		/// <returns>An IEnumerable of String.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`filter` cannot be null.</exception>
		public static IEnumerable<string> GetFiles(string path, SearchOption searchOption, Func<FileInfo, bool> filter)
		{
			if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path), "`path` cannot be null");
			if (!Directory.Exists(path)) throw new DirectoryNotFoundException($"{path} doesn't exist");
			if (filter is null) throw new ArgumentNullException(nameof(filter), "`filter` cannot be null");

			return searchOption == SearchOption.TopDirectoryOnly ? ExTopLevelFilesTraversal(path, filter) : ExTraverseFilesCore(path, filter);
		}

		/// <summary>
		/// Iteratess files using search option and filters based on the common size
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption"> Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="commonSize">Windows's explorer-like size filtering option.</param>
		/// <returns>An IEnumerable of String.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		public static IEnumerable<string> GetFiles(string path, SearchOption searchOption, CommonSize commonSize)
		{
			return GetFiles(path, searchOption, (fileInfo) => MatchByCommonSize(fileInfo, commonSize));
		}

		/// <summary>
		/// Iterates files using search option and filters based on search file by name option.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption"> Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="searchFileByName">Specified option to filter files based on the name.</param>
		/// <returns>An IEnumerable of String.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`searchFileByName` cannot be null.</exception>
		public static IEnumerable<string> GetFiles(string path, SearchOption searchOption, SearchFileByNameOption searchFileByName)
		{
			if (searchFileByName is null) throw new ArgumentNullException(nameof(searchFileByName), "`searchFileByName` cannot be null");

			var stringComparison = searchFileByName.CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;

			return searchFileByName.IncludeExtension
				? GetFiles(path, searchOption, (fileInfo) => MatchByNameWithExtension(fileInfo, searchFileByName.Name, stringComparison))
				: GetFiles(path, searchOption, (fileInfo) => MatchByName(fileInfo, searchFileByName.Name, stringComparison));
		}

		/// <summary>
		/// Iterates files using search option and filters based on size.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="searchFileBySize">Specifies size option (B, KB, MB.. PB)</param>
		/// <returns>An IEnumerable of String.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`searchFileBySize` cannot be null.</exception>
		public static IEnumerable<string> GetFiles(string path, SearchOption searchOption, SearchFileBySizeOption searchFileBySize)
		{
			if (searchFileBySize is null) throw new ArgumentNullException(nameof(searchFileBySize), "`searchFileBySize` cannot be null");

			return GetFiles(path, searchOption, (fileInfo) => MatchBySize(fileInfo, searchFileBySize.Size, searchFileBySize.SizeType));
		}

		/// <summary>
		/// Iterates files using search option and filters based on size range.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="searchFileBySizeRange">Specifies size range option (B, KB, MB.. PB).</param>
		/// <returns>An IEnumerable of String.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`searchFileBySizeRange` cannot be null.</exception>
		public static IEnumerable<string> GetFiles(string path, SearchOption searchOption, SearchFileBySizeRangeOption searchFileBySizeRange)
		{
			if (searchFileBySizeRange is null) throw new ArgumentNullException(nameof(searchFileBySizeRange), "`searchFileBySizeRange` cannot be null");

			return GetFiles(path, searchOption, (fileInfo) => MatchBySizeRange(fileInfo, searchFileBySizeRange.LowerBoundSize, searchFileBySizeRange.UpperBoundSize, searchFileBySizeRange.SizeType));
		}

		/// <summary>
		/// Iterates files using search option and filters based on date (creation, last access, last modified).
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="searchFileByDate">Date option used for filtering.</param>
		/// <returns>An IEnumerable of String.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`searchFileByDate` cannot be null.</exception>
		public static IEnumerable<string> GetFiles(string path, SearchOption searchOption, SearchFileByDateOption searchFileByDate)
		{
			if (searchFileByDate is null) throw new ArgumentNullException(nameof(searchFileByDate), "`searchFileByDate` cannot be null");

			return GetFiles(path, searchOption, (fileInfo) => MatchByDate(fileInfo, searchFileByDate.Date, searchFileByDate.DateComparisonType));
		}

		/// <summary>
		/// Iterates files using search option and filters based on date range (creation, last access, last modified).
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="searchFileByDateRange">Date range option used for filtering.</param>
		/// <returns>An IEnumerable of String.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`searchFileByDateRange` cannot be null.</exception>
		public static IEnumerable<string> GetFiles(string path, SearchOption searchOption, SearchFileByDateRangeOption searchFileByDateRange)
		{
			if (searchFileByDateRange is null) throw new ArgumentNullException(nameof(searchFileByDateRange), "`searchFileByDateRange` cannot be null");

			return GetFiles(path, searchOption, (fileInfo) => MatchByDateRange(fileInfo, searchFileByDateRange.LowerBoundDate, searchFileByDateRange.UpperBoundDate, searchFileByDateRange.DateComparisonType));
		}

		/// <summary>
		/// Iterates files using search option and filters based on regular expression pattern.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="searchFileByRegularExpressionPattern">Regular expression pattern.</param>
		/// <returns>An IEnumerable of String.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`searchFileByRegularExpressionPattern` cannot be null.</exception>
		public static IEnumerable<string> GetFiles(string path, SearchOption searchOption, SearchFileByRegularExpressionOption searchFileByRegularExpressionPattern)
		{
			if (searchFileByRegularExpressionPattern is null) throw new ArgumentNullException(nameof(searchFileByRegularExpressionPattern), "`searchFileByRegularExpressionPattern` cannot be null");

			return searchFileByRegularExpressionPattern.IncludeExtension
				? GetFiles(path, searchOption, (fileInfo) => MatchByPatternWithExtension(fileInfo, searchFileByRegularExpressionPattern.Pattern))
				: GetFiles(path, searchOption, (fileInfo) => MatchByPattern(fileInfo, searchFileByRegularExpressionPattern.Pattern));
		}

		/// <summary>
		/// Iterates files using search option and filters based on composite option.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="fileSearchOptions">Composite option that holds many options for filtering.</param>
		/// <returns>An IEnumerable of String.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`fileSearchOptions` cannot be null.</exception>
		public static IEnumerable<string> GetFiles(string path, SearchOption searchOption, SafeTraversalFileSearchOptions fileSearchOptions)
		{
			if (fileSearchOptions is null) throw new ArgumentNullException(nameof(fileSearchOptions), "`fileSearchOptions` cannot be null");

			return GetFiles(path, searchOption, (fileInfo) => TranslateFileOptions(fileInfo, fileSearchOptions));
		}

		/// <summary>
		/// Iterate top level directories.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <returns>IEnumerable of String.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		public static IEnumerable<string> GetDirectories(string path) => GetDirectories(path, SearchOption.TopDirectoryOnly);

		/// <summary>
		/// Iterate directories using search option.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <returns>IEnumerable of String.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		public static IEnumerable<string> GetDirectories(string path, SearchOption searchOption)
		{
			if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path), "`path` cannot be null");
			if (!Directory.Exists(path)) throw new DirectoryNotFoundException($"{path} doesn't exist");

			return searchOption == SearchOption.TopDirectoryOnly ? ExTopLevelDirectoriesTraversal(path) : ExTraverseDirectoriesCore(path);
		}

		/// <summary>
		/// Iterate directories using search option and custom filter.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="filter">Custom filter to filter files based on condition.</param>
		/// <returns>IEnumerable of String.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`filter` cannot be null.</exception> 
		public static IEnumerable<string> GetDirectories(string path, SearchOption searchOption, Func<DirectoryInfo, bool> filter)
		{
			if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path), "`path` cannot be null");
			if (!Directory.Exists(path)) throw new DirectoryNotFoundException($"{path} doesn't exist");
			if (filter is null) throw new ArgumentNullException(nameof(filter), "`filter` cannot be null");

			return searchOption == SearchOption.TopDirectoryOnly ? ExTopLevelDirectoriesTraversal(path, filter) : ExTraverseDirectoriesCore(path, filter);
		}

		/// <summary>
		/// Iterate directories using search option and directory attributes.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="attributes">Directory attributes.</param>
		/// <returns>IEnumerable of String.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		public static IEnumerable<string> GetDirectories(string path, SearchOption searchOption, FileAttributes attributes)
		{
			return GetDirectories(path, searchOption, (dirInfo) => MatchDirByAttributes(dirInfo, attributes)); // TODO: ArgumentException?
		}

		/// <summary>
		/// Iterate directories using search option and filters based on date (creation, last access, last modified).
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="searchDirectoryByDateOption">Date option used for filtering.</param>
		/// <returns>IEnumerable of String.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`searchDirectoryByDateOption` cannot be null.</exception> 
		public static IEnumerable<string> GetDirectories(string path, SearchOption searchOption, SearchDirectoryByDateOption searchDirectoryByDateOption)
		{
			if (searchDirectoryByDateOption is null) throw new ArgumentNullException(nameof(searchDirectoryByDateOption), "`searchDirectoryByDateOption` cannot be null");

			return GetDirectories(path, searchOption, (dirInfo) => MatchDirByDate(dirInfo, searchDirectoryByDateOption.Date, searchDirectoryByDateOption.DateComparisonType));
		}

		/// <summary>
		/// Iterate directories using search option and filters based on name.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="searchDirectoryByName">Specified option to filter files based on the name.</param>
		/// <returns>IEnumerable of String.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`searchDirectoryByName` cannot be null.</exception> 
		public static IEnumerable<string> GetDirectories(string path, SearchOption searchOption, SearchDirectoryByNameOption searchDirectoryByName)
		{
			if (searchDirectoryByName is null) throw new ArgumentNullException(nameof(searchDirectoryByName), "`searchDirectoryByName` cannot be null");

			var stringComparison = searchDirectoryByName.CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;

			return GetDirectories(path, searchOption, (dirInfo) => MatchDirByName(dirInfo, searchDirectoryByName.Name, stringComparison));
		}

		/// <summary>
		/// Iterate directories using search option and filters based on regular expression pattern.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="searchDirectoryByRegularExpressionPattern">Regular expression pattern.</param>
		/// <returns>IEnumerable of String.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`searchDirectoryByRegularExpressionPattern` cannot be null.</exception> 
		public static IEnumerable<string> GetDirectories(string path, SearchOption searchOption, SearchDirectoryByRegularExpressionOption searchDirectoryByRegularExpressionPattern)
		{
			if (searchDirectoryByRegularExpressionPattern is null) throw new ArgumentNullException(nameof(searchDirectoryByRegularExpressionPattern), "`searchDirectoryByRegularExpressionPattern` cannot be null");

			return GetDirectories(path, searchOption, (dirInfo) => MatchDirByPattern(dirInfo, searchDirectoryByRegularExpressionPattern.Pattern));
		}

		/// <summary>
		/// Iterate directories using search option and filters based on composite option.
		/// </summary>
		/// <param name="path">Target path.</param>
		/// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
		/// <param name="directorySearchOptions">Composite option that holds many options for filtering.</param>
		/// <returns>IEnumerable of DirectoryInfo.</returns>
		/// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
		/// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
		/// <exception cref="ArgumentNullException">`searchDirectoryByRegularExpressionPattern` cannot be null.</exception>
		public static IEnumerable<string> GetDirectories(string path, SearchOption searchOption, SafeTraversalDirectorySearchOptions directorySearchOptions)
		{
			if (directorySearchOptions is null) throw new ArgumentNullException(nameof(directorySearchOptions), "`searchDirectoryByRegularExpressionPattern` cannot be null");

			return GetDirectories(path, searchOption, (dirInfo) => TranslateDirOptions(dirInfo, directorySearchOptions));
		}
		#endregion
	}
}
