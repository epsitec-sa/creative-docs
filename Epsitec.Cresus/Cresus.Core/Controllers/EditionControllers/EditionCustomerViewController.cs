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
			wall.AddBrick (x => x)
				.Input ()
				  .Title ("N° de client").Field (x => x.IdA)
				  .Field (x => x.CustomerCategory)
				  .Field (x => x.SalesRepresentative)
				  .Field (x => x.DefaultDebtorBookAccount)
				.End ()
				;
			wall.AddBrick (x => x.MainRelation)
				;
			wall.AddBrick ()
				.Include (x => x.MainRelation.Person)
				;
		}
	}
}
