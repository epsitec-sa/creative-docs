//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionOptionValueArticleParameterDefinitionViewController : EditionViewController<Entities.OptionValueArticleParameterDefinitionEntity>
	{
#if true
		protected override void CreateBricks(BrickWall<OptionValueArticleParameterDefinitionEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Name)
				  .Field (x => x.Description)
				  .Field (x => x.Cardinality)
				.End ()
				;
		}
#else
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

			builder.CreateAutoCompleteTextField (tile, 0, "Cardinalité", Marshaler.Create (() => this.Entity.Cardinality, x => this.Entity.Cardinality = x), EnumKeyValues.FromEnum<EnumValueCardinality> ());
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
#endif
	}
}
