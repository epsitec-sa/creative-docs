//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryBusinessSettingsViewController : SummaryViewController<Entities.BusinessSettingsEntity>
	{
		protected override void CreateBricks(Bricks.BrickWall<BusinessSettingsEntity> wall)
		{
			wall.AddBrick ()
				.Name ("BusinessSettings")
				.Text (x => x.Company.Person.GetSummary ());

			wall.AddBrick (x => x.Finance.IsrDefs)
				.Name ("IsrDefinition")
				.Template ()
				 .Text (x => x.GetSummary ())
				.End ();

			wall.AddBrick (x => x.Finance.PaymentReminderDefs)
				.Name ("PaymentReminderDefinition")
				.Attribute (BrickMode.AutoGroup)
				.Template ()
				 .Text (x => x.GetSummary ())
				.End ();

			wall.AddBrick (x => x.Tax.VatDefinitions)
				.Template ()
				 .Text (x => x.GetSummary ())
				 .TextCompact (x => x.GetCompactSummary ())
				.End ();

			wall.AddBrick (x => x.Generators)
				.Template ()
				 .Text (x => x.GetSummary ())
				 .TextCompact (x => x.GetCompactSummary ())
				.End ();
		}
	}
}
