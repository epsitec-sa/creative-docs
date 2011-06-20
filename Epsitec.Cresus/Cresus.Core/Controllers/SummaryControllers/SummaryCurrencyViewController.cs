//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryCurrencyViewController : SummaryViewController<CurrencyEntity>
	{
		protected override void CreateBricks(Bricks.BrickWall<CurrencyEntity> wall)
		{
			wall.AddBrick ();
		}
	}
	public class SummaryExchangeRateSourceViewController : SummaryViewController<ExchangeRateSourceEntity>
	{
		protected override void CreateBricks(Bricks.BrickWall<ExchangeRateSourceEntity> wall)
		{
			wall.AddBrick ();
		}
	}
}
