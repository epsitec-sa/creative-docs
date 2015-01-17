//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderPersonWarningViewController : EditionViewController<AiderPersonWarningEntity>
	{
		protected override void CreateBricks(BrickWall<AiderPersonWarningEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.Description)
				.End ();
		}
	}
}
