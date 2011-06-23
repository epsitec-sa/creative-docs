//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryUnitOfMeasureViewController : SummaryViewController<UnitOfMeasureEntity>
	{
#if true
		protected override void CreateBricks(Bricks.BrickWall<UnitOfMeasureEntity> wall)
		{
			wall.AddBrick (x => x);
		}
#else
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
