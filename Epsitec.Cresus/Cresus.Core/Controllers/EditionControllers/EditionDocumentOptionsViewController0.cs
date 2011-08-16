//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

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
	[ControllerSubType (0)]
	public class EditionDocumentOptionsViewController0 : EditionViewController<DocumentOptionsEntity>
	{
		public override double GetPreferredWidth(int columnIndex, int columnCount)
		{
			return 640;
		}

		protected override void CreateBricks(BrickWall<DocumentOptionsEntity> wall)
		{
			wall.AddBrick ()
				.Attribute (BrickMode.FullHeightStretch)
				.Input ()
				  .Field (x => x).WithSpecialController ()
				.End ()
				;
		}
	}
}
