//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionStateProvinceCountyViewController : EditionViewController<StateProvinceCountyEntity>
	{
		protected override void CreateBricks(BrickWall<StateProvinceCountyEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Name)
				  .Field (x => x.RegionCode)
				  .Field (x => x.Country)
				.End ()
				;
		}
	}
}
