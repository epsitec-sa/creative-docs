//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Helpers
{
	public sealed class AiderParticipationRole
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