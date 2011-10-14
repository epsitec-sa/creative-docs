//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Factories;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Bricks;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionCustomerViewController : EditionViewController<CustomerEntity>
	{
		protected override void CreateBricks(BrickWall<CustomerEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .HorizontalGroup ("N° de client (principal, externe et interne)")
				    .Field (x => x.IdA).Width (72)
				    .Field (x => x.IdB).Width (72)
				    .Field (x => x.IdC).Width (72)
				  .End ()
				  .Field (x => x.DefaultDebtorBookAccount)
				.End ()
				.Include (x => x.MainRelation)
				;
		}
	}
}
