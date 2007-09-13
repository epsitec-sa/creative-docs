using Epsitec.Common.Types;
using System.Collections.Generic;

namespace Epsitec.Common.Support.ResourceAccessors
{
	using CultureInfo = System.Globalization.CultureInfo;

	public class PanelResourceAccessor : IResourceAccessor
	{
		public PanelResourceAccessor()
		{
			this.items = new CultureMapList (null);
			this.items.CollectionChanged += this.HandleItemsCollectionChanged;
			this.dirtyItems = new Dictionary<CultureMap, bool> ();
		}

		public void Load(ResourceManager manager)
		{
			this.manager = manager;

			this.items.Clear ();
			this.dirtyItems.Clear ();

			string[] names = this.manager.GetBundleIds ("*", Strings.PanelType, ResourceLevel.Default);

#if false
			if (names.Length == 0)
			{
				//	S'il n'existe aucun panneau, crée un premier panneau vide.
				//	Ceci est nécessaire, car il n'existe pas de commande pour créer un panneau à partir
				//	de rien, mais seulement une commande pour dupliquer un panneau existant.
				Druid druid = this.CreateUniqueDruid ();
				string prefix = this.resourceManager.ActivePrefix;
				System.Globalization.CultureInfo culture = this.BaseCulture;
				ResourceBundle bundle = ResourceBundle.Create (this.resourceManager, prefix, druid.ToBundleId (), ResourceLevel.Default, culture);

				bundle.DefineType (this.BundleName (false));
				bundle.DefineCaption (Res.Strings.Viewers.Panels.New);
				bundle.DefineRank (0);

				this.panelsList.Add (bundle);
				this.panelsToCreate.Add (bundle);
			}
#endif
			List<ResourceBundle> bundles = new List<ResourceBundle> ();

			foreach (string name in names)
			{
				bundles.Add (this.manager.GetBundle (name, ResourceLevel.Default, null));
			}

			bundles.Sort (
				delegate (ResourceBundle a, ResourceBundle b)
				{
					if (a.Rank < b.Rank)
					{
						return -1;
					}
					if (a.Rank > b.Rank)
					{
						return 1;
					}
					return 0;
				});

			using (this.SuspendNotifications ())
			{
				foreach (ResourceBundle bundle in bundles)
				{
					CultureMap item   = new CultureMap (this, bundle.Id, CultureMapSource.ReferenceModule);

					item.Name = bundle.Caption;

					StructuredData data = new StructuredData (Res.Types.ResourcePanel);

					ResourceBundle.Field panelSourceField = bundle[Strings.XmlSource];
					ResourceBundle.Field panelSizeField   = bundle[Strings.DefaultSize];

					string panelSource = panelSourceField.IsValid ? panelSourceField.AsString : null;
					string panelSize   = panelSizeField.IsValid   ? panelSizeField.AsString   : null;

					data.SetValue (Res.Fields.ResourcePanel.Bundle, bundle);
					data.SetValue (Res.Fields.ResourcePanel.XmlSource, panelSource);
					data.SetValue (Res.Fields.ResourcePanel.DefaultSize, panelSize);

					item.RecordCultureData (Resources.DefaultTwoLetterISOLanguageName, data);
					this.Collection.Add (item);
				}
			}
#if false

			if (field.IsValid)
			{
				UI.Panel panel = UserInterface.DeserializePanel (field.AsString, this.resourceManager);
				panel.DrawDesignerFrame = true;
				UI.Panel.SetPanel (bundle, panel);
				panel.SetupSampleDataSource ();
			}

			this.panelsList.Sort (new Comparers.BundleRank ());  // trie selon les rangs
#endif
		}

		public void Save()
		{
			this.PersistChanges ();
			this.RenumberBundles ();

			foreach (ResourceBundle bundle in this.deletedPanels)
			{
				this.manager.RemoveBundle (bundle.Id.ToBundleId (), bundle.ResourceLevel, bundle.Culture);
			}
			foreach (ResourceBundle bundle in this.createdPanels)
			{
				this.manager.SetBundle (bundle, ResourceSetMode.CreateOnly);
			}
			foreach (ResourceBundle bundle in this.editedPanels)
			{
				this.manager.SetBundle (bundle, ResourceSetMode.UpdateOnly);
			}

			this.deletedPanels.Clear ();
			this.createdPanels.Clear ();
			this.editedPanels.Clear ();
		}


		#region IResourceAccessor Members

