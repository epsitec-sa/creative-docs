﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
				builder.CreateEditionTitleTile ("Data.ArticleCategory", "Catégorie");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}

		protected override EditionStatus GetEditionStatus()
		{
			return EditionStatus.Valid;
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateWarning               (tile);
			builder.CreateTextField             (tile, 0, "Nom",                    Marshaler.Create (() => this.Entity.Name,                 x => this.Entity.Name = x));
			builder.CreateAutoCompleteTextField (tile, 0, "Code TVA à l'achat",		Marshaler.Create (() => this.Entity.DefaultInputVatCode,  x => this.Entity.DefaultInputVatCode = x),  Business.Enumerations.GetAllPossibleVatCodes (),     x => TextFormatter.FormatText (x.Values[0], "-", x.Values[1]));
			builder.CreateAutoCompleteTextField (tile, 0, "Code TVA à la vente",	Marshaler.Create (() => this.Entity.DefaultOutputVatCode, x => this.Entity.DefaultOutputVatCode = x), Business.Enumerations.GetAllPossibleVatCodes (),     x => TextFormatter.FormatText (x.Values[0], "-", x.Values[1]));
			builder.CreateTextField             (tile, 0, "Numéro TVA",             Marshaler.Create (() => this.Entity.VatNumber,            x => this.Entity.VatNumber = x));
			builder.CreateAutoCompleteTextField (tile, 0, "Type d'article",         Marshaler.Create (() => this.Entity.ArticleType,          x => this.Entity.ArticleType = x),          Business.Enumerations.GetAllPossibleArticleTypes (), x => TextFormatter.FormatText (x.Values[0]));
		}
	}
}
