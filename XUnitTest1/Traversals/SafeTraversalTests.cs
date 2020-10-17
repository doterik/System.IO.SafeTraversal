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
			IEnumerable<FileInfo> result1 = safeTraversal.TraverseFiles(dirinfo);
			IEnumerable<FileInfo> result2 = dirinfo.GetFiles();
			IEnumerable<string> result3 = safeTraversal.TraverseFiles(strpath);
			IEnumerable<string> result4 = strpath.GetFiles();

			// Assert
			Assert.Equal(91, result1?.Count());
			Assert.Equal(91, result2?.Count());
			Assert.Equal(91, result3?.Count());
			Assert.Equal(91, result4?.Count());
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
			var result1 = safeTraversal.TraverseFiles(dirinfo, searchOption, filter);
			var result2 = dirinfo.GetFiles(searchOption, filter);
			var result3 = safeTraversal.TraverseFiles(strpath, searchOption, filter);
			var result4 = strpath.GetFiles(searchOption, filter);

			// Assert
			Assert.Equal(2, result1?.Count());
			Assert.Equal(2, result2?.Count());
			Assert.Equal(2, result3?.Count());
			Assert.Equal(2, result4?.Count());
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
			var result1 = safeTraversal.TraverseFiles(dirinfo, searchOption, commonSize);
			var result2 = dirinfo.GetFiles(searchOption, commonSize);
			var result3 = safeTraversal.TraverseFiles(strpath, searchOption, commonSize);
			var result4 = strpath.GetFiles(searchOption, commonSize);

			// Assert
			Assert.Equal(12, result1?.Count());
			Assert.Equal(12, result2?.Count());
			Assert.Equal(12, result3?.Count());
			Assert.Equal(12, result4?.Count());
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
			var result1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileByName);
			var result2 = dirinfo.GetFiles(searchOption, searchFileByName);
			var result3 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileByName);
			var result4 = strpath.GetFiles(searchOption, searchFileByName);

			// Assert
			Assert.Equal(1, result1?.Count());
			Assert.Equal(1, result2?.Count());
			Assert.Equal(1, result3?.Count());
			Assert.Equal(1, result4?.Count());
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
			var result1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileBySize);
			var result2 = dirinfo.GetFiles(searchOption, searchFileBySize);
			var result3 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileBySize);
			var result4 = strpath.GetFiles(searchOption, searchFileBySize);

			// Assert
			Assert.Equal(3, result1?.Count());
			Assert.Equal(3, result2?.Count());
			Assert.Equal(3, result3?.Count());
			Assert.Equal(3, result4?.Count());
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
			var result1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileBySizeRange);
			var result2 = dirinfo.GetFiles(searchOption, searchFileBySizeRange);
			var result3 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileBySizeRange);
			var result4 = strpath.GetFiles(searchOption, searchFileBySizeRange);

			// Assert
			Assert.Equal(5, result1?.Count());
			Assert.Equal(5, result2?.Count());
			Assert.Equal(5, result3?.Count());
			Assert.Equal(5, result4?.Count());
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
			var result1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileByDate);
			var result2 = dirinfo.GetFiles(searchOption, searchFileByDate);
			var result3 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileByDate);
			var result4 = strpath.GetFiles(searchOption, searchFileByDate);

			// Assert
			Assert.Equal(4, result1?.Count());
			Assert.Equal(4, result2?.Count());
			Assert.Equal(4, result3?.Count());
			Assert.Equal(4, result4?.Count());
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
			var result1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileByDateRange);
			var result2 = dirinfo.GetFiles(searchOption, searchFileByDateRange);
			var result3 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileByDateRange);
			var result4 = strpath.GetFiles(searchOption, searchFileByDateRange);

			// Assert
			Assert.Equal(10, result1?.Count());
			Assert.Equal(10, result2?.Count());
			Assert.Equal(10, result3?.Count());
			Assert.Equal(10, result4?.Count());
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
			var result1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileByRegularExpressionPattern);
			var result2 = dirinfo.GetFiles(searchOption, searchFileByRegularExpressionPattern);
			var result3 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileByRegularExpressionPattern);
			var result4 = strpath.GetFiles(searchOption, searchFileByRegularExpressionPattern);

			// Assert
			Assert.Equal(2, result1?.Count());
			Assert.Equal(2, result2?.Count());
			Assert.Equal(2, result3?.Count());
			Assert.Equal(2, result4?.Count());
		}

		[Fact]
		public void TraverseFiles_TopDirectories_SafeTraversalFileSearchOptions()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.TopDirectoryOnly;

			var fileSearchOptions = new SafeTraversalFileSearchOptions
			{
				Extension = "csv",
				SizeOption = new SearchFileBySizeOption(21, SizeType.KiloBytes)
			};

			// Act
			var result1 = safeTraversal.TraverseFiles(dirinfo, searchOption, fileSearchOptions);
			var result2 = dirinfo.GetFiles(searchOption, fileSearchOptions);
			var result3 = safeTraversal.TraverseFiles(strpath, searchOption, fileSearchOptions);
			var result4 = strpath.GetFiles(searchOption, fileSearchOptions);

			// Assert
			Assert.Equal(4, result1?.Count());
			Assert.Equal(4, result2?.Count());
			Assert.Equal(4, result3?.Count());
			Assert.Equal(4, result4?.Count());
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
			IEnumerable<FileInfo> result1 = safeTraversal.TraverseFiles(dirinfo, searchOption);
			IEnumerable<FileInfo> result2 = dirinfo.GetFiles(searchOption);
			IEnumerable<string> result3 = safeTraversal.TraverseFiles(strpath, searchOption);
			IEnumerable<string> result4 = strpath.GetFiles(searchOption);

			// Assert
			Assert.Equal(1255, result1?.Count());
			Assert.Equal(1255, result2?.Count());
			Assert.Equal(1255, result3?.Count());
			Assert.Equal(1255, result4?.Count());
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
			var result1 = safeTraversal.TraverseFiles(dirinfo, searchOption, filter);
			var result2 = dirinfo.GetFiles(searchOption, filter);
			var result3 = safeTraversal.TraverseFiles(strpath, searchOption, filter);
			var result4 = strpath.GetFiles(searchOption, filter);

			// Assert
			Assert.Equal(503, result1?.Count());
			Assert.Equal(503, result2?.Count());
			Assert.Equal(503, result3?.Count());
			Assert.Equal(503, result4?.Count());
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
			var result1 = safeTraversal.TraverseFiles(dirinfo, searchOption, commonSize);
			var result2 = dirinfo.GetFiles(searchOption, commonSize);
			var result3 = safeTraversal.TraverseFiles(strpath, searchOption, commonSize);
			var result4 = strpath.GetFiles(searchOption, commonSize);

			// Assert
			Assert.Equal(97, result1?.Count());
			Assert.Equal(97, result2?.Count());
			Assert.Equal(97, result3?.Count());
			Assert.Equal(97, result4?.Count());
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
			var result1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileByName);
			var result2 = dirinfo.GetFiles(searchOption, searchFileByName);
			var result3 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileByName);
			var result4 = strpath.GetFiles(searchOption, searchFileByName);

			// Assert
			Assert.Equal(2, result1?.Count());
			Assert.Equal(2, result2?.Count());
			Assert.Equal(2, result3?.Count());
			Assert.Equal(2, result4?.Count());
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
			var result1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileBySize);
			var result2 = dirinfo.GetFiles(searchOption, searchFileBySize);
			var result3 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileBySize);
			var result4 = strpath.GetFiles(searchOption, searchFileBySize);

			// Assert
			Assert.Equal(10, result1?.Count());
			Assert.Equal(10, result2?.Count());
			Assert.Equal(10, result3?.Count());
			Assert.Equal(10, result4?.Count());
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
			var result1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileBySizeRange);
			var result2 = dirinfo.GetFiles(searchOption, searchFileBySizeRange);
			var result3 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileBySizeRange);
			var result4 = strpath.GetFiles(searchOption, searchFileBySizeRange);

			// Assert
			Assert.Equal(53, result1?.Count());
			Assert.Equal(53, result2?.Count());
			Assert.Equal(53, result3?.Count());
			Assert.Equal(53, result4?.Count());
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
			var result1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileByDate);
			var result2 = dirinfo.GetFiles(searchOption, searchFileByDate);
			var result3 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileByDate);
			var result4 = strpath.GetFiles(searchOption, searchFileByDate);

			// Assert
			Assert.Equal(6, result1?.Count());
			Assert.Equal(6, result2?.Count());
			Assert.Equal(6, result3?.Count());
			Assert.Equal(6, result4?.Count());
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
			var result1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileByDateRange);
			var result2 = dirinfo.GetFiles(searchOption, searchFileByDateRange);
			var result3 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileByDateRange);
			var result4 = strpath.GetFiles(searchOption, searchFileByDateRange);

			// Assert
			Assert.Equal(26, result1?.Count());
			Assert.Equal(26, result2?.Count());
			Assert.Equal(26, result3?.Count());
			Assert.Equal(26, result4?.Count());
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
			var result1 = safeTraversal.TraverseFiles(dirinfo, searchOption, searchFileByRegularExpressionPattern);
			var result2 = dirinfo.GetFiles(searchOption, searchFileByRegularExpressionPattern);
			var result3 = safeTraversal.TraverseFiles(strpath, searchOption, searchFileByRegularExpressionPattern);
			var result4 = strpath.GetFiles(searchOption, searchFileByRegularExpressionPattern);

			// Assert
			Assert.Equal(4, result1?.Count());
			Assert.Equal(4, result2?.Count());
			Assert.Equal(4, result3?.Count());
			Assert.Equal(4, result4?.Count());
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
			var result1 = safeTraversal.TraverseFiles(dirinfo, searchOption, fileSearchOptions);
			var result2 = dirinfo.GetFiles(searchOption, fileSearchOptions);
			var result3 = safeTraversal.TraverseFiles(strpath, searchOption, fileSearchOptions);
			var result4 = strpath.GetFiles(searchOption, fileSearchOptions);

			// Assert
			Assert.Equal(4, result1?.Count());
			Assert.Equal(4, result2?.Count());
			Assert.Equal(4, result3?.Count());
			Assert.Equal(4, result4?.Count());
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
			IEnumerable<DirectoryInfo> result1 = safeTraversal.TraverseDirectories(dirinfo);
			IEnumerable<DirectoryInfo> result2 = dirinfo.GetDirectories();
			IEnumerable<string> result3 = safeTraversal.TraverseDirectories(strpath);
			IEnumerable<string> result4 = strpath.GetDirectories();

			// Assert
			Assert.Equal(19, result1?.Count());
			Assert.Equal(19, result2?.Count());
			Assert.Equal(19, result3?.Count());
			Assert.Equal(19, result4?.Count());
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
			var result1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, filter);
			var result2 = dirinfo.GetDirectories(searchOption, filter);
			var result3 = safeTraversal.TraverseDirectories(strpath, searchOption, filter);
			var result4 = strpath.GetDirectories(searchOption, filter);

			// Assert
			Assert.Equal(1, result1?.Count());
			Assert.Equal(1, result2?.Count());
			Assert.Equal(1, result3?.Count());
			Assert.Equal(1, result4?.Count());
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
			var result1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, attributes);
			var result2 = dirinfo.GetDirectories(searchOption, attributes);
			var result3 = safeTraversal.TraverseDirectories(strpath, searchOption, attributes);
			var result4 = strpath.GetDirectories(searchOption, attributes);

			// Assert
			Assert.Equal(1, result1?.Count());
			Assert.Equal(1, result2?.Count());
			Assert.Equal(1, result3?.Count());
			Assert.Equal(1, result4?.Count());
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
			var result1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, searchDirectoryByDateOption);
			var result2 = dirinfo.GetDirectories(searchOption, searchDirectoryByDateOption);
			var result3 = safeTraversal.TraverseDirectories(strpath, searchOption, searchDirectoryByDateOption);
			var result4 = strpath.GetDirectories(searchOption, searchDirectoryByDateOption);

			// Assert
			Assert.Equal(2, result1?.Count());
			Assert.Equal(2, result2?.Count());
			Assert.Equal(2, result3?.Count());
			Assert.Equal(2, result4?.Count());
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
			var result1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, searchDirectoryByName);
			var result2 = dirinfo.GetDirectories(searchOption, searchDirectoryByName);
			var result3 = safeTraversal.TraverseDirectories(strpath, searchOption, searchDirectoryByName);
			var result4 = strpath.GetDirectories(searchOption, searchDirectoryByName);

			// Assert
			Assert.Equal(1, result1?.Count());
			Assert.Equal(1, result2?.Count());
			Assert.Equal(1, result3?.Count());
			Assert.Equal(1, result4?.Count());
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
			var result1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, searchDirectoryByRegularExpressionPattern);
			var result2 = dirinfo.GetDirectories(searchOption, searchDirectoryByRegularExpressionPattern);
			var result3 = safeTraversal.TraverseDirectories(strpath, searchOption, searchDirectoryByRegularExpressionPattern);
			var result4 = strpath.GetDirectories(searchOption, searchDirectoryByRegularExpressionPattern);

			// Assert
			//Assert.Equal(1, result1?.Count());
			//Assert.Equal(1, result2?.Count());
			//Assert.Equal(1, result3?.Count());
			//Assert.Equal(1, result4?.Count());
			Assert.Single(result1);
			Assert.Single(result2);
			Assert.Single(result3);
			Assert.Single(result4);
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
			var result1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, directorySearchOptions);
			var result2 = dirinfo.GetDirectories(searchOption, directorySearchOptions);
			var result3 = safeTraversal.TraverseDirectories(strpath, searchOption, directorySearchOptions);
			var result4 = strpath.GetDirectories(searchOption, directorySearchOptions);

			// Assert
			Assert.Equal(1, result1?.Count());
			Assert.Equal(1, result2?.Count());
			Assert.Equal(1, result3?.Count());
			Assert.Equal(1, result4?.Count());
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
			IEnumerable<DirectoryInfo> result1 = safeTraversal.TraverseDirectories(dirinfo, searchOption);
			IEnumerable<DirectoryInfo> result2 = dirinfo.GetDirectories(searchOption);
			IEnumerable<string> result3 = safeTraversal.TraverseDirectories(strpath, searchOption);
			IEnumerable<string> result4 = strpath.GetDirectories(searchOption);

			// Assert
			Assert.Equal(667, result1?.Count());
			Assert.Equal(667, result2?.Count());
			Assert.Equal(667, result3?.Count());
			Assert.Equal(667, result4?.Count());
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
			var result1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, filter);
			var result2 = dirinfo.GetDirectories(searchOption, filter);
			var result3 = safeTraversal.TraverseDirectories(strpath, searchOption, filter);
			var result4 = strpath.GetDirectories(searchOption, filter);

			// Assert
			Assert.Equal(19, result1?.Count());
			Assert.Equal(19, result2?.Count());
			Assert.Equal(19, result3?.Count());
			Assert.Equal(19, result4?.Count());
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
			var result1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, attributes);
			var result2 = dirinfo.GetDirectories(searchOption, attributes);
			var result3 = safeTraversal.TraverseDirectories(strpath, searchOption, attributes);
			var result4 = strpath.GetDirectories(searchOption, attributes);

			// Assert
			Assert.Equal(157, result1?.Count());
			Assert.Equal(157, result2?.Count());
			Assert.Equal(157, result3?.Count());
			Assert.Equal(157, result4?.Count());
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
			var result1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, searchDirectoryByDateOption);
			var result2 = dirinfo.GetDirectories(searchOption, searchDirectoryByDateOption);
			var result3 = safeTraversal.TraverseDirectories(strpath, searchOption, searchDirectoryByDateOption);
			var result4 = strpath.GetDirectories(searchOption, searchDirectoryByDateOption);

			// Assert
			Assert.Equal(8, result1?.Count());
			Assert.Equal(8, result2?.Count());
			Assert.Equal(8, result3?.Count());
			Assert.Equal(8, result4?.Count());
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
			var result1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, searchDirectoryByName);
			var result2 = dirinfo.GetDirectories(searchOption, searchDirectoryByName);
			var result3 = safeTraversal.TraverseDirectories(strpath, searchOption, searchDirectoryByName);
			var result4 = strpath.GetDirectories(searchOption, searchDirectoryByName);

			// Assert
			Assert.Equal(4, result1?.Count());
			Assert.Equal(4, result2?.Count());
			Assert.Equal(4, result3?.Count());
			Assert.Equal(4, result4?.Count());
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
			var result1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, searchDirectoryByRegularExpressionPattern);
			var result2 = dirinfo.GetDirectories(searchOption, searchDirectoryByRegularExpressionPattern);
			var result3 = safeTraversal.TraverseDirectories(strpath, searchOption, searchDirectoryByRegularExpressionPattern);
			var result4 = strpath.GetDirectories(searchOption, searchDirectoryByRegularExpressionPattern);

			// Assert
			//Assert.Equal(1, result1?.Count());
			//Assert.Equal(1, result2?.Count());
			//Assert.Equal(1, result3?.Count());
			//Assert.Equal(1, result4?.Count());
			Assert.Single(result1);
			Assert.Single(result2);
			Assert.Single(result3);
			Assert.Single(result4);
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
			var result1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, directorySearchOptions);
			var result2 = dirinfo.GetDirectories(searchOption, directorySearchOptions);
			var result3 = safeTraversal.TraverseDirectories(strpath, searchOption, directorySearchOptions);
			var result4 = strpath.GetDirectories(searchOption, directorySearchOptions);

			// Assert
			Assert.Equal(14, result1?.Count());
			Assert.Equal(14, result2?.Count());
			Assert.Equal(14, result3?.Count());
			Assert.Equal(14, result4?.Count());
		}
		#endregion
	}
}
