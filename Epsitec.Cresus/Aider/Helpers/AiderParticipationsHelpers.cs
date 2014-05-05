﻿using System;
using System.Collections.Generic;
using Epsitec.Common.Support.Extensions;
using System.Linq;
using System.Text;
using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Aider.Helpers
{
	public sealed class AiderParticipationsHelpers
	{
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
										participation.Group.GroupDef.FunctionNameInTheFeminine : 
										participation.Group.Name;
					group		= participation.Group.Parents.ElementAt (1).Name;
					sgroup		= participation.Group.Parents.ElementAt (0).Name;
					break;
				default:
					parish		= participation.Group.Parents.ElementAt (1).Name;
					function	= isFemale && participation.Group.GroupDef.IsFunction () ?
										participation.Group.GroupDef.FunctionNameInTheFeminine :
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

	public class AiderParticipationRole
	{
		public string Function
		{
			get;
			set;
		}

		public string Group
		{
			get;
			set;
		}

		public string SuperGroup
		{
			get;
			set;
		}

		public string Parish
		{
			get;
			set;
		}

		public string GetRole(AiderGroupParticipantEntity participation)
		{		
			var groupDef			= participation.Group.GroupDef;		
			var isGroupFonctional	= groupDef.Classification == Enumerations.GroupClassification.Function ? true : false;
			var isWithinParish		= AiderGroupIds.IsWithinParish (participation.Group.Path);
			var isWithinRegion		= AiderGroupIds.IsWithinRegion (participation.Group.Path);

			if (isWithinParish)
			{
				return this.Function + " " + this.Group + " de la " + this.Parish;
			}

			if (!isWithinParish && isWithinRegion)
			{
				return this.Function + " " + this.Group + " " + this.SuperGroup;
			}

			return this.Function + " " + this.Group;		
		}
		
	}
}
