//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionRelationViewController : EditionViewController<RelationEntity>
	{
		protected override void CreateBricks(BrickWall<RelationEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.FirstContactDate).Width (90)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.VatNumber)
				  .Field (x => x.TaxMode)
				  .Field (x => x.DefaultCurrencyCode)
				.End ()
				.Include (x => x.Person)
				;
		}
	}
}
