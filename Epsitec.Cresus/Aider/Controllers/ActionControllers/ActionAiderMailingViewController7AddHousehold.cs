//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (7)]
	public sealed class ActionAiderMailingViewController7AddHousehold : ActionViewController<AiderMailingEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Ajouter un ménage");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderHouseholdEntity> (this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderMailingEntity, SimpleBrick<AiderMailingEntity>> form)
		{
			form
				.Title ("Ajouter un ménage")
				.Field<AiderHouseholdEntity> ()
					.Title ("Ménage")
				.End ()
			.End ();
		}

		private void Execute(AiderHouseholdEntity household)
		{
			this.Entity.AddHousehold (this.BusinessContext, household);
		}
	}
}
