//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionPersonTitleViewController : EditionViewController<PersonTitleEntity>
	{
		protected override void CreateBricks(Bricks.BrickWall<PersonTitleEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Name)
				  .Field (x => x.ShortName)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.CompatibleGenders)
				.End ()
				;
		}
	}
}
