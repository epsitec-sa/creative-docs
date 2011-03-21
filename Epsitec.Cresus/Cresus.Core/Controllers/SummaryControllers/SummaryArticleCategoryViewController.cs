//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

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
	public class SummaryArticleCategoryViewController : SummaryViewController<Entities.ArticleCategoryEntity>
	{
		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUICategory    (data);
				this.CreateUIAccountings (data);
			}
		}

		private void CreateUICategory(TileDataItems data)
		{
			data.Add (
				new TileDataItem
				{
					Name				= "ArticleCategory",
					IconUri				= "Data.Category",
					Title				= TextFormatter.FormatText ("Catégorie"),
					CompactTitle		= TextFormatter.FormatText ("Catégorie"),
					TextAccessor		= this.CreateAccessor (x => x.GetSummary ()),
					CompactTextAccessor = this.CreateAccessor (x => x.GetCompactSummary ()),
					EntityMarshaler		= this.CreateEntityMarshaler (),
				});
		}

		private void CreateUIAccountings(TileDataItems data)
		{
			data.Add (
				new TileDataItem
				{
					AutoGroup    = true,
					Name		 = "ArticleAccountingDefinitions",
					IconUri		 = "Data.ArticleAccountingDefinition",
					Title		 = TextFormatter.FormatText ("Comptabilisation par défaut"),
					CompactTitle = TextFormatter.FormatText ("Comptabilisation par défaut"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<ArticleAccountingDefinitionEntity> ("ArticleAccountingDefinitions", this.BusinessContext);

			template.DefineText        (x => x.GetSummary ());
			template.DefineCompactText (x => x.GetCompactSummary ());

			data.Add (this.CreateCollectionAccessor (template, x => x.DefaultAccounting));
		}
	}
}