//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderGroupDefViewController : EditionViewController<AiderGroupDefEntity>
	{
		protected override void CreateBricks(BrickWall<AiderGroupDefEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.Name)
						.ReadOnly ()
					.Field (x => x.PathTemplate)
						.ReadOnly ()
					.Field (x => x.NodeType)
						.ReadOnly ()
					.Field (x => x.Classification)
						.ReadOnly ()
					.Field (x => x.Mutability)
						.ReadOnly ()
				.End ();
		}
	}
}
