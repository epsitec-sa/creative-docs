//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Aider.Controllers.ActionControllers;

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SpecializedSummaryAiderPersonWarningViewController_EChPersonMissing : SpecializedSummaryAiderPersonWarningViewController
	{
		protected override void CreateBricks(BrickWall<AiderPersonWarningEntity> wall)
		{
			wall.AddBrick ()
				.Title (x => x.WarningType)
				.Attribute (BrickMode.DefaultToSummarySubView)
				.WithSpecialController (typeof (SummaryAiderPersonWarningViewController1Details))
/*				.EnableActionButton<ActionAiderPersonWarningViewController10ProcessPersonChanges> () */;
#if false
			if (this.Entity.Person.Contacts.Count > 0)
			{
				wall.AddBrick (x => x.Person.Contacts.Where (c => c.Household.Address.IsNotNull ()).First ())
					.Title ("Nouvelle adresse (si connue)")
					.Icon ("Data.AiderAddress")
					.Text ("(merci de saisir une adresse hors du canton)")
					.WithSpecialController (typeof (EditionAiderContactViewController1Address));
			}

			wall.AddBrick (x => x.Person)
					.Title ("En cas de décès (entrer une date)")
					.Icon (this.Entity.Person.GetIconName ("Data"))
					.WithSpecialController (typeof (EditionAiderPersonViewController0DeceaseDateController));

			this.AddDefaultBrick (wall)
				.EnableActionButton<ActionAiderPersonWarningViewController1ProcessPersonMissing> ();
#endif
		}
	}
}
