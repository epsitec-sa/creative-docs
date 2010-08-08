//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

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
	public class EditionArticleGroupViewController : EditionViewController<Entities.ArticleGroupEntity>
	{
		public EditionArticleGroupViewController(string name, Entities.ArticleGroupEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(TileContainer container)
		{
			using (var builder = new UIBuilder (container, this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.ArticleGroup", "Groupe d'article");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}

		protected override EditionStatus GetEditionStatus()
		{
			return EditionStatus.Valid;
		}

		protected override void UpdateEmptyEntityStatus(DataLayer.Context.DataContext context, bool isEmpty)
		{
			var entity = this.Entity;

			context.UpdateEmptyEntityStatus (entity, isEmpty);
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateWarning   (tile);
			builder.CreateTextField (tile, 0, "Code", Marshaler.Create (() => this.Entity.Code, x => this.Entity.Code = x));
			builder.CreateTextField (tile, 0, "Nom", Marshaler.Create (() => this.Entity.Name, x => this.Entity.Name = x));
		}
	}
}
