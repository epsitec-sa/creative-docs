//	Copyright � 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Entities;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Bricks;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderRefereeViewController : SummaryViewController<AiderRefereeEntity>
	{
		protected override void CreateBricks(BrickWall<AiderRefereeEntity> wall)
		{
			wall.AddBrick ();
		}
	}
}
