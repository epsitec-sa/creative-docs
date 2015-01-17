//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (0)]
	public sealed class ActionAiderOfficeManagementViewController0CreateSettings : ActionViewController<AiderOfficeManagementEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Ajouter un expéditeur");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderContactEntity> (this.Execute);
		}

		private void Execute(AiderContactEntity officialContact)
		{
			AiderOfficeSenderEntity.Create (this.BusinessContext, this.Entity, officialContact);
		}

		protected override void GetForm(ActionBrick<AiderOfficeManagementEntity, SimpleBrick<AiderOfficeManagementEntity>> form)
		{
			form
				.Title ("Expéditeur")
				.Field<AiderContactEntity> ()
					.Title ("Contact de l'expéditeur")
				.End ()
			.End ();
		}
	}
}
