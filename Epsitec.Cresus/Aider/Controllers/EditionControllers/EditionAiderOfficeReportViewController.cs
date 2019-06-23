//	Copyright © 2014-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: ~Pierre ARNAUD~

using Epsitec.Aider.Entities;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderOfficeReportViewController : EditionViewController<AiderOfficeReportEntity>
	{
		protected override void CreateBricks(BrickWall<AiderOfficeReportEntity> wall)
		{
            wall.AddBrick ()
                .Input ()
                    .Field (x => x.Name).ReadOnly ()
                .End ()
                .Input ()
                    .Field (x => x.CreationDate).ReadOnly ()
                .End ()
                .Input ()
                    .Field (x => x.ProcessingDate).ReadOnly ()
                .End ()
                .Input ()
                    .Field (x => x.RemovalDate)
                .End ();
        }
    }
}
