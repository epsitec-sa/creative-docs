//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Enumerations;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Data.Platform;
using Epsitec.Aider.Data.ECh;
using Epsitec.Cresus.Core.Business.UserManagement;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (61)]
	public sealed class ActionAiderPersonWarningViewController61ProcessDepartureDeceased : ActionAiderPersonWarningViewControllerInteractive
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
			return Resources.FormattedText ("La personne est décédée");
		}
		
		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<Date> (this.Execute);
		}

		private void Execute(Date date)
		{
			var warning = this.Entity;
			var person  = warning.Person;

			var contacts   = person.Contacts.ToList ();
			var households = person.Households.ToList ();

			person.Visibility = PersonVisibilityStatus.Deceased;
			person.eCH_Person.PersonDateOfDeath = date;
			
			foreach (var contact in contacts)
			{
				AiderContactEntity.Delete (this.BusinessContext, contact);
			}

			this.DeleteEmptyHouseholds (households);
			this.ClearWarningAndRefreshCaches ();
		}


		protected override void GetForm(ActionBrick<AiderPersonWarningEntity, SimpleBrick<AiderPersonWarningEntity>> form)
		{
			form
				.Title ("Marquer la personne comme décédée")
				.Field<Date> ()
					.Title ("Date de décès")
					.InitialValue (Date.Today)
				.End ()
			.End ();           
		}
	}
}
