//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;

using System;
using System.Linq;
using System.Collections.Generic;

namespace Epsitec.Aider.Data.Common
{
	public static class AiderGroupIds
	{
		private const string Function				= "FNCT.";
		private const string Parish					= "P___.";
		private const string Region					= "R___.";
		private const string NoParish				= "NOPA.";
		private const string ParishOfGermanLanguage = "POGL.";

		private const string GroupFormat			= "{0}{1:000}.";
		private const string PlaceholderFormat		= "<{0}>.";

		public const string GlobalPrefix			= "D";
		public const string ParishPrefix			= "P";
		public const string RegionPrefix			= "R";
		public const string Suffix					= ".";

		private const int PartLength				= 5;
		private const int PrefixStart				= 0;
		private const int PrefixLength				= 1;
		private const int MiddleStart				= 1;
		private const int MiddleLength				= 3;
		private const int SuffixStart				= 4;
		private const int SuffixLength				= 1;
		
		private const string GroupPrefixCustom		= "C";
		private const string GroupPrefixDefinition	= "D";
		private const string GroupPrefixFunction	= "F";

		public const string SubgroupSqlWildcard	=	"____.";

		public const int MaxGroupNumber				= 1000;
		public const int MaxGroupLevel				= 6;

		public const int FunctionLevel				= 1;
		public const int ParishLevel				= 1;
		public const int RegionLevel				= 0;
		public const int NoParishLevel				= 0;
		public const int TopLevel					= 0;
		public const int ParishOfGermanLanguageLevel = 0;
		public const string ParishTemplatePath		= "R___.P___.";

		
		public static string DefaultToNoParish(string path)
		{
			if (string.IsNullOrEmpty (path))
			{
				return AiderGroupIds.NoParish;
			}
			else
			{
				return path;
			}
		}

		public static string GetRegionId(int regionCode)
		{
			return AiderGroupIds.CreateSubgroupPath ("", AiderGroupIds.RegionPrefix, regionCode);
		}

		public static string GetParishId(int parishCode)
		{
			return AiderGroupIds.CreateSubgroupPath ("", AiderGroupIds.ParishPrefix, parishCode);
		}

		public static bool IsRegion(string path)
		{
			if (path == null)
			{
				return false;
			}
			if ((path.Length == 5) &&
				(path.StartsWith (AiderGroupIds.RegionPrefix)) &&
				(path.EndsWith (AiderGroupIds.Suffix)))
			{
				return true;
			}

			return false;
		}

		public static bool IsParishOfGermanLanguage(string path)
		{
			if (path == null)
			{
				return false;
			}
			if ((path.Length == 5) &&
				(path.StartsWith (AiderGroupIds.ParishOfGermanLanguage)) &&
				(path.EndsWith (AiderGroupIds.Suffix)))
			{

				return true;
				
			}

			return false;
		}

		public static bool IsParish(string path)
		{
			if (path == null)
			{
				return false;
			}
			
			if ((path.Length == 10) &&
				(AiderGroupIds.IsRegion (path.Substring (0, 5)) || AiderGroupIds.IsParishOfGermanLanguage (path.Substring (0, 5)))
			)
			{
				path = path.Substring (5);

				if ((path.StartsWith (AiderGroupIds.ParishPrefix)) &&
					(path.EndsWith (AiderGroupIds.Suffix)))
				{
					return true;
				}
			}

			return false;
		}


		public static string ReplaceSubgroupWithWildcard(string path, int index)
		{
			var tokens = path.Split ('.');

			if ((index < 0) ||
				(index >= tokens.Length - 1))
			{
				throw new System.ArgumentOutOfRangeException ("index");
			}

			tokens[index] = new string (tokens[index].Select (x => char.IsDigit (x) ? '_' : x).ToArray ());

			return string.Join (".", tokens);
		}

		public static string GetShortName(string longName)
		{
			if (longName == null)
			{
				return "";
			}

			foreach (var item in AiderGroupIds.longToShort)
			{
				if (longName.StartsWith (item.LongName))
				{
					return item.ShortName + longName.Substring (item.LongName.Length);
				}
			}
			
			return longName;
		}

		
		
		public static bool IsWithinRegion(string path)
		{
			return AiderGroupIds.IsWithinGroup (path, AiderGroupIds.RegionLevel, AiderGroupIds.RegionPrefix);
		}

