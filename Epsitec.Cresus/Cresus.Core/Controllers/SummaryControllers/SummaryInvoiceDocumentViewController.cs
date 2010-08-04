﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryInvoiceDocumentViewController : SummaryViewController<Entities.InvoiceDocumentEntity>
	{
		public SummaryInvoiceDocumentViewController(string name, Entities.InvoiceDocumentEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI(TileContainer container)
		{
			this.TileContainerController = new TileContainerController (this, container, this.DataContext);
			var data = this.TileContainerController.DataItems;

			this.CreateUIInvoiceDocument (data);

			this.TileContainerController.GenerateTiles ();
		}

		protected override EditionStatus GetEditionStatus()
		{
			var entity = this.Entity;
			return EditionStatus.Valid;
		}

		protected override void UpdateEmptyEntityStatus(DataLayer.Context.DataContext context, bool isEmpty)
		{
			var entity = this.Entity;
			
			context.UpdateEmptyEntityStatus (entity, isEmpty);
		}

		private void CreateUIInvoiceDocument(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					Name				= "InvoiceDocument",
					IconUri				= "Data.InvoiceDocument",
					Title				= UIBuilder.FormatText ("Facture"),
					CompactTitle		= UIBuilder.FormatText ("Facture"),
					TextAccessor		= Accessor.Create (this.EntityGetter, x => InvoiceDocumentHelper.GetSummary (x)),
					CompactTextAccessor = Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText ("N°", x.IdA)),
					EntityMarshaler		= this.EntityMarshaler,
				});
		}
	}
}
