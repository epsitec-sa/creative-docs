//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderCountryViewController : EditionViewController<AiderCountryEntity>
	{
		protected override void CreateBricks(BrickWall<AiderCountryEntity> wall)
		{
			if (this.Entity.Mutability == Mutability.SystemDefined)
			{
				wall.AddBrick ()
					.Input ()
						.Field (x => x.Mutability)
							.ReadOnly ()
						.Field (x => x.IsoCode)
							.ReadOnly ()
						.Field (x => x.Name)
							.ReadOnly ()
						.Field (x => x.IsPreferred)
					.End ();
			}
			else
			{
				wall.AddBrick ()
					.Input ()
						.Field (x => x.Mutability)
							.ReadOnly ()
						.Field (x => x.IsoCode)
						.Field (x => x.Name)
						.Field (x => x.IsPreferred)
					.End ();
			}
		}
	}
}
