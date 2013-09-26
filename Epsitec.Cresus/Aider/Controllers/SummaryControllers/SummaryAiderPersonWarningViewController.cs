//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public sealed class SummaryAiderPersonWarningViewController : SummaryViewController<AiderPersonWarningEntity>
	{
		public override EntityViewController GetController()
		{
			switch (this.Entity.WarningType)
			{
				case WarningType.EChPersonDataChanged:
					return new SpecializedSummaryAiderPersonWarningViewController_EChPersonDataChanged ();

				case WarningType.EChPersonMissing:
					return new SpecializedSummaryAiderPersonWarningViewController_EChPersonMissing ();

				case WarningType.EChPersonNew:
					return new SpecializedSummaryAiderPersonWarningViewController_EChPersonNew ();

				case WarningType.EChProcessDeparture:
					return new SpecializedSummaryAiderPersonWarningViewController_EChProcessDeparture ();

				case WarningType.EChProcessArrival:
					return new SpecializedSummaryAiderPersonWarningViewController_EChProcessArrival ();

				case WarningType.EChHouseholdAdded:
					return new SpecializedSummaryAiderPersonWarningViewController_EChHouseholdAdded ();
			}
			
			return base.GetController ();
		}


		protected override void CreateBricks(BrickWall<AiderPersonWarningEntity> wall)
		{
			var hasDisplayableHousehold = this.Entity.Person.Contacts.Where (c => c.Household.Address.IsNotNull ()).Any ();
			
			var warning = this.Entity;
			var person  = warning.Person;
			var members = person.GetAllHouseholdMembers ();

			SimpleBrick<AiderPersonWarningEntity> brick;

			switch (this.Entity.WarningType)
			{
				case WarningType.EChHouseholdChanged:
					this.AddDefaultBrick(wall)
						.EnableActionButton<ActionAiderPersonWarningViewController5ProcessHouseholdChange> ();
					break;

				case WarningType.EChAddressChanged:
					this.AddDefaultBrick (wall)
						.EnableActionButton<ActionAiderPersonWarningViewController9ProcessAddressChange> ()
						.EnableActionButton<ActionAiderPersonWarningViewController91ProcessAddressChange> ();
					break;
				
				case WarningType.ParishArrival:
					
					brick = this.AddDefaultBrick (wall)
								.EnableActionButton<ActionAiderPersonWarningViewController2ProcessParishArrival> ();

					if (members.Count () > 1)
					{
						brick.EnableActionButton<ActionAiderPersonWarningViewController21ProcessParishArrival> ();
					}
					break;
				
				case WarningType.ParishDeparture:
					
					brick = this.AddDefaultBrick (wall)
						.EnableActionButton<ActionAiderPersonWarningViewController3ProcessParishDeparture> ();

					if (members.Count () > 1)
					{
						brick.EnableActionButton<ActionAiderPersonWarningViewController31ProcessParishDeparture> ();
					}
					break;
				
				default:
					System.Diagnostics.Trace.WriteLine ("Unhandled warning: " + this.Entity.WarningType.GetQualifiedName ());
					this.AddDefaultBrick (wall)
					.EnableActionButton<ActionAiderPersonWarningViewController0DiscardWarning> ();
					break;
			}

			if (hasDisplayableHousehold)
			{
				this.AddHouseholdBrick (wall);
			}
		}

		private void AddHouseholdBrick(BrickWall<AiderPersonWarningEntity> wall)
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
		
		private SimpleBrick<AiderPersonWarningEntity> AddDefaultBrick(BrickWall<AiderPersonWarningEntity> wall)
		{
			var brick = wall.AddBrick ()
							.Title (x => x.WarningType);

			wall.AddBrick (x => x.Person);

			return brick;
		}
	}
}
