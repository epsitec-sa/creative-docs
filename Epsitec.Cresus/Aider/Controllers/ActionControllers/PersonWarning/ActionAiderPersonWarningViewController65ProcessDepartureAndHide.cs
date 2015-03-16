//	Copyright © 2013-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (65)]
	public sealed class ActionAiderPersonWarningViewController65ProcessDepartureAndHide : ActionAiderPersonWarningViewControllerInteractive
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Confirmer la sortie");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			var warning = this.Entity;
			var person  = warning.Person;
			//TODO: participation removal
			person.HidePerson (this.BusinessContext);

			this.ClearWarningAndRefreshCaches ();
			this.ClearWarningAndRefreshCachesForAll (warning.WarningType);
		}
	}
}
