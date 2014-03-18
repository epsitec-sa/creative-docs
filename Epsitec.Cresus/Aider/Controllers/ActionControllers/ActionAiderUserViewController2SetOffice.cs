//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP
using System.Linq;
using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using System.Collections.Generic;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (2)]
	public sealed class ActionAiderUserViewController2SetOffice : ActionViewController<AiderUserEntity>
	{

		public override FormattedText GetTitle()
		{
			return "Définir ou changer le secrétariat";
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderOfficeManagementEntity> (this.Execute);
		}

		private void Execute(AiderOfficeManagementEntity newOffice)
		{
			AiderOfficeManagementEntity.JoinOfficeManagement (this.BusinessContext, newOffice, this.Entity);
		}

		protected override void GetForm(ActionBrick<AiderUserEntity, SimpleBrick<AiderUserEntity>> form)
		{
			form
				.Title ("Définir le secrétariat")		
					.Field<AiderOfficeManagementEntity> ()
						.Title ("Choix du secrétariat")
						.InitialValue (this.Entity.Office)
					.End ()
				.End ();
		}
	}
}
