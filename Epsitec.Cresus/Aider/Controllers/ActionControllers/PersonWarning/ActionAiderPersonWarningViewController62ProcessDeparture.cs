//	Copyright © 2013-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (6)]
	public sealed class ActionAiderPersonWarningViewController62ProcessDeparture : ActionAiderPersonWarningViewControllerInteractive
	{
		public override bool IsEnabled
		{
			get
			{
				return true;
			}
		}

		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("La personne a déménagé hors canton");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<bool, bool> (this.Execute);
		}

		private void Execute(bool confirmAddress, bool hidePerson)
		{
			var warning = this.Entity;
			var person  = warning.Person;

			if (confirmAddress == hidePerson)
			{
				var message = "Il faut choisir l'une des deux options...";

				throw new BusinessRuleException (message);
			}

			//TODO: participation removal
			if (hidePerson)
			{
				person.HidePerson (this.BusinessContext);
				person.DeleteNonParishGroupParticipations (this.BusinessContext);
				person.DeleteParishGroupParticipation (this.BusinessContext);
			}
			else
			{
				person.eCH_Person.RemovalReason = RemovalReason.Departed;
			}

			this.ClearWarningAndRefreshCaches ();
		}

		protected override void GetForm(ActionBrick<AiderPersonWarningEntity, SimpleBrick<AiderPersonWarningEntity>> form)
		{
			form
				.Title (this.GetTitle ())
				.Text (this.Entity.Person.GetNonParishGroupParticipationsNumberedSummary (this.BusinessContext))
				.Field<bool> ()
					.Title ("J'ai traité manuellement le déménagement")
					.InitialValue (false)
				.End ()
				.Field<bool> ()
					.Title ("Cacher la personne")
					.InitialValue (false)
				.End ()
			.End ();
		}
	}
}
