//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer
{
	using CultureInfo = System.Globalization.CultureInfo;
	
	/// <summary>
	/// La classe StringEditController gère l'édition d'un bundle de ressources
	/// textuelles.
	/// </summary>
	public class StringEditController : AbstractController
	{
		public StringEditController(Application application) : base (application)
		{
			this.bundles    = new System.Collections.Hashtable ();
			this.panels     = new System.Collections.ArrayList ();
			this.provider   = new BundleStringProvider (this);
			
			this.save_command_state = CommandState.Find ("SaveStringBundle", this.dispatcher);
			this.UpdateCommandStates ();
		}
		
		
		public override void Initialise()
		{
			System.Diagnostics.Debug.Assert (this.is_initialised == false);
			
			this.CreateMainPanel ();
			this.UpdateCommandStates ();
			this.SyncCommandStates ();
			
			this.is_initialised = true;
		}
		
		public override void FillToolBar(AbstractToolBar tool_bar)
		{
			tool_bar.Items.Add (IconButton.CreateSimple ("CreateStringBundle", "manifest:Epsitec.Common.Designer.Images.New.icon"));
			tool_bar.Items.Add (IconButton.CreateSimple ("OpenStringBundle()", "manifest:Epsitec.Common.Designer.Images.Open.icon"));
			tool_bar.Items.Add (IconButton.CreateSimple ("SaveStringBundle()", "manifest:Epsitec.Common.Designer.Images.Save.icon"));
			
			this.SyncCommandStates ();
		}
		
		
		public void AttachNewBundle(string prefix, string name)
		{
			string full_name = Resources.MakeFullName (prefix, name);
			
			if (this.ActivateExistingBundle (name))
			{
				return;
			}
			
			ResourceBundleCollection bundles = new ResourceBundleCollection ();
			ResourceBundle           bundle  = ResourceBundle.Create (prefix, name, ResourceLevel.Default);
			
			bundle.DefineType ("String");
			bundles.Add (bundle);
			bundles.Name = full_name;
			
			this.CreateMissingBundles (bundles, prefix, name);
			
			this.bundles.Add (name, bundles);
			this.panels.Add (this.CreatePanel (full_name, bundles));
			
			this.ActivateTabBookPage ();
		}
		
		public void AttachExistingBundle(string prefix, string name)
		{
			string full_name = Resources.MakeFullName (prefix, name);
			
			if (this.ActivateExistingBundle (name))
			{
				return;
			}
			
			string[] ids = Resources.GetBundleIds (full_name, null, ResourceLevel.All, null);
			ResourceBundleCollection bundles = new ResourceBundleCollection (prefix, ids);
			
			bundles.Name = full_name;
			
			this.CreateMissingBundles (bundles, prefix, name);
			
			this.bundles.Add (name, bundles);
			this.panels.Add (this.CreatePanel (full_name, bundles));
			
			this.ActivateTabBookPage ();
		}
		
		
		public bool SaveAllBundles()
		{
			foreach (ResourceBundleCollection bundles in this.bundles.Values)
			{
				this.SaveBundles (bundles);
			}
			
			return true;
		}
		
		public bool SaveBundles(string name)
		{
			return this.SaveBundles (this.bundles[name] as ResourceBundleCollection);
		}
		
		public bool SaveBundles(ResourceBundleCollection bundles)
		{
			if (bundles != null)
			{
				Store store = this.FindStore (bundles);
				
				System.Diagnostics.Debug.Assert (store != null);
				System.Diagnostics.Debug.Assert (store.FullName == bundles.Name);
				
				//	On s'assure que lors de la sauvegarde des bundles, l'édition n'est plus active; ceci
				//	permet de garantir qu'aucun bundle partiel n'est enregistré...
				
				this.ValidateEdition ();
				
				foreach (ResourceBundle bundle in bundles)
				{
					if (! bundle.IsEmpty)
					{
						Resources.SetBundle (bundle, ResourceSetMode.Write);
					}
				}
				
				store.ResetChanges ();
				
				return true;
			}
			
			return false;
		}
		
		public void CloseBundles(string name)
		{
			ResourceBundleCollection bundles = this.bundles[name] as ResourceBundleCollection;
			
			if (bundles != null)
			{
				for (int i = 0; i < this.panels.Count; i++)
				{
					Panels.StringEditPanel panel = this.panels[i] as Panels.StringEditPanel;
					
					if (panel.Store.Name == name)
					{
						this.bundles.Remove (name);
						this.panels.RemoveAt (i);
						this.tab_book.Items.RemoveAt (i);
						return;
					}
				}
			}
		}
		
		
		public void ValidateEdition()
		{
			for (int i = 0; i < this.panels.Count; i++)
			{
				Panels.StringEditPanel panel = this.panels[i] as Panels.StringEditPanel;
				EditArray              edit  = panel.EditArray;
				
				if (edit.InteractionMode == ScrollInteractionMode.Edition)
				{
					edit.ValidateEdition (false);
				}
			}
		}
		
		
		public Store FindStore(string bundle_name)
		{
			if (this.bundles.ContainsKey (bundle_name))
			{
				for (int i = 0; i < this.panels.Count; i++)
				{
					Panels.StringEditPanel panel = this.panels[i] as Panels.StringEditPanel;
					
					if (panel.Store.Name == bundle_name)
					{
						return panel.Store;
					}
				}
			}
			
			return null;
		}
		
		public Store FindStore(ResourceBundleCollection bundles)
		{
			for (int i = 0; i < this.panels.Count; i++)
			{
				Panels.StringEditPanel panel = this.panels[i] as Panels.StringEditPanel;
				
				if (panel.Store.Bundles == bundles)
				{
					return panel.Store;
				}
			}
			
			return null;
		}
		
		
		protected void CreateMissingBundles(ResourceBundleCollection bundles, string prefix, string name)
		{
			foreach (CultureInfo culture in Resources.Cultures)
			{
				if (bundles[ResourceLevel.Localised, culture] == null)
				{
					bundles.Add (ResourceBundle.Create (prefix, name, ResourceLevel.Localised, culture));
				}
			}
		}
		
		protected bool ActivateExistingBundle(string name)
		{
			if (this.bundles.ContainsKey (name))
			{
				for (int i = 0; i < this.panels.Count; i++)
				{
					Panels.StringEditPanel panel = this.panels[i] as Panels.StringEditPanel;
					
					if (panel.Store.Name == name)
					{
						this.tab_book.ActivePageIndex = i;
						return true;
					}
				}
				
				throw new System.InvalidOperationException (string.Format ("Bundle '{0}' cannot be recycled.", name));
			}
			
			return false;
		}
		
		
		
		public Store							ActiveStore
		{
			get
			{
				Panels.StringEditPanel panel = this.ActivePanel;
				
				if (panel != null)
				{
					return panel.Store;
				}
				
				return null;
			}
		}
		
		public ResourceBundleCollection			ActiveBundleCollection
		{
			get
			{
				Panels.StringEditPanel panel = this.ActivePanel;
				
				if (panel != null)
				{
					return panel.Store.Bundles;
				}
				
				return null;
			}
		}
		
		public Panels.StringEditPanel			ActivePanel
		{
			get
			{
				int active = this.tab_book.ActivePageIndex;
				
				if (active > -1)
				{
					return this.panels[active] as Panels.StringEditPanel;
				}
				
				return null;
			}
		}
		
		public Widget							MainPanel
		{
			get
			{
				return this.main_panel;
			}
		}
		
		public Support.Data.IPropertyProvider	Provider
		{
			get
			{
				return this.provider;
			}
		}
		
		
		public bool IsStringBundleLoaded(string name)
		{
			return this.bundles.ContainsKey (name);
		}
		
		public bool LoadStringBundle(string name)
		{
			if (this.IsStringBundleLoaded (name))
			{
				return true;
			}
			
			try
			{
				this.AttachExistingBundle (this.default_prefix, name);
			}
			catch
			{
				return false;
			}
			
			return this.IsStringBundleLoaded (name);
		}
		
		
		public string[] GetStringBundleNames()
		{
			string[] names = new string[this.bundles.Count];
			this.bundles.Keys.CopyTo (names, 0);
			System.Array.Sort (names);
			return names;
		}
		
		public string[] GetStringFieldNames(string bundle_name)
		{
			ResourceBundleCollection bundles = this.bundles[bundle_name] as ResourceBundleCollection;
			
			if (bundles != null)
			{
				ResourceBundle bundle = bundles[ResourceLevel.Default];
				
				if (bundle != null)
				{
					return bundle.FieldNames;
				}
			}
			
			return new string[0];
		}
		
		
		#region Store Class
		public class Store : Support.Data.ITextArrayStore
		{
			public Store(StringEditController controller, string full_name, ResourceBundleCollection bundles)
			{
				this.controller = controller;
				this.full_name  = full_name;
				this.bundles    = bundles;
				
				this.bundles.FieldsChanged += new Epsitec.Common.Support.EventHandler (this.HandleBundleFieldsChanged);
				
				this.SetActive (ResourceLevel.Default, null);
			}
			
			
			#region ITextArrayStore Members
			public void InsertRows(int row, int num)
			{
				this.changing++;
				
				//	L'insertion de nouvelles lignes doit se faire dans le bundle par défaut; les variantes
				//	localisées s'adapteront automatiquement par la suite :
				
				for (int i = 0; i < num; i++)
				{
					ResourceBundle.Field field = this.DefaultBundle.CreateField (ResourceFieldType.Data);
					this.DefaultBundle.Insert (row++, field);
				}
				
				this.changing--;
				this.OnStoreContentsChanged ();
			}
			
			public void RemoveRows(int row, int num)
			{
				this.changing++;
				
				//	La destruction des lignes doit se faire dans tous les bundles :
				
				for (int i = 0; i < num; i++)
				{
					string name = this.DefaultBundle[row].Name;
					
					//	Supprime du bundle par défaut :
					
					this.DefaultBundle.Remove (row);
					
					//	Supprime aussi ce champ dans tous les autres bundles :
					
					foreach (ResourceBundle bundle in this.Bundles)
					{
						bundle.Remove (name);
					}
				}
				
				this.changing--;
				this.OnStoreContentsChanged ();
			}
			
			public void MoveRow(int row, int distance)
			{
				this.changing++;
				
				//	La réorganisation de l'ordre des champs d'un bundle n'a réellement de sens
				//	que pour le bundle par défaut, vu que les autres bundles sont uniquement
				//	accédés par leur nom :
				
				int row_a = row;
				int row_b = row + distance;
				
				ResourceBundle.Field field_a = this.DefaultBundle[row_a];
				ResourceBundle.Field field_b = this.DefaultBundle[row_b];
				
				string name_a  = field_a.Name;
				string name_b  = field_b.Name;
				string value_a = field_a.AsString;
				string value_b = field_b.AsString;
				string about_a = field_a.About;
				string about_b = field_b.About;
				
				//	Permute :
				
				field_a.SetName (name_b);
				field_b.SetName (name_a);
				field_a.SetStringValue (value_b);
				field_b.SetStringValue (value_a);
				field_a.SetAbout (about_b);
				field_b.SetAbout (about_a);
				
				this.changing--;
				this.OnStoreContentsChanged ();
			}
			
			public string GetCellText(int row, int column)
			{
				ResourceBundle.Field field;
				string name;
				
				switch (column)
				{
					case 0:
						//	Le nom provient toujours du bundle par défaut :
						
						field = this.DefaultBundle[row];
						return TextLayout.ConvertToTaggedText (field.Name);
					
					case 1:
						//	La valeur provient soit du bundle réel, soit du bundle par défaut s'il s'avère
						//	que le bundle réel ne définit aucune valeur :
						
						name  = this.DefaultBundle[row].Name;
						field = this.ActiveBundle[name];
						
						if (field.IsEmpty)
						{
							return this.DefaultBundle[row].AsString;
						}
						
						return field.AsString;
				}
				
				return null;
			}
			
			public void SetCellText(int row, int column, string value)
			{
				this.changing++;
				
				ResourceBundle.Field field_1;
				ResourceBundle.Field field_2;
				
				string value_1;
				string value_2;
				
				string old_name;
				string new_name;
				
				switch (column)
				{
					case 0:
						//	Le nom est toujours stocké dans le bundle par défaut, mais il faut aussi
						//	renommer tous les champs des autres bunldes :
						
						field_1 = this.DefaultBundle[row];
						
						old_name = field_1.Name;
						new_name = TextLayout.ConvertToSimpleText (value);
						
						if (old_name != new_name)
						{
							//	Renomme tous les bundles d'un coup.
							
							foreach (ResourceBundle bundle in this.Bundles)
							{
								field_2 = bundle[old_name];
								
								if (! field_2.IsEmpty)
								{
									field_2.SetName (new_name);
								}
							}
						}
						break;
					
					case 1:
						//	La valeur doit être stockée dans le bundle actif. Par contre, s'il s'avère que
						//	le contenu est exactement le même que celui du bundle par défaut, on supprime
						//	le champ du bundle actif :
						
						field_1 = this.DefaultBundle[row];
						field_2 = this.ActiveBundle[field_1.Name];
						
						value_1 = field_1.AsString;
						value_2 = value;
						
						if ((value_1 == value_2) ||
							(value_2 == ""))
						{
							//	Les valeurs sont identiques, on peut/doit donc supprimer le champ dans le
							//	bundle actif, si celui-ci n'est pas le bundle par défaut :
							
							if ((! field_2.IsEmpty) &&
								(field_2 != field_1))
							{
								this.ActiveBundle.Remove (field_1.Name);
							}
						}
						else if (field_2.IsEmpty)
						{
							//	Les valeurs ont changé et il n'y a pas encore de champ dans le bundle
							//	actif; on ajoute donc un champ tout neuf :
							
							field_2 = this.ActiveBundle.CreateField (ResourceFieldType.Data);
							field_2.SetName (field_1.Name);
							field_2.SetStringValue (value_2);
							this.ActiveBundle.Insert (field_2);
						}
						else
						{
							//	Les valeurs ont changé et il y avait déjà un champ correspondant dans le
							//	bundle actif; on met simplement à jour sa valeur :
							
							field_2.SetStringValue (value_2);
						}
						break;
				}
				
				this.changing--;
				this.OnStoreContentsChanged ();
			}
			
			public int GetColumnCount()
			{
				return 2;
			}
			
			public int GetRowCount()
			{
				return this.CountBundleFields;
			}
			
			public bool CheckSetRow(int row)
			{
				return (row >= 0) && (row < this.GetRowCount ());
			}
			
			public bool CheckInsertRows(int row, int num)
			{
				if (! this.IsDefaultActive)
				{
					return false;
				}
				
				return (row >= 0) && (row <= this.GetRowCount ());
			}
			
			public bool CheckRemoveRows(int row, int num)
			{
				if (! this.IsDefaultActive)
				{
					return false;
				}
				
				return (row >= 0) && (row < this.GetRowCount ()) && (row + num <= this.GetRowCount ());
			}
			
			public bool CheckMoveRow(int row, int distance)
			{
				int max = this.GetRowCount ();
				return (row >= 0) && (row < max) && (row + distance >= 0) && (row + distance < max);
			}
			
			public bool CheckEnabledCell(int row, int column)
			{
				if (this.IsDefaultActive)
				{
					return true;
				}
				
				return this.ActiveBundle[this.DefaultBundle[row].Name].IsEmpty ? false : true;
			}
			
			public event Support.EventHandler	StoreContentsChanged;
			#endregion
			
			public bool							Changed
			{
				get
				{
					return this.changes > 0;
				}
			}
			
			public bool							IsDefaultActive
			{
				get
				{
					return this.active_bundle == this.default_bundle;
				}
			}
			
			public ResourceBundle				ActiveBundle
			{
				get
				{
					return this.active_bundle;
				}
			}
			
			public ResourceBundle				DefaultBundle
			{
				get
				{
					return this.default_bundle;
				}
			}
			
			public string						Name
			{
				get
				{
					return Resources.ExtractName (this.full_name);
				}
			}
			
			public string						FullName
			{
				get
				{
					return this.full_name;
				}
			}
			
			public ResourceBundleCollection		Bundles
			{
				get
				{
					return this.bundles;
				}
			}
			
			public int							CountBundleFields
			{
				get
				{
					if (this.DefaultBundle != null)
					{
						return this.DefaultBundle.CountFields;
					}
					
					return 0;
				}
			}
			
			public Panels.StringEditPanel		Panel
			{
				get
				{
					return this.panel;
				}
				set
				{
					this.panel = value;
				}
			}
			
			public void SetActive(ResourceLevel level, CultureInfo culture)
			{
				this.active_bundle  = this.bundles[level, culture];
				this.default_bundle = this.bundles[ResourceLevel.Default];
			}
			
			public void GetLevelNamesAndCaptions(out string[] names, out string[] captions)
			{
				names    = this.bundles.Suffixes;
				captions = new string[names.Length];
				
				for (int i = 0; i < names.Length; i++)
				{
					string        suffix = names[i];
					ResourceLevel level;
					CultureInfo   culture;
					
					Resources.MapFromSuffix (suffix, out level, out culture);
					
					captions[i] = Resources.GetLevelCaption (level, culture);
				}
			}
			
			public void ResetChanges()
			{
				if (this.changes > 0)
				{
					this.changes = 0;
					this.controller.UpdateCommandStates ();
				}
			}
			
			
			private void HandleBundleFieldsChanged(object sender)
			{
				this.OnStoreContentsChanged ();
			}
			
			
			protected virtual void OnStoreContentsChanged ()
			{
				this.changes++;
				
				if (this.changing == 0)
				{
					this.controller.OnStoreContentsChanged ();
					
					if (this.StoreContentsChanged != null)
					{
						this.StoreContentsChanged (this);
					}
				}
			}
			
			
			private string						full_name;
			private StringEditController		controller;
			private ResourceBundleCollection	bundles;
			private ResourceBundle				active_bundle;
			private ResourceBundle				default_bundle;
			private int							changing;
			private int							changes;
			private Panels.StringEditPanel		panel;
		}
		#endregion
		
		#region BundleStringProvider Class
		private class BundleStringProvider : Support.Data.IPropertyProvider
		{
			public BundleStringProvider(StringEditController host)
			{
				this.host = host;
			}
			
			
			#region IPropertyProvider Members
			public string[] GetPropertyNames()
			{
				System.Collections.ArrayList list = new System.Collections.ArrayList ();
				
				foreach (System.Collections.DictionaryEntry entry in this.host.bundles)
				{
					string         bundle_name = entry.Key as string;
					ResourceBundle bundle      = (entry.Value as ResourceBundleCollection)[ResourceLevel.Default];
					string[]       field_names = bundle.FieldNames;
					
					for (int i = 0; i < field_names.Length; i++)
					{
						list.Add (ResourceBundle.MakeTarget (bundle_name, field_names[i]));
					}
				}
				
				string[] names = new string[list.Count];
				list.CopyTo (names);
				System.Array.Sort (names);
				
				return names;
			}
		
			public object GetProperty(string key)
			{
				string bundle_name;
				string field_name;
				
				ResourceBundle.SplitTarget (key, out bundle_name, out field_name);
				
				ResourceBundleCollection bundles = this.host.bundles[bundle_name] as ResourceBundleCollection;
				
				if (bundles != null)
				{
					ResourceBundle.Field field = bundles[this.level][field_name];
					
					if (field.IsValid)
					{
						return field.AsString;
					}
				}
				
				return null;
			}

			public bool IsPropertyDefined(string key)
			{
				string bundle_name;
				string field_name;
				
				ResourceBundle.SplitTarget (key, out bundle_name, out field_name);
				
				ResourceBundleCollection bundles = this.host.bundles[bundle_name] as ResourceBundleCollection;
				
				if (bundles != null)
				{
					ResourceBundle.Field field = bundles[this.level][field_name];
					
					if (field.IsValid)
					{
						return true;
					}
				}
				
				return false;
			}

			public void ClearProperty(string key)
			{
				// TODO:  Add Resource.ClearProperty implementation
				
				throw new System.NotImplementedException ("ClearProperty not implemented.");
			}

			public void SetProperty(string key, object value)
			{
				// TODO:  Add Resource.SetProperty implementation
				
				throw new System.NotImplementedException ("SetProperty not implemented.");
			}
			#endregion
			
			private ResourceLevel				level = ResourceLevel.Default;
			private StringEditController		host;
		}
		#endregion
		
		
		protected Panels.StringEditPanel CreatePanel(string full_name, ResourceBundleCollection bundles)
		{
			Store                  store = new Store (this, full_name, bundles);
			Panels.StringEditPanel panel = new Panels.StringEditPanel (store);
			
			store.Panel = panel;
			
			Widget  widget = panel.Widget;
			TabPage page   = new TabPage ();
			
			System.Diagnostics.Debug.Assert (this.tab_book != null);
			
			widget.Dock   = DockStyle.Fill;
			widget.Parent = page;
			
			page.TabTitle = bundles[0].Name;
			
			this.tab_book.Items.Add (page);
			
			return panel;
		}
		
		protected void CreateMainPanel()
		{
			Drawing.Size size = Panels.StringEditPanel.DefaultSize;
			
			this.main_panel = new Widget ();
			this.main_panel.Size = size + new Drawing.Size (20, 60);
			this.main_panel.CommandDispatcher = this.dispatcher;
			this.main_panel.VisibleChanged   += new EventHandler (this.HandleMainPanelVisibleChanged);
			
			this.tab_book = new TabBook (this.main_panel);
			this.tab_book.Dock = DockStyle.Fill;
			this.tab_book.DockMargins = new Drawing.Margins (4, 4, 4, 4);
			this.tab_book.HasCloseButton = true;
			this.tab_book.CloseClicked      += new EventHandler (this.HandleTabBookCloseClicked);
			this.tab_book.ActivePageChanged += new EventHandler (this.HandleTabBookActivePageChanged);
		}
		
		
		[Command ("CreateStringBundle")]	void CommandCreateStringBundle(CommandDispatcher d, CommandEventArgs e)
		{
			if (e.CommandArgs.Length == 0)
			{
				Dialogs.BundleName dialog = new Dialogs.BundleName ("CreateStringBundle (\"{0}\", \"{1}\")", this.dispatcher);
				dialog.Owner = this.main_panel.Window;
				dialog.Show ();
			}
			else if (e.CommandArgs.Length == 2)
			{
				string bundle_prefix = e.CommandArgs[0];
				string bundle_name   = e.CommandArgs[1];
				
				this.AttachNewBundle (bundle_prefix, bundle_name);
			}
			else
			{
				this.ThrowInvalidOperationException (e, 2);
			}
		}
		
		[Command ("OpenStringBundle")]		void CommandOpenStringBundle(CommandDispatcher d, CommandEventArgs e)
		{
			if (e.CommandArgs.Length == 0)
			{
				Dialogs.OpenExistingBundle dialog = new Dialogs.OpenExistingBundle ("OpenStringBundle (\"{0}\")", this.dispatcher);
				dialog.SubBundleSpec.TypeFilter = "String";
				dialog.Owner = this.main_panel.Window;
				dialog.UpdateListContents ();
				dialog.Show ();
			}
			else if (e.CommandArgs.Length == 1)
			{
				string         full_id = e.CommandArgs[0];
				string         prefix  = Resources.ExtractPrefix (full_id);
				string         name    = Resources.ExtractName (full_id);
				
				this.AttachExistingBundle (prefix, name);
			}
			else
			{
				this.ThrowInvalidOperationException (e, 1);
			}
		}
		
		[Command ("SaveStringBundle")]		void CommandSaveStringBundle(CommandDispatcher d, CommandEventArgs e)
		{
			if (e.CommandArgs.Length == 1)
			{
				this.SaveBundles (e.CommandArgs[0]);
				return;
			}
			
			if (e.CommandArgs.Length > 0)
			{
				this.ThrowInvalidOperationException (e, 0);
			}
			
			this.SaveBundles (this.ActiveBundleCollection);
		}
		
		[Command ("CloseStringBundle")]		void CommandCloseStringBundle(CommandDispatcher d, CommandEventArgs e)
		{
			if (e.CommandArgs.Length != 1)
			{
				this.ThrowInvalidOperationException (e, 1);
			}
			
			this.CloseBundles (e.CommandArgs[0]);
		}
		
		
		private void HandleTabBookCloseClicked(object sender)
		{
			int index = this.tab_book.ActivePageIndex;
			
			if (index > -1)
			{
				//	Ferme la page active.
				
				Panels.StringEditPanel panel = this.panels[index] as Panels.StringEditPanel;
				Store                  store = panel.Store;
				string                 name  = store.Name;
				
				if (store.Changed == false)
				{
					this.CloseBundles (name);
				}
				else
				{
					string question    = "The bundle has been modified.<br/><br/>Would you like to save it ?";
					string command_yes = "SaveStringBundle (\""+name+"\") -> CloseStringBundle (\""+name+"\")";
					string command_no  = "CloseStringBundle (\""+name+"\")";
					
					Common.Dialogs.IDialog dialog = Common.Dialogs.Message.CreateYesNoCancel (this.application.Name, Common.Dialogs.Icon.Warning, question, command_yes, command_no, this.dispatcher);
					
					dialog.Owner = this.main_panel.Window;
					dialog.Show ();
				}
			}
		}
		
		private void HandleTabBookActivePageChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.tab_book == sender);
			
			this.OnActiveStoreChanged ();
		}
		
		private void HandleMainPanelVisibleChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.main_panel == sender);
			this.ValidateEdition ();
		}
		
		
		private void ActivateTabBookPage()
		{
			this.tab_book.ActivePageIndex = this.bundles.Count - 1;
		}
		
		private void UpdateCommandStates()
		{
			System.Diagnostics.Debug.Assert (this.save_command_state != null);
			
			if ((this.tab_book != null) &&
				(this.tab_book.Items.Count > 0))
			{
				this.tab_book.CloseButton.SetEnabled (true);
				
				Store store = this.ActiveStore;
				
				if ((store != null) &&
					(store.Changed))
				{
					this.save_command_state.Enabled = true;
				}
				else
				{
					this.save_command_state.Enabled = false;
				}
			}
			else
			{
				if (this.tab_book != null)
				{
					this.tab_book.CloseButton.SetEnabled (false);
				}
				this.save_command_state.Enabled = false;
			}
		}
		
		private void SyncCommandStates()
		{
			this.save_command_state.Synchronise ();
		}
		
		
		protected virtual void OnActiveStoreChanged()
		{
			this.ValidateEdition ();
			this.UpdateCommandStates ();
			
			if (this.ActiveStoreChanged != null)
			{
				this.ActiveStoreChanged (this);
			}
		}
		
		protected virtual void OnStoreContentsChanged()
		{
			this.UpdateCommandStates ();
			
			if (this.StoreContentsChanged != null)
			{
				this.StoreContentsChanged (this);
			}
		}
		
		
		public event Support.EventHandler		ActiveStoreChanged;
		public event Support.EventHandler		StoreContentsChanged;
		
		
		private bool							is_initialised;
		
		protected System.Collections.Hashtable	bundles;
		protected System.Collections.ArrayList	panels;
		
		private BundleStringProvider			provider;
		
		protected string						default_prefix = "file";
		protected Widget						main_panel;
		protected TabBook						tab_book;
		protected CommandState					save_command_state;
	}
}
