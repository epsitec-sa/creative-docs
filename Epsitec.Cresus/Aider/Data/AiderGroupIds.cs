//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;

using System;

using System.Collections.Generic;

namespace Epsitec.Aider.Data
{
	public static class AiderGroupIds
	{
		private const string Canton					= "SCC.";
		private const string Common					= "MIC.";
		private const string External				= "EXT.";
		private const string Function				= "FNC.";
		private const string Parish					= "P__.";
		private const string Region					= "R__.";
		private const string Staff					= "PRS.";
		private const string StaffAssociation		= "ASP.";
		private const string NoParish				= "NOP.";

		private const string GroupFormat			= "{0}{1:00}.";
		private const string PlaceholderFormat		= "<{0}>.";

		private const string ParishPrefix			= "P";
		private const string RegionPrefix			= "R";
		private const string Suffix					= ".";

		private const int PartLength				= 4;
		private const int PrefixStart				= 0;
		private const int PrefixLength				= 1;
		private const int MiddleStart				= 1;
		private const int MiddleLength				= 2;
		private const int SuffixStart				= 3;
		private const int SuffixLength				= 1;
		
		private const string GroupPrefixCustom		= "C";
		private const string GroupPrefixDefinition	= "D";
		private const string GroupPrefixFunction	= "F";

		public const string SubgroupSqlWildcard	=	"___.";

		public const int MaxGroupNumber				= 100;
		public const int MaxGroupLevel				= 6;

		public const int ParishLevel				= 1;
		public const int RegionLevel				= 0;
		public const int NoParishLevel				= 0;
		public const int TopLevel					= 0;


		public static string GetRegionId(int regionCode)
		{
			return AiderGroupIds.CreateSubgroupPath ("", AiderGroupIds.RegionPrefix, regionCode);
		}

		public static string GetParishId(int parishCode)
		{
			return AiderGroupIds.CreateSubgroupPath ("", AiderGroupIds.ParishPrefix, parishCode);
		}

		public static bool IsWithinRegion(string path)
		{
			return AiderGroupIds.IsWithinGroup (path, AiderGroupIds.RegionLevel, AiderGroupIds.RegionPrefix);
		}

		public static bool IsWithinParish(string path)
		{
			return AiderGroupIds.IsWithinGroup (path, AiderGroupIds.ParishLevel, AiderGroupIds.ParishPrefix);
		}

		private static bool IsWithinGroup(string path, int index, string prefix)
		{
			if (path == null)
			{
				return false;
			}

			var start = index * AiderGroupIds.PartLength;

			if (path.Length < start + AiderGroupIds.PartLength)
			{
				return false;
			}

			var p = path.Substring (start + AiderGroupIds.PrefixStart, AiderGroupIds.PrefixLength);
			var m = path.Substring (start + AiderGroupIds.MiddleStart, AiderGroupIds.MiddleLength);
			var s = path.Substring (start + AiderGroupIds.SuffixStart, AiderGroupIds.SuffixLength);

			return p == prefix
				&& m.IsNumeric ()
				&& s == AiderGroupIds.Suffix;
		}

		public static bool IsWithinSameRegion(string path1, string path2)
		{
			return AiderGroupIds.IsWithinSameGroup (path1, path2, AiderGroupIds.RegionLevel, AiderGroupIds.RegionPrefix);
		}

		public static bool IsWithinSameParish(string path1, string path2)
		{
			return AiderGroupIds.IsWithinSameGroup (path1, path2, AiderGroupIds.ParishLevel, AiderGroupIds.ParishPrefix);
		}

		private static bool IsWithinSameGroup(string path1, string path2, int index, string prefix)
		{
			var length = (index + 1) * AiderGroupIds.PartLength;

			return AiderGroupIds.IsWithinGroup (path1, index, prefix)
				&& AiderGroupIds.IsWithinGroup (path2, index, prefix)
				&& path1.Substring (0, length) == path2.Substring (0, length);
		}

		public static bool IsWithinGroup(string path1, string path2)
		{
			if (string.IsNullOrEmpty (path1) || string.IsNullOrEmpty (path2))
			{
				return false;
			}

			return path1.Contains (path2);
		}

