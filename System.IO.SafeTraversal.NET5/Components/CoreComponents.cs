using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace System.IO.SafeTraversal.Core
{
	public partial class SafeTraversal
	{
		private enum SizeConverterType { LowerBound, UpperBound }

		private static long SizeConverter(double size, SizeType type, SizeConverterType converterType)
		{
			try
			{
				if (size == 0) size = +1;
				var result = converterType switch
				{
					SizeConverterType.LowerBound => Math.Floor((size - 1) * Math.Pow(1024, (int)type)),
					SizeConverterType.UpperBound => Math.Ceiling((size + 1) * Math.Pow(1024, (int)type)),
					_ => 0
				};
				return result >= long.MaxValue ? -1 : Convert.ToInt64(result);
			}
			catch { return -1; }
		}

		private static DateTime GetDateComparisonType(FileSystemInfo fileSystemInfo, DateComparisonType dateComparisonType) => dateComparisonType switch
		{
			DateComparisonType.CreationDate => fileSystemInfo.CreationTime.Date,
			DateComparisonType.LastModificationDate => fileSystemInfo.LastWriteTime.Date,
			DateComparisonType.LastAccessDate => fileSystemInfo.LastAccessTime.Date,
			_ => new DateTime()
		};

		#region MatchBy
		internal static bool MatchByAttributes(FileSystemInfo fileSystemInfo, FileAttributes fileAttributes) => fileSystemInfo.Attributes.HasFlag(fileAttributes); // fileSystemInfo.Attributes & fileAttributes == fileAttributes

		internal static bool MatchByCommonSize(FileInfo fileInfo, CommonSize commonSize) => commonSize switch
		{
			CommonSize.Empty => fileInfo.Length == 0,
			CommonSize.Tiny => MatchBySizeRange(fileInfo, 1, 10, SizeType.KiloBytes),
			CommonSize.Small => MatchBySizeRange(fileInfo, 11, 100, SizeType.KiloBytes),
			CommonSize.Medium => MatchBySizeRange(fileInfo, 101, 1000, SizeType.KiloBytes),
			CommonSize.Large => MatchBySizeRange(fileInfo, 2, 16, SizeType.MegaBytes),
			CommonSize.Huge => MatchBySizeRange(fileInfo, 17, 128, SizeType.MegaBytes),
			_ => fileInfo.Length > SizeConverter(129, SizeType.MegaBytes, SizeConverterType.LowerBound) // CommonSize.Gigantic
		};

		internal static bool MatchByDate(FileSystemInfo fileSystemInfo, DateTime dateTime, DateComparisonType dateComparisonType)
		{
			var fsiDate = GetDateComparisonType(fileSystemInfo, dateComparisonType);
			return DateTime.Equals(fsiDate.Date, dateTime.Date); // TODO: Precision?

			//switch (fileSystemInfo)
			//{
			//	case FileInfo fi:
			//		var fiDate = GetDateComparisonType(fi, dateComparisonType);
			//		return DateTime.Equals(fiDate.Date, dateTime.Date); // TODO: Precision?

			//	case DirectoryInfo di:
			//		var diDate = GetDateComparisonType(di, dateComparisonType);
			//		return DateTime.Equals(diDate.Date, dateTime.Date); // TODO: Precision?

			//	default: return false; 
			//};
		}

		internal static bool MatchByDateRange(FileInfo fileInfo, DateTime lowerBoundDate, DateTime upperBoundDate, DateComparisonType comparisonType)
		{
			var fileInfoDate = GetDateComparisonType(fileInfo, comparisonType);
			return fileInfoDate >= lowerBoundDate.Date && fileInfoDate <= upperBoundDate.Date;
		}

		private static bool MatchByExtension(FileInfo fileInfo, string extension)
		{
			extension = Regex.Match(extension, @"(\.)?\w+").Value;
			if (!extension.StartsWith(".")) extension = $".{extension}";
			return fileInfo.Extension.Equals(extension, StringComparison.InvariantCultureIgnoreCase);
		}

		internal static bool MatchByName(FileSystemInfo fileSystemInfo, string keyword, bool caseSensitive = false, bool withExtension = false)
		{
			var name = (fileSystemInfo is FileInfo && !withExtension) ? Path.GetFileNameWithoutExtension(fileSystemInfo.Name) : fileSystemInfo.Name;

			var stringComparison = caseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;

			return name.Equals(keyword, stringComparison);
		}

		//private static bool MatchByNameWithExtension(FileInfo fileInfo, string keyword, StringComparison stringComparison)
		//	=> fileInfo.Name.Equals(keyword, stringComparison);

		internal static bool MatchByPattern(FileSystemInfo fileSystemInfo, string pattern, bool withExtension = false)
		{
			var name = (fileSystemInfo is FileInfo && !withExtension) ? Path.GetFileNameWithoutExtension(fileSystemInfo.Name) : fileSystemInfo.Name;

			bool result;
			try { result = Regex.IsMatch(name, pattern/*, RegexOptions.Compiled*/); } // TODO: Compiled !?
			catch { result = false; }
			return result;
		}

		//internal static bool MatchByPattern(string fileName, string pattern, bool withExtension = false) // TODO: Check this (fileName?).
		//{
		//	var name = (!withExtension) ? Path.GetFileNameWithoutExtension(fileName) : fileName;

		//	bool result;
		//	try { result = Regex.IsMatch(name, pattern/*, RegexOptions.Compiled*/); } // TODO: Compiled !?
		//	catch { result = false; }
		//	return result;
		//}

		internal static bool MatchBySize(FileInfo fileInfo, double size, SizeType sizeType)
		{
			var lowerBound = SizeConverter(size, sizeType, SizeConverterType.LowerBound); if (lowerBound < 0) return false;
			var upperBound = SizeConverter(size, sizeType, SizeConverterType.UpperBound); if (upperBound < 0) return false;

			return fileInfo.Length >= lowerBound && fileInfo.Length <= upperBound;
		}

		internal static bool MatchBySizeRange(FileInfo fileInfo, double lowerBoundSize, double upperBoundSize, SizeType sizeType)
		{
			lowerBoundSize++; // TODO: Hmm...

			if (lowerBoundSize < 0 || upperBoundSize < 0 || lowerBoundSize >= upperBoundSize) return false;

			var lowerBound = SizeConverter(lowerBoundSize, sizeType, SizeConverterType.LowerBound); if (lowerBound < 0) return false;
			var upperBound = SizeConverter(upperBoundSize, sizeType, SizeConverterType.UpperBound); if (upperBound < 0) return false;

			return fileInfo.Length >= lowerBound && fileInfo.Length <= upperBound;
		}
		#endregion

		#region FILE OPTIONS
		internal static bool TranslateFileOptions(FileInfo fileInfo, SafeTraversalFileSearchOptions options)
		{
			var queueResult = new Queue<bool>();

			if (options.FileNameOption is not null) queueResult.Enqueue(MatchByName(fileInfo, options.FileNameOption.Name, options.FileNameOption.CaseSensitive, options.FileNameOption.IncludeExtension));
			if (!string.IsNullOrEmpty(options.Extension)) queueResult.Enqueue(MatchByExtension(fileInfo, options.Extension));
			if (options.CommonSize != 0) queueResult.Enqueue(MatchByCommonSize(fileInfo, options.CommonSize));
			if (options.SizeOption is not null) queueResult.Enqueue(MatchBySize(fileInfo, options.SizeOption.Size, options.SizeOption.SizeType));
			if (options.SizeRangeOption is not null) queueResult.Enqueue(MatchBySizeRange(fileInfo, options.SizeRangeOption.LowerBoundSize, options.SizeRangeOption.UpperBoundSize, options.SizeRangeOption.SizeType));
			if (options.DateOption is not null) queueResult.Enqueue(MatchByDate(fileInfo, options.DateOption.Date, options.DateOption.DateComparisonType));
			if (options.DateRangeOption is not null) queueResult.Enqueue(MatchByDateRange(fileInfo, options.DateRangeOption.LowerBoundDate, options.DateRangeOption.UpperBoundDate, options.DateRangeOption.DateComparisonType));
			if (options.FileAttributes != 0) queueResult.Enqueue(MatchByAttributes(fileInfo, options.FileAttributes));
			if (options.RegularExpressionOption is not null) MatchByPattern(fileInfo, options.RegularExpressionOption.Pattern, options.RegularExpressionOption.IncludeExtension);

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
		internal static bool TranslateDirOptions(DirectoryInfo directoryInfo, SafeTraversalDirectorySearchOptions options)
		{
			var queueResult = new Queue<bool>();

			if (options.DirectoryNameOption is not null) queueResult.Enqueue(MatchByName(directoryInfo, options.DirectoryNameOption.Name, options.DirectoryNameOption.CaseSensitive));
			if (options.DateOption is not null) queueResult.Enqueue(MatchByDate(directoryInfo, options.DateOption.Date, options.DateOption.DateComparisonType));
			queueResult.Enqueue(MatchByAttributes(directoryInfo, options.DirectoryAttributes)); // Init: FileAttributes.Directory
			if (options.RegularExpressionOption is not null) queueResult.Enqueue(MatchByPattern(directoryInfo, options.RegularExpressionOption.Pattern));

			if (queueResult.Count == 0) return false;

			var result = true;
			while (queueResult.Count > 0)
			{
				//_ = queueResult.Dequeue(); // TODO: Dequeue here ?? ** REMOVED **
				result = result && queueResult.Dequeue();
				if (!result) break;
			}
			return result;
		}
		#endregion
	}
}
