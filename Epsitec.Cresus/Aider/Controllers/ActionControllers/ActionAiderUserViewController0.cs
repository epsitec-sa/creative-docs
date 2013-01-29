using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using System.Collections.Generic;


namespace Epsitec.Aider.Controllers.ActionControllers
{


	[ControllerSubType (0)]
	public sealed class ActionAiderUserViewController0 : ActionViewController<AiderUserEntity>
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
			var user = this.Entity;

			if (password == null)
			{
				var message = Res.Strings.AiderUserPasswordEmpty.ToString ();

				throw new BusinessRuleException (user, message);
			}

			if (password.Length < 8)
			{
				var message = Res.Strings.AiderUserPasswordTooShort.ToString ();

				throw new BusinessRuleException (user, message);
			}

			if (password != confirmation)
			{
				var message = Res.Strings.AiderUserPasswordMismatch.ToString ();

				throw new BusinessRuleException (user, message);
			}

			user.SetPassword (password);
		}


		protected override void GetForm(ActionBrick<AiderUserEntity, SimpleBrick<AiderUserEntity>> form)
		{
			form
				.Title (Res.Strings.AiderUserPasswordResetTitle)
				.Text (Res.Strings.AiderUserPasswordResetText)
				.Field<string> ()
					.Title (Res.Strings.AiderUserPasswordTitle)
					.Password()
				.End ()
				.Field<string> ()
					.Title (Res.Strings.AiderUserPasswordConfirmationTitle)
					.Password()
				.End ()
			.End ();
		}
	}


}
