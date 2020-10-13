﻿using System.Collections.Generic;

namespace System.IO.SafeTraversal.Core
{
    /// <summary>
    /// Core class to perform all traversal operations.
    /// </summary>
    public partial class SafeTraversal
    {
        /// <summary>
        /// Event that holds information regarding error that occurs during operation.
        /// </summary>
        public event EventHandler<TraversalError> LogError;

        /// <summary>
        /// Overridable method for error logging.
        /// </summary>
        /// <param name="traversalError">An instance of TraversalError class.</param>
        protected virtual void OnLogError(TraversalError traversalError)
        {
            LogError?.Invoke(this, traversalError);
        }

        #region FileInfo and DirectoryInfo
        /// <summary>
        /// Iterates files within top level directories.
        /// </summary>
        /// <param name="path">Target path.</param>
        /// <returns>An IEnumerable of FileInfo.</returns>
        /// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
        /// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
        public IEnumerable<FileInfo> TraverseFiles(DirectoryInfo path) => TraverseFiles(path, SearchOption.TopDirectoryOnly);

        /// <summary>
        /// Iterates files using search option.
        /// </summary>
        /// <param name="path">Target path.</param>
        /// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
        /// <returns>An IEnumerable of FileInfo.</returns>
        /// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
        /// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
        public IEnumerable<FileInfo> TraverseFiles(DirectoryInfo path, SearchOption searchOption)
        {
            if (path == null) throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!path.Exists) throw new DirectoryNotFoundException($"{path.FullName} doesn't exist");

            return searchOption == SearchOption.TopDirectoryOnly ? TopLevelFilesTraversal(path) : TraverseFilesCore(path);
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
        public IEnumerable<FileInfo> TraverseFiles(DirectoryInfo path, SearchOption searchOption, Func<FileInfo, bool> filter)
        {
            if (path == null) throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!path.Exists) throw new DirectoryNotFoundException($"{path.FullName} doesn't exist");
            if (filter == null) throw new ArgumentNullException(nameof(filter), "`filter` cannot be null");

            return searchOption == SearchOption.TopDirectoryOnly ? TopLevelFilesTraversal(path, filter) : TraverseFilesCore(path, filter);
        }

