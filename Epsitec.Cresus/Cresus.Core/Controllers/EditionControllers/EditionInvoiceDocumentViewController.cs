//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionInvoiceDocumentViewController : EditionViewController<Entities.InvoiceDocumentEntity>
	{
		public EditionInvoiceDocumentViewController(string name, Entities.InvoiceDocumentEntity entity)
			: base (name, entity)
		{
		}


		protected override EditionStatus GetEditionStatus()
		{
			if (string.IsNullOrEmpty (this.Entity.IdA) &&
				string.IsNullOrEmpty (this.Entity.IdB) &&
				string.IsNullOrEmpty (this.Entity.IdC))
			{
				return EditionStatus.Empty;
			}

			if (string.IsNullOrEmpty (this.Entity.IdA) &&
				(!string.IsNullOrEmpty (this.Entity.IdB) ||
				 !string.IsNullOrEmpty (this.Entity.IdC)))
			{
				return EditionStatus.Invalid;
			}

			return EditionStatus.Valid;
		}

		protected override void UpdateEmptyEntityStatus(DataLayer.Context.DataContext context, bool isEmpty)
		{
			var entity = this.Entity;
			context.UpdateEmptyEntityStatus (entity, isEmpty);
		}

	
		protected override void CreateUI(TileContainer container)
		{
			this.tileContainer = container;

			using (var builder = new UIBuilder (container, this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.InvoiceDocument", "Facture");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}

#if false
			//	Summary:
			this.TileContainerController = new TileContainerController (this, container, this.DataContext);
			var data = this.TileContainerController.DataItems;

			this.TileContainerController.GenerateTiles ();
#endif
		}


		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField      (tile, 150, "Numéro de la facture",           Marshaler.Create (() => this.Entity.IdA, x => this.Entity.IdA = x));
			builder.CreateTextField      (tile, 150, "Numéro externe",                 Marshaler.Create (() => this.Entity.IdB, x => this.Entity.IdB = x));
			builder.CreateTextField      (tile, 150, "Numéro interne",                 Marshaler.Create (() => this.Entity.IdC, x => this.Entity.IdC = x));
			builder.CreateMargin         (tile, horizontalSeparator: true);
			builder.CreateTextFieldMulti (tile,  36, "Texte <i>Concerne</i> imprimé",  Marshaler.Create (() => this.Entity.DocumentTitle, x => this.Entity.DocumentTitle = x));
			builder.CreateTextField      (tile,   0, "Description interne",            Marshaler.Create (() => this.Entity.Description,   x => this.Entity.Description = x));

#if true
			// TODO: à supprimer un jour...
			var button = builder.CreateButton (tile, 0, "Action", "Recalculer la facture (provisoire)");

			button.Clicked += delegate
			{
				InvoiceDocumentHelper.UpdatePrices (this.Entity);
				this.tileContainer.UpdateAllWidgets ();
			};
#endif
		}


		private TileContainer					tileContainer;
	}
}
