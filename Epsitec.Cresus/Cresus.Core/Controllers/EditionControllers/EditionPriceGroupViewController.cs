//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionPriceGroupViewController : EditionViewController<PriceGroupEntity>
	{
		protected override void CreateBricks(Bricks.BrickWall<PriceGroupEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Name)
				  .Field (x => x.Description)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.NeverApplyDiscount)
				  .Field (x => x.DefaultDivideRatio).Width (100)
				  .Field (x => x.DefaultMultiplyRatio).Width (100)
				  .Field (x => x.DefaultRoundingMode)
				.End ()
				;
		}
	}
}
