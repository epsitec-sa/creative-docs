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
using Epsitec.Aider.BusinessCases;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (31)]
	public sealed class ActionAiderPersonWarningViewController31ProcessParishDeparture : ActionAiderPersonWarningViewControllerPassive
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
			var warning = this.Entity;
			var person  = warning.Person;
			var members = person.GetAllHouseholdMembers ();
			foreach (var member in members)
			{
				AiderPersonsProcess.StartProcess (this.BusinessContext, member, OfficeProcessType.PersonsParishChangeProcess);
			}
			this.ClearWarningAndRefreshCaches ();
			this.ClearWarningAndRefreshCachesForAll (WarningType.ParishDeparture);
		}
	}
}
