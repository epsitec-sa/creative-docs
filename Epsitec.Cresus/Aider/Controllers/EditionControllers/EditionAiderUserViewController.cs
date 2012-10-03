//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Controllers.EditionControllers;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderUserViewController : EditionViewController<AiderUserEntity>
	{
		protected override void CreateBricks(Cresus.Bricks.BrickWall<AiderUserEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.DisplayName)
					.Field (x => x.LoginName).ReadOnly ()
					.Field (x => x.Disabled)
				.End ();

			wall.AddBrick ()
				.Input ()
					.Field (x => x.Person)
					.Field (x => x.Role)
					.Field (x => x.PreferredScope).ReadOnly ()
				.End ();

			wall.AddBrick ()
				.Input ()
					.Field (x => x.CustomScopes)
				.End ();
		}
	}
}
