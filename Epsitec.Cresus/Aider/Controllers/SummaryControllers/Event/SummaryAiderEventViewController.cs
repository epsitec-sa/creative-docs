//	Copyright © 2014-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Controllers.SetControllers;

using Epsitec.Aider.Entities;
using Epsitec.Aider.Override;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderEventViewController : SummaryViewController<AiderEventEntity>
	{
		protected override void CreateBricks(BrickWall<AiderEventEntity> wall)
		{
			var user = AiderUserManager.Current.AuthenticatedUser;

            var userCanValidate = user.CanValidateEvents () || AiderUserManager.Current.AuthenticatedUser.LoginName == "root";
            var userCanEditDoc  = user.IsAdmin ();

            switch (this.Entity.State)
            {
                case Enumerations.EventState.InPreparation:
                    this.CreateBricksInPreparation (wall);
                    break;

                case Enumerations.EventState.ToValidate:
                    this.CreateBricksToValidate (wall, userCanValidate);
                    break;

                case Enumerations.EventState.Validated:
                    this.CreateBricksValidated (wall, userCanEditDoc);
                    break;
            }
        }


        private void CreateBricksValidated(BrickWall<AiderEventEntity> wall, bool userCanEditDoc)
        {
            wall.AddBrick ()
                .Icon ("Data.AiderEvent")
                .Title (x => "Acte N° " + x.Report.GetEventNumber ())
                .Text (x => x.GetSummary ())
                .Attribute (BrickMode.DefaultToNoSubView)
                .EnableActionMenu<ActionAiderEventViewController11Delete> ().IfTrue (userCanEditDoc);

            if (this.Entity.Report.IsNotNull ())
            {
                wall.AddBrick (x => x.Report)
                    .Attribute (BrickMode.DefaultToNoSubView).IfFalse (userCanEditDoc);

                wall.AddBrick (x => x.Report.Office)
                    .Icon ("Base.AiderGroup.Parish")
                    .Title ("Créateur")
                    .Text (x => x.GetCompactSummary ())
                    .Attribute (BrickMode.DefaultToSummarySubView);
            }


            wall.AddBrick ()
                .Title ("Participants")
                .Text (p => p.GetParticipantsSummary ())
                .Attribute (BrickMode.DefaultToSetSubView)
                .WithSpecialController (typeof (SetAiderEventViewController0Participants));
        }

        private void CreateBricksToValidate(BrickWall<AiderEventEntity> wall, bool userCanValidate)
        {
            wall.AddBrick ()
                .Icon ("Data.AiderEvent")
                .Title ("Acte à valider")
                .Text (x => x.GetSummary ())
                .EnableActionButton<ActionAiderEventViewController2Rollback> ()
                .EnableActionButton<ActionAiderEventViewController9PreviewReport> ().IfTrue (userCanValidate)
                .EnableActionButton<ActionAiderEventViewController3Validate> ().IfTrue (userCanValidate)
                .Attribute (BrickMode.DefaultToNoSubView);

            if (this.Entity.Report.IsNotNull ())
            {
                wall.AddBrick (x => x.Report)
                .Attribute (BrickMode.DefaultToCreationOrEditionSubView);
            }


            wall.AddBrick (x => x.Participants)
                .Attribute (BrickMode.HideAddButton)
                .Attribute (BrickMode.HideRemoveButton)
                .Attribute (BrickMode.AutoGroup)
                .Attribute (BrickMode.DefaultToSummarySubView)
                .Template ()
                .End ();
        }

        private void CreateBricksInPreparation(BrickWall<AiderEventEntity> wall)
        {
            wall.AddBrick ()
                .Icon ("Data.AiderEvent")
                .Title ("Acte en préparation")
                .Text (x => x.GetSummary ())
                .EnableActionButton<ActionAiderEventViewController1SetToValidate> ()
                .EnableActionButton<ActionAiderEventViewController4DeleteDraft> ()
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
                .EnableActionButton<ActionAiderEventViewController10AddMinister> ()
                .EnableActionButton<ActionAiderEventViewController6RemoveParticipant> ()
                .EnableActionMenu<ActionAiderEventViewController5AddParticipantFromScratch> ()
                .EnableActionMenu<ActionAiderEventViewController8AddParticipantExternal> ()
                .Template ()
                .End ();
        }
    }
}