		public CultureMapList Collection
		{
			get
			{
				return this.items;
			}
		}

		public bool BasedOnPatchModule
		{
			get
			{
				return this.manager.BasedOnPatchModule;
			}
		}

		public bool ContainsChanges
		{
			get
			{
				return this.dirtyItems.Count > 0;
			}
		}

		public IDataBroker GetDataBroker(Epsitec.Common.Types.StructuredData container, string fieldId)
		{
			return null;
		}

		public CultureMap CreateItem()
		{
			CultureMap item = new CultureMap (this, this.CreateId (), CultureMapSource.ReferenceModule);
			item.IsNewItem = true;
			return item;
		}

		/// <summary>
		/// Persists the changes to the underlying data store.
		/// </summary>
		/// <returns>
		/// The number of items which have been persisted.
		/// </returns>
		public int PersistChanges()
		{
			List<CultureMap> list = new List<CultureMap> (this.dirtyItems.Keys);

			this.dirtyItems.Clear ();

			foreach (CultureMap item in list)
			{
				if (this.Collection.Contains (item))
				{
					this.PersistItem (item);
					item.IsNewItem = false;
				}
				else
				{
					this.DeleteItem (item);
				}
			}

			return list.Count;
		}

		/// <summary>
		/// Reverts the changes applied to the accessor.
		/// </summary>
		/// <returns>
		/// The number of items which have been reverted.
		/// </returns>
		public int RevertChanges()
		{
			List<CultureMap> list = new List<CultureMap> (this.dirtyItems.Keys);

			this.dirtyItems.Clear ();

			using (this.SuspendNotifications ())
			{
				foreach (CultureMap item in list)
				{
					item.ClearCultureData ();

					if (this.Collection.Contains (item))
					{
						if (item.IsNewItem)
						{
							this.Collection.Remove (item);
						}
					}
					else
					{
						if (!item.IsNewItem)
						{
							this.Collection.Add (item);
						}
					}
				}
			}

			return list.Count;
		}

		public void NotifyItemChanged(CultureMap item)
		{
			if (this.suspendNotifications == 0)
			{
				this.dirtyItems[item] = true;
			}
		}

		public void NotifyCultureDataCleared(CultureMap item, string twoLetterISOLanguageName, Epsitec.Common.Types.StructuredData data)
		{
		}

		public StructuredData LoadCultureData(CultureMap item, string twoLetterISOLanguageName)
		{
			if (twoLetterISOLanguageName == Resources.DefaultTwoLetterISOLanguageName)
			{
				StructuredData data = new StructuredData (Res.Types.ResourcePanel);

//-				data.SetValue (Res.Fields.ResourcePanel.Bundle, null);
				data.SetValue (Res.Fields.ResourcePanel.DefaultSize, "");
				data.SetValue (Res.Fields.ResourcePanel.XmlSource, "");
				
				return data;
			}
			else
			{
				throw new System.InvalidOperationException ();
			}
		}

		#endregion

		protected void HandleItemsCollectionChanged(object sender, Types.CollectionChangedEventArgs e)
		{
			if (this.suspendNotifications == 0)
			{
				switch (e.Action)
				{
					case Epsitec.Common.Types.CollectionChangedAction.Reset:
						throw new System.InvalidOperationException ("The collection may not be reset");

					case Epsitec.Common.Types.CollectionChangedAction.Add:
					case Epsitec.Common.Types.CollectionChangedAction.Replace:
					case Epsitec.Common.Types.CollectionChangedAction.Remove:
						if (e.OldItems != null)
						{
							foreach (CultureMap item in e.OldItems)
							{
								this.dirtyItems[item] = true;
							}
						}
						if (e.NewItems != null)
						{
							foreach (CultureMap item in e.NewItems)
							{
								this.dirtyItems[item] = true;
							}
						}
						break;

					case Epsitec.Common.Types.CollectionChangedAction.Move:
						break;

					default:
						throw new System.InvalidOperationException (string.Format ("Unknown operation {0} applied to collection", e.Action));
				}
			}
		}

		/// <summary>
		/// Disables any change notifications until <c>Dispose</c> is called
		/// on the returned object. Use this with the <c>using</c> construct.
		/// </summary>
		/// <returns>The object which will automatically restore notifications on disposal.</returns>
		protected System.IDisposable SuspendNotifications()
		{
			return new Suspender (this);
		}

