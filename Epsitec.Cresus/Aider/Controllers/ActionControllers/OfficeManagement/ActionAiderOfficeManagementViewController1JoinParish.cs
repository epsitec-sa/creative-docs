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
using Epsitec.Aider.Override;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (1)]
	public sealed class ActionAiderOfficeManagementViewController1JoinParish : ActionViewController<AiderOfficeManagementEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Rejoindre");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderUserEntity> (this.Execute);
		}

		private void Execute(AiderUserEntity user)
		{
			var currentOffice = user.Office;
			var currentSender = user.OfficeSender;
			var contact		  = user.Contact;

			if (contact.IsNull ())
			{
				var message = new NotificationMessage ()
				{
					Title     = "Attention AIDER",
					Body      = "Veuillez associer votre contact à votre profil. Cliquez sur ce message pour accéder à votre profil...",
					Dataset   = Res.CommandIds.Base.ShowAiderUser,
					EntityKey = this.BusinessContext.DataContext.GetNormalizedEntityKey (user).Value,
					HeaderErrorMessage = "Contact non défini",
					ErrorField        = LambdaUtils.Convert ((AiderUserEntity e) => e.Contact),
					ErrorFieldMessage = "votre contact"
				};
				
				var notifManager = NotificationManager.GetCurrentNotificationManager ();
				notifManager.WarnUser (user.LoginName, message, When.Now);
				throw new BusinessRuleException ("Vous n'avez pas associé votre contact à votre profil. Il est impossible de rejoindre l'entité " + this.Entity.OfficeName + ".");
			}

			AiderOfficeManagementEntity.JoinOfficeManagement (this.BusinessContext, this.Entity, user);		
		}

		protected override void GetForm(ActionBrick<AiderOfficeManagementEntity, SimpleBrick<AiderOfficeManagementEntity>> form)
		{
			var userManager		= AiderUserManager.Current;
			var aiderUser       = userManager.AuthenticatedUser;

			form
				.Title ("Rejoindre")
				.Field<AiderUserEntity> ().ReadOnly ()
					.Title ("Utilisateur AIDER")
					.InitialValue (this.BusinessContext.GetLocalEntity (aiderUser))
				.End ()
			.End ();
		}
	}
}
