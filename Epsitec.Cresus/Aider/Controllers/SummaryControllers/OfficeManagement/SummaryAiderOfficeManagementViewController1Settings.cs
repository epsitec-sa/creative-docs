//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Controllers.EditionControllers;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Override;


using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Library;

using System.Linq;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Aider.Controllers.SetControllers;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	[ControllerSubType (1)]
	public sealed class SummaryAiderOfficeManagementViewController1Settings : SummaryViewController<AiderOfficeManagementEntity>
	{
		protected override void CreateBricks(BrickWall<AiderOfficeManagementEntity> wall)
		{
			var currentUser	  = AiderUserManager.Current.AuthenticatedUser;
			SummaryAiderOfficeManagementViewController1Settings.CreateBricksEmployees (wall, currentUser);

			wall.AddBrick (p => p.OfficeSenders)
				.Attribute (BrickMode.DefaultToCreationOrEditionSubView)
				.Attribute (BrickMode.AutoGroup)
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.HideRemoveButton)
				.EnableActionMenu<ActionAiderOfficeManagementViewController0CreateSettings> ()
				.EnableActionMenu<ActionAiderOfficeManagementViewController2RemoveSettings> ()
				.Template ()
					.Title ("Liste des expéditeurs")
				.End ();

			wall.AddBrick ()
				.Icon ("Base.AiderGoup.Parish")
				.Title ("Editer les paramètres")
				.Text ("...")
				.Attribute (BrickMode.DefaultToCreationOrEditionSubView);
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
	}
}
