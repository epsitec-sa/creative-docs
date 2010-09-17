//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryArticleDefinitionViewController : SummaryViewController<Entities.ArticleDefinitionEntity>
	{
		public SummaryArticleDefinitionViewController(string name, Entities.ArticleDefinitionEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIArticleDefinition (data);
				this.CreateUIParameters        (data);
				this.CreateUIPrices            (data);
				this.CreateUIComments          (data);
			}
		}

		private void CreateUIArticleDefinition(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					Name				= "ArticleDefinition",
					IconUri				= "Data.ArticleDefinition",
					Title				= TextFormatter.FormatText ("Article", "(", TextFormatter.Join (", ", this.Entity.ArticleGroups.Select (group => group.Name)), ")"),
					CompactTitle		= TextFormatter.FormatText ("Article"),
					TextAccessor		= this.CreateAccessor (x => TextFormatter.FormatText ("N°~", x.IdA, "\n", x.ShortDescription, "\n", x.LongDescription)),
					CompactTextAccessor = this.CreateAccessor (x => TextFormatter.FormatText ("N°~", x.IdA, "~, ~", x.ShortDescription)),
					EntityMarshaler		= this.CreateEntityMarshaler (),
				});
		}

		private void CreateUIParameters(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "ArticleParameterDefinition",
					IconUri		 = "Data.ArticleParameter",
					Title		 = TextFormatter.FormatText ("Paramètres"),
					CompactTitle = TextFormatter.FormatText ("Paramètres"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<AbstractArticleParameterDefinitionEntity> ("ArticleParameterDefinition", this.BusinessContext);

			template.DefineText (x => x.GetSummary ());
			template.DefineCompactText (x => x.GetSummary ());
			template.CreateItemsOfType<NumericValueArticleParameterDefinitionEntity> ();

			data.Add (this.CreateCollectionAccessor (template, x => x.ArticleParameterDefinitions));
		}

		private void CreateUIPrices(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "ArticlePrice",
					IconUri		 = "Data.ArticlePrice",
					Title		 = TextFormatter.FormatText ("Prix"),
					CompactTitle = TextFormatter.FormatText ("Prix"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<ArticlePriceEntity> ("ArticlePrice", this.BusinessContext);

			template.DefineText (x => x.GetSummary ());
			template.DefineCompactText (x => x.GetSummary ());

			data.Add (this.CreateCollectionAccessor (template, x => x.ArticlePrices));
		}
		
		private void CreateUIComments(SummaryDataItems data)
		{
			SummaryControllers.Common.CreateUIComments (this.DataContext, data, this.EntityGetter, x => x.Comments);
		}


		protected override EditionStatus GetEditionStatus()
		{
			var entity = this.Entity;
			return entity.IsEmpty () ? EditionStatus.Empty : EditionStatus.Valid;
		}
	}
}
