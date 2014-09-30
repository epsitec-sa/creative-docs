//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Override;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (19)]
	public sealed class ActionAiderMailingViewController19RemoveCurrentQuery : ActionViewController<AiderMailingEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Supprimer le filtre");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}


		private void Execute()
		{
			this.Entity.RecipientQuery = null;
			this.Entity.RecipientQueryName = null;
			this.Entity.UpdateMailingParticipants (this.BusinessContext);
		}
	}
}