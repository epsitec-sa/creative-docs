//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryArticleDefinitionViewController : SummaryViewController<Entities.ArticleDefinitionEntity>
	{
#if true
		protected override void CreateBricks(Bricks.BrickWall<ArticleDefinitionEntity> wall)
		{
			wall.AddBrick ()
				;
			wall.AddBrick (x => x.ArticleParameterDefinitions)  // TODO: pas correct !
				.Template ()
				.End ()
				;
			wall.AddBrick (x => x.ArticlePrices)
				.Template ()
				.End ()
				;
			wall.AddBrick (x => x.Accounting)
				.Template ()
				.End ()
				;
			wall.AddBrick (x => x.ArticleSupplies)
				.Template ()
				.End ()
				;
			wall.AddBrick (x => x.Comments)
				.Template ()
				.End ()
				;
		}
#else
		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIArticleDefinition (data);
				this.CreateUIParameters        (data);
				this.CreateUIPrices            (data);
				this.CreateUIAccountings       (data);
				this.CreateUIComments          (data);
			}
		}

		private void CreateUIArticleDefinition(TileDataItems data)
		{
			data.Add (
				new TileDataItem
				{
					Name				= "ArticleDefinition",
					IconUri				= "Data.ArticleDefinition",
					Title				= TextFormatter.FormatText ("Article", "(", TextFormatter.Join (", ", this.Entity.ArticleGroups.Select (group => group.Name)), ")"),
					CompactTitle		= TextFormatter.FormatText ("Article"),
					TextAccessor		= this.CreateAccessor (x => x.GetSummary ()),
					CompactTextAccessor = this.CreateAccessor (x => x.GetCompactSummary ()),
					EntityMarshaler		= this.CreateEntityMarshaler (),
				});
		}

		private void CreateUIParameters(TileDataItems data)
		{
			data.Add (
				new TileDataItem
				{
					AutoGroup    = true,
					Name		 = "ArticleParameterDefinition",
					IconUri		 = "Data.ArticleParameter",
					Title		 = TextFormatter.FormatText ("Paramètres"),
					CompactTitle = TextFormatter.FormatText ("Paramètres"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<AbstractArticleParameterDefinitionEntity> ("ArticleParameterDefinition", this.BusinessContext);

			template.DefineText        (x => x.GetSummary ());
			template.DefineCompactText (x => x.GetSummary ());
			template.CreateItemsOfType<NumericValueArticleParameterDefinitionEntity> ();

			data.Add (this.CreateCollectionAccessor (template, x => x.ArticleParameterDefinitions));
		}

		private void CreateUIPrices(TileDataItems data)
		{
			data.Add (
				new TileDataItem
				{
					AutoGroup    = true,
					Name		 = "ArticlePrice",
					IconUri		 = "Data.ArticlePrice",
					Title		 = TextFormatter.FormatText ("Prix"),
					CompactTitle = TextFormatter.FormatText ("Prix"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<ArticlePriceEntity> ("ArticlePrice", this.BusinessContext);

			template.DefineText        (x => x.GetSummary ());
			template.DefineCompactText (x => x.GetSummary ());

			data.Add (this.CreateCollectionAccessor (template, x => x.ArticlePrices));
		}

		private void CreateUIAccountings(TileDataItems data)
		{
			data.Add (
				new TileDataItem
				{
					AutoGroup    = true,
					Name		 = "ArticleAccountingDefinitions",
					IconUri		 = "Data.ArticleAccountingDefinition",
					Title		 = TextFormatter.FormatText ("Comptabilisation"),
					CompactTitle = TextFormatter.FormatText ("Comptabilisation"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<ArticleAccountingDefinitionEntity> ("ArticleAccountingDefinitions", this.BusinessContext);

			template.DefineText        (x => x.GetSummary ());
			template.DefineCompactText (x => x.GetCompactSummary ());

			data.Add (this.CreateCollectionAccessor (template, x => x.Accounting));
		}

		private void CreateUIComments(TileDataItems data)
		{
			SummaryControllers.Common.CreateUIComments (this.BusinessContext, data, this.EntityGetter, x => x.Comments);
		}
#endif
	}
}
