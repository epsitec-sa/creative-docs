//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Types;

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
					.Icon ("Base.AiderGoup.Parish")
					.Title (x => x.GetCompactSummary ())
					.Text ("Collaborateurs, expéditeurs, paramètres...")
					.Attribute (BrickMode.DefaultToSummarySubView)
					.WithSpecialController (typeof (SummaryAiderOfficeManagementViewController1Settings));

			if (this.IsParish)
			{
				wall.AddBrick ()
					.Icon ("Base.AiderGoup.Parish")
					.Title ("Membres et dérogations")
					.Text ( x => x.ParishGroup.FindParticipantCount (this.DataContext) + " membres")
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

			SummaryAiderOfficeManagementViewController.CreateBricksTasks (wall);
		}

		private static void CreateBricksDocuments(BrickWall<AiderOfficeManagementEntity> wall)
		{
			wall.AddBrick ()
				.Icon ("Base.AiderGoup.Parish")
				.Title (p => p.GetDocumentTitleSummary ())
				.Text (p => p.GetDocumentsSummary ())
				.Attribute (BrickMode.DefaultToSummarySubView)
				.WithSpecialController (typeof (SummaryAiderOfficeManagementViewController2Documents));
		}

		private static void CreateBricksTasks(BrickWall<AiderOfficeManagementEntity> wall)
		{
			wall.AddBrick ()
				.Icon ("Base.AiderGoup.Parish")
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
				.Icon ("Base.AiderGoup.Parish")
				.Title (p => p.GetEventsInPreparationTitleSummary ())
				.Text (p => p.GetEventsInPreparationSummary ())
				.Attribute (BrickMode.DefaultToSummarySubView)
				.EnableActionButton<ActionAiderOfficeManagementViewController5PrepareEvent> ()
				.WithSpecialController (typeof (SummaryAiderOfficeManagementViewController3EventsInPreparation));

			wall.AddBrick ()
				.Icon ("Base.AiderGoup.Parish")
				.Title (p => p.GetEventsToValidateTitleSummary ())
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
