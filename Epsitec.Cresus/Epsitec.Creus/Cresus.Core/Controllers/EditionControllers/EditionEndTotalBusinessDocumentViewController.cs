//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	[ControllerSubType (1)]
	public class EditionEndTotalBusinessDocumentViewController : EditionViewController<BusinessDocumentEntity>
	{
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.TotalDocumentItem", "Total");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}

		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField (tile, 80, false, "Total arrêté TTC", Marshaler.Create (() => this.GetFixedPriceTTC (), this.SetFixedPriceTTC));
		}


		private string GetFixedPriceTTC()
		{
			return Misc.PriceToString (InvoiceDocumentHelper.GetFixedPriceTTC (this.Entity));
		}

		private void SetFixedPriceTTC(string value)
		{
			InvoiceDocumentHelper.SetFixedPriceTTC (this.Entity, Misc.StringToDecimal (value));
		}
	}
}
