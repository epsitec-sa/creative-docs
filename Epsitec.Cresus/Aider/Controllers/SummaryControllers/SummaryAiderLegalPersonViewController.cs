//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers.EditionControllers;
using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;

using Epsitec.Common.Support;

using System.Linq;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderLegalPersonViewController : SummaryViewController<AiderLegalPersonEntity>
	{
		protected override void CreateBricks(Cresus.Bricks.BrickWall<AiderLegalPersonEntity> wall)
		{
			wall.AddBrick ();

			wall.AddBrick ()
				.Title ("Adresse de base")
				.Icon ("Data.AiderAddress")
				.WithSpecialController (typeof (EditionAiderLegalPersonViewController1Address));

			var contacts = this.Entity.Contacts;

			if (contacts.Any ())
			{
				wall.AddBrick (x => x.Contacts)
					.Title (contacts.Count > 1 ? Resources.Text ("Contacts") : Resources.Text ("Contact"))
					.Attribute (BrickMode.HideAddButton)
					.Attribute (BrickMode.HideRemoveButton)
					.Attribute (BrickMode.AutoGroup)
					.Template ()
					.End ()
					.Attribute (BrickMode.DefaultToSummarySubView);
			}
		}
	}
}
