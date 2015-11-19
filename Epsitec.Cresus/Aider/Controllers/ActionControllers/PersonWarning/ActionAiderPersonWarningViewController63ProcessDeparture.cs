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
using Epsitec.Aider.BusinessCases;

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
			return Resources.FormattedText ("Traiter<br/>pour tout le ménage");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			var warning = this.Entity;
			var person  = warning.Person;
			var members = person.GetAllHouseholdMembers ();
			foreach (var member in members)
			{
				AiderPersonsProcess.StartProcess (this.BusinessContext, member, OfficeProcessType.PersonsOutputProcess);
			}
			this.ClearWarningAndRefreshCaches ();
			this.ClearWarningAndRefreshCachesForAll (WarningType.ParishDeparture);
		}
	}
}