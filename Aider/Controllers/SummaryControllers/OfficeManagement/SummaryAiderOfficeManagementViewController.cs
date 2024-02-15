//	Copyright © 2014-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Types;

using Epsitec.Aider.Controllers.ActionControllers;
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
		private bool							IsParish
		{
			get
			{
				return (this.Entity.ParishGroup.IsNotNull ())
					&& (this.Entity.ParishGroup.IsParish ());
			}
		}

		private bool							IsRegion
		{
			get
			{
				return (this.Entity.ParishGroup.IsNotNull ())
					&& (this.Entity.ParishGroup.IsRegion ());
			}
		}

		
		protected override void CreateBricks(BrickWall<AiderOfficeManagementEntity> wall)
		{
			var currentUser	  = AiderUserManager.Current.AuthenticatedUser;
			var currentOffice = this.BusinessContext.GetLocalEntity (currentUser.Office);

			if ((currentOffice.IsNotNull ()) ||
				(currentUser.CanViewOfficeDetails ()))
			{
				this.CreateBricksTrustedUser (wall, currentUser);
			}
			else
			{
				this.CreateBricksGuestUser (wall);
			}

			if ((currentOffice.IsNotNull ()) ||
				(currentUser.CanViewOfficeEvents ()))
			{
				this.CreateBricksForParishCollaborators (wall);
                this.CreateBricksForAnyCollaborators (wall);
			}

			if (this.IsRegion)
			{
				SummaryAiderOfficeManagementViewController.CreateBricksReferees (wall);
			}
		}

		
		private void CreateBricksGuestUser(BrickWall<AiderOfficeManagementEntity> wall)
		{
			if (this.IsParish)
			{
				SummaryAiderOfficeManagementViewController5Members.CreateBricksParishMembers (wall);
			}

			SummaryAiderOfficeManagementViewController.CreateBricksEmployeesReadOnly (wall);
		}

		private void CreateBricksTrustedUser(BrickWall<AiderOfficeManagementEntity> wall, AiderUserEntity user)
		{
			wall.AddBrick ()
					.Icon ("Data.AiderGroup.Parish")
					.Title (x => x.GetCompactSummary ())
					.Text ("Collaborateurs, expéditeurs, paramètres...")
					.Attribute (BrickMode.DefaultToSummarySubView)
					.WithSpecialController (typeof (SummaryAiderOfficeManagementViewController1Settings));

			if (this.IsParish)
			{
				wall.AddBrick ()
					.Icon ("Data.AiderGroup.Parish")
					.Title ("Dérogations")
					.Text ("Liste des dérogations")
					.Attribute (BrickMode.DefaultToSummarySubView)
					.WithSpecialController (typeof (SummaryAiderOfficeManagementViewController5Members));
			}

			SummaryAiderOfficeManagementViewController.CreateBricksAssociatedGroups (wall, this.Entity);
			SummaryAiderOfficeManagementViewController.CreateBricksDocuments (wall);
		}

        private void CreateBricksForParishCollaborators(BrickWall<AiderOfficeManagementEntity> wall)
        {
            if (this.IsParish)
            {
                SummaryAiderOfficeManagementViewController.CreateBricksEventsManagement (wall);
            }
        }

        private void CreateBricksForAnyCollaborators(BrickWall<AiderOfficeManagementEntity> wall)
        {
            SummaryAiderOfficeManagementViewController.CreateBricksTasks (wall);
		}

		private static void CreateBricksDocuments(BrickWall<AiderOfficeManagementEntity> wall)
		{
            wall.AddBrick ()
                .Icon ("Data.ArticleAccountingDefinition")
                .Title ("Registre des actes")
                .Text ("Liste des actes")
                .Attribute (BrickMode.DefaultToSummarySubView)
                .WithSpecialController (typeof (SummaryAiderOfficeManagementViewController7Documents));
		}

		private static void CreateBricksTasks(BrickWall<AiderOfficeManagementEntity> wall)
		{
			wall.AddBrick ()
				.Icon ("Data.AiderGroup.Parish")
				.Title (p => "Tâches")
				.Text (p => p.GetTasksSummary ())
				.Attribute (BrickMode.DefaultToSummarySubView)
				.WithSpecialController (typeof (SummaryAiderOfficeManagementViewController6Tasks));
		}


		private static void CreateBricksEmployeesReadOnly(BrickWall<AiderOfficeManagementEntity> wall)
		{
			wall.AddBrick (p => p.Employees)
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.HideRemoveButton)
				.Attribute (BrickMode.AutoGroup)
				.Attribute (BrickMode.DefaultToSummarySubView)
				.WithSpecialController (typeof (SummaryAiderEmployeeViewController1ReadOnly))
				.Template ()
					.Title ("Collaborateurs et ministres")
					.Text (x => TextFormatter.FormatText (x.Person.DisplayName, ":", x.EmployeeType))
				.End ();
		}

		private static void CreateBricksEventsManagement(BrickWall<AiderOfficeManagementEntity> wall)
		{
			wall.AddBrick ()
				.Icon ("Data.ArticleAccountingDefinition")
				.Title ("Actes en préparation")
                .Text (p => p.GetEventsInPreparationSummary ())
				.Attribute (BrickMode.DefaultToSummarySubView)
				.EnableActionButton<ActionAiderOfficeManagementViewController5PrepareEvent> ()
				.WithSpecialController (typeof (SummaryAiderOfficeManagementViewController3EventsInPreparation));

			wall.AddBrick ()
				.Icon ("Data.ArticleAccountingDefinition")
				.Title ("Actes à valider")
				.Text (p => p.GetEventsToValidateSummary ())
				.Attribute (BrickMode.DefaultToSummarySubView)
				.WithSpecialController (typeof (SummaryAiderOfficeManagementViewController4EventsToValidate));
		}
		
		private static void CreateBricksReferees(BrickWall<AiderOfficeManagementEntity> wall)
		{
			wall.AddBrick (p => p.RegionalReferees)
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.HideRemoveButton)
				.Attribute (BrickMode.AutoGroup)
				.Attribute (BrickMode.DefaultToSummarySubView)
				.WithSpecialController (typeof (SummaryAiderRefereeViewController1WithPersonDetails))
				.Template ()
					.Title ("Répondants régionaux")
					.Text (x => TextFormatter.FormatText (x.Employee.Person.DisplayName, ":", x.ReferenceType))
				.End ();
		}

		private static void CreateBricksAssociatedGroups(BrickWall<AiderOfficeManagementEntity> wall, AiderOfficeManagementEntity entity)
		{
			var associatedGroups = entity.AssociatedGroups;

			if (associatedGroups.Any ())
			{
				wall.AddBrick (p => p.AssociatedGroups)
					.Icon ("Data.AiderGroup.People")
					.Title ("Groupes associés")
					.Attribute (BrickMode.DefaultToSummarySubView)
					.Attribute (BrickMode.AutoGroup)
					.Attribute (BrickMode.HideAddButton)
					.Attribute (BrickMode.HideRemoveButton)
					.WithSpecialController (typeof (SummaryAiderGroupViewController1OfficeManagement))
					.Template ()
						.Text (x => x.GetNameParishNameWithRegion ())
					.End ();
			}
		}
	}
}
