//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types.Converters;

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
		protected override void CreateBricks(Bricks.BrickWall<NaturalPersonEntity> wall)
		{
			wall.AddBrick ()
//				.Name ("NaturalPerson")
//				.Icon ("Data.NaturalPerson")
//				.Title (TextFormatter.FormatText ("Personne physique"))
//				.TitleCompact (TextFormatter.FormatText ("Personne physique"))
				.Input ()
				 .Field (x => x.Title)
				.End ()
				.Input ()
				 .Field (x => x.Firstname)
				 .Field (x => x.Lastname)
				.End ()
				.Separator ()
				.Input ()
				 .Field (x => x.Gender)
				.End ()
				.Separator ()
				.Input ()
				 .Field (x => x.DateOfBirth).Width (90)
				.End ()
				.Input ()
				 .Field (x => x.Pictures)
				.End ();
		}
	}
}
