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
				  .Title ("Nom du chantier").Field (x => x.Name)
				  .Title ("Description du chantier").Field (x => x.Description)
				  .Field (x => x.Groups)
				.End ()
				.Include (x => x.Person)	//	TODO: Include pas possible si x.Person pas initialisé au préalable !
				;
		}
	}
}
