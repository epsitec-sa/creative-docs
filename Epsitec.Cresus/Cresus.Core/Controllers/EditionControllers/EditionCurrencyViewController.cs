//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionCurrencyViewController : EditionViewController<CurrencyEntity>
	{
		protected override void CreateBricks(BrickWall<CurrencyEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.BeginDate).Width (150)
				  .Field (x => x.EndDate).Width (150)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.CurrencyCode)
				  .Field (x => x.ExchangeRateBase).Width (100)
				  .Field (x => x.ExchangeRate).Width (100)
				  .Field (x => x.ExchangeRateSource)
				.End ()
				;
		}
	}
}
