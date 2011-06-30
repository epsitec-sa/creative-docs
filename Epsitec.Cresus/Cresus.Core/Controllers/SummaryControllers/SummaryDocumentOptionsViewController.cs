//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryDocumentOptionsViewController : SummaryViewController<DocumentOptionsEntity>
	{
		protected override void CreateBricks(BrickWall<DocumentOptionsEntity> wall)
		{
			wall.AddBrick ();
		}
	}
}
