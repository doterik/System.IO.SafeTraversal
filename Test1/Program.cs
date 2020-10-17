using System;
using System.Collections.Generic;
using System.IO;
using System.IO.SafeTraversal.Core;
using System.Linq;
using Xunit;

namespace Test1
{
	internal class Program
	{
		private static readonly string testdir = @"c:\temp";

		private static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			TraverseDirectories_TopDirectories_SearchDirectoryByDateOption();
			TraverseDirectories_TopDirectories_SearchDirectoryByRegularExpressionOption();
			TraverseFiles_StateUnderTest_ExpectedBehavior();
			TraverseFiles_DirectoryInfo_AllDirectories_SearchFileByNameOption();
		}
		void A()

		{
			var x = new DirectoryInfo("c:\temp");
			x.GetDirectories("abra");
			x.GetFiles(SearchOption.AllDirectories, CommonSize.Huge);
			var y = new FileInfo("");
			string z = "c:\temp";
			var q = z.GetFiles(SearchOption.TopDirectoryOnly, new SearchFileByDateOption(new DateTime(2020, 10, 6), DateComparisonType.LastAccessDate));
			foreach (var item in z)
			{
				Console.WriteLine(item);
			}
		}

		public static void TraverseDirectories_TopDirectories_SearchDirectoryByDateOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.TopDirectoryOnly;

			var searchDirectoryByDateOption = new SearchDirectoryByDateOption(new DateTime(2016, 03, 09), DateComparisonType.LastModificationDate);

			// Act
			var result1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, searchDirectoryByDateOption);
			var result2 = dirinfo.GetDirectories(searchOption, searchDirectoryByDateOption);
			var result3 = safeTraversal.TraverseDirectories(strpath, searchOption, searchDirectoryByDateOption);
			var result4 = strpath.GetDirectories(searchOption, searchDirectoryByDateOption);

			foreach (var res in result1) Console.WriteLine(res.Name); //.FullName);
			foreach (var res in result2) Console.WriteLine(res.Name); //.FullName);
			foreach (var res in result3) Console.WriteLine(res); //.FullName);
			foreach (var res in result4) Console.WriteLine(res); //.FullName);

			// Assert
			Assert.Equal(8, result1?.Count());
			Assert.Equal(8, result2?.Count());
			Assert.Equal(8, result3?.Count());
			Assert.Equal(8, result4?.Count());
		}

		public static void TraverseDirectories_TopDirectories_SearchDirectoryByRegularExpressionOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var dirinfo = new DirectoryInfo(testdir);
			var strpath = testdir;
			var searchOption = SearchOption.TopDirectoryOnly;

			var searchDirectoryByRegularExpressionPattern = new SearchDirectoryByRegularExpressionOption(@"Diag");

			// Act
			var result1 = safeTraversal.TraverseDirectories(dirinfo, searchOption, searchDirectoryByRegularExpressionPattern);
			var result2 = dirinfo.GetDirectories(searchOption, searchDirectoryByRegularExpressionPattern);
			var result3 = safeTraversal.TraverseDirectories(strpath, searchOption, searchDirectoryByRegularExpressionPattern);
			var result4 = strpath.GetDirectories(searchOption, searchDirectoryByRegularExpressionPattern);


			foreach (var res in result1) Console.WriteLine(res.Name); //.FullName);
			foreach (var res in result2) Console.WriteLine(res.Name); //.FullName);
			foreach (var res in result3) Console.WriteLine(res); //.FullName);
			foreach (var res in result4) Console.WriteLine(res); //.FullName);

			// Assert
			Assert.Equal(1, result1?.Count());
			Assert.Equal(1, result2?.Count());
			Assert.Equal(1, result3?.Count());
			Assert.Equal(1, result4?.Count());
			//Assert.Single(result1);
			//Assert.Single(result2);
			//Assert.Single(result3);
			//Assert.Single(result4);
		}

		public static void TraverseFiles_StateUnderTest_ExpectedBehavior()
		{
			// Arrange
			SafeTraversal safeTraversal = new SafeTraversal();
			DirectoryInfo path = new DirectoryInfo(testdir);

			// Act
			IEnumerable<FileInfo> result = safeTraversal.TraverseFiles(path);

			// Assert
			Assert.True(result is not null);
			Assert.Equal(89, result?.Count());
		}

		public static void TraverseFiles_DirectoryInfo_AllDirectories_SearchFileByNameOption()
		{
			// Arrange
			var safeTraversal = new SafeTraversal();
			var path = new DirectoryInfo(testdir);
			var searchOption = SearchOption.AllDirectories;

			var searchFileByName = new SearchFileByNameOption("ngen2x32");

			// Act
			var result = safeTraversal.TraverseFiles(path, searchOption, searchFileByName);

			foreach (var res in result)
			{
				Console.WriteLine(res.FullName);
			}

			// Assert
			Assert.True(result is not null);
			Assert.Equal(2, result?.Count());
		}

	}
}
