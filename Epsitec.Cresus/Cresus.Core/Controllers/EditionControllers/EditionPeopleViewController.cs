//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Factories;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Bricks;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionPeopleViewController : EditionViewController<PeopleEntity>
	{
		protected override void CreateBricks(BrickWall<PeopleEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Person)
				  .Field (x => x.PeopleCategory)
				.End ()
				;
		}
	}
}
