//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryUnitOfMeasureViewController : SummaryViewController<UnitOfMeasureEntity>
	{
#if false
		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				data.Add (
					new TileDataItem
					{
						Name				= "UnitOfMeasure",
						IconUri				= "Data.UnitOfMeasure",
						Title				= TextFormatter.FormatText ("Unité de mesure"),
						CompactTitle		= TextFormatter.FormatText ("Unité"),
						TextAccessor		= this.CreateAccessor (x => x.GetSummary ()),
						CompactTextAccessor = this.CreateAccessor (x => x.GetCompactSummary ()),
						EntityMarshaler		= this.CreateEntityMarshaler (),
					});
			}
		}
#endif
	}
}
