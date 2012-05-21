//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionArticleGroupViewController : EditionViewController<Entities.ArticleGroupEntity>
	{
		protected override void CreateBricks(BrickWall<ArticleGroupEntity> wall)
		{
			wall.AddBrick ()
				.GlobalWarning ()
				.Input ()
				  .Field (x => x.Name)
				  .Field (x => x.Description)
				  .Field (x => x.Type)
				.End ()
				;
		}
	}
}
