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

namespace Epsitec.Cresus.Core.Controllers
{
	public class BrowserViewController : CoreViewController, INotifyCurrentChanged
	{
		public BrowserViewController(string name, CoreData data)
			: base (name)
		{
			this.data       = data;
			this.collection = new List<AbstractEntity> ();

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


		public AbstractEntity GetActiveEntity()
		{
			if (this.activeEntityKey.IsEmpty)
			{
				return null;
			}

			int active   = this.scrollList.SelectedItemIndex;
			var entity   = BrowserViewController.GetActiveItem (this.collection, active);
			var entityId = entity.GetEntityStructuredTypeId ();

			entity = this.data.DataContext.ResolveEntity (this.activeEntityKey);

			return entity;
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
		
		public void SetContents(System.Func<IEnumerable<AbstractEntity>> collectionGetter)
		{
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

			this.scrollList.SelectedItemChanged +=
				delegate
				{
					if (this.suspendUpdates == 0)
					{
						int active = this.scrollList.SelectedItemIndex;
						var entity = BrowserViewController.GetActiveItem (this.collection, active);
						var key    = DataContextPool.Instance.FindEntityKey (entity) ?? EntityKey.Empty;

						if (this.activeEntityKey != key)
						{
							this.activeEntityKey = key;
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
					this.SetContents (() => this.data.GetCustomers ());
					break;

				case "ArticleDefinitions":
					this.SetContents (() => this.data.GetArticleDefinitions ());
					break;

				case "InvoiceDocuments":
					this.SetContents (() => this.data.GetInvoiceDocuments ());
					break;
			}
		}
		
		private void UpdateCollection(bool silent = false)
		{
			if (this.collectionGetter != null)
			{
				if (silent == false)
				{
					this.OnCurrentChanging (new CurrentChangingEventArgs (isCancelable: false));
				}

				var data = this.collectionGetter ().ToArray ();

				this.collection.Clear ();
				this.collection.AddRange (data);
				
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
				var updatedList = new List<FormattedText> ();

				foreach (var entity in this.collection)
				{
					var text = BrowserViewController.GetEntityDisplayText (entity);

					if (text.IsNullOrEmpty)
					{
						text = CollectionTemplate.DefaultEmptyText;
					}

					updatedList.Add (text);
				}

				int oldActive = this.scrollList.SelectedItemIndex;
				int newActive = oldActive < updatedList.Count ? oldActive : updatedList.Count-1;

				this.suspendUpdates++;
				this.scrollList.Items.Clear ();
				this.scrollList.Items.AddRange (updatedList.Select (x => x.ToString ()));
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

		private static FormattedText GetEntityDisplayText(AbstractEntity entity)
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
		private readonly List<AbstractEntity> collection;
		private string dataSetName;
		private System.Func<IEnumerable<AbstractEntity>> collectionGetter;
		private int suspendUpdates;

		private ScrollList scrollList;
		private EntityKey activeEntityKey;
		private FrameBox settingsPanel;
	}
}
