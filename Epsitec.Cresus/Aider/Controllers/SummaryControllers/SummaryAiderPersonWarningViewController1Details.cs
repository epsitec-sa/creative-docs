//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Controllers.EditionControllers;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Entities;

using System.Linq;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	[ControllerSubType (1)]
	public sealed class SummaryAiderPersonWarningViewController1Details : SummaryViewController<AiderPersonWarningEntity>
	{
		protected override void CreateBricks(BrickWall<AiderPersonWarningEntity> wall)
		{
			var warning   = this.Entity;
			var person    = warning.Person;
			var contact   = person.MainContact;
			var household = person.Households.FirstOrDefault ();

			if (contact.IsNotNull ())
			{
				var contactSummary = SummaryAiderContactViewController.GetPersonContactSummary (contact);

				wall.AddBrick (x => x.Person)
					.Icon (person.GetIconName ("Data"))
					.Text (contactSummary)
					.Attribute (BrickMode.DefaultToSummarySubView);
			}
			else
			{
				wall.AddBrick (x => x.Person)
					.Icon (person.GetIconName ("Data"))
					.Text ("Contact inexistant")
					.Attribute (BrickMode.DefaultToSummarySubView);
			}
			if (household.IsNotNull ())
			{
				if (contact.Address.IsNotNull () && contact.Address.GetEntityStatus () == EntityStatus.Valid)
				{
					wall.AddBrick (x => x.Person.MainContact)
						.Title (Resources.Text ("Adresse de domicile"))
						.Text (contact.Address.GetSummary ())
						.Icon ("Data.AiderAddress")
						.WithSpecialController (typeof (EditionAiderContactViewController1Address));
				}

				if (household.Members.Count > 1)
				{
					wall.AddBrick (x => x.Person.MainContact.Household.Members)
						.Title (Resources.Text ("Membres du ménage"))
						.Icon ("Data.AiderPersons")
						.Attribute (BrickMode.HideAddButton)
						.Attribute (BrickMode.HideRemoveButton)
						.Attribute (BrickMode.AutoGroup)
						.Template ()
							.Text (x => x.GetCompactSummary (household))
						.End ()
						.Attribute (BrickMode.DefaultToSummarySubView);
				}
			}
		}
	}
}

