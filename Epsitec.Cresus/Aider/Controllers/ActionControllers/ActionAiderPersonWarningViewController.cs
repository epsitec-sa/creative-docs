//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Controllers.ActionControllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	public abstract class ActionAiderPersonWarningViewController : ActionViewController<AiderPersonWarningEntity>
	{
		protected void ClearWarningAndRefreshCaches()
		{
			var warning = this.Entity;
			this.ClearWarningAndRefreshCaches (warning);
		}

		protected void ClearWarningAndRefreshCachesForAll(WarningType type)
		{
			var warning = this.Entity;
			var person  = warning.Person;
			var members = person.GetAllHouseholdMembers ();
			var warnings = members.SelectMany (x => x.Warnings.Where (w => w.WarningType == type)).ToList ();

			warnings.ForEach (x => this.ClearWarningAndRefreshCaches (x));
		}

		protected void ClearWarningAndRefreshCaches(AiderPersonWarningEntity warning)
		{
			var person  = warning.Person;
			var context = this.BusinessContext;

			person.Contacts.ForEach (x => x.RefreshCache ());
			person.Households.ForEach (x => x.RefreshCache ());

			person.RemoveWarningInternal (warning);
			context.DeleteEntity (warning);

			ActionAiderPersonWarningViewController.CleanUpEchPerson (person);
		}

		private static void CleanUpEchPerson(AiderPersonEntity person)
		{
			var reportedPersons = person.eCH_Person.ReportedPersons.ToArray ();

			foreach (var reportedPerson in reportedPersons)
			{
				reportedPerson.RemoveDuplicates ();
			}
		}
	}
}

