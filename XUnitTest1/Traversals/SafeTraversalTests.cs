using System;
using System.Collections;
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

		#region TraverseFiles(DirectoryInfo,...) -> IEnumerable<FileInfo> 
		[Fact]
		public void TraverseFiles_DirectoryInfo()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = new DirectoryInfo(testdir);

			// Act
			IEnumerable<FileInfo> result = safeTraversal.TraverseFiles(path);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(89, result.Count());
		}

		[Fact]
		public void TraverseFiles_DirectoryInfo_AllDirectories()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = new DirectoryInfo(testdir);
			var searchOption = SearchOption.AllDirectories;

			// Act
			var result = safeTraversal.TraverseFiles(path, searchOption);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(1251, result.Count());
		}

		[Fact]
		public void TraverseFiles_DirectoryInfo_AllDirectories_Filter_IsReadOnly()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = new DirectoryInfo(testdir);
			var searchOption = SearchOption.AllDirectories;

			Func<FileInfo, bool> filte1 = x => x.IsReadOnly;
			static bool filter(FileInfo x) => x.IsReadOnly;

			// Act
			var result = safeTraversal.TraverseFiles(path, searchOption, filter);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(501, result.Count());
		}

		[Fact]
		public void TraverseFiles_DirectoryInfo_AllDirectories_CommonSize_Large()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = new DirectoryInfo(testdir);
			var searchOption = SearchOption.AllDirectories;

			var commonSize = CommonSize.Large;

			// Act
			var result = safeTraversal.TraverseFiles(path, searchOption, commonSize);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(96, result.Count());
		}

		[Fact]
		public void TraverseFiles_DirectoryInfo_AllDirectories_SearchFileByNameOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = new DirectoryInfo(testdir);
			var searchOption = SearchOption.AllDirectories;

			var searchFileByName = new SearchFileByNameOption("ngen2x32");

			// Act
			var result = safeTraversal.TraverseFiles(path, searchOption, searchFileByName);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(2, result.Count());
		}

		[Fact]
		public void TraverseFiles_DirectoryInfo_AllDirectories_SearchFileBySizeOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = new DirectoryInfo(testdir);
			var searchOption = SearchOption.AllDirectories;

			var searchFileBySize = new SearchFileBySizeOption(2, SizeType.MegaBytes);

			// Act
			var result = safeTraversal.TraverseFiles(path, searchOption, searchFileBySize);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(10, result.Count());
		}

		[Fact]
		public void TraverseFiles_DirectoryInfo_AllDirectories_SearchFileBySizeRangeOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = new DirectoryInfo(testdir);
			var searchOption = SearchOption.AllDirectories;

			var searchFileBySizeRange = new SearchFileBySizeRangeOption(4, 10, SizeType.MegaBytes);

			// Act
			var result = safeTraversal.TraverseFiles(path, searchOption, searchFileBySizeRange);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(53, result.Count());
		}

		[Fact]
		public void TraverseFiles_DirectoryInfo_AllDirectories_SearchFileByDateOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = new DirectoryInfo(testdir);
			var searchOption = SearchOption.AllDirectories;

			var searchFileByDate = new SearchFileByDateOption(new DateTime(2017, 09, 01), DateComparisonType.CreationDate);

			// Act
			var result = safeTraversal.TraverseFiles(path, searchOption, searchFileByDate);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(6, result.Count());
		}

		[Fact]
		public void TraverseFiles_DirectoryInfo_AllDirectories_SearchFileByDateRangeOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = new DirectoryInfo(testdir);
			var searchOption = SearchOption.AllDirectories;

			var searchFileByDateRange = new SearchFileByDateRangeOption(
				new DateTime(2017, 09, 01),
				new DateTime(2017, 09, 30), DateComparisonType.CreationDate);

			// Act
			var result = safeTraversal.TraverseFiles(path, searchOption, searchFileByDateRange);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(26, result.Count());
		}

		[Fact]
		public void TraverseFiles_DirectoryInfo_AllDirectories_SearchFileByRegularExpressionOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = new DirectoryInfo(testdir);
			var searchOption = SearchOption.AllDirectories;

			var searchFileByRegularExpressionPattern = new SearchFileByRegularExpressionOption(@"^ng.*x6");

			// Act
			var result = safeTraversal.TraverseFiles(path, searchOption, searchFileByRegularExpressionPattern);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(4, result.Count());
		}

		[Fact]
		public void TraverseFiles_DirectoryInfo_AllDirectories_SafeTraversalFileSearchOptions()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = new DirectoryInfo(testdir);
			var searchOption = SearchOption.AllDirectories;

			var fileSearchOptions = new SafeTraversalFileSearchOptions
			{
				Extension = "csv",
				SizeOption = new SearchFileBySizeOption(21, SizeType.KiloBytes)
			};

			// Act
			var result = safeTraversal.TraverseFiles(path, searchOption, fileSearchOptions);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(4, result.Count());
		}
		#endregion

		#region TraverseDirectories(DirectoryInfo,...) -> IEnumerable<DirectoryInfo>
		[Fact]
		public void TraverseDirectories_DirectoryInfo()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = new DirectoryInfo(testdir);

			// Act
			IEnumerable<DirectoryInfo>? result = safeTraversal.TraverseDirectories(path);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(18, result.Count());
		}

		[Fact]
		public void TraverseDirectories_DirectoryInfo_AllDirectories()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = new DirectoryInfo(testdir);

			var searchOption = SearchOption.AllDirectories;

			// Act
			var result = safeTraversal.TraverseDirectories(path, searchOption);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(666, result.Count());
		}

		[Fact]
		public void TraverseDirectories_DirectoryInfo_AllDirectories_Filter_IsReadOnly()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = new DirectoryInfo(testdir);
			var searchOption = SearchOption.AllDirectories;

			Func<DirectoryInfo, bool> filter = x => x.Parent.FullName == testdir;

			// Act
			var result = safeTraversal.TraverseDirectories(path, searchOption, filter);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(18, result.Count());
		}

		[Fact]
		public void TraverseDirectories_DirectoryInfo_AllDirectories_FileAttributes()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = new DirectoryInfo(testdir);
			var searchOption = SearchOption.AllDirectories;

			var attributes = FileAttributes.Directory;//| FileAttributes.Archive|FileAttributes.Compressed;

			// Act
			var result = safeTraversal.TraverseDirectories(path, searchOption, attributes);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(502, result.Count());
		}

		[Fact]
		public void TraverseDirectories_DirectoryInfo_AllDirectories_SearchDirectoryByDateOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = new DirectoryInfo(testdir);
			var searchOption = SearchOption.AllDirectories;

			var searchDirectoryByDateOption = new SearchDirectoryByDateOption(new DateTime(2017, 03, 26), DateComparisonType.CreationDate);

			// Act
			var result = safeTraversal.TraverseDirectories(path, searchOption, searchDirectoryByDateOption);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(8, result.Count());
		}

		[Fact]
		public void TraverseDirectories_DirectoryInfo_AllDirectories_SearchDirectoryByNameOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = new DirectoryInfo(testdir);
			var searchOption = SearchOption.AllDirectories;

			var searchDirectoryByName = new SearchDirectoryByNameOption("src");

			// Act
			var result = safeTraversal.TraverseDirectories(path, searchOption, searchDirectoryByName);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(4, result.Count());
		}

		[Fact]
		public void TraverseDirectories_DirectoryInfo_AllDirectories_SearchDirectoryByRegularExpressionOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = new DirectoryInfo(testdir);
			var searchOption = SearchOption.AllDirectories;

			var searchDirectoryByRegularExpressionPattern = new SearchDirectoryByRegularExpressionOption(@"Diag$");

			// Act
			var result = safeTraversal.TraverseDirectories(path, searchOption, searchDirectoryByRegularExpressionPattern);

			// Assert
			Assert.True(result is not null);
			//Assert.Equal(1, result.Count());
			Assert.Single(result);
		}

		[Fact]
		public void TraverseDirectories_DirectoryInfo_AllDirectories_SafeTraversalDirectorySearchOptions()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = new DirectoryInfo(testdir);
			var searchOption = SearchOption.AllDirectories;

			var directorySearchOptions = new SafeTraversalDirectorySearchOptions { DirectoryNameOption = new SearchDirectoryByNameOption(@"c:\temp\test\") };

			// Act
			var result = safeTraversal.TraverseDirectories(path, searchOption, directorySearchOptions);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(506, result.Count());
		}
		#endregion

		#region TraverseFiles(string,...) -> IEnumerable<string> 
		[Fact]
		public void TraverseFiles_String()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = testdir;

			// Act
			IEnumerable<string> result = safeTraversal.TraverseFiles(path);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(89, result.Count());
		}

		[Fact]
		public void TraverseFiles_String_AllDirectories()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = testdir;
			var searchOption = SearchOption.AllDirectories;

			// Act
			var result = safeTraversal.TraverseFiles(path, searchOption);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(1251, result.Count());
		}

		[Fact]
		public void TraverseFiles_String_AllDirectories_Filter_IsReadOnly()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = testdir;
			var searchOption = SearchOption.AllDirectories;

			Func<FileInfo, bool> filte1 = x => x.IsReadOnly;
			static bool filter(FileInfo x) => x.IsReadOnly;

			// Act
			var result = safeTraversal.TraverseFiles(path, searchOption, filter);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(501, result.Count());
		}

		[Fact]
		public void TraverseFiles_String_AllDirectories_CommonSize_Large()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = testdir;
			var searchOption = SearchOption.AllDirectories;

			var commonSize = CommonSize.Large;

			// Act
			var result = safeTraversal.TraverseFiles(path, searchOption, commonSize);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(96, result.Count());
		}

		[Fact]
		public void TraverseFiles_String_AllDirectories_SearchFileByNameOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = testdir;
			var searchOption = SearchOption.AllDirectories;

			var searchFileByName = new SearchFileByNameOption("ngen2x32");

			// Act
			var result = safeTraversal.TraverseFiles(path, searchOption, searchFileByName);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(2, result.Count());
		}

		[Fact]
		public void TraverseFiles_String_AllDirectories_SearchFileBySizeOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = testdir;
			var searchOption = SearchOption.AllDirectories;

			var searchFileBySize = new SearchFileBySizeOption(2, SizeType.MegaBytes);

			// Act
			var result = safeTraversal.TraverseFiles(path, searchOption, searchFileBySize);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(10, result.Count());
		}

		[Fact]
		public void TraverseFiles_String_AllDirectories_SearchFileBySizeRangeOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = testdir;
			var searchOption = SearchOption.AllDirectories;

			var searchFileBySizeRange = new SearchFileBySizeRangeOption(4, 10, SizeType.MegaBytes);

			// Act
			var result = safeTraversal.TraverseFiles(path, searchOption, searchFileBySizeRange);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(53, result.Count());
		}

		[Fact]
		public void TraverseFiles_String_AllDirectories_SearchFileByDateOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = testdir;
			var searchOption = SearchOption.AllDirectories;

			var searchFileByDate = new SearchFileByDateOption(new DateTime(2017, 09, 01), DateComparisonType.CreationDate);

			// Act
			var result = safeTraversal.TraverseFiles(path, searchOption, searchFileByDate);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(6, result.Count());
		}

		[Fact]
		public void TraverseFiles_String_AllDirectories_SearchFileByDateRangeOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = testdir;
			var searchOption = SearchOption.AllDirectories;

			var searchFileByDateRange = new SearchFileByDateRangeOption(
				new DateTime(2017, 09, 01),
				new DateTime(2017, 09, 30), DateComparisonType.CreationDate);

			// Act
			var result = safeTraversal.TraverseFiles(path, searchOption, searchFileByDateRange);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(26, result.Count());
		}

		[Fact]
		public void TraverseFiles_String_AllDirectories_SearchFileByRegularExpressionOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = testdir;
			var searchOption = SearchOption.AllDirectories;

			var searchFileByRegularExpressionPattern = new SearchFileByRegularExpressionOption(@"^ng.*x6");

			// Act
			var result = safeTraversal.TraverseFiles(path, searchOption, searchFileByRegularExpressionPattern);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(4, result.Count());
		}

		[Fact]
		public void TraverseFiles_String_AllDirectories_SafeTraversalFileSearchOptions()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = testdir;
			var searchOption = SearchOption.AllDirectories;

			var fileSearchOptions = new SafeTraversalFileSearchOptions
			{
				Extension = "csv",
				SizeOption = new SearchFileBySizeOption(21, SizeType.KiloBytes)
			};

			// Act
			var result = safeTraversal.TraverseFiles(path, searchOption, fileSearchOptions);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(4, result.Count());
		}
		#endregion

		#region TraverseDirectories(string,...) -> IEnumerable<string> 
		[Fact]
		public void TraverseDirectories_String()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = testdir;

			// Act
			IEnumerable<string>? result = safeTraversal.TraverseDirectories(path);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(18, result.Count());
		}

		[Fact]
		public void TraverseDirectories_String_AllDirectories()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = testdir;

			var searchOption = SearchOption.AllDirectories;

			// Act
			var result = safeTraversal.TraverseDirectories(path, searchOption);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(666, result.Count());
		}

		[Fact]
		public void TraverseDirectories_String_AllDirectories_Filter_IsReadOnly()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = testdir;
			var searchOption = SearchOption.AllDirectories;

			Func<DirectoryInfo, bool> filter = x => x.Parent.FullName == testdir;

			// Act
			var result = safeTraversal.TraverseDirectories(path, searchOption, filter);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(18, result.Count());
		}

		[Fact]
		public void TraverseDirectories_String_AllDirectories_FileAttributes()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = testdir;
			var searchOption = SearchOption.AllDirectories;

			var attributes = FileAttributes.Normal;

			// Act
			var result = safeTraversal.TraverseDirectories(path, searchOption, attributes);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(502, result.Count());
		}

		[Fact]
		public void TraverseDirectories_String_AllDirectories_SearchDirectoryByDateOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = testdir;
			var searchOption = SearchOption.AllDirectories;

			var searchDirectoryByDateOption = new SearchDirectoryByDateOption(new DateTime(2017, 03, 26), DateComparisonType.CreationDate);

			// Act
			var result = safeTraversal.TraverseDirectories(path, searchOption, searchDirectoryByDateOption);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(8, result.Count());
		}

		[Fact]
		public void TraverseDirectories_String_AllDirectories_SearchDirectoryByNameOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = testdir;
			var searchOption = SearchOption.AllDirectories;

			var searchDirectoryByName = new SearchDirectoryByNameOption("src");

			// Act
			var result = safeTraversal.TraverseDirectories(path, searchOption, searchDirectoryByName);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(4, result.Count());
		}

		[Fact]
		public void TraverseDirectories_String_AllDirectories_SearchDirectoryByRegularExpressionOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = testdir;
			var searchOption = SearchOption.AllDirectories;

			var searchDirectoryByRegularExpressionPattern = new SearchDirectoryByRegularExpressionOption(@"Diag$");

			// Act
			var result = safeTraversal.TraverseDirectories(path, searchOption, searchDirectoryByRegularExpressionPattern);

			// Assert
			Assert.True(result is not null);
			//Assert.Equal(1, result.Count());
			Assert.Single(result);
		}

		[Fact]
		public void TraverseDirectories_String_AllDirectories_SafeTraversalDirectorySearchOptions()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = testdir;
			var searchOption = SearchOption.AllDirectories;

			var directorySearchOptions = new SafeTraversalDirectorySearchOptions { DirectoryNameOption = new SearchDirectoryByNameOption("bin") };

			// Act
			var result = safeTraversal.TraverseDirectories(path, searchOption, directorySearchOptions);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(506, result.Count());
		}
		#endregion

		#region REM
		//[Fact]
		//public void TraverseFiles_StateUnderTest_ExpectedBehavior11()
		//{
		//	// Arrange
		//	var safeTraversal = new SafeTraversal();
		//	string path = null;

		//	// Act
		//	var result = safeTraversal.TraverseFiles(
		//		path);

		//	// Assert
		//	Assert.True(false);
		//}

		//[Fact]
		//public void TraverseFiles_StateUnderTest_ExpectedBehavior12()
		//{
		//	// Arrange
		//	var safeTraversal = new SafeTraversal();
		//	string path = null;
		//	SearchOption searchOption = default(global::System.IO.SearchOption);

		//	// Act
		//	var result = safeTraversal.TraverseFiles(
		//		path,
		//		searchOption);

		//	// Assert
		//	Assert.True(false);
		//}

		//[Fact]
		//public void TraverseFiles_StateUnderTest_ExpectedBehavior13()
		//{
		//	// Arrange
		//	var safeTraversal = new SafeTraversal();
		//	var path = new DirectoryInfo( testdir);
		//	SearchOption searchOption = default(global::System.IO.SearchOption);
		//	Func<FileInfo, bool> filter = null;

		//	// Act
		//	var result = safeTraversal.TraverseFiles(
		//		path,
		//		searchOption,
		//		filter);

		//	// Assert
		//	Assert.True(false);
		//}

		//[Fact]
		//public void TraverseFiles_StateUnderTest_ExpectedBehavior14()
		//{
		//	// Arrange
		//	var safeTraversal = new SafeTraversal();
		//	string path = null;
		//	SearchOption searchOption = default(global::System.IO.SearchOption);
		//	CommonSize commonSize = default(global::System.IO.SafeTraversal.Core.CommonSize);

		//	// Act
		//	var result = safeTraversal.TraverseFiles(
		//		path,
		//		searchOption,
		//		commonSize);

		//	// Assert
		//	Assert.True(false);
		//}

		//[Fact]
		//public void TraverseFiles_StateUnderTest_ExpectedBehavior15()
		//{
		//	// Arrange
		//	var safeTraversal = new SafeTraversal();
		//	string path = null;
		//	SearchOption searchOption = default(global::System.IO.SearchOption);
		//	SearchFileByNameOption searchFileByName = null;

		//	// Act
		//	var result = safeTraversal.TraverseFiles(
		//		path,
		//		searchOption,
		//		searchFileByName);

		//	// Assert
		//	Assert.True(false);
		//}

		//[Fact]
		//public void TraverseFiles_StateUnderTest_ExpectedBehavior16()
		//{
		//	// Arrange
		//	var safeTraversal = new SafeTraversal();
		//	string path = null;
		//	SearchOption searchOption = default(global::System.IO.SearchOption);
		//	SearchFileBySizeOption searchFileBySize = null;

		//	// Act
		//	var result = safeTraversal.TraverseFiles(
		//		path,
		//		searchOption,
		//		searchFileBySize);

		//	// Assert
		//	Assert.True(false);
		//}

		//[Fact]
		//public void TraverseFiles_StateUnderTest_ExpectedBehavior17()
		//{
		//	// Arrange
		//	var safeTraversal = new SafeTraversal();
		//	string path = null;
		//	SearchOption searchOption = default(global::System.IO.SearchOption);
		//	SearchFileBySizeRangeOption searchFileBySizeRange = null;

		//	// Act
		//	var result = safeTraversal.TraverseFiles(
		//		path,
		//		searchOption,
		//		searchFileBySizeRange);

		//	// Assert
		//	Assert.True(false);
		//}

		//[Fact]
		//public void TraverseFiles_StateUnderTest_ExpectedBehavior18()
		//{
		//	// Arrange
		//	var safeTraversal = new SafeTraversal();
		//	string path = null;
		//	SearchOption searchOption = default(global::System.IO.SearchOption);
		//	SearchFileByDateOption searchFileByDate = null;

		//	// Act
		//	var result = safeTraversal.TraverseFiles(
		//		path,
		//		searchOption,
		//		searchFileByDate);

		//	// Assert
		//	Assert.True(false);
		//}

		//[Fact]
		//public void TraverseFiles_StateUnderTest_ExpectedBehavior19()
		//{
		//	// Arrange
		//	var safeTraversal = new SafeTraversal();
		//	string path = null;
		//	SearchOption searchOption = default(global::System.IO.SearchOption);
		//	SearchFileByDateRangeOption searchFileByDateRange = null;

		//	// Act
		//	var result = safeTraversal.TraverseFiles(
		//		path,
		//		searchOption,
		//		searchFileByDateRange);

		//	// Assert
		//	Assert.True(false);
		//}

		//[Fact]
		//public void TraverseFiles_StateUnderTest_ExpectedBehavior20()
		//{
		//	// Arrange
		//	var safeTraversal = new SafeTraversal();
		//	string path = null;
		//	SearchOption searchOption = default(global::System.IO.SearchOption);
		//	SearchFileByRegularExpressionOption searchFileByRegularExpressionPattern = null;

		//	// Act
		//	var result = safeTraversal.TraverseFiles(
		//		path,
		//		searchOption,
		//		searchFileByRegularExpressionPattern);

		//	// Assert
		//	Assert.True(false);
		//}

		//[Fact]
		//public void TraverseFiles_StateUnderTest_ExpectedBehavior21()
		//{
		//	// Arrange
		//	var safeTraversal = new SafeTraversal();
		//	string path = null;
		//	SearchOption searchOption = default(global::System.IO.SearchOption);
		//	SafeTraversalFileSearchOptions fileSearchOptions = null;

		//	// Act
		//	var result = safeTraversal.TraverseFiles(
		//		path,
		//		searchOption,
		//		fileSearchOptions);

		//	// Assert
		//	Assert.True(false);
		//}

		//[Fact]
		//public void TraverseDirectories_StateUnderTest_ExpectedBehavior8()
		//{
		//	// Arrange
		//	var safeTraversal = new SafeTraversal();
		//	string path = null;

		//	// Act
		//	var result = safeTraversal.TraverseDirectories(
		//		path);

		//	// Assert
		//	Assert.True(false);
		//}

		//[Fact]
		//public void TraverseDirectories_StateUnderTest_ExpectedBehavior9()
		//{
		//	// Arrange
		//	var safeTraversal = new SafeTraversal();
		//	string path = null;
		//	SearchOption searchOption = default(global::System.IO.SearchOption);

		//	// Act
		//	var result = safeTraversal.TraverseDirectories(
		//		path,
		//		searchOption);

		//	// Assert
		//	Assert.True(false);
		//}

		//[Fact]
		//public void TraverseDirectories_StateUnderTest_ExpectedBehavior10()
		//{
		//	// Arrange
		//	var safeTraversal = new SafeTraversal();
		//	string path = null;
		//	SearchOption searchOption = default(global::System.IO.SearchOption);
		//	Func<DirectoryInfo, bool> filter = null;

		//	// Act
		//	var result = safeTraversal.TraverseDirectories(
		//		path,
		//		searchOption,
		//		filter);

		//	// Assert
		//	Assert.True(false);
		//}

		//[Fact]
		//public void TraverseDirectories_StateUnderTest_ExpectedBehavior11()
		//{
		//	// Arrange
		//	var safeTraversal = new SafeTraversal();
		//	string path = null;
		//	SearchOption searchOption = default(global::System.IO.SearchOption);
		//	FileAttributes attributes = default(global::System.IO.FileAttributes);

		//	// Act
		//	var result = safeTraversal.TraverseDirectories(
		//		path,
		//		searchOption,
		//		attributes);

		//	// Assert
		//	Assert.True(false);
		//}

		//[Fact]
		//public void TraverseDirectories_StateUnderTest_ExpectedBehavior12()
		//{
		//	// Arrange
		//	var safeTraversal = new SafeTraversal();
		//	string path = null;
		//	SearchOption searchOption = default(global::System.IO.SearchOption);
		//	SearchDirectoryByDateOption searchDirectoryByDateOption = null;

		//	// Act
		//	var result = safeTraversal.TraverseDirectories(
		//		path,
		//		searchOption,
		//		searchDirectoryByDateOption);

		//	// Assert
		//	Assert.True(false);
		//}

		//[Fact]
		//public void TraverseDirectories_StateUnderTest_ExpectedBehavior13()
		//{
		//	// Arrange
		//	var safeTraversal = new SafeTraversal();
		//	string path = null;
		//	SearchOption searchOption = default(global::System.IO.SearchOption);
		//	SearchDirectoryByNameOption searchDirectoryByName = null;

		//	// Act
		//	var result = safeTraversal.TraverseDirectories(
		//		path,
		//		searchOption,
		//		searchDirectoryByName);

		//	// Assert
		//	Assert.True(false);
		//}

		//[Fact]
		//public void TraverseDirectories_StateUnderTest_ExpectedBehavior14()
		//{
		//	// Arrange
		//	var safeTraversal = new SafeTraversal();
		//	string path = null;
		//	SearchOption searchOption = default(global::System.IO.SearchOption);
		//	SearchDirectoryByRegularExpressionOption searchDirectoryByRegularExpressionPattern = null;

		//	// Act
		//	var result = safeTraversal.TraverseDirectories(
		//		path,
		//		searchOption,
		//		searchDirectoryByRegularExpressionPattern);

		//	// Assert
		//	Assert.True(false);
		//}

		//[Fact]
		//public void TraverseDirectories_StateUnderTest_ExpectedBehavior15()
		//{
		//	// Arrange
		//	var safeTraversal = new SafeTraversal();
		//	string path = null;
		//	SearchOption searchOption = default(global::System.IO.SearchOption);
		//	SafeTraversalDirectorySearchOptions directorySearchOptions = null;

		//	// Act
		//	var result = safeTraversal.TraverseDirectories(
		//		path,
		//		searchOption,
		//		directorySearchOptions);

		//	// Assert
		//	Assert.True(false);
		//}

		//[Fact]
		//public void FindParents_StateUnderTest_ExpectedBehavior()
		//{
		//	// Arrange
		//	//SafeTraversal safeTraversal;// = new SafeTraversal();
		//	DirectoryInfo path = null;

		//	// Act
		//	var result = SafeTraversal.FindParents(
		//		path);

		//	// Assert
		//	Assert.True(false);
		//}

		//[Fact]
		//public void FindParents_StateUnderTest_ExpectedBehavior1()
		//{
		//	// Arrange
		//	var safeTraversal = new SafeTraversal();
		//	FileInfo file = null;

		//	// Act
		//	var result = SafeTraversal.FindParents(
		//		file);

		//	// Assert
		//	Assert.True(false);
		//}
		#endregion
	}
}
