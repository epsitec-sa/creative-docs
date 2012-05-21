//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	[ControllerSubType (2)]
	public class SummaryArticleGroupListViewController : SummaryViewController<ArticleDefinitionEntity>
	{
		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIArticleGroups (data);
			}
		}


		private void CreateUIArticleGroups(TileDataItems data)
		{
			data.Add (
				new TileDataItem
				{
					AutoGroup    = true,
					Name		 = "ArticleGroups",
					IconUri		 = "Data.Group",
					Title		 = TextFormatter.FormatText ("Tous les groupes d'article connus"),
					CompactTitle = TextFormatter.FormatText ("Tous les groupes d'article connus"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<ArticleGroupEntity> ("ArticleGroups", this.BusinessContext);

			template.DefineText        (x => x.GetSummary ());
			template.DefineCompactText (x => x.GetCompactSummary ());

			data.Add (this.CreateCollectionAccessor (template));
		}
	}
}
