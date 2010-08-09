//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	public class BrowserViewController : CoreViewController, INotifyCurrentChanged
	{
		public BrowserViewController(string name, CoreData data)
			: base (name)
		{
			this.data        = data;
			base.DataContext = data.CreateDataContext ();
			this.collection  = new BrowserList (this.DataContext);

			this.data.SaveRecordCommandExecuted +=
				(sender, e) =>
				{
					this.UpdateCollection ();
				};

			this.data.DataContextChanged +=
				(sender, e) =>
				{
					if (this.data.IsDataContextActive)
					{
						System.Diagnostics.Debug.WriteLine ("DataContext changed: BrowserViewController should do something about it");
//-						this.UpdateCollection (silent: true);
					}
				};
		}

		public override DataContext DataContext
		{
			get
			{
				return base.DataContext;
			}
			set
			{
				throw new System.InvalidOperationException ("Cannot set DataContext");
			}
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


		public AbstractEntity GetActiveEntity(DataContext context)
		{
			if (this.activeEntityKey == null)
			{
				return null;
			}

			int active    = this.scrollList.SelectedItemIndex;
			var entityKey = this.collection.GetEntityKey (active);

			if (entityKey.HasValue)
			{
				return context.ResolveEntity (entityKey.Value);
			}
			else
			{
				return null;
			}
		}

		public void SelectDataSet(string dataSetName)
		{
			if (this.dataSetName != dataSetName)
			{
				this.dataSetName = dataSetName;

				this.UpdateDataSet ();
				this.OnDataSetSelected ();
			}
		}
		
		public void SetContents(System.Func<DataContext, IEnumerable<AbstractEntity>> collectionGetter)
		{
			//	When switching to some other contents, the browser first has to ensure that the
			//	UI no longer has an actively selected entity; clearing the active entity will
			//	also make sure that any changes will be automatically persisted:

			this.Orchestrator.Controller.ClearActiveEntity ();

			this.collectionGetter = collectionGetter;
			this.data.SetupDataContext ();
			this.UpdateCollection ();
		}

		public void CreateNewItem()
		{
			var item = this.data.CreateNewEntity (this.DataSetName, EntityCreationScope.Independent);

			if (item != null)
			{
				var controller = EntityViewController.CreateEntityViewController ("ItemCreation", item, ViewControllerMode.Creation, this.Orchestrator);
				this.Orchestrator.ShowSubView (null, controller);
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

			this.scrollList.SelectedItemChanged +=
				delegate
				{
					if (this.suspendUpdates == 0)
					{
						int active    = this.scrollList.SelectedItemIndex;
						var entityKey = this.collection.GetEntityKey (active);

						System.Diagnostics.Debug.WriteLine ("SelectedItemChanged : old key = " + this.activeEntityKey + " / new key = " + entityKey);

						if (this.activeEntityKey != entityKey)
						{
							this.activeEntityKey = entityKey;
							this.OnCurrentChanging (new CurrentChangingEventArgs (isCancelable: false));
							this.OnCurrentChanged ();
						}
					}
				};

			this.RefreshScrollList ();
		}


		private void UpdateDataSet()
		{
			switch (this.dataSetName)
			{
				case "Customers":
					this.SetContents (context => this.data.GetCustomers (context));
					break;

				case "ArticleDefinitions":
					this.SetContents (context => this.data.GetArticleDefinitions (context));
					break;

				case "InvoiceDocuments":
					this.SetContents (context => this.data.GetInvoiceDocuments (context));
					break;
			}
		}
		
		private void UpdateCollection()
		{
			if (this.collectionGetter != null)
			{
				int active   = this.scrollList.SelectedItemIndex;
				var entityKey1 = this.collection.GetEntityKey (active);
				
				var data = this.collectionGetter (this.DataContext).ToArray ();

				this.collection.DefineCollection (data);

				var entityKey2 = this.collection.GetEntityKey (active);

				if (entityKey1 != entityKey2)
                {
					this.OnCurrentChanging (new CurrentChangingEventArgs (isCancelable: false));
					this.OnCurrentChanged ();
                }
				
				this.RefreshScrollList ();
			}
		}
		
		protected void OnCurrentChanged()
		{
			var handler = this.CurrentChanged;

			if (handler != null)
			{
				handler (this);
			}
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
			var handler = this.DataSetSelected;

			if (handler != null)
			{
				handler (this);
			}
		}

		private void RefreshScrollList()
		{
			if (this.scrollList != null)
			{
				int newCount = this.collection.Count;

				int oldActive = this.scrollList.SelectedItemIndex;
				int newActive = oldActive < newCount ? oldActive : newCount-1;

				this.suspendUpdates++;
				this.scrollList.Items.Clear ();
				this.scrollList.Items.AddRange (collection);
				this.suspendUpdates--;

				this.scrollList.SelectedItemIndex = newActive;
			}
		}

		private static T GetActiveItem<T>(IList<T> collection, int index)
		{
			if (index < 0)
			{
				return default (T);
			}
			else
			{
				return collection[index];
			}
		}

		internal static FormattedText GetEntityDisplayText(AbstractEntity entity)
		{
			if (entity == null)
			{
				return FormattedText.Empty;
			}

			if (entity is LegalPersonEntity)
			{
				var person = entity as LegalPersonEntity;
				return UIBuilder.FormatText (person.Name);
			}
			if (entity is NaturalPersonEntity)
			{
				var person = entity as NaturalPersonEntity;
				return UIBuilder.FormatText (person.Firstname, person.Lastname);
			}
			if (entity is RelationEntity)
			{
				var customer = entity as RelationEntity;
				return UIBuilder.FormatText (BrowserViewController.GetEntityDisplayText (customer.Person), customer.DefaultAddress.Location.PostalCode, customer.DefaultAddress.Location.Name);
			}
			if (entity is ArticleDefinitionEntity)
			{
				var article = entity as ArticleDefinitionEntity;
				return UIBuilder.FormatText (article.ShortDescription);
			}
			if (entity is InvoiceDocumentEntity)
			{
				var invoice = entity as InvoiceDocumentEntity;
				return UIBuilder.FormatText (invoice.IdA);
			}
			
			return FormattedText.Empty;
		}

		#region INotifyCurrentChanged Members

		public event EventHandler  CurrentChanged;

		public event EventHandler<CurrentChangingEventArgs>  CurrentChanging;

		#endregion

		public event EventHandler				DataSetSelected;

		private readonly CoreData data;
		private readonly BrowserList collection;
		private string dataSetName;
		private System.Func<DataContext, IEnumerable<AbstractEntity>> collectionGetter;
		private int suspendUpdates;

		private ScrollList scrollList;
		private EntityKey? activeEntityKey;
		private FrameBox settingsPanel;
	}
}
