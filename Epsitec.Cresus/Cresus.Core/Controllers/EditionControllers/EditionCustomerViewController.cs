//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Factories;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionCustomerViewController : EditionViewController<CustomerEntity>
	{
		public EditionCustomerViewController(string name, Entities.CustomerEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateBricks()
		{
			this.AddBrick ()
				.Input ()
				  .HorizontalGroup ("N° de client (principal, externe et interne)")
				    .Field (x => x.IdA).Width (74)
				    .Field (x => x.IdB).Width (74)
				    .Field (x => x.IdC).Width (74)
				  .End ()
				.End ();

			this.AddBrick (x => x.Relation)
				.Icon ("none");
		}
	}
}
