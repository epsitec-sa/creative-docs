//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionUnitOfMeasureViewController : EditionViewController<Entities.UnitOfMeasureEntity>
	{
#if true
		protected override void CreateBricks(BrickWall<UnitOfMeasureEntity> wall)
		{
			wall.AddBrick ()
				.GlobalWarning ()
				.Input ()
				  .Field (x => x.Name)
				  .Field (x => x.Description)
				  .Field (x => x.Category).Width (100)
				  .Field (x => x.DivideRatio)
				  .Field (x => x.MultiplyRatio)
				  .Field (x => x.SmallestIncrement)
				.End ()
				;
		}
#endif

#if false
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.UnitOfMeasure", "Unité de mesure");

				this.CreateUIWarning (builder);
				this.CreateUIMain    (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIWarning(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateWarning (tile);
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField             (tile,   0, "Nom",       Marshaler.Create (() => this.Entity.Name,     x => this.Entity.Name = x));
			builder.CreateAutoCompleteTextField (tile, 100, "Catégorie", Marshaler.Create (() => this.Entity.Category, x => this.Entity.Category = x), EnumKeyValues.FromEnum<UnitOfMeasureCategory> ());

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateTextField (tile, 50, "Diviseur",       Marshaler.Create (() => this.Entity.DivideRatio,       x => this.Entity.DivideRatio = x));
			builder.CreateTextField (tile, 50, "Multiplicateur", Marshaler.Create (() => this.Entity.MultiplyRatio,     x => this.Entity.MultiplyRatio = x));
			builder.CreateTextField (tile, 50, "Incrément",      Marshaler.Create (() => this.Entity.SmallestIncrement, x => this.Entity.SmallestIncrement = x));
		}
#endif
	}
}
