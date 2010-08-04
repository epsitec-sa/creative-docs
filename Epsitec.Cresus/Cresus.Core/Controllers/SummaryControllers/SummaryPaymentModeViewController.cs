﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			this.TileContainerController = new TileContainerController (this, container, this.DataContext);
			var data = this.TileContainerController.DataItems;

			data.Add (
				new SummaryData
				{
					Name				= "PaymentMode",
					IconUri				= "Data.PaymentMode",
					Title				= UIBuilder.FormatText ("Mode de paiement"),
					CompactTitle		= UIBuilder.FormatText ("Mode de paiement"),
					TextAccessor		= Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText ("Code: ", x.Code, "\n", "Résumé: ", x.Name, "\n", "Description: ", x.Description, "\n", "Compte: ", x.BookAccount)),
					CompactTextAccessor = Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText (x.Name)),
					EntityMarshaler		= this.EntityMarshaler,
				});

			this.TileContainerController.GenerateTiles ();
		}
	}
}
