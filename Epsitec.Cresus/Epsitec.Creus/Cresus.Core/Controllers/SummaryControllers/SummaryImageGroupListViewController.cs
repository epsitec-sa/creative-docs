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
#if false
	[ControllerSubType (6)]
	public class SummaryImageGroupListViewController : SummaryViewController<NaturalPersonEntity>
	{
		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIImageGroups (data);
			}
		}


		private void CreateUIImageGroups(TileDataItems data)
		{
			data.Add (
				new TileDataItem
				{
					AutoGroup    = true,
					Name		 = "ImageGroups",
					IconUri		 = "Data.Group",
					Title		 = TextFormatter.FormatText ("Tous les groupes d'image connus"),
					CompactTitle = TextFormatter.FormatText ("Tous les groupes d'image connus"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<ImageGroupEntity> ("ImageGroups", this.BusinessContext);

			template.DefineText        (x => x.GetSummary ());
			template.DefineCompactText (x => x.GetCompactSummary ());

			data.Add (this.CreateCollectionAccessor (template));
		}
	}
#endif
}
