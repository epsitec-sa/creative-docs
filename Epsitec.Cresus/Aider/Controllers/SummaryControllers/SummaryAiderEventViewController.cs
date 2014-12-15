//	Copyright � 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

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

			if(currentEvent.State == Enumerations.EventState.InPreparation)
			{
				wall.AddBrick ()
				.Icon ("Data.AiderEvent")
				.Title ("Acte en pr�paration")
				.Text (x => x.GetSummary ())
				.EnableActionButton<ActionAiderEventViewController1SetToValidate> ()
				.EnableActionButton<ActionAiderEventViewController4Delete> ()
				.Attribute (BrickMode.DefaultToCreationOrEditionSubView);

				wall.AddBrick (x => x.Participants)
					.Title ("G�rer les participations")
					.Attribute (BrickMode.HideAddButton)
					.Attribute (BrickMode.HideRemoveButton)
					.Attribute (BrickMode.AutoGroup)
					.Attribute (BrickMode.DefaultToCreationOrEditionSubView)
					.EnableActionButton<ActionAiderEventViewController0AddParticipantsFromBag> ()
					.Template ()
					.End ();
			}

			if (currentEvent.State == Enumerations.EventState.ToValidate)
			{
				wall.AddBrick ()
				.Icon ("Data.AiderEvent")
				.Title ("Acte � valider")
				.Text (x => x.GetSummary ())
				.EnableActionButton<ActionAiderEventViewController2Rollback> ()
				//.EnableActionButton<ActionAiderEventViewController3Validate> ()
				.Attribute (BrickMode.DefaultToNoSubView);

				wall.AddBrick (x => x.Participants)
					.Attribute (BrickMode.HideAddButton)
					.Attribute (BrickMode.HideRemoveButton)
					.Attribute (BrickMode.AutoGroup)
					.Attribute (BrickMode.DefaultToSummarySubView)
					.Template ()
					.End ();
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
				.Title ("Pr�paration")
				.Text ("")
				.EnableActionOnDrop<ActionAiderEventViewController0AddParticipantsFromBag> ()
				.Attribute (BrickMode.DefaultToSetSubView);
		}

		private static void CreateBricksForBaptismSetup(BrickWall<AiderEventEntity> wall)
		{
			wall.AddBrick ()
				.Icon ("Data.AiderEvent")
				.Title ("Pr�paration")
				.Text ("")
				.EnableActionOnDrop<ActionAiderEventViewController0AddParticipantsFromBag> ()
				.Attribute (BrickMode.DefaultToSetSubView);
		}

		private static void CreateBricksForMarriageSetup(BrickWall<AiderEventEntity> wall)
		{
			wall.AddBrick ()
				.Icon ("Data.AiderEvent")
				.Title ("Pr�paration")
				.Text ("")
				.EnableActionOnDrop<ActionAiderEventViewController0AddParticipantsFromBag> ()
				.Attribute (BrickMode.DefaultToSetSubView);
		}
	}
}