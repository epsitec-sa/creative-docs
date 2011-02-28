//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Controllers.CreationControllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Orchestrators;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	/// <summary>
	/// The <c>BrowserViewController</c> manages the browser view, which lists
	/// entities in a compact form (this is used as the entity selector in the
	/// UI).
	/// </summary>
	public sealed partial class BrowserViewController : CoreViewController, INotifyCurrentChanged, IWidgetUpdater
	{
		public BrowserViewController(Orchestrators.DataViewOrchestrator orchestrator)
			: base ("Browser", orchestrator)
		{
			this.data = orchestrator.Data;
			this.Orchestrator.Host.RegisterComponent (this);
		}


		public FrameBox							SettingsPanel
		{
			get
			{
				return this.settingsPanel;
			}
		}

		public string							DataSetName
		{
			get
			{
				return this.dataSetName;
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

				this.DisposeBrowserDataContext ();
				this.dataSetName = dataSetName;
				this.CreateBrowserDataContext ();

				this.SelectContentsBasedOnDataSet ();
				this.OnDataSetSelected ();
			}
		}

		/// <summary>
		/// Selects the specified entity in the list.
		/// </summary>
		/// <param name="entity">The entity.</param>
		public void Select(AbstractEntity entity)
		{
			if ((this.collection == null) ||
				(this.scrollList == null))
			{
				return;
			}

			this.scrollList.SelectedItemIndex = this.collection.GetIndex (this.data.FindEntityKey (entity));
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
			this.collection         = new BrowserList (this.browserDataContext);

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
			var getter = DataSetGetter.ResolveDataSet (this.data, this.dataSetName);
			this.SetContents (getter);
		}

		private void SetContents(DataSetCollectionGetter collectionGetter)
		{
			//	When switching to some other contents, the browser first has to ensure that the
			//	UI no longer has an actively selected entity; clearing the active entity will
			//	also make sure that any changes will be automatically persisted:

			this.Orchestrator.ClearActiveEntity ();

			this.collectionGetter = collectionGetter;
//-			this.data.SetupDataContext (this.Orchestrator.DefaultDataContext);
			this.UpdateCollection ();
		}

		private bool SelectActiveEntity()
		{
			if (this.activeEntityKey.HasValue)
			{
				var activeEntityKey       = this.activeEntityKey.Value;
				var navigationPathElement = new BrowserNavigationPathElement (this, activeEntityKey);

				this.Orchestrator.SetActiveEntity (activeEntityKey, navigationPathElement);

				return true;
			}
			else
			{
				this.Orchestrator.ClearActiveEntity ();
				return false;
			}
		}

		private bool ReselectActiveEntity()
		{
			if (this.activeEntityKey.HasValue)
			{
				var activeEntityKey       = this.activeEntityKey.Value;
				var navigationPathElement = new BrowserNavigationPathElement (this, activeEntityKey);
				
				this.Orchestrator.ResetActiveEntity (activeEntityKey, navigationPathElement);

				return true;
			}
			else
			{
				this.Orchestrator.ClearActiveEntity ();
				return false;
			}
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
			if (this.collectionGetter == null)
			{
				return;
			}

			this.collection.DefineEntities (this.GetCollectionEntities ());
			this.RefreshScrollList (reset: true);
		}

		private void InsertIntoCollection(AbstractEntity entity)
		{
			this.collection.Insert (entity);
//-			this.RefreshScrollList (reset: true);
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

		private void OnCurrentChanged()
		{
			var handler = this.CurrentChanged;

			if (handler != null)
			{
				handler (this);
			}

			this.SelectActiveEntity ();
		}

		private void OnCurrentChanging(CurrentChangingEventArgs e)
		{
			var handler = this.CurrentChanging;

			if (handler != null)
			{
				handler (this, e);
			}
		}

		private void OnDataSetSelected()
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


		public event EventHandler				DataSetSelected;

		private readonly CoreData				data;
		
		private BrowserList						collection;
		private string							dataSetName;
		private DataContext						browserDataContext;

		private DataSetCollectionGetter			collectionGetter;
		private int								suspendUpdates;

		private ScrollList						scrollList;
		private EntityKey?						activeEntityKey;
		private FrameBox						settingsPanel;
	}
}
