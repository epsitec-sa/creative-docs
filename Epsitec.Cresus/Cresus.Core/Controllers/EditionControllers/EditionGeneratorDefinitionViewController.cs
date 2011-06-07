//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionGeneratorDefinitionViewController : EditionViewController<GeneratorDefinitionEntity>
	{
		protected override void CreateBricks(Bricks.BrickWall<GeneratorDefinitionEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Name)
				  .Field (x => x.Description)
				  .Field (x => x.Entity)
				  .Field (x => x.IdField)
				  .Field (x => x.Format)
				  .Field (x => x.Key)
				.End ();
		}
	}
}
