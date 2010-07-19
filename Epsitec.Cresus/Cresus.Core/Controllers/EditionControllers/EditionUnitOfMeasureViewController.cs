//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionUnitOfMeasureViewController : EditionViewController<Entities.UnitOfMeasureEntity>
	{
		public EditionUnitOfMeasureViewController(string name, Entities.UnitOfMeasureEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(TileContainer container)
		{
			using (var builder = new UIBuilder (container, this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.UnitOfMeasure", "Unité de mesure");

				this.CreateUIWarning (builder);
				this.CreateUIMain    (builder);

				builder.CreateFooterEditorTile ();
			}
		}

		protected override EditionStatus GetEditionStatus()
		{
			if (string.IsNullOrEmpty (this.Entity.Name) &&
				string.IsNullOrEmpty (this.Entity.Code))
			{
				return EditionStatus.Empty;
			}

			if (!string.IsNullOrEmpty (this.Entity.Name) ||
				!string.IsNullOrEmpty (this.Entity.Code))
			{
				return EditionStatus.Invalid;
			}

			return EditionStatus.Valid;
		}

		protected override void UpdateEmptyEntityStatus(DataLayer.DataContext context, bool isEmpty)
		{
			var entity = this.Entity;

			context.UpdateEmptyEntityStatus (entity, isEmpty);
		}


		private void CreateUIWarning(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateWarning (tile);
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin    (tile, horizontalSeparator: true);
			builder.CreateTextField (tile,   0, "Nom",            Marshaler.Create (() => this.Entity.Name,              x => this.Entity.Name = x));
			builder.CreateTextField (tile, 100, "Code",           Marshaler.Create (() => this.Entity.Code,              x => this.Entity.Code = x));
			builder.CreateAutoCompleteTextField (tile, 100, "Catégorie", Marshaler.Create (() => this.Entity.Category, x => this.Entity.Category = x), BusinessLogic.Enumerations.GetGetAllPossibleItemsUnitOfMeasureCategory (), x => UIBuilder.FormatText (x.Values));

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateTextField (tile,  50, "Diviseur",       Marshaler.Create (() => this.Entity.DivideRatio,       x => this.Entity.DivideRatio = x));
			builder.CreateTextField (tile,  50, "Multiplicateur", Marshaler.Create (() => this.Entity.MultiplyRatio,     x => this.Entity.MultiplyRatio = x));
			builder.CreateTextField (tile,  50, "Incrément",      Marshaler.Create (() => this.Entity.SmallestIncrement, x => this.Entity.SmallestIncrement = x));
			
		}
	}
}
