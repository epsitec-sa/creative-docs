//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;

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
			var containerController = new TileContainerController (this, container);
			var data = containerController.DataItems;

			this.CreateUIInvoiceDocument (data);

			containerController.GenerateTiles ();
		}

		protected override EditionStatus GetEditionStatus()
		{
			var entity = this.Entity;
			return EditionStatus.Valid;
		}

		protected override void UpdateEmptyEntityStatus(DataLayer.DataContext context, bool isEmpty)
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
					IconUri				= "Data.Document",
					Title				= UIBuilder.FormatText ("Facture"),
					CompactTitle		= UIBuilder.FormatText ("Facture"),
					TextAccessor		= Accessor.Create (this.EntityGetter, x => this.GetText (x)),
					CompactTextAccessor = Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText ("N°", x.IdA)),
					EntityMarshaler		= this.EntityMarshaler,
				});
		}


		private FormattedText GetText(Entities.InvoiceDocumentEntity x)
		{
			string date = this.GetDate (x);
			string total = Misc.DecimalToString (this.GetTotal (x));

			return UIBuilder.FormatText ("N°", x.IdA, "/~", x.IdB, "/~", x.IdC, "\n", "Date: ", date, "\n", "Total: ", total);
		}

		private string GetDate(Entities.InvoiceDocumentEntity x)
		{
			System.DateTime date;
			if (x.LastModificationDate.HasValue)
			{
				date = x.LastModificationDate.Value;
			}
			else
			{
				date = System.DateTime.Now;
			}

			return Misc.GetDateTimeDescription (date);
		}

		private decimal GetTotal(Entities.InvoiceDocumentEntity x)
		{
			if (x.BillingDetails.Count > 0)
			{
				return Misc.CentRound (x.BillingDetails[0].AmountDue.Amount);
			}

			return 0;
		}

	}
}
