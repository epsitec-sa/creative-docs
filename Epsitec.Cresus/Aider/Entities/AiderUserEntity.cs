//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;

using System.Linq;

namespace Epsitec.Aider.Entities
{
	public partial class AiderUserEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.DisplayName, "\n", this.LoginName, "\n", "Rôle: ", this.Role.Name);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.DisplayName);
		}

		public void AssignGroup(BusinessContext businessContext, UserPowerLevel powerLevel)
		{
			var group = AiderUserEntity.GetSoftwareUserGroup (businessContext, powerLevel);

			if (group != null)
			{
				this.UserGroups.Add (group);
			}
		}

		private static SoftwareUserGroupEntity GetSoftwareUserGroup(BusinessContext businessContext, UserPowerLevel powerLevel)
		{
			var example = new SoftwareUserGroupEntity ()
			{
				UserPowerLevel = powerLevel
			};

			var dataContext = businessContext.DataContext;

			return dataContext.GetByExample (example).FirstOrDefault ();
		}
	}
}
