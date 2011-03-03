//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionEnumValueArticleParameterDefinitionViewController : EditionViewController<Entities.EnumValueArticleParameterDefinitionEntity>
	{
		public EditionEnumValueArticleParameterDefinitionViewController(string name, Entities.EnumValueArticleParameterDefinitionEntity entity)
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
			Common.CreateAbstractArticleParameterTabBook (builder, this, ArticleParameterTabId.Enum);
		}


		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField (tile, 80, "Nom court (un ou deux caractères)", Marshaler.Create (() => this.Entity.Name, x => this.Entity.Name = x));
			builder.CreateTextField             (tile,  0, "Nom long",    Marshaler.Create (() => this.Entity.Description, x => this.Entity.Description = x));
			builder.CreateAutoCompleteTextField (tile,  0, "Cardinalité", Marshaler.Create (() => this.Entity.Cardinality, x => this.Entity.Cardinality = x), EnumKeyValues.FromEnum<EnumValueCardinality> (), x => TextFormatter.FormatText (x.Values[0]));

			builder.CreateMargin (tile, horizontalSeparator: true);

			var group = builder.CreateGroup (tile, "Contenu de l'énumération");
			this.parameterController = new ArticleParameterControllers.ArticleParameterListEnumValuesController (this.TileContainer, this.Entity);
			this.parameterController.CreateUI (group, builder.ReadOnly);
		}


		private ArticleParameterControllers.ArticleParameterListEnumValuesController		parameterController;
	}
}
