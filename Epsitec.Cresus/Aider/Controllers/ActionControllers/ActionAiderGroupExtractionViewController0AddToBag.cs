//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
	[ControllerSubType (0)]
	public sealed class ActionAiderGroupExtractionViewController0AddToBag : ActionViewController<AiderGroupExtractionEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Ajouter � l'arche");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			var aiderUser = this.BusinessContext.GetLocalEntity (AiderUserManager.Current.AuthenticatedUser);
			var id = this.BusinessContext.DataContext.GetNormalizedEntityKey (this.Entity).Value.ToString ().Replace ('/', '-');

			EntityBagManager.GetCurrentEntityBagManager ().AddToBag (
				aiderUser.LoginName,
				"Groupe transversal",
				this.Entity.GetSummary (),
				id,
				When.Now
			);
		}
	}
}