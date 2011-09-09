//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryArticleCategoryViewController : SummaryViewController<ArticleCategoryEntity>
	{
		protected override void CreateBricks(BrickWall<ArticleCategoryEntity> wall)
		{
			wall.AddBrick (x => x);

			wall.AddBrick (x => x.Accounting)
				.Template ()
				.End ()
				;
		}
	}
}