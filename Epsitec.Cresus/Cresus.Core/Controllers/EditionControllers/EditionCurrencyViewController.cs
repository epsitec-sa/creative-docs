//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionCurrencyViewController : EditionViewController<CurrencyEntity>
	{
		protected override void CreateBricks(Bricks.BrickWall<CurrencyEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.BeginDate)
				  .Field (x => x.EndDate)
				  .Field (x => x.ExchangeRateBase)
				  .Field (x => x.ExchangeRate)
				  .Field (x => x.ExchangeRateSource)
				.End ();
		}
	}
}
