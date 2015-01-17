//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (8)]
	public sealed class ActionAiderGroupViewController8PurgeGroupMembers : ActionViewController<AiderGroupEntity>
	{
		public override FormattedText GetTitle()
		{
			return "Purger les membres du groupe";
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			if (!this.Entity.CanBeEditedByCurrentUser ())
			{
				var message = "Vous n'avez pas le droit d'éditer ce groupe";

				throw new BusinessRuleException (message);
			}

			this.Entity.PurgeMembers (this.BusinessContext);
		}
	}
}
