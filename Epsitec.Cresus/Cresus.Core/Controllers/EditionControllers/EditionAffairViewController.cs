//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Orchestrators.Navigation;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.Bricks;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionAffairViewController : EditionViewController<Entities.AffairEntity>
	{
		protected override void CreateBricks(BrickWall<AffairEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Title ("N° de l'affaire").Field (x => x.IdA)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.ActiveSalesRepresentative)
				  .Field (x => x.ActiveAffairOwner)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.DebtorBookAccount)
				.End ()
				;
		}
	}
}