        /// <summary>
        /// Iteratess files using search option and filters based on the common size
        /// </summary>
        /// <param name="path">Target path.</param>
        /// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
        /// <param name="commonSize">Windows's explorer-like size filtering option.</param>
        /// <returns>An IEnumerable of FileInfo.</returns>
        /// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
        /// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
        public IEnumerable<FileInfo> TraverseFiles(DirectoryInfo path, SearchOption searchOption, CommonSize commonSize)
        {
            return TraverseFiles(path, searchOption, (fileInfo) => MatchByCommonSize(fileInfo, commonSize));
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
        public IEnumerable<FileInfo> TraverseFiles(DirectoryInfo path, SearchOption searchOption, SearchFileByNameOption searchFileByName)
        {
            if (searchFileByName == null) throw new ArgumentNullException(nameof(searchFileByName), "`searchFileByName` cannot be null");

            StringComparison stringComparison = searchFileByName.CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;

            return searchFileByName.IncludeExtension
                ? TraverseFiles(path, searchOption, (fileInfo) => MatchByNameWithExtension(fileInfo, searchFileByName.Name, stringComparison))
                : TraverseFiles(path, searchOption, (fileInfo) => MatchByName(fileInfo, searchFileByName.Name, stringComparison));

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
        public IEnumerable<FileInfo> TraverseFiles(DirectoryInfo path, SearchOption searchOption, SearchFileBySizeOption searchFileBySize)
        {
            if (searchFileBySize == null) throw new ArgumentNullException(nameof(searchFileBySize), "`searchFileBySize` cannot be null");

            return TraverseFiles(path, searchOption, (fileInfo) => MatchBySize(fileInfo, searchFileBySize.Size, searchFileBySize.SizeType));
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
        public IEnumerable<FileInfo> TraverseFiles(DirectoryInfo path, SearchOption searchOption, SearchFileBySizeRangeOption searchFileBySizeRange)
        {
            if (searchFileBySizeRange == null) throw new ArgumentNullException(nameof(searchFileBySizeRange), "`searchFileBySizeRange` cannot be null");

            return TraverseFiles(path, searchOption, (fileInfo) => MatchBySizeRange(fileInfo, searchFileBySizeRange.LowerBoundSize, searchFileBySizeRange.UpperBoundSize, searchFileBySizeRange.SizeType));
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
        public IEnumerable<FileInfo> TraverseFiles(DirectoryInfo path, SearchOption searchOption, SearchFileByDateOption searchFileByDate)
        {
            if (searchFileByDate == null) throw new ArgumentNullException(nameof(searchFileByDate), "`searchFileByDate` cannot be null");

            return TraverseFiles(path, searchOption, (fileInfo) => MatchByDate(fileInfo, searchFileByDate.Date, searchFileByDate.DateComparisonType));
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
        public IEnumerable<FileInfo> TraverseFiles(DirectoryInfo path, SearchOption searchOption, SearchFileByDateRangeOption searchFileByDateRange)
        {
            if (searchFileByDateRange == null) throw new ArgumentNullException(nameof(searchFileByDateRange), "`searchFileByDateRange` cannot be null");

            return TraverseFiles(path, searchOption, (fileInfo) => MatchByDateRange(fileInfo, searchFileByDateRange.LowerBoundDate, searchFileByDateRange.UpperBoundDate, searchFileByDateRange.DateComparisonType));
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
        public IEnumerable<FileInfo> TraverseFiles(DirectoryInfo path, SearchOption searchOption, SearchFileByRegularExpressionOption searchFileByRegularExpressionPattern)
        {
            if (searchFileByRegularExpressionPattern == null)
                throw new ArgumentNullException(nameof(searchFileByRegularExpressionPattern), "`searchFileByRegularExpressionPattern` cannot be null");

            return searchFileByRegularExpressionPattern.IncludeExtension
                ? TraverseFiles(path, searchOption, (fileInfo) => MatchByPatternWithExtension(fileInfo, searchFileByRegularExpressionPattern.Pattern))
                : TraverseFiles(path, searchOption, (fileInfo) => MatchByPattern(fileInfo, searchFileByRegularExpressionPattern.Pattern));
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
        public IEnumerable<FileInfo> TraverseFiles(DirectoryInfo path, SearchOption searchOption, SafeTraversalFileSearchOptions fileSearchOptions)
        {
            if (fileSearchOptions == null) throw new ArgumentNullException(nameof(fileSearchOptions), "`fileSearchOptions` cannot be null");

            return TraverseFiles(path, searchOption, (fileInfo) => TranslateFileOptions(fileInfo, fileSearchOptions));
        }



        /// <summary>
        /// Iterate top level directories.
        /// </summary>
        /// <param name="path">Target path.</param>
        /// <returns>IEnumerable of DirectoryInfo.</returns>
        /// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
        /// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
        public IEnumerable<DirectoryInfo> TraverseDirectories(DirectoryInfo path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!path.Exists)
                throw new DirectoryNotFoundException($"{path.FullName} doesn't exist");
            return TopLevelDirectoriesTraversal(path);
        }

        /// <summary>
        /// Iterate directories using search option.
        /// </summary>
        /// <param name="path">Target path.</param>
        /// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
        /// <returns>IEnumerable of DirectoryInfo.</returns>
        /// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
        /// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
        public IEnumerable<DirectoryInfo> TraverseDirectories(DirectoryInfo path, SearchOption searchOption)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!path.Exists)
                throw new DirectoryNotFoundException($"{path.FullName} doesn't exist");
            if (searchOption == SearchOption.TopDirectoryOnly)
                return TopLevelDirectoriesTraversal(path);
            else
                return TraverseDirectoriesCore(path);
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
        public IEnumerable<DirectoryInfo> TraverseDirectories(DirectoryInfo path, SearchOption searchOption, Func<DirectoryInfo, bool> filter)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!path.Exists)
                throw new DirectoryNotFoundException($"{path.FullName} doesn't exist");
            if (filter == null)
                throw new ArgumentNullException(nameof(filter), "`filter` cannot be null");


            if (searchOption == SearchOption.TopDirectoryOnly)
                return TopLevelDirectoriesTraversal(path, filter);
            else
                return TraverseDirectoriesCore(path, filter);
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
        public IEnumerable<DirectoryInfo> TraverseDirectories(DirectoryInfo path, SearchOption searchOption, FileAttributes attributes)
        {

            if (path == null)
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!path.Exists)
                throw new DirectoryNotFoundException($"{path.FullName} doesn't exist");
            Func<DirectoryInfo, bool> filter = (dirInfo) => MatchDirByAttributes(dirInfo, attributes);
            if (searchOption == SearchOption.TopDirectoryOnly)
                return TopLevelDirectoriesTraversal(path, filter);
            else
                return TraverseDirectoriesCore(path, filter);
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
        public IEnumerable<DirectoryInfo> TraverseDirectories(DirectoryInfo path, SearchOption searchOption, SearchDirectoryByDateOption searchDirectoryByDateOption)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!path.Exists)
                throw new DirectoryNotFoundException($"{path.FullName} doesn't exist");
            if (searchDirectoryByDateOption == null)
                throw new ArgumentNullException(nameof(searchDirectoryByDateOption), "`searchDirectoryByDateOption` cannot be null");
            Func<DirectoryInfo, bool> filter = (dirInfo) => MatchDirByDate(dirInfo, searchDirectoryByDateOption.Date, searchDirectoryByDateOption.DateComparisonType);
            if (searchOption == SearchOption.TopDirectoryOnly)
                return TopLevelDirectoriesTraversal(path, filter);
            else
                return TraverseDirectoriesCore(path, filter);
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
        public IEnumerable<DirectoryInfo> TraverseDirectories(DirectoryInfo path, SearchOption searchOption, SearchDirectoryByNameOption searchDirectoryByName)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!path.Exists)
                throw new DirectoryNotFoundException($"{path.FullName} doesn't exist");
            if (searchDirectoryByName == null)
                throw new ArgumentNullException(nameof(searchDirectoryByName), "`searchDirectoryByName` cannot be null");
            StringComparison stringComparison = searchDirectoryByName.CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
            Func<DirectoryInfo, bool> filter = (dirInfo) => MatchDirByName(dirInfo, searchDirectoryByName.Name, stringComparison);
            if (searchOption == SearchOption.TopDirectoryOnly)
                return TopLevelDirectoriesTraversal(path, filter);
            else
                return TraverseDirectoriesCore(path, filter);
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
        public IEnumerable<DirectoryInfo> TraverseDirectories(DirectoryInfo path, SearchOption searchOption, SearchDirectoryByRegularExpressionOption searchDirectoryByRegularExpressionPattern)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!path.Exists)
                throw new DirectoryNotFoundException($"{path.FullName} doesn't exist");
            if (searchDirectoryByRegularExpressionPattern == null)
                throw new ArgumentNullException(nameof(searchDirectoryByRegularExpressionPattern), "`searchDirectoryByRegularExpressionPattern` cannot be null");
            Func<DirectoryInfo, bool> filter = (dirInfo) => MatchDirByPattern(dirInfo, searchDirectoryByRegularExpressionPattern.Pattern);
            if (searchOption == SearchOption.TopDirectoryOnly)
                return TopLevelDirectoriesTraversal(path, filter);
            else
                return TraverseDirectoriesCore(path, filter);
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
        public IEnumerable<DirectoryInfo> TraverseDirectories(DirectoryInfo path, SearchOption searchOption, SafeTraversalDirectorySearchOptions directorySearchOptions)
        {

            if (path == null)
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!path.Exists)
                throw new DirectoryNotFoundException($"{path.FullName} doesn't exist");
            if (directorySearchOptions == null)
                throw new ArgumentNullException(nameof(directorySearchOptions), "`searchDirectoryByRegularExpressionPattern` cannot be null");
            Func<DirectoryInfo, bool> filter = (dirInfo) => TranslateDirOptions(dirInfo, directorySearchOptions);
            if (searchOption == SearchOption.TopDirectoryOnly)
                return TopLevelDirectoriesTraversal(path, filter);
            else
                return TraverseDirectoriesCore(path, filter);
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
        public IEnumerable<string> TraverseFiles(string path)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"{path} doesn't exist");
            return TopLevelFilesTraversal(path);
        }


        /// <summary>
        /// Iterates files using search option.
        /// </summary>
        /// <param name="path">Target path.</param>
        /// <param name="searchOption"> Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
        /// <returns>An IEnumerable of String.</returns>
        /// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
        /// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
        public IEnumerable<string> TraverseFiles(string path, SearchOption searchOption)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"{path} doesn't exist");
            if (searchOption == SearchOption.TopDirectoryOnly)
                return TopLevelFilesTraversal(path);
            else
                return TraverseFilesCore(path);
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
        public IEnumerable<string> TraverseFiles(string path, SearchOption searchOption, Func<FileInfo, bool> filter)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"{path} doesn't exist");
            if (filter == null)
                throw new ArgumentNullException(nameof(filter), "`filter` cannot be null");

            if (searchOption == SearchOption.TopDirectoryOnly)
                return TopLevelFilesTraversal(path, filter);
            else
                return TraverseFilesCore(path, filter);
        }

        /// <summary>
        /// Iteratess files using search option and filters based on the common size
        /// </summary>
        /// <param name="path">Target path.</param>
        /// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
        /// <param name="commonSize">Windows's explorer-like size filtering option.</param>
        /// <returns>An IEnumerable of String.</returns>
        /// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
        /// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
        public IEnumerable<string> TraverseFiles(string path, SearchOption searchOption, CommonSize commonSize)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"{path} doesn't exist");
            Func<FileInfo, bool> filter = (fileInfo) => MatchByCommonSize(fileInfo, commonSize);
            if (searchOption == SearchOption.TopDirectoryOnly)
                return TopLevelFilesTraversal(path, filter);
            else
                return TraverseFilesCore(path, filter);
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
        public IEnumerable<string> TraverseFiles(string path, SearchOption searchOption, SearchFileByNameOption searchFileByName)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"{path} doesn't exist");
            if (searchFileByName == null)
                throw new ArgumentNullException(nameof(searchFileByName), "`searchFileByName` cannot be null");
            Func<FileInfo, bool> filter = null;

            StringComparison stringComparison = searchFileByName.CaseSensitive ?
                StringComparison.InvariantCulture :
                StringComparison.InvariantCultureIgnoreCase;

            if (searchFileByName.IncludeExtension)
                filter = (fileInfo) => MatchByNameWithExtension(fileInfo, searchFileByName.Name, stringComparison);
            else
                filter = (fileInfo) => MatchByName(fileInfo, searchFileByName.Name, stringComparison);
            if (searchOption == SearchOption.TopDirectoryOnly)
                return TopLevelFilesTraversal(path, filter);
            else
                return TraverseFilesCore(path, filter);
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
        public IEnumerable<string> TraverseFiles(string path, SearchOption searchOption, SearchFileBySizeOption searchFileBySize)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"{path} doesn't exist");
            if (searchFileBySize == null)
                throw new ArgumentNullException(nameof(searchFileBySize), "`searchFileBySize` cannot be null");
            Func<FileInfo, bool> filter = (fileInfo) => MatchBySize(fileInfo, searchFileBySize.Size, searchFileBySize.SizeType);
            if (searchOption == SearchOption.TopDirectoryOnly)
                return TopLevelFilesTraversal(path, filter);
            else
                return TraverseFilesCore(path, filter);
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
        public IEnumerable<string> TraverseFiles(string path, SearchOption searchOption, SearchFileBySizeRangeOption searchFileBySizeRange)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"{path} doesn't exist");
            if (searchFileBySizeRange == null)
                throw new ArgumentNullException(nameof(searchFileBySizeRange), "`searchFileBySizeRange` cannot be null");
            Func<FileInfo, bool> filter = (fileInfo) => MatchBySizeRange(fileInfo, searchFileBySizeRange.LowerBoundSize, searchFileBySizeRange.UpperBoundSize, searchFileBySizeRange.SizeType);
            if (searchOption == SearchOption.TopDirectoryOnly)
                return TopLevelFilesTraversal(path, filter);
            else
                return TraverseFilesCore(path, filter);
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
        public IEnumerable<string> TraverseFiles(string path, SearchOption searchOption, SearchFileByDateOption searchFileByDate)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"{path} doesn't exist");
            if (searchFileByDate == null)
                throw new ArgumentNullException(nameof(searchFileByDate), "`searchFileByDate` cannot be null");
            Func<FileInfo, bool> filter = (fileInfo) => MatchByDate(fileInfo, searchFileByDate.Date, searchFileByDate.DateComparisonType);
            if (searchOption == SearchOption.TopDirectoryOnly)
                return TopLevelFilesTraversal(path, filter);
            else
                return TraverseFilesCore(path, filter);
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
        public IEnumerable<string> TraverseFiles(string path, SearchOption searchOption, SearchFileByDateRangeOption searchFileByDateRange)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"{path} doesn't exist");
            if (searchFileByDateRange == null)
                throw new ArgumentNullException(nameof(searchFileByDateRange), "`searchFileByDateRange` cannot be null");
            Func<FileInfo, bool> filter = (fileInfo) => MatchByDateRange(fileInfo, searchFileByDateRange.LowerBoundDate, searchFileByDateRange.UpperBoundDate, searchFileByDateRange.DateComparisonType);
            if (searchOption == SearchOption.TopDirectoryOnly)
                return TopLevelFilesTraversal(path, filter);
            else
                return TraverseFilesCore(path, filter);
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
        public IEnumerable<string> TraverseFiles(string path, SearchOption searchOption, SearchFileByRegularExpressionOption searchFileByRegularExpressionPattern)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"{path} doesn't exist");
            if (searchFileByRegularExpressionPattern == null)
                throw new ArgumentNullException(nameof(searchFileByRegularExpressionPattern), "`searchFileByRegularExpressionPattern` cannot be null");
            Func<FileInfo, bool> filter = null;

            if (searchFileByRegularExpressionPattern.IncludeExtension)
                filter = (fileInfo) => MatchByPatternWithExtension(fileInfo, searchFileByRegularExpressionPattern.Pattern);
            else
                filter = (fileInfo) => MatchByPattern(fileInfo, searchFileByRegularExpressionPattern.Pattern);
            if (searchOption == SearchOption.TopDirectoryOnly)
                return TopLevelFilesTraversal(path, filter);
            else
                return TraverseFilesCore(path, filter);
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
        public IEnumerable<string> TraverseFiles(string path, SearchOption searchOption, SafeTraversalFileSearchOptions fileSearchOptions)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"{path} doesn't exist");
            if (fileSearchOptions == null)
                throw new ArgumentNullException(nameof(fileSearchOptions), "`fileSearchOptions` cannot be null");
            Func<FileInfo, bool> filter = (fileInfo) => TranslateFileOptions(fileInfo, fileSearchOptions);
            if (searchOption == SearchOption.TopDirectoryOnly)
                return TopLevelFilesTraversal(path, filter);
            else
                return TraverseFilesCore(path, filter);
        }


        /// <summary>
        /// Iterate top level directories.
        /// </summary>
        /// <param name="path">Target path.</param>
        /// <returns>IEnumerable of String.</returns>
        /// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
        /// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
        public IEnumerable<string> TraverseDirectories(string path)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"{path} doesn't exist");
            return TopLevelDirectoriesTraversal(path);
        }

        /// <summary>
        /// Iterate directories using search option.
        /// </summary>
        /// <param name="path">Target path.</param>
        /// <param name="searchOption">Specifies whether to search the current directory, or the current directory and all subdirectories.</param>
        /// <returns>IEnumerable of String.</returns>
        /// <exception cref="ArgumentNullException">`path` cannot be null.</exception>
        /// <exception cref="DirectoryNotFoundException">`path` doesn't exist.</exception>
        public IEnumerable<string> TraverseDirectories(string path, SearchOption searchOption)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"{path} doesn't exist");
            if (searchOption == SearchOption.TopDirectoryOnly)
                return TopLevelDirectoriesTraversal(path);
            else
                return TraverseDirectoriesCore(path);
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
        public IEnumerable<string> TraverseDirectories(string path, SearchOption searchOption, Func<DirectoryInfo, bool> filter)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"{path} doesn't exist");
            if (filter == null)
                throw new ArgumentNullException(nameof(filter), "`filter` cannot be null");

            if (searchOption == SearchOption.TopDirectoryOnly)
                return TopLevelDirectoriesTraversal(path, filter);
            else
                return TraverseDirectoriesCore(path, filter);
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
        public IEnumerable<string> TraverseDirectories(string path, SearchOption searchOption, FileAttributes attributes)
        {

            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"{path} doesn't exist");
            Func<DirectoryInfo, bool> filter = (dirInfo) => MatchDirByAttributes(dirInfo, attributes);
            if (searchOption == SearchOption.TopDirectoryOnly)
                return TopLevelDirectoriesTraversal(path, filter);
            else
                return TraverseDirectoriesCore(path, filter);
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
        public IEnumerable<string> TraverseDirectories(string path, SearchOption searchOption, SearchDirectoryByDateOption searchDirectoryByDateOption)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"{path} doesn't exist");
            if (searchDirectoryByDateOption == null)
                throw new ArgumentNullException(nameof(searchDirectoryByDateOption), "`searchDirectoryByDateOption` cannot be null");

            Func<DirectoryInfo, bool> filter = (dirInfo) => MatchDirByDate(dirInfo, searchDirectoryByDateOption.Date, searchDirectoryByDateOption.DateComparisonType);
            if (searchOption == SearchOption.TopDirectoryOnly)
                return TopLevelDirectoriesTraversal(path, filter);
            else
                return TraverseDirectoriesCore(path, filter);
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
        public IEnumerable<string> TraverseDirectories(string path, SearchOption searchOption, SearchDirectoryByNameOption searchDirectoryByName)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"{path} doesn't exist");
            if (searchDirectoryByName == null)
                throw new ArgumentNullException(nameof(searchDirectoryByName), "`searchDirectoryByName` cannot be null");
            StringComparison stringComparison = searchDirectoryByName.CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
            Func<DirectoryInfo, bool> filter = (dirInfo) => MatchDirByName(dirInfo, searchDirectoryByName.Name, stringComparison);
            if (searchOption == SearchOption.TopDirectoryOnly)
                return TopLevelDirectoriesTraversal(path, filter);
            else
                return TraverseDirectoriesCore(path, filter);
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
        public IEnumerable<string> TraverseDirectories(string path, SearchOption searchOption, SearchDirectoryByRegularExpressionOption searchDirectoryByRegularExpressionPattern)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"{path} doesn't exist");
            if (searchDirectoryByRegularExpressionPattern == null)
                throw new ArgumentNullException(nameof(searchDirectoryByRegularExpressionPattern), "`searchDirectoryByRegularExpressionPattern` cannot be null");
            Func<DirectoryInfo, bool> filter = (dirInfo) => MatchDirByPattern(dirInfo, searchDirectoryByRegularExpressionPattern.Pattern);
            if (searchOption == SearchOption.TopDirectoryOnly)
                return TopLevelDirectoriesTraversal(path, filter);
            else
                return TraverseDirectoriesCore(path, filter);
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
        public IEnumerable<string> TraverseDirectories(string path, SearchOption searchOption, SafeTraversalDirectorySearchOptions directorySearchOptions)
        {

            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path), "`path` cannot be null");
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"{path} doesn't exist");
            if (directorySearchOptions == null)
                throw new ArgumentNullException(nameof(directorySearchOptions), "`searchDirectoryByRegularExpressionPattern` cannot be null");
            Func<DirectoryInfo, bool> filter = (dirInfo) => TranslateDirOptions(dirInfo, directorySearchOptions);
            if (searchOption == SearchOption.TopDirectoryOnly)
                return TopLevelDirectoriesTraversal(path, filter);
            else
                return TraverseDirectoriesCore(path, filter);
        }
        #endregion


        #region Find Parents
        /// <summary>
        /// Find all parents all the way up to the root (ie: C:\ or D:\) from current path.
        /// </summary>
        /// <param name="path">Valid path. If path is not found, DirectoryNotFoundException will be thrown.</param>
        /// <returns>IEnumerable of DirectoryInfo representing all parents. Null if current path is a root.</returns>
        public IEnumerable<DirectoryInfo> FindParents(DirectoryInfo path)
        {

            if (!path.Exists)
                throw new DirectoryNotFoundException();
            while (path.Parent != null)
            {
                yield return new DirectoryInfo(path.Parent.Name);
                path = path.Parent;
            }
        }
        /// <summary>
        /// Find all parents all the way up to the root (ie: C:\ or D:\) from current path.
        /// </summary>
        /// <param name="file">Valid file location. If file is not found, FileNotFoundException will be thrown.</param>
        /// <returns>IEnumerable of DirectoryInfo representing all parents.</returns>
        public IEnumerable<DirectoryInfo> FindParents(FileInfo file)
        {

            if (!file.Exists)
                throw new FileNotFoundException();
            DirectoryInfo path = new DirectoryInfo(Path.GetDirectoryName(file.FullName));
            yield return new DirectoryInfo(path.Name);
            while (path.Parent != null)
            {
                yield return new DirectoryInfo(path.Parent.Name);
                path = path.Parent;
            }
        }
        #endregion
    }
}
