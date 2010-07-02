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
	public class SummaryLocationViewController : SummaryViewController<Entities.LocationEntity>
	{
		public SummaryLocationViewController(string name, Entities.LocationEntity entity)
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
					Name				= "Location",
					IconUri				= "Data.Location",
					Title				= UIBuilder.FormatText ("Ville"),
					CompactTitle		= UIBuilder.FormatText ("Ville"),
					TextAccessor		= Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText ("Pays: ", x.Country.Name, "\n", "Numéro postal: ", x.PostalCode, "\n", "Ville: ", x.Name)),
					CompactTextAccessor = Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText (x.PostalCode, " ", x.Name)),
					EntityMarshaler		= this.EntityMarshaler,
				});

			containerController.GenerateTiles ();
		}
	}
}
