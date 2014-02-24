//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
			return Resources.Text ("Ajouter un réglage");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderContactEntity, AiderContactEntity, AiderContactEntity> (this.Execute);
		}

		private void Execute(AiderContactEntity officialContact, AiderContactEntity officeAddress, AiderContactEntity ppFrankingAddress)
		{
			AiderOfficeManagementEntity.CreateSettings (this.BusinessContext, this.Entity, officialContact, officeAddress, ppFrankingAddress);
		}

		protected override void GetForm(ActionBrick<AiderOfficeManagementEntity, SimpleBrick<AiderOfficeManagementEntity>> form)
		{
			form
				.Title ("Réglages")
				.Field<AiderContactEntity> ()
					.Title ("Contact officiel")
				.End ()
				.Field<AiderContactEntity> ()
					.Title ("Adresse du secrétariat")
				.End ()
				.Field<AiderContactEntity> ()
					.Title ("Adresse envoi P.P")
				.End ()
			.End ();
		}
	}
}
