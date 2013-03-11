//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Enumerations;

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

		private const string GroupPrefixCustom		= "C";
		private const string GroupPrefixDefinition	= "D";
		private const string GroupPrefixFunction	= "F";

		public const string SubgroupSqlWildcard	=	"___.";

		public const int SubgroupLength				= 4;
		public const int MinSubGroupNumber			= 1;
		public const int MaxSubGroupNumber			= 99;
		public const int MaxGroupLevel				= 6;


		public static string GetRegionId(int regionCode)
		{
			return string.Format ("R{0:00}.", regionCode);
		}

		public static string GetParishId(int parishCode)
		{
			return string.Format ("P{0:00}.", parishCode);
		}

		public static bool IsWithinRegion(string path)
		{
			return AiderGroupIds.IsWithinGroup (path, 0, 'R');
		}

		public static bool IsWithinParish(string path)
		{
			return AiderGroupIds.IsWithinGroup (path, 1, 'P');
		}

		private static bool IsWithinGroup(string path, int index, char prefix)
		{
			if (path == null)
			{
				return false;
			}

			var start = index * AiderGroupIds.SubgroupLength;

			if (path.Length < start + AiderGroupIds.SubgroupLength)
			{
				return false;
			}

			return prefix == path[start + 0]
				&& Char.IsDigit (path[start + 1])
				&& Char.IsDigit (path[start + 2])
				&& '.' == path[start + 3];
		}

		public static bool IsWithinSameRegion(string path1, string path2)
		{
			return AiderGroupIds.IsWithinSameGroup (path1, path2, 0, 'R');
		}

		public static bool IsWithinSameParish(string path1, string path2)
		{
			return AiderGroupIds.IsWithinSameGroup (path1, path2, 1, 'P');
		}

		private static bool IsWithinSameGroup(string path1, string path2, int index, char prefix)
		{
			var length = (index + 1) * AiderGroupIds.SubgroupLength;

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
			var childPath = string.Format ("{0}{1:00}.", prefix, groupNumber);

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
			var part = path.Substring (path.Length - 3, 2);

			return int.Parse (part);
		}

		public static IEnumerable<string> GetParentPaths(string path)
		{
			var step =  AiderGroupIds.SubgroupLength;

			for (int i = step; i <= path.Length - 1; i += step)
			{
				yield return path.Substring (0, i);
			}
		}

		public static string GetParentPath(string path)
		{
			if (path == null || path.Length <= AiderGroupIds.SubgroupLength)
			{
				return "";
			}

			return path.Substring (0, path.Length - AiderGroupIds.SubgroupLength);
		}

		public static string ReplacePlaceholders(string path, string parishPath)
		{
			path = AiderGroupIds.ReplacePlaceholder (path, "<R>.", parishPath, 0);
			path = AiderGroupIds.ReplacePlaceholder (path, "<P>.", parishPath, 1);

			return path;
		}

		private static string ReplacePlaceholder(string path, string placeholder, string parishPath, int positionInParishPath)
		{
			if (parishPath != null)
			{
				var startIndex = positionInParishPath * AiderGroupIds.SubgroupLength;
				var length = AiderGroupIds.SubgroupLength;
				var replacement = parishPath.Substring (startIndex, length);

				path = path.Replace (placeholder, replacement);
			}

			return path;
		}
	}
}
