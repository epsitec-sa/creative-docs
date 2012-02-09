//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Entities;

using System.Linq;

namespace Epsitec.Aider.Controllers.SummaryControllers
{


	[ControllerSubType (2)]
	public sealed class SummaryAiderPersonViewController2 : SummaryViewController<AiderPersonEntity>
	{


		protected override void CreateBricks(BrickWall<AiderPersonEntity> wall)
		{
			if (this.Entity.Housemates.Any ())
			{
				wall.AddBrick (x => x.Housemates)
						.Template ()
					.End ()
					.Attribute (BrickMode.DefaultToSummarySubView);
			}

			if (this.Entity.Parents.Any ())
			{
				wall.AddBrick (x => x.Parents)
						.Template ()
					.End ()
					.Attribute (BrickMode.DefaultToSummarySubView);
			}

			if (this.Entity.Children.Any ())
			{
				wall.AddBrick (x => x.Children)
						.Template ()
					.End ()
					.Attribute (BrickMode.DefaultToSummarySubView);
			}
		}


	}


}

