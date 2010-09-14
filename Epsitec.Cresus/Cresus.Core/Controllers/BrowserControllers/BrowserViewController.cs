//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers.CreationControllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Orchestrators;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	public class BrowserViewController : CoreViewController, INotifyCurrentChanged, IWidgetUpdater
	{
		public BrowserViewController(Orchestrators.DataViewOrchestrator orchestrator)
			: base ("Browser", orchestrator)
		{
			this.data = orchestrator.Data;

			this.data.DiscardRecordCommandExecuted +=
				delegate
				{
					var key = this.activeEntityKey;
					this.SetActiveEntityKey (null);
					this.SetActiveEntityKey (key);
				};
		}

		
		public FrameBox SettingsPanel
		{
			get
			{
				return this.settingsPanel;
			}
		}

		public string DataSetName
		{
			get
			{
				return this.dataSetName;
			}
		}


		public void SelectDataSet(string dataSetName)
		{
			if (this.dataSetName != dataSetName)
			{
				this.Orchestrator.ClearActiveEntity ();

				this.DisposeBrowserDataContext ();
				this.dataSetName = dataSetName;
				this.CreateBrowserDataContext ();

				this.SelectContentsBasedOnDataSet ();
				this.OnDataSetSelected ();
			}
		}

		public void SelectActiveEntity()
		{
			this.Orchestrator.ClearActiveEntity ();

			if (this.activeEntityKey.HasValue)
			{
				var activeEntityKey       = this.activeEntityKey.Value;
				var navigationPathElement = new BrowserNavigationPathElement (this, activeEntityKey);

				this.Orchestrator.SetActiveEntity (activeEntityKey, navigationPathElement);
			}
		}

		public void CreateNewItem()
		{
			this.Orchestrator.CloseSubViews ();

			var item = this.CreateDummyEntity ();

			if (item != null)
			{
				var controller = EntityViewControllerFactory.Create ("ItemCreation", item, ViewControllerMode.Creation, this.Orchestrator);

				if (controller is ICreationController)
				{
					//	OK, we have really been able to create a specific creation controller, which
					//	will be used to bootstrap the entity creation...
				}
				else
				{
					controller.Dispose ();

					//	Create the real entity, then re-create the user interface to edit it:

					var businessContext = this.Orchestrator.BusinessContext;
					item = businessContext.CreateEntity (BrowserViewController.GetRootEntityId (this.DataSetName));
					controller = EntityViewControllerFactory.Create ("EmptyItem", item, ViewControllerMode.Summary, this.Orchestrator);
				}

				this.Orchestrator.ShowSubView (null, controller);
			}
		}

		public AbstractEntity GetActiveEntity(DataContext context)
		{
			if (this.activeEntityKey.HasValue)
			{
				return context.ResolveEntity (this.activeEntityKey);
			}
			else
			{
				return null;
			}
		}


		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			var frame = new FrameBox ()
			{
				Parent = container,
				Dock = DockStyle.Fill,
			};

			this.settingsPanel = new FrameBox
			{
				Parent = frame,
				Dock = DockStyle.Top,
				PreferredHeight = 28,
			};

			var listFrame = new FrameBox
			{
				Parent = frame,
				Dock = DockStyle.Fill,
			};

			this.scrollList = new ScrollList ()
			{
				Parent = listFrame,
				Anchor = AnchorStyles.All,
				ScrollListStyle = ScrollListStyle.Standard,
				Margins = new Common.Drawing.Margins (-1, -1, -1, -1),
			};

			this.scrollList.Items.ValueConverter = BrowserList.ValueConverterFunction;

			this.scrollList.SelectedItemChanged += this.HandleScrollListSelectedItemChanged;

			this.RefreshScrollList ();
		}


		private void CreateBrowserDataContext()
		{
			this.browserDataContext = this.data.CreateDataContext (string.Format ("Browser.DataSet={0}", this.DataSetName));
			this.collection  = new BrowserList (this.browserDataContext);

			this.browserDataContext.EntityChanged += this.HandleDataContextEntityChanged;
		}

		private void DisposeBrowserDataContext()
		{
			if (this.browserDataContext != null)
			{
				this.browserDataContext.EntityChanged -= this.HandleDataContextEntityChanged;
				this.data.DisposeDataContext (this.browserDataContext);
				this.browserDataContext = null;
				this.collection = null;
			}
		}

		private void HandleDataContextEntityChanged(object sender, EntityChangedEventArgs e)
		{
			if ((this.collection != null) &&
				(this.scrollList != null))
			{
				if (this.scrollList.InvalidateTextLayouts ())
				{
					this.collection.Invalidate ();
				}
			}
		}

		private void HandleScrollListSelectedItemChanged(object sender)
		{
			if (this.suspendUpdates == 0)
			{
				this.NotifySelectedItemChange ();
			}
		}
		
		private void SelectContentsBasedOnDataSet()
		{
			switch (this.dataSetName)
			{
				case "Customers":
					this.SetContents (context => this.data.GetCustomers (context));
					break;

				case "ArticleDefinitions":
					this.SetContents (context => this.data.GetArticleDefinitions (context));
					break;

				case "Documents":
					this.SetContents (context => this.data.GetDocuments (context));
					break;

				case "InvoiceDocuments":
					this.SetContents (context => this.data.GetInvoiceDocuments (context));
					break;
			}
		}

		private AbstractEntity CreateDummyEntity()
		{
			return this.data.CreateDummyEntity (BrowserViewController.GetRootEntityId (this.DataSetName));
		}

		public static Druid GetRootEntityId(string dataSetName)
		{
			switch (dataSetName)
			{
				case "Customers":
					return EntityInfo.GetTypeId<RelationEntity> ();

				case "ArticleDefinitions":
					return EntityInfo.GetTypeId<ArticleDefinitionEntity> ();

				case "Documents":
				case "InvoiceDocuments":
					return EntityInfo.GetTypeId<InvoiceDocumentEntity> ();
			}

			throw new System.NotImplementedException ();
		}
	
		private void SetContents(System.Func<DataContext, IEnumerable<AbstractEntity>> collectionGetter)
		{
			//	When switching to some other contents, the browser first has to ensure that the
			//	UI no longer has an actively selected entity; clearing the active entity will
			//	also make sure that any changes will be automatically persisted:

			this.Orchestrator.ClearActiveEntity ();

			this.collectionGetter = collectionGetter;
			this.data.SetupDataContext (this.Orchestrator.DefaultDataContext);
			this.UpdateCollection ();
		}


		private AbstractEntity[] GetCollectionEntities()
		{
			if (this.collectionGetter == null)
			{
				return new AbstractEntity[0];
			}
			else
			{
				return this.collectionGetter (this.browserDataContext).ToArray ();
			}
		}
		
		private void UpdateCollection()
		{
			if (this.collectionGetter != null)
			{
				this.collection.DefineEntities (this.GetCollectionEntities ());
				this.RefreshScrollList (reset: true);
			}
		}
		
		private void NotifySelectedItemChange()
		{
			int active    = this.scrollList.SelectedItemIndex;
			var entityKey = this.collection == null ? null : this.collection.GetEntityKey (active);

			System.Diagnostics.Debug.WriteLine (string.Format ("SelectedItemChanged : old key = {0} / new key = {1}", this.activeEntityKey, entityKey));

			this.SetActiveEntityKey (entityKey);
		}


		private void SetActiveEntityKey(EntityKey? entityKey)
		{
			if (this.activeEntityKey != entityKey)
			{
				this.activeEntityKey = entityKey;
				this.OnCurrentChanging (new CurrentChangingEventArgs (isCancelable: false));
				this.OnCurrentChanged ();
			}
		}
		
		protected void OnCurrentChanged()
		{
			var handler = this.CurrentChanged;

			if (handler != null)
			{
				handler (this);
			}

			this.SelectActiveEntity ();
		}

		protected void OnCurrentChanging(CurrentChangingEventArgs e)
		{
			var handler = this.CurrentChanging;

			if (handler != null)
			{
				handler (this, e);
			}
		}

		protected void OnDataSetSelected()
		{
			var commandHandler = this.Orchestrator.MainViewController.CommandContext.GetCommandHandler<Epsitec.Cresus.Core.CommandHandlers.DatabaseCommandHandler> ();
			commandHandler.UpdateActiveCommandState (this.dataSetName);

			var handler = this.DataSetSelected;

			if (handler != null)
			{
				handler (this);
			}
		}

		
		private void RefreshScrollList(bool reset = false)
		{
			if ((this.scrollList != null) &&
				(this.collection != null))
			{
				int newCount = this.collection == null ? 0 : this.collection.Count;

				int oldActive = reset ? 0 : this.collection.GetIndex (this.activeEntityKey);
				int newActive = oldActive < newCount ? oldActive : newCount-1;

				this.suspendUpdates++;
				this.scrollList.Items.Clear ();
				this.scrollList.Items.AddRange (this.collection);
				this.scrollList.SelectedItemIndex = newActive;
				this.suspendUpdates--;

				this.NotifySelectedItemChange ();
			}
		}

		#region IWidgetUpdater Members

		public void Update()
		{
			if (this.collection != null)
			{
				this.collection.Invalidate ();
				this.RefreshScrollList ();
			}
		}

		#endregion

		#region INotifyCurrentChanged Members

		public event EventHandler  CurrentChanged;

		public event EventHandler<CurrentChangingEventArgs>  CurrentChanging;

		#endregion

		#region BrowserNavigationPathElement Class

		private class BrowserNavigationPathElement : Epsitec.Cresus.Core.Orchestrators.Navigation.NavigationPathElement
		{
			public BrowserNavigationPathElement(BrowserViewController controller, EntityKey entityKey)
			{
				this.dataSetName = controller.DataSetName;
				this.entityKey   = entityKey;
			}

			public override bool Navigate(Orchestrators.NavigationOrchestrator navigator)
			{
				var browserViewController = navigator.BrowserViewController;

				browserViewController.SelectDataSet (this.dataSetName);
				browserViewController.SetActiveEntityKey (this.entityKey);
				browserViewController.RefreshScrollList ();
				
				//	Don't access the DataContext before this point in the function, or else
				//	we would end up using an outdated and disposed data context !
				
				var browserDataContext = browserViewController.browserDataContext;

				return browserViewController.GetActiveEntity (browserDataContext) != null;
			}

			public override string ToString()
			{
				return string.Concat ("<Browser:", this.dataSetName, ":", this.entityKey.RowKey.ToString (), ">");
			}


			private readonly string dataSetName;
			private readonly EntityKey entityKey;
		}

		#endregion


		public event EventHandler				DataSetSelected;

		private readonly CoreData data;
		private BrowserList collection;
		private string dataSetName;
		private DataContext browserDataContext;
		private System.Func<DataContext, IEnumerable<AbstractEntity>> collectionGetter;
		private int suspendUpdates;

		private ScrollList scrollList;
		private EntityKey? activeEntityKey;
		private FrameBox settingsPanel;
	}
}
