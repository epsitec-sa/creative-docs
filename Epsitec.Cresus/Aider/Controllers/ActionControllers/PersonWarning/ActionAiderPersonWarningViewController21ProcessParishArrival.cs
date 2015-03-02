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
			return Resources.FormattedText ("Marquer comme lu<br/>pour tout le ménage");
		}

		protected override void Execute()
		{
			this.ClearWarningAndBackupPersonInSubParishGroup (GroupClassification.LastArrived);
			this.ClearWarningAndRefreshCachesForAll (WarningType.ParishArrival);
		}
	}
}

