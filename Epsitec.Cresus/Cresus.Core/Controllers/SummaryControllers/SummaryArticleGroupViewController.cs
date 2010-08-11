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
	public class SummaryArticleGroupViewController : SummaryViewController<Entities.ArticleGroupEntity>
	{
		public SummaryArticleGroupViewController(string name, Entities.ArticleGroupEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI(TileContainer container)
		{
			using (var data = TileContainerController.Setup (container))
			{
				data.Add (
					new SummaryData
					{
						Name				= "ArticleGroup",
						IconUri				= "Data.ArticleGroup",
						Title				= TextFormater.FormatText ("Groupe d'articles"),
						CompactTitle		= TextFormater.FormatText ("Groupe"),
						TextAccessor		= Accessor.Create (this.EntityGetter, x => TextFormater.FormatText (x.Name)),
						CompactTextAccessor = Accessor.Create (this.EntityGetter, x => TextFormater.FormatText (x.Name)),
						EntityMarshaler		= this.EntityMarshaler,
					});
			}
		}
	}
}