		public static bool IsWithinParish(string path)
		{
			return AiderGroupIds.IsWithinGroup (path, AiderGroupIds.ParishLevel, AiderGroupIds.ParishPrefix);
		}

		public static bool IsWithinNoParish(string path)
		{
			if (string.IsNullOrEmpty (path))
			{
				return false;
			}
			if (path.StartsWith (AiderGroupIds.NoParish))
			{
				return true;
			}

			return false;
		}

		public static bool IsWithinGlobal(string path)
		{
			return AiderGroupIds.IsWithinGroup (path, AiderGroupIds.TopLevel, AiderGroupIds.GlobalPrefix);
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

		public static bool IsSameOrWithinGroup(string path1, string path2)
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
				case GroupClassification.Function:
					return AiderGroupIds.Function;

				case GroupClassification.NoParish:
					return AiderGroupIds.NoParish;

				case GroupClassification.Region:
					return AiderGroupIds.Region;

				case GroupClassification.ParishOfGermanLanguage:
					return AiderGroupIds.ParishOfGermanLanguage;

				case GroupClassification.None:
					return null;

				case GroupClassification.Parish:
				default:
					throw new NotImplementedException ();
			}
		}

		public static string CreateTopLevelPathTemplate(int definitionNumber)
		{
			return AiderGroupIds.CreateDefinitionSubgroupPath ("", definitionNumber);
		}

		public static string CreateSubgroupPathFromFullPath(string parentPath, string fullChildPath)
		{
			var childPath = AiderGroupIds.GetGroupPathPart (fullChildPath);

			return AiderGroupIds.CreateSubgroupPath (parentPath, childPath);
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

		public static int? GetRegionNumber(string path)
		{
			if ((string.IsNullOrEmpty (path)) ||
				(path.StartsWith (AiderGroupIds.RegionPrefix) == false))
			{
				return null;
			}

			return int.Parse (path.Substring (AiderGroupIds.MiddleStart, AiderGroupIds.MiddleLength), System.Globalization.CultureInfo.InvariantCulture);
		}

		public static int GetGroupNumber(string path)
		{
			var part = AiderGroupIds.GetGroupPathPart (path);
			var number = part.Substring (AiderGroupIds.MiddleStart, AiderGroupIds.MiddleLength);

			return int.Parse (number, System.Globalization.CultureInfo.InvariantCulture);
		}

		public static int FindNextSubGroupDefNumber(IEnumerable<string> subGroupsPath,char prefixChar)
		{
			var subGroupsPathFiltered = subGroupsPath.Where (g => g.ElementAt (g.Length - AiderGroupIds.PartLength) == prefixChar);
			if (subGroupsPathFiltered.Count () == 0)
			{
				return 0;
			}

			var subgroupsNumber = subGroupsPathFiltered.Select (g => System.Convert.ToInt32 (g.Substring (g.Length - AiderGroupIds.SuffixStart).TrimEnd ('.')));
			return subgroupsNumber.Max () + 1;
		}

		private static string GetGroupPathPart(string path)
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
			if ((path == null) || (path.Length <= AiderGroupIds.PartLength))
			{
				return "";
			}
			else
			{
				return path.Substring (0, path.Length - AiderGroupIds.PartLength);
			}
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



		private class LongToShort
		{
			public LongToShort(string longName, string shortName)
			{
				this.LongName  = longName;
				this.ShortName = shortName;
			}

			public string LongName
			{
				get;
				private set;
			}

			public string ShortName
			{
				get;
				private set;
			}
		}

		private static readonly List<LongToShort> longToShort = new List<LongToShort>
		{
			new LongToShort ("Région ", "R"),
			new LongToShort ("Paroisse de ", ""),
			new LongToShort ("Paroisse de la ", ""),
			new LongToShort ("Paroisse du ", ""),
			new LongToShort ("Paroisse d'", ""),
			new LongToShort ("Paroisse des ", "Les "),
			new LongToShort ("Conseil paroissial", "CP"),
			new LongToShort ("Conseil synodal", "CS"),
			new LongToShort ("Conseil régional", "CR"),
			new LongToShort ("Conseil SC", "CSC "),
		};

	}
}
