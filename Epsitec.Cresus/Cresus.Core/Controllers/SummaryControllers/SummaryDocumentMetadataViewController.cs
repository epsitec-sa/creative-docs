//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Factories;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryDocumentMetadataViewController : SummaryViewController<DocumentMetadataEntity>
	{
		public SummaryDocumentMetadataViewController()
		{
			var doc = this.Entity.BusinessDocument;

			if (doc.IsNotNull ())
			{
				this.businessDocumentController = EntityViewControllerFactory.Create ("Meta", doc, ViewControllerMode.Summary, this.Orchestrator);
			}
		}


		public override IEnumerable<CoreController> GetSubControllers()
		{
			if (this.businessDocumentController != null)
			{
				yield return this.businessDocumentController;
			}
		}

		protected override void CreateUI()
		{
			using (var builder = UIBuilder.Create (this))
			{
				using (var data = TileContainerController.Setup (builder))
				{
					this.CreateUIMetadata (data);
				}
				
				this.CreateUIBusinessDocument ();
			}
		}

		private void CreateUIMetadata(TileDataItems data)
		{
			data.Add (
				new TileDataItem
				{
					Name				= "InvoiceDocument",
					IconUri				= "Data.InvoiceDocument",
					Title				= TextFormatter.FormatText ("Document"),
					CompactTitle		= TextFormatter.FormatText ("Document"),
					TextAccessor		= this.CreateAccessor (x => x.GetSummary ()),
					CompactTextAccessor = this.CreateAccessor (x => x.GetCompactSummary ()),
					EntityMarshaler		= this.CreateEntityMarshaler (),
				});
		}


		private void CreateUIBusinessDocument()
		{
			if (this.businessDocumentController != null)
			{
				this.businessDocumentController.CreateUI (this.TileContainer);
			}
		}
		
		
		private EntityViewController businessDocumentController;
	}
}
