//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Bricks;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderUserRoleViewController : SummaryViewController<AiderUserRoleEntity>
	{
		protected override void CreateBricks(Cresus.Bricks.BrickWall<AiderUserRoleEntity> wall)
		{
			wall.AddBrick ();

			wall.AddBrick (x => x.DefaultScopes)
				.Attribute (BrickMode.DefaultToSummarySubView)
				.Attribute (BrickMode.AutoGroup)
				.Template ()
				.End ();
		}
	}
}
