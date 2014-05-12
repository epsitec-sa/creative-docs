//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Bricks;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderEmployeeViewController : SummaryViewController<AiderEmployeeEntity>
	{
		protected override void CreateBricks(BrickWall<AiderEmployeeEntity> wall)
		{
			wall.AddBrick ();
			
			wall.AddBrick (x => x.EmployeeJobs)
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.HideRemoveButton)
				.Attribute (BrickMode.AutoGroup)
				.EnableActionMenu<ActionAiderEmployeeViewController01AddJob> ()
				.Template ()
				.End ()
				.Attribute (BrickMode.DefaultToSummarySubView);
			
			wall.AddBrick (x => x.RefereeEntries)
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.HideRemoveButton)
				.Attribute (BrickMode.AutoGroup)
				.EnableActionMenu<ActionAiderEmployeeViewController02AddReferee> ()
				.Template ()
				.End ();
		}
	}
}
