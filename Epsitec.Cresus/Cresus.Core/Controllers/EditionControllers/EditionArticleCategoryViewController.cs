//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Bricks;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionArticleCategoryViewController : EditionViewController<Entities.ArticleCategoryEntity>
	{
		protected override void CreateBricks(BrickWall<ArticleCategoryEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Name)
				  .Field (x => x.Description)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.ArticleType)
				  .Field (x => x.VatRateType)
				  .Field (x => x.UnitOfMeasureCategory)
				  .Field (x => x.DefaultPictures)
				  .Field (x => x.RoundingMode)
				  .Field (x => x.NeverApplyDiscount)
				.End ()
				;
		}
	}
}
