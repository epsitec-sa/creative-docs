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
	public class EditionOtherRelationViewController : EditionViewController<OtherRelationEntity>
	{
		protected override void CreateBricks(BrickWall<OtherRelationEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Name)
				  .Field (x => x.Description)
//				  .Field (x => x.Person)
				  .Field (x => x.Groups)
				.End ()
				.Include (x => x.Person)
				;
		}
	}
}
