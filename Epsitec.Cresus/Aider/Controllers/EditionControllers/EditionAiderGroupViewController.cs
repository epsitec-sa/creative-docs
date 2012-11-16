//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderGroupViewController : EditionViewController<AiderGroupEntity>
	{
		protected override void CreateBricks(Cresus.Bricks.BrickWall<AiderGroupEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
					.HorizontalGroup()
						.Title ("Dates de début et de fin")
						.Field (x => x.StartDate)
						.Field (x => x.EndDate)
					.End ()
					.Field (x => x.Path).ReadOnly ()
					.Field (x => x.GroupLevel).ReadOnly ()
					.Field (x => x.GroupDef)
					.Field (x => x.Name)
//-					.Field (x => x.Description)
				.End ()
//-				.Input ()
//-					.Field (x => x.Root)
//-				.End ()
				.Input ()
					.Field (x => x.Comment.Text)
				.End ();
		}
	}
}
