//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Controllers.EditionControllers;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderUserRoleViewController : EditionViewController<AiderUserRoleEntity>
	{
		protected override void CreateBricks(Cresus.Bricks.BrickWall<AiderUserRoleEntity> wall)
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
