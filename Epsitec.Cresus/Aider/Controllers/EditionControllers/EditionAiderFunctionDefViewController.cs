//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderFunctionDefViewController : EditionViewController<AiderFunctionDefEntity>
	{
		protected override void CreateBricks(BrickWall<AiderFunctionDefEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.Type)
					.Field (x => x.Name)
					.Field (x => x.Comment.Text)
				.End ();
		}
	}
}
