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
	[ControllerSubType (2)]
	public sealed class SummaryAiderEmployeeViewController2WithoutContact : SummaryViewController<AiderEmployeeEntity>
	{
		protected override void CreateBricks(BrickWall<AiderEmployeeEntity> wall)
		{
			var user = AiderUserManager.Current.AuthenticatedUser;

			wall.AddBrick ()
				.Attribute (BrickMode.DefaultToNoSubView).IfTrue(!user.CanEditEmployee ())
				.Attribute (BrickMode.DefaultToCreationOrEditionSubView).IfTrue(user.CanEditEmployee ());

			wall.AddBrick (x => x.EmployeeJobs)
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.HideRemoveButton)
				.Attribute (BrickMode.AutoGroup)
				.EnableActionMenu<ActionAiderEmployeeViewController01AddJob> ().IfTrue (user.CanEditEmployee ())
				.EnableActionMenu<ActionAiderEmployeeViewController03RemoveJob> ().IfTrue (user.CanEditEmployee ())
				.Template ()
				.End ();
			
			wall.AddBrick (x => x.RefereeEntries)
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.HideRemoveButton)
				.Attribute (BrickMode.AutoGroup)
				.Attribute (BrickMode.DefaultToNoSubView).IfFalse (user.CanEditReferee ())
				.EnableActionMenu<ActionAiderEmployeeViewController02AddReferee> ().IfTrue (user.CanEditReferee ())
				.EnableActionMenu<ActionAiderEmployeeViewController04RemoveReferee> ().IfTrue (user.CanEditReferee ())
				.Template ()
				.End ();
		}
	}
}