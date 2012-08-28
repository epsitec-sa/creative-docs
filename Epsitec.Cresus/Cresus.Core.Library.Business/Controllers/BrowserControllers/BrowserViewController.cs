//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.BigList;
using Epsitec.Common.BigList.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers.CreationControllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Metadata;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Widgets;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	/// <summary>
	/// The <c>BrowserViewController</c> manages the browser view, which lists
	/// entities in a compact form (this is used as the entity selector in the
	/// UI).
	/// </summary>
	public sealed partial class BrowserViewController : CoreViewController, INotifyCurrentChanged, IWidgetUpdater
	{
		public BrowserViewController(DataViewOrchestrator orchestrator)
			: base ("Browser", orchestrator)
		{
			this.data = orchestrator.Data;
			this.Orchestrator.Host.RegisterComponent (this);
		}


		public FrameBox							TopPanel
		{
			get
			{
				return this.topPanel;
			}
		}

		public string							DataSetName
		{
			get
			{
				return this.dataSetName;
			}
		}

		public System.Type						DataSetEntityType
		{
			get
			{
				return DataSetGetter.GetRootEntityType (this.dataSetName);
			}
		}


		/// <summary>
		/// Selects the specified data set.
		/// </summary>
		/// <param name="dataSetName">Name of the data set.</param>
		public void SelectDataSet(string dataSetName)
		{
			if (this.dataSetName != dataSetName)
			{
				this.Orchestrator.ClearActiveEntity ();

				this.DisposeBrowserListController ();
				this.dataSetName = dataSetName;
				this.CreateBrowserListController ();
				this.OnDataSetSelected ();
			}
		}

		/// <summary>
		/// Selects the specified entity in the list.
		/// </summary>
		/// <param name="entity">The entity.</param>
		public void SelectEntity(AbstractEntity entity)
		{
			this.SelectEntity (this.data.FindEntityKey (entity));
		}

		/// <summary>
		/// Selects the specified entity in the list.
		/// </summary>
		/// <param name="entityKey">The entity key.</param>
		public void SelectEntity(EntityKey? entityKey)
		{
			if (this.browserListController != null)
			{
				this.browserListController.SelectedEntityKey = entityKey;
				this.browserListController.RefreshScrollList ();
				this.SelectActiveEntity ();
			}
		}

		/// <summary>
		/// Adds a new entity to the list. This will start an interactive creation if
		/// there is an associated creation controller.
		/// </summary>
		public void AddNewEntity()
		{
			using (var creator = new BrowserViewController.ItemCreator (this))
			{
				creator.Create ();
			}
		}

		/// <summary>
		/// Removes the active entity from the list. This will either archive the entity
		/// or delete it from the database, if it does not implement <see cref="ILifetime"/>.
		/// </summary>
		public void DeleteActiveEntity()
		{
			var entity = this.browserListController.SelectedEntity;
			var active = this.browserListController.SelectedIndex;

			if (entity != null)
			{
				this.Orchestrator.ClearActiveEntity ();
				this.browserListController.Delete (entity);
				this.browserListController.SelectedIndex = active;
			}
		}
		
		
		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			base.CreateUI (container);

			var frame = new FrameBox ()
			{
				Parent = container,
				Dock = DockStyle.Fill,
			};

			this.CreateUITopPanel (frame);
			this.CreateUIItemScrollList (frame);
		}


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.DisposeBrowserListController ();
			}
			
			base.Dispose (disposing);
		}

		private void CreateUITopPanel(FrameBox frame)
		{
			this.topPanel = new FrameBox
			{
				Parent = frame,
				Dock   = DockStyle.Top,
				Name   = "Top",
				
				PreferredHeight = 28,
			};
		}
		
		private void CreateUIItemScrollList(FrameBox frame)
		{
			this.itemScrollList = new ItemScrollList ()
			{
				Parent = frame,
				Dock   = DockStyle.Fill,
				Name   = "Body",
			};
		}
		
		
		private void CreateBrowserListController()
		{
			//	When switching to some other contents, the browser first has to ensure that the
			//	UI no longer has an actively selected entity; clearing the active entity will
			//	also make sure that any changes will be automatically persisted:

			this.Orchestrator.ClearActiveEntity ();

			this.browserListController = new BrowserListController (this.Orchestrator, this.itemScrollList, this.DataSetEntityType);
			
			this.browserListController.CurrentChanged  += this.HandleBrowserListControllerCurrentChanged;
			this.browserListController.CurrentChanging += this.HandleBrowserListControllerCurrentChanging;
			
			this.SelectActiveEntity ();
		}

		private void DisposeBrowserListController()
		{
			if (this.browserListController != null)
			{
				this.browserListController.CurrentChanged  -= this.HandleBrowserListControllerCurrentChanged;
				this.browserListController.CurrentChanging -= this.HandleBrowserListControllerCurrentChanging;
				
				this.browserListController.Dispose ();
				
				this.browserListController = null;
			}
		}

		
		private void HandleBrowserListControllerCurrentChanged(object sender)
		{
			this.OnCurrentChanged ();
		}

		private void HandleBrowserListControllerCurrentChanging(object sender, CurrentChangingEventArgs e)
		{
			this.OnCurrentChanging (e);
		}
		

		private bool SelectActiveEntity()
		{
			if (this.browserListController.SelectedEntityKey.HasValue)
			{
				this.SelectActiveEntity (this.browserListController.SelectedEntityKey.Value);
				return true;
			}
			else
			{
				this.DeselectActiveEntity ();
				return false;
			}
		}

		private void SelectActiveEntity(EntityKey entityKey)
		{
			var navigationPathElement = new BrowserNavigationPathElement (this, entityKey);
			this.Orchestrator.ResetActiveEntity (entityKey, navigationPathElement);
		}

		private void DeselectActiveEntity()
		{
			this.Orchestrator.ClearActiveEntity ();
		}

		
		private void OnCurrentChanged()
		{
			this.CurrentChanged.Raise (this);
			this.SelectActiveEntity ();
		}

		private void OnCurrentChanging(CurrentChangingEventArgs e)
		{
			this.CurrentChanging.Raise (this, e);
		}

		private void OnDataSetSelected()
		{
			this.Orchestrator.MainViewController.CommandContext.UpdateCommandStates (this);
			this.DataSetSelected.Raise (this);
		}


		#region IWidgetUpdater Members

		public void Update()
		{
		}

		#endregion

		#region INotifyCurrentChanged Members

		public event EventHandler				CurrentChanged;

		public event EventHandler<CurrentChangingEventArgs>  CurrentChanging;

		#endregion


		public event EventHandler				DataSetSelected;

		private readonly CoreData				data;
		private string							dataSetName;

		private FrameBox						topPanel;
		private ItemScrollList					itemScrollList;
		private BrowserListController			browserListController;
	}
}