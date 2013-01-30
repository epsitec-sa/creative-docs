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
						wall.AddBrick (x => x.Person);
					}
					if (contact.Household.IsNotNull ())
					{
						wall.AddBrick (x => x.Household);
					}
					break;

				case Enumerations.ContactType.PersonAddress:
					if (contact.Person.IsNotNull ())
					{
						wall.AddBrick (x => x.Person)
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
					if (contact.Person.IsNotNull ())
					{
						wall.AddBrick ().Include (x => x.Person);
					}
					if (contact.LegalPerson.IsNotNull ())
					{
						wall.AddBrick (x => x.LegalPerson);
					}
					break;
				
				default:
					break;
			}
		}
	}
}
