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
		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				data.Add (
					new TileDataItem
					{
						Name				= "Location",
						IconUri				= "Data.Location",
						Title				= TextFormatter.FormatText ("Ville"),
						CompactTitle		= TextFormatter.FormatText ("Ville"),
						TextAccessor		= this.CreateAccessor (x => x.GetSummary ()),
						CompactTextAccessor = this.CreateAccessor (x => x.GetCompactSummary ()),
						EntityMarshaler		= this.CreateEntityMarshaler (),
					});
			}
		}
	}
}
