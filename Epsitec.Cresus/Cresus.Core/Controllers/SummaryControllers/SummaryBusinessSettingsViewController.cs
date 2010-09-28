//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryBusinessSettingsViewController : SummaryViewController<Entities.BusinessSettingsEntity>
	{
		public SummaryBusinessSettingsViewController(string name, Entities.BusinessSettingsEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				data.Add (
					new SummaryData
					{
						Name				= "BusinessSettings",
						IconUri				= "Data.BusinessSettings",
						Title				= TextFormatter.FormatText ("Réglages de l'entreprise"),
						CompactTitle		= TextFormatter.FormatText ("Réglages de l'entreprise"),
						TextAccessor		= this.CreateAccessor (x => TextFormatter.FormatText ("Réglages")),
						CompactTextAccessor = this.CreateAccessor (x => TextFormatter.FormatText ("Réglages")),
						EntityMarshaler		= this.CreateEntityMarshaler (),
					});
			}
		}
	}
}
