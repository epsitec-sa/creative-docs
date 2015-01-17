//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	[ControllerSubType (1)]
	public sealed class SummaryAiderEmployeeViewController1ReadOnly : SummaryViewController<AiderEmployeeEntity>
	{
		protected override void CreateBricks(BrickWall<AiderEmployeeEntity> wall)
		{
			wall.AddBrick (x => x.PersonContact)
				.Attribute (BrickMode.DefaultToSummarySubView);

			wall.AddBrick ()
				.Attribute (BrickMode.DefaultToNoSubView);

			wall.AddBrick (x => x.EmployeeJobs)
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.HideRemoveButton)
				.Attribute (BrickMode.AutoGroup)
				.Attribute (BrickMode.DefaultToNoSubView)
				.Template ()
				.End ();

			wall.AddBrick (x => x.RefereeEntries)
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.HideRemoveButton)
				.Attribute (BrickMode.AutoGroup)
				.Attribute (BrickMode.DefaultToNoSubView)
				.Template ()
				.End ();
		}
	}
}