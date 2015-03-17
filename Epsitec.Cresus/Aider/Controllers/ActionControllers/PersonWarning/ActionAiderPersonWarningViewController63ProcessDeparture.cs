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
	[ControllerSubType (63)]
	public sealed class ActionAiderPersonWarningViewController63ProcessDeparture : ActionAiderPersonWarningViewControllerInteractive
	{
		public override bool IsEnabled
		{
			get
			{
				var warning = this.Entity;
				var person  = warning.Person;
				var members = person.GetAllHouseholdMembers ();

				return members.Skip (1).Any ();
			}
		}

		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Tout le ménage a déménagé hors canton");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<bool, bool> (this.Execute);
		}

		private void Execute(bool confirmAddress, bool hideHousehold)
		{
			var warning    = this.Entity;
			var person     = warning.Person;
			var households = new HashSet<AiderHouseholdEntity> (person.Households);
			var members    = households.SelectMany (x => x.Members).Distinct ().ToList ();

			if (confirmAddress == hideHousehold)
			{
				var message = "Il faut choisir l'une des deux options...";

				throw new BusinessRuleException (message);
			}
			
			if (hideHousehold)
			{
				foreach (var member in members)
				{
					member.HidePerson (this.BusinessContext);
					member.DeleteParishGroupParticipation (this.BusinessContext);
					member.DeleteNonParishGroupParticipations (this.BusinessContext);
				}
			}
			else
			{
				foreach (var member in members)
				{
					member.eCH_Person.RemovalReason = RemovalReason.Departed;
				}
			}

			this.ClearWarningAndRefreshCaches ();
			this.ClearWarningAndRefreshCachesForAll (this.Entity.WarningType);
		}

		protected override void GetForm(ActionBrick<AiderPersonWarningEntity, SimpleBrick<AiderPersonWarningEntity>> form)
		{
			form
				.Title (this.GetTitle ())
				.Field<bool> ()
					.Title ("J'ai saisi la nouvelle adresse du ménage")
					.InitialValue (false)
				.End ()
				.Field<bool> ()
					.Title ("Cacher tous les membres du ménage")
					.InitialValue (false)
				.End ()
			.End ();
		}
	}
}