//	Copyright � 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionExchangeRateSourceViewController : EditionViewController<ExchangeRateSourceEntity>
	{
		protected override void CreateBricks(BrickWall<ExchangeRateSourceEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Type)
				  .Field (x => x.Name)
				  .Field (x => x.Description)
				  .Field (x => x.Originator)
				.End ()
				;
		}
	}
}
