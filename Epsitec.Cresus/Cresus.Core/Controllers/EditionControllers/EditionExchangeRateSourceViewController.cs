//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionExchangeRateSourceViewController : EditionViewController<ExchangeRateSourceEntity>
	{
		protected override void CreateBricks(Bricks.BrickWall<ExchangeRateSourceEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Type)
				  .Field (x => x.Originator)
				.End ();
		}
	}
}
