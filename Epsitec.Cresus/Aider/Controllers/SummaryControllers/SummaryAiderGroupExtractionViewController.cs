//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Bricks;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderGroupExtractionViewController : SummaryViewController<AiderGroupExtractionEntity>
	{
		protected override void CreateBricks(BrickWall<AiderGroupExtractionEntity> wall)
		{
			wall.AddBrick ();
		}
	}
}

