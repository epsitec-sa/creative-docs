//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using System.Collections.Generic;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (1)]
	public sealed class ActionAiderUserViewController1SetAdministrator : ActionViewController<AiderUserEntity>
	{

		public override FormattedText GetTitle()
		{
			return Res.Strings.AiderUserAdminSelectionTitle;
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<bool> (this.Execute);
		}

		private void Execute(bool shouldBeAdmin)
		{
			this.Entity.SetAdmininistrator (this.BusinessContext, shouldBeAdmin);
		}

		protected override void GetForm(ActionBrick<AiderUserEntity, SimpleBrick<AiderUserEntity>> form)
		{
			form
				.Title (Res.Strings.AiderUserAdminSelectionTitle)
				.Text (Res.Strings.AiderUserAdminSelectionText)
				.Field<bool> ()
					.Title (Res.Strings.AiderUserAdminTitle)
					.InitialValue (x => x.HasPowerLevel (UserPowerLevel.Administrator))
				.End ()
			.End ();
		}
	}
}
