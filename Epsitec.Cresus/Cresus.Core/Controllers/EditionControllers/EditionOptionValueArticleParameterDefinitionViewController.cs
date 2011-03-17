//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.TabIds;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionOptionValueArticleParameterDefinitionViewController : EditionViewController<Entities.OptionValueArticleParameterDefinitionEntity>
	{
		public EditionOptionValueArticleParameterDefinitionViewController(string name, Entities.OptionValueArticleParameterDefinitionEntity entity)
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

				using (var data = TileContainerController.Setup (this))
				{
					this.CreateUIOptions (data);
				}

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateTabBook(UIBuilder builder)
		{
			Common.CreateAbstractArticleParameterTabBook (builder, this, ArticleParameterTabId.Option);
		}


		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField (tile, 80, "Nom court (un ou deux caractères)", Marshaler.Create (() => this.Entity.Name, x => this.Entity.Name = x));
			builder.CreateTextField             (tile,  0, "Nom long",    Marshaler.Create (() => this.Entity.Description, x => this.Entity.Description = x));

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateAutoCompleteTextField (tile, 0, "Cardinalité", Marshaler.Create (() => this.Entity.Cardinality, x => this.Entity.Cardinality = x), EnumKeyValues.FromEnum<EnumValueCardinality> (), x => TextFormatter.FormatText (x));
		}

		private void CreateUIOptions(TileDataItems data)
		{
			data.Add (
				new TileDataItem
				{
					AutoGroup    = true,
					Name		 = "OptionValue",
					IconUri		 = "Data.OptionValue",
					Title		 = TextFormatter.FormatText ("Options"),
					CompactTitle = TextFormatter.FormatText ("Options"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<OptionValueEntity> ("OptionValue", this.BusinessContext);

			template.DefineText        (x => x.GetSummary ());
			template.DefineCompactText (x => x.GetSummary ());

			data.Add (this.CreateCollectionAccessor (template, x => x.Options));
		}
	}
}
