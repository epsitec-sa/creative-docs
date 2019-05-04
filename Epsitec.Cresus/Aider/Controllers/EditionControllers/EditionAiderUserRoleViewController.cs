//	Copyright © 2012-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Aider.Override;
using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderUserRoleViewController : EditionViewController<AiderUserRoleEntity>
	{
		protected override void CreateBricks(BrickWall<AiderUserRoleEntity> wall)
		{
            var user = AiderUserManager.Current.AuthenticatedUser;

            wall.AddBrick ()
				.Input ()
					.Field (x => x.Name)
						.ReadOnly ()
                        .IfFalse (user.IsSysAdmin ())
					.Field (x => x.Mutability)
						.ReadOnly ()
                        .IfFalse (user.IsSysAdmin ())
                .End ();

			wall.AddBrick ()
				.Input ()
					.Field (x => x.DefaultScopes)
						.ReadOnly ()
				.End ();
		}
	}
}
