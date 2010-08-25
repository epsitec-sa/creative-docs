//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	[ControllerSubType (2)]
	public class SummaryArticleGroupsViewController : SummaryViewController<ArticleDefinitionEntity>
	{
		public SummaryArticleGroupsViewController(string name, ArticleDefinitionEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIArticleGroups (data);
			}
		}


		private void CreateUIArticleGroups(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "ArticleGroups",
					IconUri		 = "Data.ArticleGroup",
					Title		 = TextFormatter.FormatText ("Groupes d'article"),
					CompactTitle = TextFormatter.FormatText ("Groupes d'article"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<ArticleGroupEntity> ("ArticleGroups", data.Controller, this.DataContext);

			template.DefineText        (x => TextFormatter.FormatText (GetArticleGroupSummary (x)));
			template.DefineCompactText (x => TextFormatter.FormatText (GetArticleGroupSummary (x)));
			template.DefineCreateItem  (this.CreateArticleGroup);

			// AllArticleGroups est une méthode d'extension de ArticleDefinitionEntity !
			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.AllArticleGroups (), template));
		}

		private static FormattedText GetArticleGroupSummary(ArticleGroupEntity group)
		{
			return TextFormatter.FormatText (group.Code, ":", group.Name);
		}

		private ArticleGroupEntity CreateArticleGroup()
		{
			//	Crée un nouvelle ligne dans la facture du type le plus courant, c'est-à-dire ArticleDocumentItemEntity.
			var group = this.DataContext.CreateEmptyEntity<ArticleGroupEntity> ();

			return group;
		}

	}
}
