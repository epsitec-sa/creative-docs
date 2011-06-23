//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryContactGroupViewController : SummaryViewController<ContactGroupEntity>
	{
		protected override void CreateBricks(Bricks.BrickWall<ContactGroupEntity> wall)
		{
			wall.AddBrick (x => x);
		}
	}
}
