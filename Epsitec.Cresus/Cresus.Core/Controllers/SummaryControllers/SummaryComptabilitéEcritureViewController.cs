//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryComptabilitéEcritureViewController : SummaryViewController<ComptabilitéEcritureEntity>
	{
		protected override void CreateBricks(BrickWall<ComptabilitéEcritureEntity> wall)
		{
			wall.AddBrick (x => x)
				;

#if false
			wall.AddBrick (x => x.Lignes)
				.Title (TextFormatter.FormatText ("Ecritures"))
				.Attribute (BrickMode.AutoGroup)
				.Template ()
				.End ()
				;
#endif
		}
	}
}