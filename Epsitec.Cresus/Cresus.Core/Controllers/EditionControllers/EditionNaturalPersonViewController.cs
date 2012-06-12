//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionNaturalPersonViewController : EditionViewController<NaturalPersonEntity>
	{
		protected override void CreateBricks(BrickWall<NaturalPersonEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Title)
				.End ()
				.Input ()
#if SLIMFIELD
				  .HorizontalGroup ()
				    .Field (x => x.Firstname)
				    .Field (x => x.Lastname)
				  .End ()
#else
				  .Field (x => x.Firstname)
				  .Field (x => x.Lastname)
#endif
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.PreferredLanguage)
				  .Field (x => x.Gender)
				  .Field (x => x.DateOfBirth)
				.End ()
				.Input ()
				  .Field (x => x.Pictures)
				.End ()
				;
		}
	}
}
