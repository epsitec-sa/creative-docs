//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderGroupViewController : EditionViewController<AiderGroupEntity>
	{
		protected override void CreateBricks(Cresus.Bricks.BrickWall<AiderGroupEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.Name)
					.Field (x => x.Description)
				.End ();
		}
	}
}
