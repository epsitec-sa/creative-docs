//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (21)]
	public sealed class ActionAiderPersonWarningViewController21ProcessParishArrival : ActionAiderPersonWarningViewControllerPassive
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Marquer comme lu (tout le ménage)");
		}

		protected override void Execute()
		{
			this.ClearWarningAndRefreshCaches ();

			var warning = this.Entity;
			var person  = warning.Person;
			var members = person.GetAllHouseholdMembers ();
			var warnings = members.SelectMany (x => x.Warnings.Where (w => w.WarningType == WarningType.ParishArrival)).ToList ();

			warnings.ForEach (x => this.ClearWarningAndRefreshCaches (x));
		}
	}
}

