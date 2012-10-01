//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderGroupDefViewController : EditionViewController<AiderGroupDefEntity>
	{
		protected override void CreateBricks(Cresus.Bricks.BrickWall<AiderGroupDefEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.Name)
					.Field (x => x.PathTemplate)
					.Field (x => x.NodeType)
					.Field (x => x.Classification)
					.Field (x => x.Category)
				.End ();
		}
	}
}
