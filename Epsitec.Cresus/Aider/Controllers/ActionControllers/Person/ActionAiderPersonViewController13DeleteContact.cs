//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Override;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Library;
using Epsitec.Aider.Reporting;
using Epsitec.Aider.BusinessCases;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (13)]
	public sealed class ActionAiderPersonViewController13DeleteContact : ActionViewController<AiderPersonEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Supprimer le contact...");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<bool> (this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderPersonEntity, SimpleBrick<AiderPersonEntity>> form)
		{
			form
				.Title ("Supprimer le contact")
				.Field<bool> ()
					.Title ("Confirmer la suppresion ?")
					.InitialValue (false)
				.End ()
			.End ();
		}

		private void Execute(bool confirmed)
		{
			if (confirmed)
			{

				AiderPersonsProcess.StartExitProcess (this.BusinessContext, this.Entity, OfficeProcessType.PersonsOutputProcess);
				var mainContact = this.Entity.MainContact;

				if (mainContact.IsNotNull ())
				{
					var currentHousehold = mainContact.Household;

					AiderContactEntity.Delete (this.BusinessContext, mainContact, deleteParticipations: true);

					if (currentHousehold.IsNotNull ())
					{
						currentHousehold.RefreshCache ();
						
						var currentSubscription = AiderSubscriptionEntity.FindSubscription (this.BusinessContext, currentHousehold);

						if (currentSubscription.IsNotNull ())
						{
							currentSubscription.RefreshCache ();
						}
					}
				}
				else
				{
					mainContact = this.Entity.Contacts.FirstOrDefault ();

					if (mainContact.IsNotNull ())
					{
						AiderContactEntity.Delete (this.BusinessContext, mainContact, deleteParticipations: true);
					}
				}

				// hide person
				this.Entity.Visibility = PersonVisibilityStatus.Hidden;
				
				
			}
		}
	}
}
