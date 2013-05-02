//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Override;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

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
				.EnableAction<ActionAiderUserViewController0SetPassword> ()
				.EnableAction<ActionAiderUserViewController1SetAdministrator> ()
				.Title (Res.Strings.AiderUserDataTitle)
				.Input ()
					.Field (x => x.Parish)
						.WithSpecialField<AiderGroupSpecialField<AiderUserEntity>> ()
					.Field (x => x.LoginName)
					.Field (x => x.DisplayName)
					.Field (x => x.Email)
					.Field (x => x.Role)
					.Field (x => x.Disabled)
					.Field (x => x.EnableGroupEditionCanton)
					.Field (x => x.EnableGroupEditionRegion)
					.Field (x => x.EnableGroupEditionParish)
				.End ();
		}

		private static void AddUserDataBrickReadonly(BrickWall<AiderUserEntity> wall)
		{
			wall.AddBrick ()
				.EnableAction<ActionAiderUserViewController0SetPassword> ()
				.Title (Res.Strings.AiderUserDataTitle)
				.Input ()
					.Field (x => x.Parish).ReadOnly ()
					.Field (x => x.LoginName).ReadOnly ()
					.Field (x => x.DisplayName).ReadOnly ()
					.Field (x => x.Email)
					.Field (x => x.Role).ReadOnly ()
					.Field (x => x.Disabled).ReadOnly ()
					.Field (x => x.EnableGroupEditionCanton).ReadOnly ()
					.Field (x => x.EnableGroupEditionRegion).ReadOnly ()
					.Field (x => x.EnableGroupEditionParish).ReadOnly ()
				.End ();
		}
	}
}
