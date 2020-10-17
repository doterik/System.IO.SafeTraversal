using System;
using System.Collections.Generic;
using System.IO;
using System.IO.SafeTraversal.Core;
using System.Linq;
using Xunit;

namespace XUnitTest1.Traversals
{
	public class SafeTraversalTests
	{
		private readonly string testdir = @"c:\temp";

		#region TraverseFiles_TopDirectories
		[Fact]
		public void TraverseFiles_TopDirectories()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.TopDirectoryOnly;  // Default, not used.

			// Act
			IEnumerable<FileInfo> infos1 = safeTraversal.TraverseFiles(dirinfo);
			IEnumerable<FileInfo> infos2 = dirinfo.GetFiles();
			IEnumerable<string> paths1 = safeTraversal.TraverseFiles(strpath);
			IEnumerable<string> paths2 = strpath.GetFiles();

			// Assert
			Assert.Equal(91, infos1?.Count());
			Assert.Equal(91, infos2?.Count());
			Assert.Equal(91, paths1?.Count());
			Assert.Equal(91, paths2?.Count());
		}

		[Fact]
		public void TraverseFiles_TopDirectories_Filter_IsReadOnly()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.TopDirectoryOnly;

			Func<FileInfo, bool> filt1 = x => x.IsReadOnly;
			static bool filter(FileInfo x) => x.IsReadOnly;

			// Act
			var infos1 = safeTraversal.TraverseFiles(dirinfo, searchOption, filter);
			var infos2 = dirinfo.GetFiles(searchOption, filter);
			var paths1 = safeTraversal.TraverseFiles(strpath, searchOption, filter);
			var paths2 = strpath.GetFiles(searchOption, filter);

