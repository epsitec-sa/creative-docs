//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionPaymentModeViewController : EditionViewController<Entities.PaymentModeEntity>
	{
		public EditionPaymentModeViewController(string name, Entities.PaymentModeEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(TileContainer container)
		{
			using (var builder = new UIBuilder (container, this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.PaymentMode", "Mode de paiement");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}

		protected override EditionStatus GetEditionStatus()
		{
			if (string.IsNullOrEmpty (this.Entity.Name))
			{
				return EditionStatus.Empty;
			}

			return EditionStatus.Valid;
		}

		protected override void UpdateEmptyEntityStatus(DataLayer.Context.DataContext context, bool isEmpty)
		{
			var entity = this.Entity;

			context.UpdateEmptyEntityStatus (entity, isEmpty);
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateWarning        (tile);
			builder.CreateTextField      (tile, 80, "Code",               Marshaler.Create (() => this.Entity.Code,        x => this.Entity.Code = x));
			builder.CreateTextField      (tile,  0, "Résumé",             Marshaler.Create (() => this.Entity.Name,        x => this.Entity.Name = x));
			builder.CreateTextFieldMulti (tile, 70, "Description",        Marshaler.Create (() => this.Entity.Description, x => this.Entity.Description = x));
			builder.CreateMargin         (tile, horizontalSeparator: true);
			builder.CreateTextField      (tile, 80, "Compte à créditer",  Marshaler.Create (() => this.Entity.BookAccount, x => this.Entity.BookAccount = x));
		}
	}
}
