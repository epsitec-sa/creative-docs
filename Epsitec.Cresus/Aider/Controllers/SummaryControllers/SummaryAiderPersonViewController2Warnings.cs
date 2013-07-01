//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;

using System.Linq;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	[ControllerSubType (2)]
	public sealed class SummaryAiderPersonViewController2Warnings : SummaryViewController<AiderPersonEntity>
	{
		protected override void CreateBricks(BrickWall<AiderPersonEntity> wall)
		{
			wall.AddBrick (p => p.Warnings)
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.HideRemoveButton)
				.Attribute (BrickMode.AutoGroup)
				.Template ()
				.End ();
		}
	}
}
