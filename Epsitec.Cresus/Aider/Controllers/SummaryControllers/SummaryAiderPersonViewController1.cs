//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;

using System.Linq;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	[ControllerSubType (1)]
	public sealed class SummaryAiderPersonViewController1 : SummaryViewController<AiderPersonEntity>
	{
		protected override void CreateBricks(BrickWall<AiderPersonEntity> wall)
		{
			wall.AddBrick (p => p.Groups)
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.HideRemoveButton)
				.Attribute (BrickMode.AutoGroup)
				// These two actions have been disabled because this functionnality is not yet fully
				// implemented and we dont' want the users to mess up this data.
				//.EnableAction (2)
				//.EnableAction (3)
				.Template ()
					.Text (g => g.GetSummaryWithGroupName ())
				.End ();
		}
	}
}

