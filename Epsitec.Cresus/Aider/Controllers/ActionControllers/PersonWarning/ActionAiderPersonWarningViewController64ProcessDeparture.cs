//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Override;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (64)]
	public sealed class ActionAiderPersonWarningViewController64ProcessDeparture : ActionAiderPersonWarningViewControllerInteractive
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Démarrer processus de sortie");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<string> (this.Execute);
		}

		private void Execute(string keys)
		{
			var currentUser	= AiderUserManager.Current.AuthenticatedUser;
			var warning     = this.Entity;
			var person      = warning.Person;

			keys = keys.Trim ();
			keys = keys.Replace ('.', ',');
			person.DeleteNumberedParticipationsNotInKeys (this.BusinessContext, this.GetParticipationsToCheck (), keys, currentUser.LoginName);
			BusinessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);

			var remainingParticipationsToCheck = this.GetRemainingParticipations ();
			var keepPerson                     = person.GetParticipations ().Any (p => p.IsSystemTaggedOkForWarning ());
			if (remainingParticipationsToCheck == 0)
			{
				this.ClearWarningAndRefreshCaches ();
				if (!keepPerson)
				{
					person.HidePerson (this.BusinessContext);
				}
			}
		}

		protected override void GetForm(ActionBrick<AiderPersonWarningEntity, SimpleBrick<AiderPersonWarningEntity>> form)
		{
			
			var person      = this.Entity.Person;
			var content     = this.Entity.Person.GetParticipationsNumberedSummary (this.BusinessContext, this.GetParticipationsToCheck ());
			form
				.Title (this.GetTitle ())
				.Text (content)
				.Field<string> ()
					.Title ("N° des participations à conserver ? Ex. (1,3,4)")
				.End ()
			.End ();
		}

		private IEnumerable<AiderGroupParticipantEntity> GetParticipationsToCheck ()
		{
			var currentUser	= AiderUserManager.Current.AuthenticatedUser;
			var person      = this.Entity.Person;
			var groupsUnderUserManagement = currentUser.GetGroupsUnderManagement (this.BusinessContext);
			return person.GetParticipations ().Where (p => groupsUnderUserManagement.Contains (p.Group));
		}

		private int GetRemainingParticipations ()
		{
			var person      = this.Entity.Person;
			return person.GetParticipations (reload : true).Where (p => !p.IsSystemTaggedOkForWarning ()).Count ();
		}
	}
}
