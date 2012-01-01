//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Bricks;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	[ControllerSubType (2)]
	public class EditionComptabilitéViewController2 : EditionViewController<ComptabilitéEntity>
	{
		public override double GetPreferredWidth(int columnIndex, int columnCount)
		{
			return 800;
		}

		protected override void CreateBricks(BrickWall<ComptabilitéEntity> wall)
		{
			wall.AddBrick ()
				.Icon ("Comptabilité.Balance")
				.Title ("Balance de vérification")
				.Attribute (BrickMode.FullHeightStretch)
				.Input ()
				  .Field (x => x).WithSpecialController (2)
				.End ()
				;
		}
	}
}
