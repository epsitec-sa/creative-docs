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
	public class SummaryTitleViewController : SummaryViewController<Entities.PersonTitleEntity>
	{
		public SummaryTitleViewController(string name, Entities.PersonTitleEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI(TileContainer container)
		{
			var containerController = new TileContainerController (this, container);
			var data = containerController.DataItems;

			data.Add (
				new SummaryData
				{
					Name				= "PersonTitle",
					IconUri				= "Data.NaturalPerson",
					Title				= UIBuilder.FormatText ("Titre"),
					CompactTitle		= UIBuilder.FormatText ("Titre"),
					TextAccessor		= Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText ("Abrégé: ", x.ShortName, "\n", "Complet: ", x.Name)),
					CompactTextAccessor = Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText (x.Name)),
					EntityMarshaler		= this.EntityMarshaler,
				});

			containerController.GenerateTiles ();
		}
	}
}
