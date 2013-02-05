//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Entities;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderLegalPersonViewController : EditionViewController<AiderLegalPersonEntity>
	{
		protected override void CreateBricks(BrickWall<AiderLegalPersonEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.Field (x => x.Type)
					.Field (x => x.Name)
					.Field (x => x.Language)
				.End ();
		}
	}
}
