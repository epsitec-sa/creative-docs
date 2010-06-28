//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryCountryViewController : SummaryViewController<Entities.CountryEntity>
	{
		public SummaryCountryViewController(string name, Entities.CountryEntity entity)
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
					Name				= "Country",
//?					IconUri				= "Data.Country",
					IconUri				= "Data.Mail",
					Title				= UIBuilder.FormatText ("Pays"),
					CompactTitle		= UIBuilder.FormatText ("Pays"),
					TextAccessor		= Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText ("Pays: ", x.Name, "\n", "Code: ", x.Code)),
					CompactTextAccessor = Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText (x.Name, "(", x.Code, ")")),
					EntityMarshaler		= this.EntityMarshaler,
				});

			containerController.GenerateTiles ();
		}
	}
}