		public static string CreateTopLevelPathTemplate(GroupClassification classification)
		{
			switch (classification)
			{
				case GroupClassification.Canton:
					return AiderGroupIds.Canton;

				case GroupClassification.Common:
					return AiderGroupIds.Common;

				case GroupClassification.External:
					return AiderGroupIds.External;

				case GroupClassification.Function:
					return AiderGroupIds.Function;

				case GroupClassification.NoParish:
					return AiderGroupIds.NoParish;

				case GroupClassification.Region:
					return AiderGroupIds.Region;

				case GroupClassification.Staff:
					return AiderGroupIds.Staff;

				case GroupClassification.StaffAssociation:
					return AiderGroupIds.StaffAssociation;

				case GroupClassification.Parish:
				case GroupClassification.None:
				default:
					throw new NotImplementedException ();
			}
		}

		public static string CreateCustomSubgroupPath(string parentPath, int groupNumber)
		{
			var prefix = AiderGroupIds.GroupPrefixCustom;

			return AiderGroupIds.CreateSubgroupPath (parentPath, prefix, groupNumber);
		}

		public static string CreateDefinitionSubgroupPath(string parentPath, int groupNumber)
		{
			var prefix = AiderGroupIds.GroupPrefixDefinition;

			return AiderGroupIds.CreateSubgroupPath (parentPath, prefix, groupNumber);
		}

		public static string CreateFunctionSubgroupPath(string parentPath, int groupNumber)
		{
			var prefix = AiderGroupIds.GroupPrefixFunction;

			return AiderGroupIds.CreateSubgroupPath (parentPath, prefix, groupNumber);
		}

		private static string CreateSubgroupPath(string parentPath, string prefix, int groupNumber)
		{
			var childPath = string.Format (AiderGroupIds.GroupFormat, prefix, groupNumber);

			return AiderGroupIds.CreateSubgroupPath (parentPath, childPath);
		}

		public static string CreateParishSubgroupPath(string parentPath)
		{
			return AiderGroupIds.CreateSubgroupPath (parentPath, AiderGroupIds.Parish);
		}

		private static string CreateSubgroupPath(string parentPath, string childPath)
		{
			return parentPath + childPath;
		}

		public static int GetGroupNumber(string path)
		{
			var part = AiderGroupIds.GetGroupPathPart (path);
			var number = part.Substring (AiderGroupIds.MiddleStart, AiderGroupIds.MiddleLength);

			return int.Parse (number);
		}

		public static string GetGroupPathPart(string path)
		{
			return path.Substring (path.Length - AiderGroupIds.PartLength);
		}

		public static IEnumerable<string> GetParentPaths(string path)
		{
			var step =  AiderGroupIds.PartLength;

			for (int i = step; i <= path.Length - 1; i += step)
			{
				yield return path.Substring (0, i);
			}
		}

		public static string GetParentPath(string path)
		{
			if (path == null || path.Length <= AiderGroupIds.PartLength)
			{
				return "";
			}

			return path.Substring (0, path.Length - AiderGroupIds.PartLength);
		}

		public static string ReplacePlaceholders(string path, string parishPath)
		{
			path = AiderGroupIds.ReplacePlaceholder (path, AiderGroupIds.RegionPrefix, parishPath, AiderGroupIds.RegionLevel);
			path = AiderGroupIds.ReplacePlaceholder (path, AiderGroupIds.ParishPrefix, parishPath, AiderGroupIds.ParishLevel);

			return path;
		}

		private static string ReplacePlaceholder(string path, string prefix, string parishPath, int positionInParishPath)
		{
			if (parishPath != null)
			{
				var placeholder = string.Format (AiderGroupIds.PlaceholderFormat, prefix);

				var startIndex = positionInParishPath * AiderGroupIds.PartLength;
				var length = AiderGroupIds.PartLength;
				var replacement = parishPath.Substring (startIndex, length);

				path = path.Replace (placeholder, replacement);
			}

			return path;
		}
	}
}
