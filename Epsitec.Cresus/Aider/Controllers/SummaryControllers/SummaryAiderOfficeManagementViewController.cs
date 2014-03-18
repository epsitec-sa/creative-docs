//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Controllers.EditionControllers;
using Epsitec.Aider.Controllers.SetControllers;

using Epsitec.Aider.Entities;
using Epsitec.Aider.Override;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Entities;

using System.Linq;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderOfficeManagementViewController : SummaryViewController<AiderOfficeManagementEntity>
	{
		protected override void CreateBricks(BrickWall<AiderOfficeManagementEntity> wall)
		{
			var currentUser	  = AiderUserManager.Current.AuthenticatedUser;
			var currentOffice = this.BusinessContext.GetLocalEntity (currentUser.Office);

			if (currentOffice.IsNull ())
			{
				this.CreateBricksGuestUser (wall);
			}
			else
			{
				this.CreateBricksTrustedUser (wall);
			}
		}

		private void CreateBricksGuestUser(BrickWall<AiderOfficeManagementEntity> wall)
		{
			wall.AddBrick (p => p.ParishGroup)
					.Icon ("Data.AiderGroup.People")
					.Title ("Membres de la paroisse")
					.Text (p => p.GetParticipantsSummary ())
					.Attribute (BrickMode.DefaultToSetSubView)
					.WithSpecialController (typeof (SetAiderGroupViewController0GroupParticipant));
		}

		private void CreateBricksTrustedUser(BrickWall<AiderOfficeManagementEntity> wall)
		{
			wall.AddBrick ()
					.Icon ("Base.AiderGoup.Parish")
					.Title (p => p.GetCompactSummary ())
					.Text (p => p.GetSummary ())
					.Attribute (BrickMode.DefaultToCreationOrEditionSubView);

			wall.AddBrick (p => p.ParishGroup)
					.Icon ("Data.AiderGroup.People")
					.Title ("Membres de la paroisse")
					.Text (p => p.GetParticipantsSummary ())
					.Attribute (BrickMode.DefaultToSetSubView)
					.WithSpecialController (typeof (SetAiderGroupViewController0GroupParticipant));

			wall.AddBrick (p => p.ParishGroup.Subgroups.Where(s => s.GroupDef.Classification == Enumerations.GroupClassification.DerogationIn).First ())
					.Icon ("Data.AiderGroup.People")
					.Title ("Dérogations entrantes")
					.Text (p => p.GetParticipantsSummary ())
					.Attribute (BrickMode.DefaultToSetSubView)
					.EnableActionButton<ActionAiderGroupViewController9GenerateOfficialReport> ()
					.WithSpecialController (typeof (SetAiderGroupViewController2DerogationsContact));

			wall.AddBrick (p => p.ParishGroup.Subgroups.Where (s => s.GroupDef.Classification == Enumerations.GroupClassification.DerogationOut).First ())
					.Icon ("Data.AiderGroup.People")
					.Title ("Dérogations sortantes")
					.Text (p => p.GetParticipantsSummary ())
					.Attribute (BrickMode.DefaultToSetSubView)
					.EnableActionButton<ActionAiderGroupViewController9GenerateOfficialReport> ()
					.WithSpecialController (typeof (SetAiderGroupViewController2DerogationsContact));


			wall.AddBrick (p => p.ParishGroup.Subgroups.Where (s => s.GroupDef.Classification == Enumerations.GroupClassification.Users).First ())
					.Icon ("Data.AiderGroup.People")
					.Title ("Gestionnaires AIDER")
					.Text (p => p.GetParticipantsSummary ())
					.Attribute (BrickMode.DefaultToSetSubView)
					.WithSpecialController (typeof (SetAiderGroupViewController0GroupParticipant));


			wall.AddBrick ()
				.Icon ("Base.AiderGoup.Parish")
				.Title (p => p.GetSettingsTitleSummary ())
				.Text (p => p.GetSettingsSummary ())
				.Attribute (BrickMode.DefaultToSummarySubView)
				.WithSpecialController (typeof (SummaryAiderOfficeManagementViewController1Settings))
				.EnableActionMenu<ActionAiderOfficeManagementViewController0CreateSettings> ();

			wall.AddBrick ()
				.Icon ("Base.AiderGoup.Parish")
				.Title (p => p.GetDocumentTitleSummary ())
				.Text (p => p.GetDocumentsSummary ())
				.Attribute (BrickMode.DefaultToSummarySubView)
				.WithSpecialController (typeof (SummaryAiderOfficeManagementViewController2Documents));
	
		}
	}
}
