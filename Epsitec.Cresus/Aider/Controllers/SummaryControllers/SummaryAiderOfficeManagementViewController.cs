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

			if (this.IsRegion)
			{
				SummaryAiderOfficeManagementViewController.CreateBricksReferees (wall);
			}
		}

		
		private void CreateBricksGuestUser(BrickWall<AiderOfficeManagementEntity> wall)
		{
			if (this.IsParish)
			{
				SummaryAiderOfficeManagementViewController.CreateBricksParishMembers (wall);
			}

			SummaryAiderOfficeManagementViewController.CreateBricksEmployeesReadOnly (wall);
		}

		private void CreateBricksTrustedUser(BrickWall<AiderOfficeManagementEntity> wall, AiderUserEntity user)
		{
			wall.AddBrick ()
					.Icon ("Base.AiderGoup.Parish")
					.Title (p => p.GetCompactSummary ())
					.Text (p => p.GetSummary ())
					.Attribute (BrickMode.DefaultToCreationOrEditionSubView);

			if (this.IsParish)
			{
				SummaryAiderOfficeManagementViewController.CreateBricksParishMembers (wall);
				SummaryAiderOfficeManagementViewController.CreateBricksDerogations (wall);
			}

			SummaryAiderOfficeManagementViewController.CreateBricksEmployees (wall, user);
			SummaryAiderOfficeManagementViewController.CreateBricksAssociatedGroups (wall, this.Entity);
			
			wall.AddBrick ()
				.Icon ("Base.AiderGoup.Parish")
				.Title (p => p.GetSettingsTitleSummary ())
				.Text (p => p.GetSettingsSummary ())
				.Attribute (BrickMode.DefaultToSummarySubView)
				.WithSpecialController (typeof (SummaryAiderOfficeManagementViewController1Settings));

			wall.AddBrick ()
				.Icon ("Base.AiderGoup.Parish")
				.Title (p => p.GetDocumentTitleSummary ())
				.Text (p => p.GetDocumentsSummary ())
				.Attribute (BrickMode.DefaultToSummarySubView)
				.WithSpecialController (typeof (SummaryAiderOfficeManagementViewController2Documents));
		}
		
		
		private static void CreateBricksParishMembers(BrickWall<AiderOfficeManagementEntity> wall)
		{
			wall.AddBrick (p => p.ParishGroup)
				.Icon ("Data.AiderGroup.People")
				.Title ("Membres de la paroisse")
				.Text ("Voir tous les paroissiens")
				.Attribute (BrickMode.DefaultToSetSubView)
				.WithSpecialController (typeof (SetAiderGroupViewController0GroupParticipant));
		}
		
		private static void CreateBricksDerogations(BrickWall<AiderOfficeManagementEntity> wall)
		{
			wall.AddBrick (p => p.ParishGroup.Subgroups.Single (s => s.GroupDef.Classification == Enumerations.GroupClassification.DerogationIn))
				.Icon ("Data.AiderGroup.People")
				.Title ("Dérogations entrantes")
				.Text (p => p.GetParticipantsSummary ())
				.Attribute (BrickMode.DefaultToSetSubView)
				.EnableActionButton<ActionAiderGroupViewController9GenerateOfficialReport> ()
				.WithSpecialController (typeof (SetAiderGroupViewController2DerogationsContact));

			wall.AddBrick (p => p.ParishGroup.Subgroups.Single (s => s.GroupDef.Classification == Enumerations.GroupClassification.DerogationOut))
				.Icon ("Data.AiderGroup.People")
				.Title ("Dérogations sortantes")
				.Text (p => p.GetParticipantsSummary ())
				.Attribute (BrickMode.DefaultToSetSubView)
				.EnableActionButton<ActionAiderGroupViewController9GenerateOfficialReport> ()
				.WithSpecialController (typeof (SetAiderGroupViewController2DerogationsContact));
		}
		
		private static void CreateBricksEmployees(BrickWall<AiderOfficeManagementEntity> wall, AiderUserEntity user)
		{
			bool canAddEmployee	   = user.CanEditEmployee ();
			bool canRemoveEmployee = user.CanEditEmployee ();

			wall.AddBrick (p => p.Employees)
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.HideRemoveButton)
				.Attribute (BrickMode.AutoGroup)
				.Attribute (BrickMode.DefaultToSummarySubView)
				.EnableActionMenu<ActionAiderOfficeManagementViewController3AddEmployeeAndJob> ().IfTrue (canAddEmployee)
				.EnableActionMenu<ActionAiderOfficeManagementViewController4DeleteEmployee> ().IfTrue (canRemoveEmployee)
				.Template ()
					.Title ("Collaborateurs et ministres")
					.Text (x => TextFormatter.FormatText (x.Person.DisplayName, ":", x.EmployeeType))
				.End ();
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
