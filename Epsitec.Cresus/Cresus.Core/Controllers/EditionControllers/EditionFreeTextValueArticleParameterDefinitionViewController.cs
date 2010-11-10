﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.TabIds;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionFreeTextValueArticleParameterDefinitionViewController : EditionViewController<Entities.FreeTextValueArticleParameterDefinitionEntity>
	{
		public EditionFreeTextValueArticleParameterDefinitionViewController(string name, Entities.FreeTextValueArticleParameterDefinitionEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.ArticleParameter", "Paramètre");

				this.CreateTabBook (builder);
				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateTabBook(UIBuilder builder)
		{
			Common.CreateAbstractArticleParameterTabBook (builder, this, ArticleParameterTabId.FreeText);
		}


		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField      (tile, 80, "Code",        Marshaler.Create (() => this.Entity.Code, x => this.Entity.Code = x));
			builder.CreateTextField      (tile,  0, "Nom",         Marshaler.Create (() => this.Entity.Name, x => this.Entity.Name = x));

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateTextField      (tile,  0, "Texte court", Marshaler.Create (() => this.Entity.ShortText, x => this.Entity.ShortText = x));
			builder.CreateTextFieldMulti (tile, 52, "Texte long",  Marshaler.Create (() => this.Entity.LongText,  x => this.Entity.LongText = x));
		}
	}
}
