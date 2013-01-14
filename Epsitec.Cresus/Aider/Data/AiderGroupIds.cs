//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Override;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;

namespace Epsitec.Aider.Data
{
	public static class AiderGroupIds
	{
		public const string Canton              = "SCC.";
		public const string Common              = "MIC.";
		public const string External            = "REX.";
		public const string Function            = "FNC.";
		public const string Parish              = "P__.";
		public const string Region              = "R__.";
		public const string Staff               = "PRS.";
		public const string StaffAssociation	= "ASP.";

		public const string GroupPrefix			= "G";
		public const string FunctionPrefix		= "F";

		public const string SubgroupSqlWildcard = "___.";

		public const int SubgroupLength         = 4;
		public const int MinSubGroupNumber		= 1;
		public const int MaxSubGroupNumber		= 99;


		public static string GetRegionId(int regionCode)
		{
			return string.Format ("R{0:00}.", regionCode);
		}

		public static string GetParishId(int parishCode)
		{
			return string.Format ("P{0:00}.", parishCode);
		}

		public static string CreateSubGroupPath(string superGroupPath, int groupNumber)
		{
			return superGroupPath + string.Format ("{0}{1:00}.", AiderGroupIds.GroupPrefix, groupNumber);
		}

		public static int GetGroupNumber(string path)
		{
			var part = path.Substring (path.Length - 3, 2);

			return int.Parse (part);
		}

		public static string ReplacePlaceholders(string path)
		{
			var parishPath = AiderGroupIds.GetParishPath ();

			path = AiderGroupIds.ReplacePlaceholder (path, "<R>.", parishPath, 0);
			path = AiderGroupIds.ReplacePlaceholder (path, "<P>.", parishPath, 1);

			return path;
		}

		public static IEnumerable<string> GetGroupChainPaths(string path)
		{
			var step =  AiderGroupIds.SubgroupLength;

			for (int i = step; i <= path.Length; i += step)
			{
				yield return path.Substring (0, i);
			}
		}

		private static string GetParishPath()
		{
			return AiderUserManager.Current.AuthenticatedUser
				.GetValueOrDefault (x => x.Person)
				.GetValueOrDefault (x => x.Parish)
				.GetValueOrDefault (x => x.Group)
				.GetValueOrDefault (x => x.Path);
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
