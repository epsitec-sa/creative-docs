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
	public class SummaryPaymentModeViewController : SummaryViewController<Entities.PaymentModeEntity>
	{
		public SummaryPaymentModeViewController(string name, Entities.PaymentModeEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI(TileContainer container)
		{
			using (var data = TileContainerController.Setup (container))
			{
				data.Add (
					new SummaryData
					{
						Name				= "PaymentMode",
						IconUri				= "Data.PaymentMode",
						Title				= TextFormater.FormatText ("Mode de paiement"),
						CompactTitle		= TextFormater.FormatText ("Mode de paiement"),
						TextAccessor		= Accessor.Create (this.EntityGetter, x => TextFormater.FormatText ("Code: ", x.Code, "\n", "Résumé: ", x.Name, "\n", "Description: ", x.Description, "\n", "Compte: ", x.BookAccount)),
						CompactTextAccessor = Accessor.Create (this.EntityGetter, x => TextFormater.FormatText (x.Name)),
						EntityMarshaler		= this.EntityMarshaler,
					});
			}
		}
	}
}
