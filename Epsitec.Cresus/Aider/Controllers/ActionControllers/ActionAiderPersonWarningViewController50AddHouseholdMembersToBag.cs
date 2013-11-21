//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Aider.Override;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (50)]
	public sealed class ActionAiderPersonWarningViewController50AddHouseholdMembersToBag : ActionViewController<AiderPersonWarningEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Ajouter les membres dans l'arche");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			var aiderUser = this.BusinessContext.GetLocalEntity (AiderUserManager.Current.AuthenticatedUser);

			foreach (var person in this.Entity.Person.MainContact.Household.Members)
			{
				var id = this.BusinessContext.DataContext.GetNormalizedEntityKey (person.MainContact).Value.ToString ().Replace ('/', '-');

				EntityBagManager.GetCurrentEntityBagManager ().AddToBag (
					aiderUser.LoginName,
					"Contact",
					person.MainContact.GetSummary (),
					id,
					When.Now
				);
			}
			
		}
	}
}
