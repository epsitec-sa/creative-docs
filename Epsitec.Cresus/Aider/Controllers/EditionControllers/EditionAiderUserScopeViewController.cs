//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderUserScopeViewController : EditionViewController<AiderUserScopeEntity>
	{
		protected override void CreateBricks(BrickWall<AiderUserScopeEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.Mutability)
					.Field (x => x.Name)
					.Field (x => x.GroupPath)
				.End ();
		}
	}
}
