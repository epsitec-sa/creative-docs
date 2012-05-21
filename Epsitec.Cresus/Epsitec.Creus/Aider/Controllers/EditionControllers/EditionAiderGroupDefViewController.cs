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
					.Field (x => x.Level)
					.Field (x => x.Type)
					.Field (x => x.DefType)
					.Field (x => x.Category)
				.End ()
				.Input ()
					.HorizontalGroup ("Occurrences")
						.Field (x => x.MinOccurs)
						.Field (x => x.MaxOccurs)
					.End ()
				.End ();
		}
	}
}
