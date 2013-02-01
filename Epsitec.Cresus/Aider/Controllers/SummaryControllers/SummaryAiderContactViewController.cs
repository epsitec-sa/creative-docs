//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Aider.Entities;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Entities;
using System.Linq;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderContactViewController : SummaryViewController<AiderContactEntity>
	{
		protected override void CreateBricks(BrickWall<AiderContactEntity> wall)
		{
			var contact = this.Entity;

			if ((contact.Person.IsNull ()) &&
				(contact.LegalPerson.IsNull ()))
			{
				wall.AddBrick ()
					.EnableAction (0);
			}

			switch (this.Entity.ContactType)
			{
				case Enumerations.ContactType.PersonHousehold:
					if (contact.Person.IsNotNull ())
					{
						wall.AddBrick (x => x.Person)
							.Icon (contact.Person.GetIconName ("Data"))
							.Attribute (BrickMode.DefaultToSummarySubView);
					}
					if (contact.Household.IsNotNull ())
					{
						wall.AddBrick ()
							.Title (Resources.Text ("Ménage associé"))
							.Text (contact.Household.DisplayName)
							.Icon ("Data.AiderHousehold")
							.Attribute (BrickMode.SpecialController2);

						if (contact.Address.IsNotNull ())
						{
							wall.AddBrick ()
								.Title (Resources.Text ("Adresse du ménage"))
								.Text (contact.Address.GetSummary ())
								.Icon ("Data.AiderAddress")
								.Attribute (BrickMode.SpecialController1);
						}

						if (contact.Household.Members.Count > 1)
						{
							wall.AddBrick (x => x.Household.Members)
								.Title (Resources.Text ("Membres du ménage"))
								.Icon ("Data.AiderPersons")
								.Attribute (BrickMode.HideAddButton)
								.Attribute (BrickMode.HideRemoveButton)
								.Attribute (BrickMode.AutoGroup)
								.Template ()
								.End ()
								.Attribute (BrickMode.DefaultToSummarySubView);
						}
					}
					break;

				case Enumerations.ContactType.PersonAddress:
					if (contact.Person.IsNotNull ())
					{
						wall.AddBrick (x => x.Person)
							.Icon (contact.Person.GetIconName ("Data"))
							.Attribute (BrickMode.DefaultToSummarySubView);
					}
					if (contact.Address.IsNotNull ())
					{
						wall.AddBrick ()
							.Title (TextFormatter.FormatText (contact.AddressType))
							.Text (contact.Address.GetSummary ())
							.Icon ("Data.AiderAddress")
							.Attribute (BrickMode.SpecialController1);
					}
					break;

				case Enumerations.ContactType.Legal:
					//	TODO: ...
					break;
				
				default:
					break;
			}
		}
	}
}
