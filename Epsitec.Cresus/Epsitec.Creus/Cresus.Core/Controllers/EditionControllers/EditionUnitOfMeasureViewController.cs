//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionUnitOfMeasureViewController : EditionViewController<UnitOfMeasureEntity>
	{
		protected override void CreateBricks(BrickWall<UnitOfMeasureEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Name)
				  .Field (x => x.ShortName)
				  .Field (x => x.Description)
				  .Field (x => x.Category).Width (100)
				  .Field (x => x.DivideRatio)
				  .Field (x => x.MultiplyRatio)
				  .Field (x => x.SmallestIncrement)
				.End ()
				;
		}
	}
}
