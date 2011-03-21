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
		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				data.Add (
					new TileDataItem
					{
						Name				= "Country",
						IconUri				= "Data.Country",
						Title				= TextFormatter.FormatText ("Pays"),
						CompactTitle		= TextFormatter.FormatText ("Pays"),
						TextAccessor		= this.CreateAccessor (x => x.GetSummary ()),
						CompactTextAccessor = this.CreateAccessor (x => x.GetCompactSummary ()),
						EntityMarshaler		= this.CreateEntityMarshaler (),
					});
			}
		}
	}
}
