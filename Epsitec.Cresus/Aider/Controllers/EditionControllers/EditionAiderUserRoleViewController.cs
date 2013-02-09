//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderUserRoleViewController : EditionViewController<AiderUserRoleEntity>
	{
		protected override void CreateBricks(BrickWall<AiderUserRoleEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.Name)
				.End ();

			wall.AddBrick ()
				.Input ()
					.Field (x => x.DefaultScopes)
				.End ();
		}
	}
}
