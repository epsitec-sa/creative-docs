//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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

namespace Epsitec.Aider.Controllers.ActionControllers
{
	public abstract class ActionAiderPersonWarningViewController : ActionViewController<AiderPersonWarningEntity>
	{
		protected void ClearWarning()
		{
			var warning = this.Entity;
			var person  = warning.Person;
			var context = this.BusinessContext;

			person.RemoveWarningInternal (warning);
			context.DeleteEntity (warning);

			ActionAiderPersonWarningViewController.CleanUpEchPerson (context, person);
		}

		private static void CleanUpEchPerson(BusinessContext context, AiderPersonEntity person)
		{
			var reportedPersons = person.eCH_Person.ReportedPersons.ToArray ();

			foreach (var reportedPerson in reportedPersons)
			{
				reportedPerson.RemoveDuplicates (context);
			}
		}
	}
}

