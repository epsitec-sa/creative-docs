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
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionArticleCategoryViewController : EditionViewController<Entities.ArticleCategoryEntity>
	{
		public EditionArticleCategoryViewController(string name, Entities.ArticleCategoryEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.Category", "Catégorie");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateWarning               (tile);
			builder.CreateTextField             (tile, 0, "Nom",                    Marshaler.Create (() => this.Entity.Name,                 x => this.Entity.Name = x));
			builder.CreateAutoCompleteTextField (tile, 0, "Code TVA à l'achat",		Marshaler.Create (() => this.Entity.DefaultInputVatCode,  x => this.Entity.DefaultInputVatCode = x),  EnumKeyValues.FromEnum<VatCode> (), x => TextFormatter.FormatText (x));
			builder.CreateAutoCompleteTextField (tile, 0, "Code TVA à la vente",	Marshaler.Create (() => this.Entity.DefaultOutputVatCode, x => this.Entity.DefaultOutputVatCode = x), EnumKeyValues.FromEnum<VatCode> (), x => TextFormatter.FormatText (x));
			builder.CreateTextField             (tile, 0, "Numéro TVA",             Marshaler.Create (() => this.Entity.VatNumber,            x => this.Entity.VatNumber = x));
			builder.CreateAutoCompleteTextField (tile, 0, "Type d'article",         Marshaler.Create (() => this.Entity.ArticleType,          x => this.Entity.ArticleType = x),          EnumKeyValues.FromEnum<ArticleType> (), x => TextFormatter.FormatText (x));
		}
	}
}
