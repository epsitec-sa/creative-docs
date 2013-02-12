//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;


namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderHouseholdViewController : SummaryViewController<AiderHouseholdEntity>
	{
		protected override void CreateBricks(BrickWall<AiderHouseholdEntity> wall)
		{
			wall.AddBrick ()
				.EnableAction (0)
				.EnableAction (1);

			wall.AddBrick ()
				.Title (Resources.Text ("Adresse"))
				.Text (this.Entity.Address.GetSummary ())
				.Icon ("Data.AiderAddress")
				.Attribute (BrickMode.SpecialController1);

			wall.AddBrick (h => h.Members)
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.HideRemoveButton)
				.Attribute (BrickMode.DefaultToSummarySubView)
				.Attribute (BrickMode.AutoGroup)
				.EnableAction (2)
				.Template ()
					.Icon ("Base.AiderPerson")
					.Title ("Membres")
					.Text (p => p.GetCompactSummary ())
				.End ();

			wall.AddBrick (x => x.Comment)
				.Attribute (BrickMode.AutoCreateNullEntity);
		}
	}
}
