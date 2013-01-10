//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Aider.Override;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.EditionControllers;

using Epsitec.Cresus.Core.Business.UserManagement;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderUserViewController : EditionViewController<AiderUserEntity>
	{
		protected override void CreateBricks(BrickWall<AiderUserEntity> wall)
		{
			var user = AiderUserManager.Current.AuthenticatedUser;

			if (user.HasPowerLevel (UserPowerLevel.Administrator))
			{
				EditionAiderUserViewController.AddUserDataBrick (wall);
			}
			else
			{
				EditionAiderUserViewController.AddUserDataBrickReadonly (wall);
			}
		}

		private static void AddUserDataBrick(BrickWall<AiderUserEntity> wall)
		{
			wall.AddBrick ()
				.EnableAction (0)
				.EnableAction (1)
				.Title (Res.Strings.AiderUserDataTitle)
				.Input ()
					.Field (x => x.Person)
					.Field (x => x.LoginName)
					.Field (x => x.Role)
					.Field (x => x.Disabled)
				.End ();
		}

		private static void AddUserDataBrickReadonly(BrickWall<AiderUserEntity> wall)
		{
			wall.AddBrick ()
				.EnableAction (0)
				.Title (Res.Strings.AiderUserDataTitle)
				.Input ()
					.Field (x => x.Person)
						.ReadOnly ()
					.Field (x => x.LoginName)
						.ReadOnly ()
					.Field (x => x.Role)
						.ReadOnly ()
					.Field (x => x.Disabled)
						.ReadOnly ()
				.End ();
		}
	}
}
