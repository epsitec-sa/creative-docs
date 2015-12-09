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
	[ControllerSubType (3)]
	public sealed class ActionAiderPersonWarningViewController3ProcessParishDeparture : ActionAiderPersonWarningViewControllerPassive
	{
		protected override void Execute()
		{
			var context = this.BusinessContext;
			var person  = this.Entity.Person;


			AiderPersonsProcess.StartExitProcess (context, person, OfficeProcessType.PersonsParishChangeProcess);
			

			this.ClearWarningAndRefreshCaches ();
		}

		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Traiter");
		}
	}
}
