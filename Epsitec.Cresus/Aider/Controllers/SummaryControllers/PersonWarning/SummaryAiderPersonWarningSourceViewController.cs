//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Controllers.EditionControllers;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Entities;
using System.Linq;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderPersonWarningSourceViewController : SummaryViewController<AiderPersonWarningSourceEntity>
	{
		protected override void CreateBricks(BrickWall<AiderPersonWarningSourceEntity> wall)
		{
			base.CreateBricks (wall);

			wall.AddBrick (x => x.PersonWarnings);
		}
	}
}

