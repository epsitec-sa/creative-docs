//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.PlugIns;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.DocumentOptionsEditor
{
	public sealed class DocumentOptionsEditorController : System.IDisposable
	{
		public DocumentOptionsEditorController(DataViewOrchestrator orchestrator, Core.Business.BusinessContext businessContext, DocumentOptionsEntity documentOptionsEntity)
		{
			System.Diagnostics.Debug.Assert (orchestrator != null);
			System.Diagnostics.Debug.Assert (businessContext != null);
			System.Diagnostics.Debug.Assert (documentOptionsEntity.IsNotNull ());

			this.orchestrator            = orchestrator;
			this.businessContext         = businessContext;
			this.documentOptionsEntity   = documentOptionsEntity;
			
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

			this.mainController = new MainController (box, this.businessContext, this.documentOptionsEntity);
			this.mainController.CreateUI (box);
		}

		public void NavigateTo(PriceCalculatorEntity entity)
		{
			var mainViewController    = this.orchestrator.MainViewController;
			var browserViewController = mainViewController.BrowserViewController;
			
			browserViewController.Select (entity);
		}


		private void HandleBusinessContextSavingChanges(object sender, CancelEventArgs e)
		{
			this.mainController.SaveDesign ();
			this.businessContext.DataContext.SaveChanges ();
		}


		#region IDisposable Members

		public void Dispose()
		{
			this.businessContext.SavingChanges -= this.HandleBusinessContextSavingChanges;
		}

		#endregion


		private readonly DataViewOrchestrator				orchestrator;
		private readonly BusinessContext					businessContext;
		private readonly DocumentOptionsEntity				documentOptionsEntity;

		private MainController								mainController;
	}
}