			// Assert
			Assert.Equal(2, infos1?.Count());
			Assert.Equal(2, infos2?.Count());
			Assert.Equal(2, paths1?.Count());
			Assert.Equal(2, paths2?.Count());
		}

		[Fact]
		public void TraverseFiles_TopDirectories_CommonSize_Large()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.TopDirectoryOnly;

			var commonSize = CommonSize.Large;

			// Act
			var infos1 = safeTraversal.TraverseFiles(dirinfo, searchOption, commonSize);
			var infos2 = dirinfo.GetFiles(searchOption, commonSize);
			var paths1 = safeTraversal.TraverseFiles(strpath, searchOption, commonSize);
			var paths2 = strpath.GetFiles(searchOption, commonSize);

			// Assert
			Assert.Equal(12, infos1?.Count());
			Assert.Equal(12, infos2?.Count());
			Assert.Equal(12, paths1?.Count());
			Assert.Equal(12, paths2?.Count());
		}

		[Fact]
		public void TraverseFiles_TopDirectories_SearchFileByNameOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.TopDirectoryOnly;

			var searchFileByName = new SearchFileByNameOption("ngen2x32");

			// Act
			var infos1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileByName);
			var infos2 = dirinfo.GetFiles(searchOption, searchFileByName);
			var paths1 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileByName);
			var paths2 = strpath.GetFiles(searchOption, searchFileByName);

			// Assert
			Assert.Equal(1, infos1?.Count());
			Assert.Equal(1, infos2?.Count());
			Assert.Equal(1, paths1?.Count());
			Assert.Equal(1, paths2?.Count());
		}

		[Theory]
		[InlineData("ngen4x32.txt", true, 1)]
		[InlineData("ngen4x32.txt", false, 0)]
		[InlineData("ngen4x32", true, 0)]
		[InlineData("ngen4x32", false, 1)]
		public void TraverseFiles_TopDirectories_SearchFileByNameOption_IncludeExtension(string name, bool includeExtension, int expected)
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.TopDirectoryOnly;

			var searchFileByName = new SearchFileByNameOption(name, includeExtension);

			// Act
			var infos1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileByName);
			var infos2 = dirinfo.GetFiles(searchOption, searchFileByName);
			var paths1 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileByName);
			var paths2 = strpath.GetFiles(searchOption, searchFileByName);

			// Assert
			Assert.Equal(expected, infos1?.Count());
			Assert.Equal(expected, infos2?.Count());
			Assert.Equal(expected, paths1?.Count());
			Assert.Equal(expected, paths2?.Count());
		}

		[Theory]
		[InlineData("Thumbs", true, 1)]
		[InlineData("thumbs", true, 0)]
		[InlineData("thumbs", false, 1)]
		public void TraverseFiles_TopDirectories_SearchFileByNameOption_CaseSensitive(string name, bool caseSensitive, int expected)
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.TopDirectoryOnly;

			var searchFileByName = new SearchFileByNameOption(name, caseSensitive: caseSensitive); // Argument name is a must here (not in position)!

			// Act
			var infos1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileByName);
			var infos2 = dirinfo.GetFiles(searchOption, searchFileByName);
			var paths1 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileByName);
			var paths2 = strpath.GetFiles(searchOption, searchFileByName);

			// Assert
			Assert.Equal(expected, infos1?.Count());
			Assert.Equal(expected, infos2?.Count());
			Assert.Equal(expected, paths1?.Count());
			Assert.Equal(expected, paths2?.Count());
		}

		[Fact]
		public void TraverseFiles_TopDirectories_SearchFileBySizeOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.TopDirectoryOnly;

			var searchFileBySize = new SearchFileBySizeOption(2, SizeType.MegaBytes);

			// Act
			var infos1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileBySize);
			var infos2 = dirinfo.GetFiles(searchOption, searchFileBySize);
			var paths1 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileBySize);
			var paths2 = strpath.GetFiles(searchOption, searchFileBySize);

			// Assert
			Assert.Equal(3, infos1?.Count());
			Assert.Equal(3, infos2?.Count());
			Assert.Equal(3, paths1?.Count());
			Assert.Equal(3, paths2?.Count());
		}

		[Fact]
		public void TraverseFiles_TopDirectories_SearchFileBySizeRangeOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.TopDirectoryOnly;

			var searchFileBySizeRange = new SearchFileBySizeRangeOption(4, 10, SizeType.MegaBytes);

			// Act
			var infos1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileBySizeRange);
			var infos2 = dirinfo.GetFiles(searchOption, searchFileBySizeRange);
			var paths1 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileBySizeRange);
			var paths2 = strpath.GetFiles(searchOption, searchFileBySizeRange);

			// Assert
			Assert.Equal(5, infos1?.Count());
			Assert.Equal(5, infos2?.Count());
			Assert.Equal(5, paths1?.Count());
			Assert.Equal(5, paths2?.Count());
		}

		[Fact]
		public void TraverseFiles_TopDirectories_SearchFileByDateOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.TopDirectoryOnly;

			var searchFileByDate = new SearchFileByDateOption(new DateTime(2016, 01, 15), DateComparisonType.CreationDate);

			// Act
			var infos1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileByDate);
			var infos2 = dirinfo.GetFiles(searchOption, searchFileByDate);
			var paths1 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileByDate);
			var paths2 = strpath.GetFiles(searchOption, searchFileByDate);

			// Assert
			Assert.Equal(4, infos1?.Count());
			Assert.Equal(4, infos2?.Count());
			Assert.Equal(4, paths1?.Count());
			Assert.Equal(4, paths2?.Count());
		}

		[Fact]
		public void TraverseFiles_TopDirectories_SearchFileByDateRangeOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.TopDirectoryOnly;

			var searchFileByDateRange = new SearchFileByDateRangeOption(
				new DateTime(2016, 01, 01),
				new DateTime(2016, 03, 31), DateComparisonType.CreationDate);

			// Act
			var infos1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileByDateRange);
			var infos2 = dirinfo.GetFiles(searchOption, searchFileByDateRange);
			var paths1 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileByDateRange);
			var paths2 = strpath.GetFiles(searchOption, searchFileByDateRange);

			// Assert
			Assert.Equal(10, infos1?.Count());
			Assert.Equal(10, infos2?.Count());
			Assert.Equal(10, paths1?.Count());
			Assert.Equal(10, paths2?.Count());
		}

		[Fact]
		public void TraverseFiles_TopDirectories_SearchFileByRegularExpressionOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.TopDirectoryOnly;

			var searchFileByRegularExpressionPattern = new SearchFileByRegularExpressionOption(@"^ng.*x6");

			// Act
			var infos1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileByRegularExpressionPattern);
			var infos2 = dirinfo.GetFiles(searchOption, searchFileByRegularExpressionPattern);
			var paths1 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileByRegularExpressionPattern);
			var paths2 = strpath.GetFiles(searchOption, searchFileByRegularExpressionPattern);

			// Assert
			Assert.Equal(2, infos1?.Count());
			Assert.Equal(2, infos2?.Count());
			Assert.Equal(2, paths1?.Count());
			Assert.Equal(2, paths2?.Count());
		}

		[Theory]
		[InlineData("csv", 4)]
		[InlineData(".csv", 4)]
		[InlineData(".xls.csv", 4)]
		public void TraverseFiles_TopDirectories_SafeTraversalFileSearchOptions(string extension, int expected)
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.TopDirectoryOnly;

			var fileSearchOptions = new SafeTraversalFileSearchOptions
			{
				Extension = extension,
				SizeOption = new SearchFileBySizeOption(21, SizeType.KiloBytes)
			};

			// Act
			var infos1 = safeTraversal.TraverseFiles(dirinfo, searchOption, fileSearchOptions);
			var infos2 = dirinfo.GetFiles(searchOption, fileSearchOptions);
			var paths1 = safeTraversal.TraverseFiles(strpath, searchOption, fileSearchOptions);
			var paths2 = strpath.GetFiles(searchOption, fileSearchOptions);

			// Assert
			Assert.Equal(expected, infos1?.Count());
			Assert.Equal(expected, infos2?.Count());
			Assert.Equal(expected, paths1?.Count());
			Assert.Equal(expected, paths2?.Count());
		}
		#endregion

		#region TraverseFiles_AllDirectories
		[Fact]
		public void TraverseFiles_AllDirectories()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.AllDirectories;

			// Act
			IEnumerable<FileInfo> infos1 = safeTraversal.TraverseFiles(dirinfo, searchOption);
			IEnumerable<FileInfo> infos2 = dirinfo.GetFiles(searchOption);
			IEnumerable<string> paths1 = safeTraversal.TraverseFiles(strpath, searchOption);
			IEnumerable<string> paths2 = strpath.GetFiles(searchOption);

			// Assert
			Assert.Equal(1255, infos1?.Count());
			Assert.Equal(1255, infos2?.Count());
			Assert.Equal(1255, paths1?.Count());
			Assert.Equal(1255, paths2?.Count());
		}

		[Fact]
		public void TraverseFiles_AllDirectories_Filter_IsReadOnly()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.AllDirectories;

			Func<FileInfo, bool> filt1 = x => x.IsReadOnly;
			static bool filter(FileInfo x) => x.IsReadOnly;

			// Act
			var infos1 = safeTraversal.TraverseFiles(dirinfo, searchOption, filter);
			var infos2 = dirinfo.GetFiles(searchOption, filter);
			var paths1 = safeTraversal.TraverseFiles(strpath, searchOption, filter);
			var paths2 = strpath.GetFiles(searchOption, filter);

			// Assert
			Assert.Equal(503, infos1?.Count());
			Assert.Equal(503, infos2?.Count());
			Assert.Equal(503, paths1?.Count());
			Assert.Equal(503, paths2?.Count());
		}

		[Fact]
		public void TraverseFiles_AllDirectories_CommonSize_Large()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.AllDirectories;

			var commonSize = CommonSize.Large;

			// Act
			var infos1 = safeTraversal.TraverseFiles(dirinfo, searchOption, commonSize);
			var infos2 = dirinfo.GetFiles(searchOption, commonSize);
			var paths1 = safeTraversal.TraverseFiles(strpath, searchOption, commonSize);
			var paths2 = strpath.GetFiles(searchOption, commonSize);

			// Assert
			Assert.Equal(97, infos1?.Count());
			Assert.Equal(97, infos2?.Count());
			Assert.Equal(97, paths1?.Count());
			Assert.Equal(97, paths2?.Count());
		}

		[Fact]
		public void TraverseFiles_AllDirectories_SearchFileByNameOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.AllDirectories;

			var searchFileByName = new SearchFileByNameOption("ngen2x32");

			// Act
			var infos1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileByName);
			var infos2 = dirinfo.GetFiles(searchOption, searchFileByName);
			var paths1 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileByName);
			var paths2 = strpath.GetFiles(searchOption, searchFileByName);

			// Assert
			Assert.Equal(2, infos1?.Count());
			Assert.Equal(2, infos2?.Count());
			Assert.Equal(2, paths1?.Count());
			Assert.Equal(2, paths2?.Count());
		}

		[Theory]
		[InlineData("ngen4x32.txt", true, 2)]
		[InlineData("ngen4x32.txt", false, 0)]
		[InlineData("ngen4x32", true, 0)]
		[InlineData("ngen4x32", false, 2)]
		public void TraverseFiles_AllDirectories_SearchFileByNameOption_IncludeExtension(string name, bool includeExtension, int expected)
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.AllDirectories;

			var searchFileByName = new SearchFileByNameOption(name, includeExtension);

			// Act
			var infos1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileByName);
			var infos2 = dirinfo.GetFiles(searchOption, searchFileByName);
			var paths1 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileByName);
			var paths2 = strpath.GetFiles(searchOption, searchFileByName);

			// Assert
			Assert.Equal(expected, infos1?.Count());
			Assert.Equal(expected, infos2?.Count());
			Assert.Equal(expected, paths1?.Count());
			Assert.Equal(expected, paths2?.Count());
		}

		[Fact]
		public void TraverseFiles_AllDirectories_SearchFileByNameOption_CaseSensitive()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.AllDirectories;

			var searchFileByName = new SearchFileByNameOption("Settings", caseSensitive: true);

			// Act
			var infos1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileByName);
			var infos2 = dirinfo.GetFiles(searchOption, searchFileByName);
			var paths1 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileByName);
			var paths2 = strpath.GetFiles(searchOption, searchFileByName);

			// Assert
			Assert.Equal(14, infos1?.Count());
			Assert.Equal(14, infos2?.Count());
			Assert.Equal(14, paths1?.Count());
			Assert.Equal(14, paths2?.Count());
		}

		[Fact]
		public void TraverseFiles_AllDirectories_SearchFileBySizeOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.AllDirectories;

			var searchFileBySize = new SearchFileBySizeOption(2, SizeType.MegaBytes);

			// Act
			var infos1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileBySize);
			var infos2 = dirinfo.GetFiles(searchOption, searchFileBySize);
			var paths1 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileBySize);
			var paths2 = strpath.GetFiles(searchOption, searchFileBySize);

			// Assert
			Assert.Equal(10, infos1?.Count());
			Assert.Equal(10, infos2?.Count());
			Assert.Equal(10, paths1?.Count());
			Assert.Equal(10, paths2?.Count());
		}

		[Fact]
		public void TraverseFiles_AllDirectories_SearchFileBySizeRangeOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.AllDirectories;

			var searchFileBySizeRange = new SearchFileBySizeRangeOption(4, 10, SizeType.MegaBytes);

			// Act
			var infos1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileBySizeRange);
			var infos2 = dirinfo.GetFiles(searchOption, searchFileBySizeRange);
			var paths1 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileBySizeRange);
			var paths2 = strpath.GetFiles(searchOption, searchFileBySizeRange);

			// Assert
			Assert.Equal(53, infos1?.Count());
			Assert.Equal(53, infos2?.Count());
			Assert.Equal(53, paths1?.Count());
			Assert.Equal(53, paths2?.Count());
		}

		[Fact]
		public void TraverseFiles_AllDirectories_SearchFileByDateOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.AllDirectories;

			var searchFileByDate = new SearchFileByDateOption(new DateTime(2017, 09, 01), DateComparisonType.CreationDate);

			// Act
			var infos1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileByDate);
			var infos2 = dirinfo.GetFiles(searchOption, searchFileByDate);
			var paths1 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileByDate);
			var paths2 = strpath.GetFiles(searchOption, searchFileByDate);

			// Assert
			Assert.Equal(6, infos1?.Count());
			Assert.Equal(6, infos2?.Count());
			Assert.Equal(6, paths1?.Count());
			Assert.Equal(6, paths2?.Count());
		}

		[Fact]
		public void TraverseFiles_AllDirectories_SearchFileByDateRangeOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.AllDirectories;

			var searchFileByDateRange = new SearchFileByDateRangeOption(
				new DateTime(2017, 09, 01),
				new DateTime(2017, 09, 30), DateComparisonType.CreationDate);

			// Act
			var infos1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileByDateRange);
			var infos2 = dirinfo.GetFiles(searchOption, searchFileByDateRange);
			var paths1 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileByDateRange);
			var paths2 = strpath.GetFiles(searchOption, searchFileByDateRange);

			// Assert
			Assert.Equal(26, infos1?.Count());
			Assert.Equal(26, infos2?.Count());
			Assert.Equal(26, paths1?.Count());
			Assert.Equal(26, paths2?.Count());
		}

		[Fact]
		public void TraverseFiles_AllDirectories_SearchFileByRegularExpressionOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.AllDirectories;

			var searchFileByRegularExpressionPattern = new SearchFileByRegularExpressionOption(@"^ng.*x6");

			// Act
			var infos1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileByRegularExpressionPattern);
			var infos2 = dirinfo.GetFiles(searchOption, searchFileByRegularExpressionPattern);
			var paths1 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileByRegularExpressionPattern);
			var paths2 = strpath.GetFiles(searchOption, searchFileByRegularExpressionPattern);

			// Assert
			Assert.Equal(4, infos1?.Count());
			Assert.Equal(4, infos2?.Count());
			Assert.Equal(4, paths1?.Count());
			Assert.Equal(4, paths2?.Count());
		}

		[Fact]
		public void TraverseFiles_AllDirectories_SafeTraversalFileSearchOptions()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.AllDirectories;

			var fileSearchOptions = new SafeTraversalFileSearchOptions
			{
				Extension = "csv",
				SizeOption = new SearchFileBySizeOption(21, SizeType.KiloBytes)
			};

			// Act
			var infos1 = safeTraversal.TraverseFiles(dirinfo, searchOption, fileSearchOptions);
			var infos2 = dirinfo.GetFiles(searchOption, fileSearchOptions);
			var paths1 = safeTraversal.TraverseFiles(strpath, searchOption, fileSearchOptions);
			var paths2 = strpath.GetFiles(searchOption, fileSearchOptions);

			// Assert
			Assert.Equal(4, infos1?.Count());
			Assert.Equal(4, infos2?.Count());
			Assert.Equal(4, paths1?.Count());
			Assert.Equal(4, paths2?.Count());
		}
		#endregion

		#region TraverseDirectories_TopDirectories
		[Fact]
		public void TraverseDirectories_TopDirectories()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.TopDirectoryOnly;  // Default, not used.

			// Act
			IEnumerable<DirectoryInfo> infos1 = safeTraversal.TraverseDirectories(dirinfo);
			IEnumerable<DirectoryInfo> infos2 = dirinfo.GetDirectories();
			IEnumerable<string> paths1 = safeTraversal.TraverseDirectories(strpath);
			IEnumerable<string> paths2 = strpath.GetDirectories();

			// Assert
			Assert.Equal(19, infos1?.Count());
			Assert.Equal(19, infos2?.Count());
			Assert.Equal(19, paths1?.Count());
			Assert.Equal(19, paths2?.Count());
		}

		[Fact]
		public void TraverseDirectories_TopDirectories_Filter_IsReadOnly()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.TopDirectoryOnly;

			Func<DirectoryInfo, bool> filter = x => x.Attributes.HasFlag(FileAttributes.ReadOnly);

			// Act
			var infos1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, filter);
			var infos2 = dirinfo.GetDirectories(searchOption, filter);
			var paths1 = safeTraversal.TraverseDirectories(strpath, searchOption, filter);
			var paths2 = strpath.GetDirectories(searchOption, filter);

			// Assert
			Assert.Equal(1, infos1?.Count());
			Assert.Equal(1, infos2?.Count());
			Assert.Equal(1, paths1?.Count());
			Assert.Equal(1, paths2?.Count());
		}

		[Fact]
		public void TraverseDirectories_TopDirectories_FileAttributes()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.TopDirectoryOnly;

			var attributes = FileAttributes.ReadOnly; // FileAttributes.Directory;

			// Act
			var infos1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, attributes);
			var infos2 = dirinfo.GetDirectories(searchOption, attributes);
			var paths1 = safeTraversal.TraverseDirectories(strpath, searchOption, attributes);
			var paths2 = strpath.GetDirectories(searchOption, attributes);

			// Assert
			Assert.Equal(1, infos1?.Count());
			Assert.Equal(1, infos2?.Count());
			Assert.Equal(1, paths1?.Count());
			Assert.Equal(1, paths2?.Count());
		}

		[Fact]
		public void TraverseDirectories_TopDirectories_SearchDirectoryByDateOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.TopDirectoryOnly;

			var searchDirectoryByDateOption = new SearchDirectoryByDateOption(new DateTime(2019, 05, 18), DateComparisonType.LastModificationDate);

			// Act
			var infos1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, searchDirectoryByDateOption);
			var infos2 = dirinfo.GetDirectories(searchOption, searchDirectoryByDateOption);
			var paths1 = safeTraversal.TraverseDirectories(strpath, searchOption, searchDirectoryByDateOption);
			var paths2 = strpath.GetDirectories(searchOption, searchDirectoryByDateOption);

			// Assert
			Assert.Equal(2, infos1?.Count());
			Assert.Equal(2, infos2?.Count());
			Assert.Equal(2, paths1?.Count());
			Assert.Equal(2, paths2?.Count());
		}

		[Fact]
		public void TraverseDirectories_TopDirectories_SearchDirectoryByNameOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.TopDirectoryOnly;

			var searchDirectoryByName = new SearchDirectoryByNameOption("e0");

			// Act
			var infos1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, searchDirectoryByName);
			var infos2 = dirinfo.GetDirectories(searchOption, searchDirectoryByName);
			var paths1 = safeTraversal.TraverseDirectories(strpath, searchOption, searchDirectoryByName);
			var paths2 = strpath.GetDirectories(searchOption, searchDirectoryByName);

			// Assert
			Assert.Equal(1, infos1?.Count());
			Assert.Equal(1, infos2?.Count());
			Assert.Equal(1, paths1?.Count());
			Assert.Equal(1, paths2?.Count());
		}

		[Fact]
		public void TraverseDirectories_TopDirectories_SearchDirectoryByRegularExpressionOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.TopDirectoryOnly;

			var searchDirectoryByRegularExpressionPattern = new SearchDirectoryByRegularExpressionOption(@"Diag$");

			// Act
			var infos1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, searchDirectoryByRegularExpressionPattern);
			var infos2 = dirinfo.GetDirectories(searchOption, searchDirectoryByRegularExpressionPattern);
			var paths1 = safeTraversal.TraverseDirectories(strpath, searchOption, searchDirectoryByRegularExpressionPattern);
			var paths2 = strpath.GetDirectories(searchOption, searchDirectoryByRegularExpressionPattern);

			// Assert
			//Assert.Equal(1, infos1?.Count());
			//Assert.Equal(1, infos2?.Count());
			//Assert.Equal(1, paths1?.Count());
			//Assert.Equal(1, paths2?.Count());
			Assert.Single(infos1);
			Assert.Single(infos2);
			Assert.Single(paths1);
			Assert.Single(paths2);
		}

		[Fact]
		public void TraverseDirectories_TopDirectories_SafeTraversalDirectorySearchOptions()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.TopDirectoryOnly;

			var directorySearchOptions = new SafeTraversalDirectorySearchOptions { DirectoryNameOption = new SearchDirectoryByNameOption("e1") };

			// Act
			var infos1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, directorySearchOptions);
			var infos2 = dirinfo.GetDirectories(searchOption, directorySearchOptions);
			var paths1 = safeTraversal.TraverseDirectories(strpath, searchOption, directorySearchOptions);
			var paths2 = strpath.GetDirectories(searchOption, directorySearchOptions);

			// Assert
			Assert.Equal(1, infos1?.Count());
			Assert.Equal(1, infos2?.Count());
			Assert.Equal(1, paths1?.Count());
			Assert.Equal(1, paths2?.Count());
		}
		#endregion

		#region TraverseDirectories_AllDirectories
		[Fact]
		public void TraverseDirectories_AllDirectories()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.AllDirectories;

			// Act
			IEnumerable<DirectoryInfo> infos1 = safeTraversal.TraverseDirectories(dirinfo, searchOption);
			IEnumerable<DirectoryInfo> infos2 = dirinfo.GetDirectories(searchOption);
			IEnumerable<string> paths1 = safeTraversal.TraverseDirectories(strpath, searchOption);
			IEnumerable<string> paths2 = strpath.GetDirectories(searchOption);

			// Assert
			Assert.Equal(667, infos1?.Count());
			Assert.Equal(667, infos2?.Count());
			Assert.Equal(667, paths1?.Count());
			Assert.Equal(667, paths2?.Count());
		}

		[Fact]
		public void TraverseDirectories_AllDirectories_Filter_IsReadOnly()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.AllDirectories;

			Func<DirectoryInfo, bool> filter = x => x.Parent?.FullName == testdir;

			// Act
			var infos1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, filter);
			var infos2 = dirinfo.GetDirectories(searchOption, filter);
			var paths1 = safeTraversal.TraverseDirectories(strpath, searchOption, filter);
			var paths2 = strpath.GetDirectories(searchOption, filter);

			// Assert
			Assert.Equal(19, infos1?.Count());
			Assert.Equal(19, infos2?.Count());
			Assert.Equal(19, paths1?.Count());
			Assert.Equal(19, paths2?.Count());
		}

		[Fact]
		public void TraverseDirectories_AllDirectories_FileAttributes()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.AllDirectories;

			var attributes = FileAttributes.Archive | FileAttributes.Compressed; // FileAttributes.Directory;

			// Act
			var infos1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, attributes);
			var infos2 = dirinfo.GetDirectories(searchOption, attributes);
			var paths1 = safeTraversal.TraverseDirectories(strpath, searchOption, attributes);
			var paths2 = strpath.GetDirectories(searchOption, attributes);

			// Assert
			Assert.Equal(157, infos1?.Count());
			Assert.Equal(157, infos2?.Count());
			Assert.Equal(157, paths1?.Count());
			Assert.Equal(157, paths2?.Count());
		}

		[Fact]
		public void TraverseDirectories_AllDirectories_SearchDirectoryByDateOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.AllDirectories;

			var searchDirectoryByDateOption = new SearchDirectoryByDateOption(new DateTime(2017, 03, 26), DateComparisonType.CreationDate);

			// Act
			var infos1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, searchDirectoryByDateOption);
			var infos2 = dirinfo.GetDirectories(searchOption, searchDirectoryByDateOption);
			var paths1 = safeTraversal.TraverseDirectories(strpath, searchOption, searchDirectoryByDateOption);
			var paths2 = strpath.GetDirectories(searchOption, searchDirectoryByDateOption);

			// Assert
			Assert.Equal(8, infos1?.Count());
			Assert.Equal(8, infos2?.Count());
			Assert.Equal(8, paths1?.Count());
			Assert.Equal(8, paths2?.Count());
		}

		[Fact]
		public void TraverseDirectories_AllDirectories_SearchDirectoryByNameOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.AllDirectories;

			var searchDirectoryByName = new SearchDirectoryByNameOption("src");

			// Act
			var infos1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, searchDirectoryByName);
			var infos2 = dirinfo.GetDirectories(searchOption, searchDirectoryByName);
			var paths1 = safeTraversal.TraverseDirectories(strpath, searchOption, searchDirectoryByName);
			var paths2 = strpath.GetDirectories(searchOption, searchDirectoryByName);

			// Assert
			Assert.Equal(4, infos1?.Count());
			Assert.Equal(4, infos2?.Count());
			Assert.Equal(4, paths1?.Count());
			Assert.Equal(4, paths2?.Count());
		}

		[Fact]
		public void TraverseDirectories_AllDirectories_SearchDirectoryByRegularExpressionOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.AllDirectories;

			var searchDirectoryByRegularExpressionPattern = new SearchDirectoryByRegularExpressionOption(@"Diag$");

			// Act
			var infos1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, searchDirectoryByRegularExpressionPattern);
			var infos2 = dirinfo.GetDirectories(searchOption, searchDirectoryByRegularExpressionPattern);
			var paths1 = safeTraversal.TraverseDirectories(strpath, searchOption, searchDirectoryByRegularExpressionPattern);
			var paths2 = strpath.GetDirectories(searchOption, searchDirectoryByRegularExpressionPattern);

			// Assert
			//Assert.Equal(1, infos1?.Count());
			//Assert.Equal(1, infos2?.Count());
			//Assert.Equal(1, paths1?.Count());
			//Assert.Equal(1, paths2?.Count());
			Assert.Single(infos1);
			Assert.Single(infos2);
			Assert.Single(paths1);
			Assert.Single(paths2);
		}

		[Fact]
		public void TraverseDirectories_AllDirectories_SafeTraversalDirectorySearchOptions()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.AllDirectories;

			var directorySearchOptions = new SafeTraversalDirectorySearchOptions { DirectoryNameOption = new SearchDirectoryByNameOption("bin") };

			// Act
			var infos1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, directorySearchOptions);
			var infos2 = dirinfo.GetDirectories(searchOption, directorySearchOptions);
			var paths1 = safeTraversal.TraverseDirectories(strpath, searchOption, directorySearchOptions);
			var paths2 = strpath.GetDirectories(searchOption, directorySearchOptions);

			// Assert
			Assert.Equal(14, infos1?.Count());
			Assert.Equal(14, infos2?.Count());
			Assert.Equal(14, paths1?.Count());
			Assert.Equal(14, paths2?.Count());
		}
		#endregion
	}
}
