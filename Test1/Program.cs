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
			TraverseFiles_StateUnderTest_ExpectedBehavior();
			TraverseFiles_DirectoryInfo_AllDirectories_SearchFileByNameOption();
		}
		void A()

		{
			var x = new DirectoryInfo("");
			x.GetDirectories("");
			x.GetFiles(SearchOption.AllDirectories, CommonSize.Huge);
			var y = new FileInfo("");
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
