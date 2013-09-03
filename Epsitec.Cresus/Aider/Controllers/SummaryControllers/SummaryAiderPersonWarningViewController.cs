//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Controllers.EditionControllers;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Common.Support;
using System.Linq;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Entities;



namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderPersonWarningViewController : SummaryViewController<AiderPersonWarningEntity>
	{
		protected override void CreateBricks(BrickWall<AiderPersonWarningEntity> wall)
		{


			var displayableHousehold = this.Entity.Person.Contacts.Where (c => c.Household.Address.IsNotNull ()).Count ();

			switch (this.Entity.WarningType)
			{
				case WarningType.EChPersonMissing:
					if (this.Entity.Person.Contacts.Count > 0)
					{
						wall.AddBrick (x => x.Person.Contacts.Where (c => c.Household.Address.IsNotNull ()).First ())
							.Title ("Nouvelle adresse (si connue)")
							.Icon ("Data.AiderAddress")
							.Text ("(merci de saisir une adresse hors du canton)")
							.WithSpecialController (typeof (EditionAiderContactViewController1Address));
					}

					wall.AddBrick (x => x.Person)
							.Title ("En cas de décès (entrer une date)")
							.Icon (this.Entity.Person.GetIconName("Data"))
							.WithSpecialController (typeof (EditionAiderPersonViewController0DeceaseDateController));

					this.AddDefaultBrick (wall)
						.EnableAction<ActionAiderPersonWarningViewController1ProcessPersonMissing> ();
					break;
				case WarningType.EChPersonNew:
                    this.AddDefaultBrick(wall)
                        .EnableAction<ActionAiderPersonWarningViewController8ProcessNewPerson> ();
                    break;
				case WarningType.EChProcessDeparture:
                    if (this.Entity.Person.Contacts.Count > 0)
                    {
                        wall.AddBrick(x => x.Person.Contacts.Where (c => c.Household.Address.IsNotNull ()).First ())
                            .Title("Nouvelle adresse (si connue)")
                            .Icon("Data.AiderAddress")
                            .Text("(merci de saisir une adresse hors du canton)")
                            .WithSpecialController(typeof(EditionAiderContactViewController1Address));
                    }
					wall.AddBrick (x => x.Person)
							.Title ("En cas de décès (entrer une date)")
							.Icon (this.Entity.Person.GetIconName ("Data"))
							.WithSpecialController (typeof (EditionAiderPersonViewController0DeceaseDateController));

					this.AddDefaultBrick (wall)
						.EnableAction<ActionAiderPersonWarningViewController6ProcessDeparture> ();
					break;
				
				case WarningType.EChProcessArrival:
					this.AddDefaultBrick (wall)
						.EnableAction<ActionAiderPersonWarningViewController7ProcessArrival> ();
					break;
				
				case WarningType.EChPersonDataChanged:
					this.AddDefaultBrick (wall)
						.EnableAction<ActionAiderPersonWarningViewController0DiscardWarning> ();
					break;

				case WarningType.EChHouseholdAdded:
                    this.AddDefaultBrick(wall)
                        .EnableAction<ActionAiderPersonWarningViewController4ProcessNewHousehold> ();
                    break;

                case WarningType.EChHouseholdChanged:
                    this.AddDefaultBrick(wall)
                        .EnableAction<ActionAiderPersonWarningViewController5ProcessHouseholdChange>();
                    break;

				case WarningType.EChAddressChanged:
					this.AddDefaultBrick (wall)
                        .EnableAction<ActionAiderPersonWarningViewController9ProcessAddressChange>();
					break;
				
				case WarningType.ParishArrival:
					this.AddDefaultBrick (wall)
						.EnableAction<ActionAiderPersonWarningViewController2ProcessParishArrival> ();
					break;
				
				case WarningType.ParishDeparture:
					this.AddDefaultBrick (wall)
						.EnableAction<ActionAiderPersonWarningViewController3ProcessParishDeparture> ();
					break;
				
				default:
					this.AddDefaultBrick (wall)
                    .EnableAction<ActionAiderPersonWarningViewController0DiscardWarning>();
					break;
			}
			//Add household view if possible
			if (displayableHousehold > 0)
			{
				wall.AddBrick (x => x.Person.Contacts.Where (c => c.Household.Address.IsNotNull ()).First ().Household.Members)
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.HideRemoveButton)
				.Attribute (BrickMode.DefaultToSummarySubView)
				.Attribute (BrickMode.AutoGroup)
				.Template ()
					.Icon ("Data.AiderPersons")
					.Title ("Membres du ménage")
					.Text (p => p.GetCompactSummary (p.Households[0]))
				.End ();
			}
		}

		private SimpleBrick<AiderPersonWarningEntity> AddDefaultBrick(BrickWall<AiderPersonWarningEntity> wall)
		{
			return wall.AddBrick ()
				.Title (x => x.WarningType);
		}
	}
}
