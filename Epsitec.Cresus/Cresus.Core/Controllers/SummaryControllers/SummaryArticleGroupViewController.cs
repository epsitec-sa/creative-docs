//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryArticleGroupViewController : SummaryViewController<Entities.ArticleGroupEntity>
	{
		protected override void CreateBricks(BrickWall<ArticleGroupEntity> wall)
		{
			wall.AddBrick (x => x);
		}
	}
}
