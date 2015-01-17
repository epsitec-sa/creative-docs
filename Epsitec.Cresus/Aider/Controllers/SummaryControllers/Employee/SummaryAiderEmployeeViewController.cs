//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Aider.Override;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderEmployeeViewController : SummaryViewController<AiderEmployeeEntity>
	{
		protected override void CreateBricks(BrickWall<AiderEmployeeEntity> wall)
		{
			var user = AiderUserManager.Current.AuthenticatedUser;
			bool canEditEmploye = user.CanEditEmployee () || user.CanEditReferee ();
			wall.AddBrick (x => x.PersonContact)
				.Attribute (BrickMode.DefaultToSummarySubView);

			wall.AddBrick ()
				.Attribute (BrickMode.DefaultToNoSubView).IfFalse (user.CanEditEmployee ());

			wall.AddBrick (x => x.EmployeeJobs)
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.HideRemoveButton)
				.Attribute (BrickMode.AutoGroup)
				.Attribute (BrickMode.DefaultToNoSubView).IfFalse (user.CanEditEmployee ())
				.EnableActionButton<ActionAiderEmployeeViewController01AddJob> ().IfTrue (user.CanEditEmployee ())
				.EnableActionMenu<ActionAiderEmployeeViewController03RemoveJob> ().IfTrue (user.CanEditEmployee ())
				.Template ()
				.End ();
			
			wall.AddBrick (x => x.RefereeEntries)
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.HideRemoveButton)
				.Attribute (BrickMode.AutoGroup)
				.Attribute (BrickMode.DefaultToNoSubView).IfFalse (user.CanEditReferee ())
				.EnableActionButton<ActionAiderEmployeeViewController02AddReferee> ().IfTrue (canEditEmploye)
				.EnableActionMenu<ActionAiderEmployeeViewController04RemoveReferee> ().IfTrue (canEditEmploye)
				.Template ()
				.End ();
		}
	}
}