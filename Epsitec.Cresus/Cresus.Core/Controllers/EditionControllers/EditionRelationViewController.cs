//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionRelationViewController : EditionViewController<RelationEntity>
	{
		protected override void CreateBricks(Bricks.BrickWall<RelationEntity> wall)
		{
			wall.AddBrick ()
				.Name ("Customer")
				.Icon ("Data.Customer")
				.Title ("Client")
				.Input ()
				  .Field (x => x.FirstContactDate).Width (90)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.VatNumber)
				  .Field (x => x.TaxMode)
				  .Field (x => x.DefaultDebtorBookAccount)
				  .Field (x => x.DefaultCurrencyCode)
				.End ();

			this.AddUIController (this.CreateEditionSubController (x => x.Person));
		}
	}
}
