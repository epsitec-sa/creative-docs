﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	[ControllerSubType (0)]
	public class EditionBusinessDocumentViewController0 : EditionViewController<BusinessDocumentEntity>
	{
		public override double GetPreferredWidth(int columnIndex, int columnCount)
		{
			return 600;
		}

		protected override void CreateBricks(BrickWall<BusinessDocumentEntity> wall)
		{
			wall.AddBrick ()
				.Title ("Lignes du document")
				.Icon ("Data.DocumentItems")
				.Input ()
				  .Field (x => x).WithSpecialController ()
				.End ()
				;
		}
	}
}
