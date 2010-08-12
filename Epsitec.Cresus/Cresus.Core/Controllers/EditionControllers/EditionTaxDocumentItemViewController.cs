﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;

using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionTaxDocumentItemViewController : EditionViewController<Entities.TaxDocumentItemEntity>
	{
		public EditionTaxDocumentItemViewController(string name, Entities.TaxDocumentItemEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.TaxDocumentItem", "Ligne de TVA");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField (tile,  0, "Description",        Marshaler.Create (() => this.Entity.Text, x => this.Entity.Text = x));
			builder.CreateTextField (tile, 80, "Taux de TVA",        Marshaler.Create (() => this.Entity.Rate, x => this.Entity.Rate = x));
			builder.CreateTextField (tile, 80, "Montant de base HT", Marshaler.Create (() => this.Entity.BaseAmount, x => this.Entity.BaseAmount = x));
			builder.CreateTextField (tile, 80, "TVA due",            Marshaler.Create (() => this.Entity.ResultingTax, x => this.Entity.ResultingTax = x));
		}


		protected override EditionStatus GetEditionStatus()
		{
			return EditionStatus.Valid;
		}
	}
}
