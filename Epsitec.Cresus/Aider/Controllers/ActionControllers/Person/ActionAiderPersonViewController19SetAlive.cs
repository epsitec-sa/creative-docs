//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Override;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Library;
using Epsitec.Aider.Reporting;
using Epsitec.Aider.BusinessCases;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (19)]
	public sealed class ActionAiderPersonViewController19SetAlive : ActionViewController<AiderPersonEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Cette personne n'est pas décédée");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			var person = this.Entity;
			if (person.IsDeceased)
			{
				if (person.eCH_Person.IsNotNull ())
				{
					person.eCH_Person.PersonDateOfDeath = null;
					person.eCH_Person.RemovalReason = RemovalReason.None;
				}

				foreach (var contact in person.Contacts)
				{
					if (contact.ContactType == ContactType.Deceased)
					{
						contact.AddressType = AddressType.Default;
						contact.ContactType = ContactType.PersonHousehold;
					}
				}
			}
			this.BusinessContext.ClearRegisteredEntities ();
			this.BusinessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			
		}
	}
}
