//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderCommentViewController : EditionViewController<AiderCommentEntity>
	{
		protected override void CreateBricks(BrickWall<AiderCommentEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.Text)
				.End ();
		}
	}
}
