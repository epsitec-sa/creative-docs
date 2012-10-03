//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Bricks;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderUserScopeViewController : SummaryViewController<AiderUserScopeEntity>
	{
		protected override void CreateBricks(Cresus.Bricks.BrickWall<AiderUserScopeEntity> wall)
		{
			wall.AddBrick ();
		}
	}
}
