//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionLegalPersonViewController : EditionViewController<Entities.LegalPersonEntity>
	{
		protected override void CreateBricks(BrickWall<LegalPersonEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				 .Field (x => x.Name)
				 .Field (x => x.ShortName)
				 .Field (x => x.LegalPersonType)
				 .Field (x => x.Complement)
				.End ()
				;
			
			base.CreateBricks (wall);
		}
	}
}
