//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Aider.Helpers
{
	public sealed class AiderParticipationsHelpers
	{
		public static void PurgeAndRebuildRoleCache (BusinessContext context)
		{
			//TODO TRIGG JOB
		}

		public static void RebuildRoleCache(IEnumerable<AiderGroupParticipantEntity> participations)
		{
			foreach (var participant in participations)
			{
				participant.RoleCache = AiderParticipationsHelpers.BuildRoleFromParticipation (participant).GetRole (participant);
			}
		}

		public static string GetRolePath(AiderGroupParticipantEntity participation)
		{
			var path = participation.Group.Parents.Select (p => p.Name).ToList ();
			path.Add(participation.Group.Name);
			return path.JoinNonEmpty (" / ");
		}

		public static AiderParticipationRole BuildRoleFromParticipation(AiderGroupParticipantEntity participation)
		{
			var level				= participation.Group.GroupLevel;
			var isWithinParish		= AiderGroupIds.IsWithinParish (participation.Group.Path);
			var person				= participation.Contact.Person;
			var isFemale			= person.eCH_Person.PersonSex == Enumerations.PersonSex.Female ? true : false;

			var function	= "";
			var group		= "";
			var sgroup		= "";
			var parish		= "";

			switch (level)
			{
				case 0:
					group	= participation.Group.Name;
					break;
				case 1:
					function	= "Membre";
					parish		= participation.Group.Name;
					sgroup		= participation.Group.Parents.ElementAt (0).Name;
					break;
				case 2:
					parish		= participation.Group.Parents.ElementAt (1).Name;
					function	= isFemale && participation.Group.GroupDef.IsFunction () ? 
										participation.Group.GroupDef.NameFeminine : 
										participation.Group.Name;
					group		= participation.Group.Parents.ElementAt (1).Name;
					sgroup		= participation.Group.Parents.ElementAt (0).Name;
					break;
				default:
					parish		= participation.Group.Parents.ElementAt (1).Name;
					function	= isFemale && participation.Group.GroupDef.IsFunction () ?
										participation.Group.GroupDef.NameFeminine :
										participation.Group.Name;
					group		= participation.Group.Parents.Skip (1).Reverse ().First ().Name == "Staff" ? 
									participation.Group.Parents.Skip (1).Reverse ().Skip(1).First ().Name :
									participation.Group.Parents.Skip (1).Reverse ().First ().Name;
					sgroup		= participation.Group.Parents.ElementAt (0).Name;
					break;
			}

			return new AiderParticipationRole
			{
				Function	= function,
				Group		= group,
				SuperGroup	= sgroup,
				Parish		= parish
			};
		}
	}
}