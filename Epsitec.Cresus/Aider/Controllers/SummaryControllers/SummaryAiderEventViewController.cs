//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
				.Title ("Acte en préparation")
				.Text (x => x.GetSummary ())
				.EnableActionButton<ActionAiderEventViewController1SetToValidate> ()
				.EnableActionButton<ActionAiderEventViewController4Delete> ()
				.Attribute (BrickMode.DefaultToCreationOrEditionSubView);

				wall.AddBrick (x => x.Participants)
					.Title ("Gérer les participations")
					.Attribute (BrickMode.HideAddButton)
					.Attribute (BrickMode.HideRemoveButton)
					.Attribute (BrickMode.AutoGroup)
					.Attribute (BrickMode.DefaultToCreationOrEditionSubView)
					.EnableActionButton<ActionAiderEventViewController0AddParticipantsFromBag> ()
					.EnableActionButton<ActionAiderEventViewController8AddParticipantExternal> ()
					.EnableActionButton<ActionAiderEventViewController7AddParticipant> ()
					.EnableActionButton<ActionAiderEventViewController6RemoveParticipant> ()
					.EnableActionMenu<ActionAiderEventViewController5AddParticipantFromScratch> ()
					.EnableActionMenu<ActionAiderEventViewController8AddParticipantExternal> ()
					.Template ()
					.End ();
			}

			if (currentEvent.State == Enumerations.EventState.ToValidate)
			{
				wall.AddBrick ()
				.Icon ("Data.AiderEvent")
				.Title ("Acte à valider")
				.Text (x => x.GetSummary ())
				.EnableActionButton<ActionAiderEventViewController2Rollback> ()
				.EnableActionButton<ActionAiderEventViewController3Validate> ()
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
	}
}