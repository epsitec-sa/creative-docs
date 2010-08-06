//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryArticleCategoryViewController : SummaryViewController<Entities.ArticleCategoryEntity>
	{
		public SummaryArticleCategoryViewController(string name, Entities.ArticleCategoryEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI(TileContainer container)
		{
			this.TileContainerController = new TileContainerController (this, container);
			var data = this.TileContainerController.DataItems;

			data.Add (
				new SummaryData
				{
					Name				= "ArticleCategory",
					IconUri				= "Data.ArticleDefinition",
					Title				= UIBuilder.FormatText ("Catégorie"),
					CompactTitle		= UIBuilder.FormatText ("Catégorie"),
					TextAccessor		= Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText (x.Name)),
					CompactTextAccessor = Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText (x.Name)),
					EntityMarshaler		= this.EntityMarshaler,
				});

			this.TileContainerController.GenerateTiles ();
		}
	}
}
