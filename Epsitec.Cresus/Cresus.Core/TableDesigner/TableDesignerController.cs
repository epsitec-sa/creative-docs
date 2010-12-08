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

namespace Epsitec.Cresus.Core.TableDesigner
{
	public sealed class TableDesignerController : System.IDisposable
	{
		public TableDesignerController(DataViewOrchestrator orchestrator, Core.Business.BusinessContext businessContext, PriceCalculatorEntity priceCalculatorEntity, ArticleDefinitionEntity articleDefinitionEntity)
		{
			System.Diagnostics.Debug.Assert (orchestrator != null);
			System.Diagnostics.Debug.Assert (businessContext != null);
			System.Diagnostics.Debug.Assert (priceCalculatorEntity.IsNotNull ());
			System.Diagnostics.Debug.Assert (articleDefinitionEntity.IsNotNull ());

			this.orchestrator            = orchestrator;
			this.businessContext         = businessContext;
			this.priceCalculatorEntity   = priceCalculatorEntity;
			this.articleDefinitionEntity = articleDefinitionEntity;
			
			this.businessContext.SavingChanges += this.HandleBusinessContextSavingChanges;
		}



		public Widget CreateUI()
		{
			this.editorUI = this.CreateTableEditorUI ();
			
			return this.editorUI;
		}

		public void NavigateTo(PriceCalculatorEntity entity)
		{
			var mainViewController    = this.orchestrator.MainViewController;
			var browserViewController = mainViewController.BrowserViewController;
			
			browserViewController.Select (entity);
		}

		private Widget CreateTableEditorUI()
		{
			var box = new FrameBox
			{
				Dock = DockStyle.Fill,
				Padding = new Margins (5),
			};

			this.mainController = new MainController (this.businessContext, this.priceCalculatorEntity, this.articleDefinitionEntity);
			this.mainController.CreateUI (box);

			return box;
		}


		private void HandleBusinessContextSavingChanges(object sender, CancelEventArgs e)
		{
			this.businessContext.DataContext.SaveChanges ();
			this.SaveTableDesignIntoEntity ();
		}

		private void SaveTableDesignIntoEntity()
		{
			this.mainController.SaveDesign ();
		}


		#region IDisposable Members

		public void Dispose()
		{
			if (this.editorUI != null)
			{
				this.editorUI.Dispose ();
				this.editorUI = null;
			}

			this.businessContext.SavingChanges -= this.HandleBusinessContextSavingChanges;
		}

		#endregion


		private readonly DataViewOrchestrator		orchestrator;
		private readonly BusinessContext			businessContext;
		private readonly PriceCalculatorEntity		priceCalculatorEntity;
		private readonly ArticleDefinitionEntity	articleDefinitionEntity;

		private Widget								editorUI;
		private MainController						mainController;
	}
}
