//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionArticleQuantityViewController : EditionViewController<Entities.ArticleQuantityEntity>
	{
		protected override void CreateBricks(BrickWall<ArticleQuantityEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.QuantityColumn)  // TODO: Comment faire pour utiliser UIBuilder.CreateEditionDetailedItemPicker et avoir des boutons radio ?
				  .HorizontalGroup ()
					.Title ("Date prévue (début et fin si connus)")
				    .Field (x => x.BeginDate)
					.Field (x => x.EndDate)
				  .End ()
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.Quantity)
				  .Field (x => x.Unit)  // TODO: Comment faire pour avoir des boutons radio ?
				.End ()
				;
		}
	}
}
