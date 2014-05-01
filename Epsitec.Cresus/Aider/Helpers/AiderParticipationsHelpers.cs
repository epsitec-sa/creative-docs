using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Aider.Entities;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Aider.Helpers
{
	public sealed class AiderParticipationsHelpers
	{
		public static AiderParticipationRole GetParticipationRole(AiderGroupParticipantEntity participation)
		{
			switch (participation.Group.GroupLevel)
			{
				case 0 :
					return new AiderParticipationRole
					{
						Function	= "",
						Group		= participation.Group.Name,
						SuperGroup	= ""
					};
				case 1 :
					return new AiderParticipationRole
					{
						Function	= "Membre",
						Group		= participation.Group.Name,
						SuperGroup	= participation.Group.Parents.ElementAt (0).Name,
					};
				default :
					return new AiderParticipationRole
					{
						Function	= participation.Group.Name,
						Group		= participation.Group.Parent.Name,
						SuperGroup	= participation.Group.Parents.ElementAt (0).Name
					};

			}
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
		
	}
}
