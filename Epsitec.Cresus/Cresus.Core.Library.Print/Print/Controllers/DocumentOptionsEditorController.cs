//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Print.Controllers
{
	public sealed class DocumentPrintingUnitsEditorController : System.IDisposable
	{
		public DocumentPrintingUnitsEditorController(IBusinessContext businessContext, DocumentPrintingUnitsEntity documentPrintingUnitsEntity)
		{
			System.Diagnostics.Debug.Assert (businessContext != null);
			System.Diagnostics.Debug.Assert (documentPrintingUnitsEntity.IsNotNull ());

			this.businessContext             = businessContext;
			this.documentPrintingUnitsEntity = documentPrintingUnitsEntity;
			
			this.businessContext.SavingChanges += this.HandleBusinessContextSavingChanges;
		}



		public void CreateUI(Widget parent)
		{
			var box = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				Padding = new Margins (5),
			};

			this.documentPrintingUnitController = new DocumentPrintingUnitController (this.businessContext, this.documentPrintingUnitsEntity);
			this.documentPrintingUnitController.CreateUI (box);
		}

		private void HandleBusinessContextSavingChanges(object sender, CancelEventArgs e)
		{
			if (this.documentPrintingUnitController != null)
			{
				this.documentPrintingUnitController.SaveDesign ();
				this.documentPrintingUnitController = null;
			}
			
			this.businessContext.DataContext.SaveChanges ();
		}


		#region IDisposable Members

		public void Dispose()
		{
			this.businessContext.SavingChanges -= this.HandleBusinessContextSavingChanges;
		}

		#endregion


		private readonly IBusinessContext					businessContext;
		private readonly DocumentPrintingUnitsEntity		documentPrintingUnitsEntity;

		private DocumentPrintingUnitController				documentPrintingUnitController;
	}
}
