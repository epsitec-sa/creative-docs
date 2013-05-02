//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using System.Collections.Generic;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (0)]
	public sealed class ActionAiderUserViewController0SetPassword : ActionViewController<AiderUserEntity>
	{

		public override FormattedText GetTitle()
		{
			return Res.Strings.AiderUserPasswordResetTitle;
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<string, string> (this.Execute);
		}

		private void Execute(string password, string confirmation)
		{
			this.Entity.SetPassword (password, confirmation);
		}

		protected override void GetForm(ActionBrick<AiderUserEntity, SimpleBrick<AiderUserEntity>> form)
		{
			form
				.Title (Res.Strings.AiderUserPasswordResetTitle)
				.Text (Res.Strings.AiderUserPasswordResetText)
				.Field<string> ()
					.Title (Res.Strings.AiderUserPasswordTitle)
					.Password ()
				.End ()
				.Field<string> ()
					.Title (Res.Strings.AiderUserPasswordConfirmationTitle)
					.Password ()
				.End ()
			.End ();
		}
	}
}
