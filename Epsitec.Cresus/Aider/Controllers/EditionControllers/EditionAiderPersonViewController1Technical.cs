//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	[ControllerSubType (1)]
	public sealed class EditionAiderPersonViewController1Technical : EditionViewController<AiderPersonEntity>
	{
		protected override void CreateBricks(BrickWall<AiderPersonEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.Visibility)
					.Field (x => x.eCH_Person.DataSource)
					.Field (x => x.eCH_Person.DeclarationStatus)
				.End ();
		}
	}
}

