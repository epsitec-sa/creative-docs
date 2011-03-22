//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Factories;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionCustomerViewController : EditionViewController<CustomerEntity>
	{
		protected override void CreateBricks(Bricks.BrickWall<CustomerEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .HorizontalGroup ("N° de client (principal, externe et interne)")
				    .Field (x => x.IdA).Width (74)
				    .Field (x => x.IdB).Width (74)
				    .Field (x => x.IdC).Width (74)
				  .End ()
				.End ();

			wall.AddBrick (x => x.Relation)
				.Icon ("none");
			
			this.AddUIController (this.CreateEditionSubController (x => x.Relation));
		}
	}
}
