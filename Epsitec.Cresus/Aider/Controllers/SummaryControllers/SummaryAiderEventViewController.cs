//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Aider.Override;
using Epsitec.Aider.Controllers.SetControllers;
using Epsitec.Aider.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderEventViewController : SummaryViewController<AiderEventEntity>
	{
		protected override void CreateBricks(BrickWall<AiderEventEntity> wall)
		{
			var currentEvent = this.Entity;

			wall.AddBrick ()
				.Icon ("Data.AiderEvent")
				.Title ("Acte")
				.Text ( x => x.GetSummary ())
				.Attribute (BrickMode.DefaultToCreationOrEditionSubView);

			if(currentEvent.State == Enumerations.EventState.InPreparation)
			{
				CreateBricksForSetup (wall, currentEvent);
			}
			
		}


		private static void CreateBricksForSetup(BrickWall<AiderEventEntity> wall, AiderEventEntity currentEvent)
		{
			switch (currentEvent.Type)
			{
				case Enumerations.EventType.Blessing:
					SummaryAiderEventViewController.CreateBricksForBlessingSetup (wall);
					break;

				case Enumerations.EventType.Baptism:
					SummaryAiderEventViewController.CreateBricksForBaptismSetup (wall);
					break;

				case Enumerations.EventType.Marriage:
					SummaryAiderEventViewController.CreateBricksForMarriageSetup (wall);
					break;

			}
		}

		private static void CreateBricksForBlessingSetup(BrickWall<AiderEventEntity> wall)
		{
			wall.AddBrick ()
				.Icon ("Data.AiderEvent")
				.Title ("Participants")
				.Text ("")
				.Attribute (BrickMode.DefaultToSetSubView)
				.WithSpecialController (typeof (SetAiderEventViewController0Participants));
		}

		private static void CreateBricksForBaptismSetup(BrickWall<AiderEventEntity> wall)
		{
			wall.AddBrick ()
				.Icon ("Data.AiderEvent")
				.Title ("Participants")
				.Text ("")
				.Attribute (BrickMode.DefaultToSetSubView)
				.WithSpecialController (typeof (SetAiderEventViewController0Participants));
		}

		private static void CreateBricksForMarriageSetup(BrickWall<AiderEventEntity> wall)
		{
			wall.AddBrick ()
				.Icon ("Data.AiderEvent")
				.Title ("Mariage")
				.Text ("Participants")
				.Attribute (BrickMode.DefaultToSetSubView)
				.WithSpecialController (typeof (SetAiderEventViewController0Participants));
		}
	}
}