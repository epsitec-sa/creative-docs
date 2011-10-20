//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionPriceDiscountViewController : EditionViewController<PriceDiscountEntity>
	{
		protected override void CreateBricks(BrickWall<PriceDiscountEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Text)
				  .Field (x => x.DiscountRate)
				  .Field (x => x.Value)
				  //?.Field (x => x.DiscountPolicy)
				  //?.Field (x => x.RoundingMode)
				.End ()
				;
		}
	}
}
