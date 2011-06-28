//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryImageViewController : SummaryViewController<ImageEntity>
	{
		protected override void CreateBricks(Bricks.BrickWall<ImageEntity> wall)
		{
			wall.AddBrick (x => x);

			wall.AddBrick ()
				.Icon ("Data.ImageMiniature")
				.Title ("Miniature")
				.Input ()
				  .Field (x => x.ImageBlob).WithSpecialController ()
				.End ()
				;
		}
	}
}
