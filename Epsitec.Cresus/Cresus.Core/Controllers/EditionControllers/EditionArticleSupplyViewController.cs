//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Bricks;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionArticleSupplyViewController : EditionViewController<ArticleSupplyEntity>
	{
		protected override void CreateBricks(BrickWall<ArticleSupplyEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Name)
				  .Field (x => x.Description)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.StockLocation)
				  .Field (x => x.RestockingDelay)
				  .Field (x => x.RestockingThreshold)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.SupplierRelation)
				  .Field (x => x.SupplierArticleId)
				  .Field (x => x.SupplierArticlePrices)
				.End ()
				;
		}
	}
}
