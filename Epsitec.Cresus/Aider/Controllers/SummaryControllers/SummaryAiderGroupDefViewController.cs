//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Bricks;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderGroupDefViewController : SummaryViewController<AiderGroupDefEntity>
	{
		protected override void CreateBricks(BrickWall<AiderGroupDefEntity> wall)
		{
			wall.AddBrick ()
				.Attribute (BrickMode.DefaultToNoSubView).IfFalse (this.HasUserPowerLevel (UserPowerLevel.Administrator));

			wall.AddBrick (x => x.Subgroups)
				.Attribute (BrickMode.DefaultToSummarySubView)
				.Attribute (BrickMode.AutoGroup)
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.HideRemoveButton)
				.Template ()
					.Title ("Sous groupes")
				.End ();

			wall.AddBrick (x => x.Comment)
				.Attribute (BrickMode.AutoCreateNullEntity);
		}
	}
}
