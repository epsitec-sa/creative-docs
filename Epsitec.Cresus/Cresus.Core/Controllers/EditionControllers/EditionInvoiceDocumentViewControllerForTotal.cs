using Epsitec.Common.Types;
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
	public class EditionInvoiceDocumentViewControllerForTotal : EditionViewController<Entities.InvoiceDocumentEntity>
	{
		public EditionInvoiceDocumentViewControllerForTotal(string name, Entities.InvoiceDocumentEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIMain (data);
			}
		}

		private void CreateUIMain(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					Name				= "TotalDocumentItem",
					IconUri				= "Data.TotalDocumentItem",
					Title				= TextFormatter.FormatText ("Total"),
					CompactTitle		= TextFormatter.FormatText ("Total"),
					TextAccessor		= Accessor.Create (this.EntityGetter, x => InvoiceDocumentHelper.GetSummary (x)),
					CompactTextAccessor = Accessor.Create (this.EntityGetter, x => TextFormatter.FormatText ("N°", x.IdA)),
					EntityMarshaler		= this.EntityMarshaler,
				});
		}
	}
}
