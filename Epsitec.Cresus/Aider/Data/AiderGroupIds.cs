//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

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


		public static string GetRegionId(int regionCode)
		{
			return string.Format ("R{0:00}.", regionCode);
		}

		public static string GetParishId(int parishCode)
		{
			return string.Format ("P{0:00}.", parishCode);
		}

		public static string ReplacePlaceholders(string path)
		{
			if (path.Contains ("<R>."))
			{
				var groupPath = AiderUserManager.Current.AuthenticatedUser.Person.Parish.Group.Path;
				path = path.Replace ("<R>.", groupPath.SubstringStart (AiderGroupIds.SubgroupLength));
			}
			
			if (path.Contains ("<P>."))
			{
				var groupPath = AiderUserManager.Current.AuthenticatedUser.Person.Parish.Group.Path;
				path = path.Replace ("<P>.", groupPath.Substring (AiderGroupIds.SubgroupLength, AiderGroupIds.SubgroupLength));
			}

			return path;
		}
	}
}