		/// <summary>
		/// Deletes the specified item.
		/// </summary>
		/// <param name="item">The item to delete.</param>
		protected void DeleteItem(CultureMap item)
		{
			StructuredData data = item.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
			ResourceBundle bundle = data.GetValue (Res.Fields.ResourcePanel.Bundle) as ResourceBundle;

			if (bundle == null)
			{
				return;
			}

			this.createdPanels.Remove (bundle);
			this.editedPanels.Remove (bundle);

			System.Diagnostics.Debug.Assert (bundle != null);

			if (!this.deletedPanels.Contains (bundle))
			{
				this.deletedPanels.Add (bundle);
			}
		}

		/// <summary>
		/// Persists the specified item.
		/// </summary>
		/// <param name="item">The item to store as a resource.</param>
		protected void PersistItem(CultureMap item)
		{
			StructuredData data = item.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
			ResourceBundle bundle = data.GetValue (Res.Fields.ResourcePanel.Bundle) as ResourceBundle;
			
			string xmlSource   = data.GetValue (Res.Fields.ResourcePanel.XmlSource) as string;
			string defaultSize = data.GetValue (Res.Fields.ResourcePanel.DefaultSize) as string;

			if (bundle == null)
			{
				string      prefix  = this.manager.ActivePrefix;
				CultureInfo culture = this.manager.ActiveCulture ?? Resources.FindCultureInfo ("fr");

				bundle = ResourceBundle.Create (this.manager, prefix, item.Id.ToBundleId (), ResourceLevel.Default, culture);
				bundle.DefineType (Strings.PanelType);
				bundle.DefineRank (this.Collection.IndexOf (item));
				
				ResourceBundle.Field field;

				field = bundle.CreateField (ResourceFieldType.Data);
				field.SetName (PanelResourceAccessor.Strings.XmlSource);
				bundle.Add (field);

				field = bundle.CreateField (ResourceFieldType.Data);
				field.SetName (Strings.DefaultSize);
				bundle.Add (field);

				this.createdPanels.Add (bundle);

				data.SetValue (Res.Fields.ResourcePanel.Bundle, bundle);
			}
			else
			{
				this.RecordBundleAsEdited (bundle);
			}

			bundle.DefineCaption (item.Name);
			bundle[Strings.XmlSource].SetXmlValue (xmlSource);
			bundle[Strings.DefaultSize].SetStringValue (defaultSize);
		}

		private void RecordBundleAsEdited(ResourceBundle bundle)
		{
			System.Diagnostics.Debug.Assert (bundle != null);

			if ((!this.createdPanels.Contains (bundle)) &&
				(!this.editedPanels.Contains (bundle)))
			{
				this.deletedPanels.Remove (bundle);
				this.editedPanels.Add (bundle);
			}
		}

		private void RenumberBundles()
		{
			int rank = 0;

			foreach (CultureMap item in this.Collection)
			{
				rank++;

				StructuredData data   = item.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
				ResourceBundle bundle = data.GetValue (Res.Fields.ResourcePanel.Bundle) as ResourceBundle;

				if ((bundle != null) &&
					(bundle.Rank != rank))
				{
					bundle.DefineRank (rank);
					this.RecordBundleAsEdited (bundle);
				}
			}
		}

		private Druid CreateId()
		{
			return AbstractResourceAccessor.CreateId (this.Collection, this.manager.DefaultModuleId, null);
		}

		#region Suspender Class

		private class Suspender : System.IDisposable
		{
			public Suspender(PanelResourceAccessor host)
			{
				this.host = host;
				System.Threading.Interlocked.Increment (ref this.host.suspendNotifications);
			}

			#region IDisposable Members

			public void Dispose()
			{
				System.Threading.Interlocked.Decrement (ref this.host.suspendNotifications);
			}

			#endregion

			private PanelResourceAccessor host;
		}

		#endregion

		private static class Strings
		{
			public const string PanelType = "Panel";
			public const string XmlSource = "Panel";
			public const string DefaultSize = "DefaultSize";
		}

		private readonly CultureMapList items;
		private readonly Dictionary<CultureMap, bool> dirtyItems;

		private readonly List<ResourceBundle> editedPanels  = new List<ResourceBundle> ();
		private readonly List<ResourceBundle> deletedPanels = new List<ResourceBundle> ();
		private readonly List<ResourceBundle> createdPanels = new List<ResourceBundle> ();
		
		private ResourceManager manager;
		private int suspendNotifications;
	}
}
