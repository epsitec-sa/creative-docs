//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderUserViewController : EditionViewController<AiderUserEntity>
	{
		protected override void CreateBricks(BrickWall<AiderUserEntity> wall)
		{
			wall.AddBrick ()
				.Title (Res.Strings.AiderUserDataTitle)
				.Input ()
					.Field (x => x.Person)
					.Field (x => x.LoginName)
					.Field (x => x.Role)
					.Field (x => x.Disabled)
				.End ();

			wall.AddBrick ()
				.Title (Res.Strings.AiderUserPasswordTitle)
				.Input ()
					.Field (x => x.ClearPassword)
						.Password ()
					.Field (x => x.ClearPasswordConfirmation)
						.Password ()
				.End ();
		}
	}
}
