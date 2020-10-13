using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace System.IO.SafeTraversal.Core
{
	public partial class SafeTraversal
	{
		enum SizeConverterType
		{
			LowerBound,
			UpperBound
		}
		#region FILE
		private static long SizeConverter(double size, SizeType type, SizeConverterType converterType)
		{
			try
			{
				double result = 0;
				if (size == 0) size = +1;
				switch (converterType)
				{
					case SizeConverterType.LowerBound: result = Math.Floor((size - 1) * Math.Pow(1024, (int)type)); break;
					case SizeConverterType.UpperBound: result = Math.Ceiling((size + 1) * Math.Pow(1024, (int)type)); break;
				}
				return result >= long.MaxValue ? -1 : Convert.ToInt64(result);
			}
			catch { return -1; }
		}
		private static long SizeConverterEx(double size, SizeType type, SizeConverterType converterType)
		{
			try
			{
				double result = 0;
				if (size == 0) size = +1;
				switch (converterType)
				{
					case SizeConverterType.LowerBound: result = Math.Floor((size - 1) * Math.Pow(1024, (int)type)); break;
					case SizeConverterType.UpperBound: result = Math.Ceiling((size + 1) * Math.Pow(1024, (int)type)); break;
				}
				return result >= long.MaxValue ? -1 : Convert.ToInt64(result);
			}
			catch { return -1; }
		}

		private static bool MatchBySize(FileInfo fileInfo, double size, SizeType sizeType)
		{
			var lowerBound = SizeConverter(size, sizeType, SizeConverterType.LowerBound); if (lowerBound < 0) return false;
			var upperBound = SizeConverter(size, sizeType, SizeConverterType.UpperBound); if (upperBound < 0) return false;

			return fileInfo.Length >= lowerBound && fileInfo.Length <= upperBound;
		}
		private static bool MatchBySizeEx(FileInfo fileInfo, double size, SizeType sizeType)
		{
			var lowerBound = SizeConverterEx(size, sizeType, SizeConverterType.LowerBound); if (lowerBound < 0) return false;
			var upperBound = SizeConverterEx(size, sizeType, SizeConverterType.UpperBound); if (upperBound < 0) return false;

			return fileInfo.Length >= lowerBound && fileInfo.Length <= upperBound;
		}

		private static bool MatchBySizeRange(FileInfo fileInfo, double lowerBoundSize, double upperBoundSize, SizeType sizeType)
		{
			lowerBoundSize++;

			if (lowerBoundSize < 0) return false;
			if (upperBoundSize < 0) return false;
			if (lowerBoundSize >= upperBoundSize) return false;

			var lowerBound = SizeConverter(lowerBoundSize, sizeType, SizeConverterType.LowerBound); if (lowerBound < 0) return false;
			var upperBound = SizeConverter(upperBoundSize, sizeType, SizeConverterType.UpperBound); if (upperBound < 0) return false;

			return fileInfo.Length >= lowerBound && fileInfo.Length <= upperBound;
		}
		private static bool MatchBySizeRangeEx(FileInfo fileInfo, double lowerBoundSize, double upperBoundSize, SizeType sizeType)
		{
			lowerBoundSize++;

			if (lowerBoundSize < 0) return false;
			if (upperBoundSize < 0) return false;
			if (lowerBoundSize >= upperBoundSize) return false;

			var lowerBound = SizeConverterEx(lowerBoundSize, sizeType, SizeConverterType.LowerBound); if (lowerBound < 0) return false;
			var upperBound = SizeConverterEx(upperBoundSize, sizeType, SizeConverterType.UpperBound); if (upperBound < 0) return false;

			return fileInfo.Length >= lowerBound && fileInfo.Length <= upperBound;
		}

		private static bool MatchByDate(FileInfo fileInfo, DateTime dateTime, DateComparisonType comparisonType)
		{
			var fileInfoDate = GetDateComparisonType(fileInfo, comparisonType);
			return DateTime.Equals(fileInfoDate.Date, dateTime.Date);
		}

		private static bool MatchByDateEx(FileInfo fileInfo, DateTime dateTime, DateComparisonType comparisonType)
		{
			var fileInfoDate = GetDateComparisonType(fileInfo, comparisonType);
			return DateTime.Equals(fileInfoDate.Date, dateTime.Date);
		}

		private static bool MatchByDateRange(FileInfo fileInfo, DateTime lowerBoundDate, DateTime upperBoundDate, DateComparisonType comparisonType)
		{
			var fileInfoDate = GetDateComparisonType(fileInfo, comparisonType);
			return fileInfoDate >= lowerBoundDate.Date && fileInfoDate <= upperBoundDate.Date;
		}
		private static bool MatchByDateRangeEx(FileInfo fileInfo, DateTime lowerBoundDate, DateTime upperBoundDate, DateComparisonType comparisonType)
		{
			var fileInfoDate = GetDateComparisonType(fileInfo, comparisonType);
			return fileInfoDate >= lowerBoundDate.Date && fileInfoDate <= upperBoundDate.Date;
		}

		private static DateTime GetDateComparisonType(FileInfo fileInfo, DateComparisonType comparisonType) => comparisonType switch
		{
			DateComparisonType.CreationDate => fileInfo.CreationTime.Date,
			DateComparisonType.LastModificationDate => fileInfo.LastWriteTime.Date,
			DateComparisonType.LastAccessDate => fileInfo.LastAccessTime.Date,
			_ => new DateTime()
		};

		private static bool MatchByPattern(FileInfo fileInfo, string pattern)
		{
			bool result; // = false;
			try { result = Regex.IsMatch(Path.GetFileNameWithoutExtension(fileInfo.Name), pattern, RegexOptions.Compiled); }
			catch { result = false; }
			return result;
		}
		private static bool MatchByPatternEx(FileInfo fileInfo, string pattern)
		{
			bool result; // = false;
			try { result = Regex.IsMatch(Path.GetFileNameWithoutExtension(fileInfo.Name), pattern, RegexOptions.Compiled); }
			catch { result = false; }
			return result;
		}

		private static bool MatchByPatternWithExtension(FileInfo fileInfo, string pattern)
		{
			bool result; // = false;
			try { result = Regex.IsMatch(fileInfo.Name, pattern, RegexOptions.Compiled); }
			catch { result = false; }
			return result;
		}
		private static bool MatchByPatternWithExtensionEx(FileInfo fileInfo, string pattern)
		{
			bool result; // = false;
			try { result = Regex.IsMatch(fileInfo.Name, pattern, RegexOptions.Compiled); }
			catch { result = false; }
			return result;
		}

		private static bool MatchByExtension(FileInfo fileInfo, string extension)
		{
			extension = Regex.Match(extension, @"(\.)?\w+").Value;
			if (!extension.StartsWith(".")) extension = "." + extension;
			return fileInfo.Extension.Equals(extension, StringComparison.InvariantCultureIgnoreCase);
		}
		private static bool MatchByExtensionEx(FileInfo fileInfo, string extension)
		{
			extension = Regex.Match(extension, @"(\.)?\w+").Value;
			if (!extension.StartsWith(".")) extension = "." + extension;
			return fileInfo.Extension.Equals(extension, StringComparison.InvariantCultureIgnoreCase);
		}

		private static bool MatchByAttributes(FileInfo fileInfo, FileAttributes fileAttributes) => fileInfo.Attributes == fileAttributes;
		private static bool MatchByAttributesEx(FileInfo fileInfo, FileAttributes fileAttributes) => fileInfo.Attributes == fileAttributes;

		private static bool MatchByName(FileInfo fileInfo, string keyword, StringComparison stringComparison) =>
			Path.GetFileNameWithoutExtension(fileInfo.Name).Equals(keyword, stringComparison);
		private static bool MatchByNameEx(FileInfo fileInfo, string keyword, StringComparison stringComparison) =>
			Path.GetFileNameWithoutExtension(fileInfo.Name).Equals(keyword, stringComparison);

		private static bool MatchByNameWithExtension(FileInfo fileInfo, string keyword, StringComparison stringComparison) =>
			fileInfo.Name.Equals(keyword, stringComparison);
		private static bool MatchByNameWithExtensionEx(FileInfo fileInfo, string keyword, StringComparison stringComparison) =>
			fileInfo.Name.Equals(keyword, stringComparison);

		private static bool MatchByCommonSize(FileInfo fileInfo, CommonSize commonSize) => commonSize switch
		{
			CommonSize.Empty => fileInfo.Length == 0,
			CommonSize.Tiny => MatchBySizeRange(fileInfo, 1, 10, SizeType.KiloBytes),
			CommonSize.Small => MatchBySizeRange(fileInfo, 11, 100, SizeType.KiloBytes),
			CommonSize.Medium => MatchBySizeRange(fileInfo, 101, 1000, SizeType.KiloBytes),
			CommonSize.Large => MatchBySizeRange(fileInfo, 2, 16, SizeType.MegaBytes),
			CommonSize.Huge => MatchBySizeRange(fileInfo, 17, 128, SizeType.MegaBytes),
			_ => fileInfo.Length > SizeConverter(129, SizeType.MegaBytes, SizeConverterType.LowerBound) // CommonSize.Gigantic
		};
		private static bool MatchByCommonSizeEx(FileInfo fileInfo, CommonSize commonSize) => commonSize switch
		{
			CommonSize.Empty => fileInfo.Length == 0,
			CommonSize.Tiny => MatchBySizeRangeEx(fileInfo, 1, 10, SizeType.KiloBytes),
			CommonSize.Small => MatchBySizeRangeEx(fileInfo, 11, 100, SizeType.KiloBytes),
			CommonSize.Medium => MatchBySizeRangeEx(fileInfo, 101, 1000, SizeType.KiloBytes),
			CommonSize.Large => MatchBySizeRangeEx(fileInfo, 2, 16, SizeType.MegaBytes),
			CommonSize.Huge => MatchBySizeRangeEx(fileInfo, 17, 128, SizeType.MegaBytes),
			_ => fileInfo.Length > SizeConverterEx(129, SizeType.MegaBytes, SizeConverterType.LowerBound)  // CommonSize.Gigantic
		};
		#endregion

		#region DIRECTORY
		private static bool MatchDirByName(DirectoryInfo directoryInfo, string keyword, StringComparison stringComparison) => directoryInfo.Name.Equals(keyword, stringComparison);
		private static bool MatchDirByNameEx(DirectoryInfo directoryInfo, string keyword, StringComparison stringComparison) => directoryInfo.Name.Equals(keyword, stringComparison);

		private static bool MatchDirByDate(DirectoryInfo directoryInfo, DateTime date, DateComparisonType dateComparisonType)
		{
			var dirInfoDate = dateComparisonType switch
			{
				DateComparisonType.CreationDate => directoryInfo.CreationTime,
				DateComparisonType.LastAccessDate => directoryInfo.LastAccessTime,
				DateComparisonType.LastModificationDate => directoryInfo.LastWriteTime,
				_ => new DateTime(),
			};
			return DateTime.Equals(dirInfoDate.Date, date.Date);
		}
		private static bool MatchDirByDateEx(DirectoryInfo directoryInfo, DateTime date, DateComparisonType dateComparisonType)
		{
			var dirInfoDate = dateComparisonType switch
			{
				DateComparisonType.CreationDate => directoryInfo.CreationTime,
				DateComparisonType.LastAccessDate => directoryInfo.LastAccessTime,
				DateComparisonType.LastModificationDate => directoryInfo.LastWriteTime,
				_ => new DateTime(),
			};
			return DateTime.Equals(dirInfoDate.Date, date.Date);
		}

		private static bool MatchDirByAttributes(DirectoryInfo directoryInfo, FileAttributes fileAttributes = FileAttributes.Directory) => directoryInfo.Attributes == fileAttributes;
		private static bool MatchDirByAttributesEx(DirectoryInfo directoryInfo, FileAttributes fileAttributes = FileAttributes.Directory) => directoryInfo.Attributes == fileAttributes;

		private static bool MatchDirByPattern(DirectoryInfo directoryInfo, string pattern)
		{
			bool result;
			try { result = Regex.IsMatch(directoryInfo.Name, pattern, RegexOptions.Compiled); }
			catch { result = false; }
			return result;
		}
		private static bool MatchDirByPatternEx(DirectoryInfo directoryInfo, string pattern)
		{
			bool result;
			try { result = Regex.IsMatch(directoryInfo.Name, pattern, RegexOptions.Compiled); }
			catch { result = false; }
			return result;
		}
		#endregion

		#region FILE OPTIONS
		private static bool TranslateFileOptions(FileInfo fileInfo, SafeTraversalFileSearchOptions options)
		{
			var queueResult = new Queue<bool>();
			if (options.FileNameOption != null)
			{
				var stringComparison = options.FileNameOption.CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
				queueResult.Enqueue(options.FileNameOption.IncludeExtension ?
					MatchByNameWithExtension(fileInfo, options.FileNameOption.Name, stringComparison) :
					MatchByName(fileInfo, options.FileNameOption.Name, stringComparison));
			}
			if (!string.IsNullOrEmpty(options.Extension))
			{
				queueResult.Enqueue(MatchByExtension(fileInfo, options.Extension!)); // Not null here!
			}
			if (options.FileAttributes != 0)
			{
				queueResult.Enqueue(MatchByAttributes(fileInfo, options.FileAttributes));
			}
			if (options.CommonSize != 0)
			{
				queueResult.Enqueue(MatchByCommonSize(fileInfo, options.CommonSize));
			}
			if (options.SizeOption != null)
			{
				queueResult.Enqueue(MatchBySize(fileInfo, options.SizeOption.Size, options.SizeOption.SizeType));
			}
			if (options.SizeRangeOption != null)
			{
				queueResult.Enqueue(MatchBySizeRange(fileInfo, options.SizeRangeOption.LowerBoundSize, options.SizeRangeOption.UpperBoundSize, options.SizeRangeOption.SizeType));
			}
			if (options.DateOption != null)
			{
				queueResult.Enqueue(MatchByDate(fileInfo, options.DateOption.Date, options.DateOption.DateComparisonType));
			}
			if (options.DateRangeOption != null)
			{
				queueResult.Enqueue(MatchByDateRange(fileInfo, options.DateRangeOption.LowerBoundDate, options.DateRangeOption.UpperBoundDate, options.DateRangeOption.DateComparisonType));
			}
			if (options.RegularExpressionOption != null)
			{
				queueResult.Enqueue(options.RegularExpressionOption.IncludeExtension ?
					MatchByPatternWithExtension(fileInfo, options.RegularExpressionOption.Pattern) :
					MatchByPattern(fileInfo, options.RegularExpressionOption.Pattern));
			}

			if (queueResult.Count == 0) return false;

			var result = true;
			while (queueResult.Count > 0)
			{
				result = result && queueResult.Dequeue();
				if (!result) break;
			}
			return result;
		}
		private static bool TranslateFileOptionsEx(FileInfo fileInfo, SafeTraversalFileSearchOptions options)
		{
			var queueResult = new Queue<bool>();
			if (options.FileNameOption != null)
			{
				var stringComparison = options.FileNameOption.CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
				queueResult.Enqueue(options.FileNameOption.IncludeExtension ?
					MatchByNameWithExtensionEx(fileInfo, options.FileNameOption.Name, stringComparison) :
					MatchByNameEx(fileInfo, options.FileNameOption.Name, stringComparison));
			}
			if (!string.IsNullOrEmpty(options.Extension))
			{
				queueResult.Enqueue(MatchByExtensionEx(fileInfo, options.Extension!)); // Not null here!
			}
			if (options.FileAttributes != 0)
			{
				queueResult.Enqueue(MatchByAttributesEx(fileInfo, options.FileAttributes));
			}
			if (options.CommonSize != 0)
			{
				queueResult.Enqueue(MatchByCommonSizeEx(fileInfo, options.CommonSize));
			}
			if (options.SizeOption != null)
			{
				queueResult.Enqueue(MatchBySizeEx(fileInfo, options.SizeOption.Size, options.SizeOption.SizeType));
			}
			if (options.SizeRangeOption != null)
			{
				queueResult.Enqueue(MatchBySizeRangeEx(fileInfo, options.SizeRangeOption.LowerBoundSize, options.SizeRangeOption.UpperBoundSize, options.SizeRangeOption.SizeType));
			}
			if (options.DateOption != null)
			{
				queueResult.Enqueue(MatchByDateEx(fileInfo, options.DateOption.Date, options.DateOption.DateComparisonType));
			}
			if (options.DateRangeOption != null)
			{
				queueResult.Enqueue(MatchByDateRangeEx(fileInfo, options.DateRangeOption.LowerBoundDate, options.DateRangeOption.UpperBoundDate, options.DateRangeOption.DateComparisonType));
			}
			if (options.RegularExpressionOption != null)
			{
				queueResult.Enqueue(options.RegularExpressionOption.IncludeExtension ?
					MatchByPatternWithExtensionEx(fileInfo, options.RegularExpressionOption.Pattern) :
					MatchByPatternEx(fileInfo, options.RegularExpressionOption.Pattern));
			}

			if (queueResult.Count == 0) return false;

			var result = true;
			while (queueResult.Count > 0)
			{
				result = result && queueResult.Dequeue();
				if (!result) break;
			}
			return result;
		}

		#endregion

		#region DIR OPTIONS
		private static bool TranslateDirOptions(DirectoryInfo directoryInfo, SafeTraversalDirectorySearchOptions options)
		{
			var queueResult = new Queue<bool>();
			if (options.DirectoryNameOption != null)
			{
				var stringComparison = options.DirectoryNameOption.CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
				queueResult.Enqueue(MatchDirByName(directoryInfo, options.DirectoryNameOption.Name, stringComparison));
			}
			queueResult.Enqueue(MatchDirByAttributes(directoryInfo, options.DirectoryAttributes));
			if (options.DateOption != null)
				queueResult.Enqueue(MatchDirByDate(directoryInfo, options.DateOption.Date, options.DateOption.DateComparisonType));
			if (options.RegularExpressionOption != null)
				queueResult.Enqueue(MatchDirByPattern(directoryInfo, options.RegularExpressionOption.Pattern));

			if (queueResult.Count == 0) return false;

			var result = true;
			while (queueResult.Count > 0)
			{
				_ = queueResult.Dequeue(); // TODO: Dequeue here?
				result = result && queueResult.Dequeue();
				if (!result) break;
			}
			return result;
		}
		private static bool TranslateDirOptionsEx(DirectoryInfo directoryInfo, SafeTraversalDirectorySearchOptions options)
		{
			var queueResult = new Queue<bool>();
			if (options.DirectoryNameOption != null)
			{
				var stringComparison = options.DirectoryNameOption.CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
				queueResult.Enqueue(MatchDirByNameEx(directoryInfo, options.DirectoryNameOption.Name, stringComparison));
			}
			queueResult.Enqueue(MatchDirByAttributesEx(directoryInfo, options.DirectoryAttributes));
			if (options.DateOption != null)
				queueResult.Enqueue(MatchDirByDateEx(directoryInfo, options.DateOption.Date, options.DateOption.DateComparisonType));
			if (options.RegularExpressionOption != null)
				queueResult.Enqueue(MatchDirByPatternEx(directoryInfo, options.RegularExpressionOption.Pattern));
			if (queueResult.Count == 0) return false;

			var result = true;
			while (queueResult.Count > 0)
			{
				_ = queueResult.Dequeue(); // TODO: Dequeue here?
				result = result && queueResult.Dequeue();
				if (!result) break;
			}
			return result;
		}

		#endregion
	}
}
