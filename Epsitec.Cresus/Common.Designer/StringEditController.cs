//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

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
		public StringEditController(CommandDispatcher dispatcher)
		{
			this.bundles    = new System.Collections.Hashtable ();
			this.panels     = new System.Collections.ArrayList ();
			this.dispatcher = dispatcher;
			this.provider   = new BundleStringProvider (this);
			
			this.dispatcher.RegisterController (this);
			
			Support.Globals.Properties.SetProperty ("$resources$string data provider", this.DataProvider);
		}
		
		
		public void AttachNewBundle(string prefix, string name, string type)
		{
			string full_name = Resources.MakeFullName (prefix, name);
			
			if (this.ActivateExistingBundle (name))
			{
				return;
			}
			
			ResourceBundleCollection bundles = new ResourceBundleCollection ();
			ResourceBundle           bundle  = ResourceBundle.Create (prefix, name, ResourceLevel.Default);
			
			bundle.DefineType (type);
			bundles.Add (bundle);
			
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
			
			this.CreateMissingBundles (bundles, prefix, name);
			
			this.bundles.Add (name, bundles);
			this.panels.Add (this.CreatePanel (full_name, bundles));
			
			this.ActivateTabBookPage ();
		}
		
		protected void CreateMissingBundles(ResourceBundleCollection bundles, string prefix, string name)
		{
			foreach (CultureInfo culture in Resources.Cultures)
			{
				if (bundles[ResourceLevel.Localised, culture] == null)
				{
					bundles.Add (ResourceBundle.Create (name, prefix, ResourceLevel.Localised, culture));
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
		
		public Window							Window
		{
			get
			{
				if (this.window == null)
				{
					this.CreateWindow ();
				}
				
				return this.window;
			}
		}
		
		public Support.Data.IPropertyProvider	DataProvider
		{
			get
			{
				return this.provider;
			}
		}
		
		
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
				this.OnStoreChanged ();
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
				this.OnStoreChanged ();
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
				this.OnStoreChanged ();
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
				this.OnStoreChanged ();
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
			
			public event Support.EventHandler	StoreChanged;
			#endregion
			
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
			
			private void HandleBundleFieldsChanged(object sender)
			{
				this.OnStoreChanged ();
			}
			
			
			protected virtual void OnStoreChanged()
			{
				if (this.changing == 0)
				{
					if (this.StoreChanged != null)
					{
						this.StoreChanged (this);
					}
				}
			}
			
			
			private string						full_name;
			private StringEditController		controller;
			private ResourceBundleCollection	bundles;
			private ResourceBundle				active_bundle;
			private ResourceBundle				default_bundle;
			private int							changing;
		}
		
		
		protected Panels.StringEditPanel CreatePanel(string full_name, ResourceBundleCollection bundles)
		{
			Panels.StringEditPanel panel = new Panels.StringEditPanel (new Store (this, full_name, bundles));
			
			Widget  widget = panel.Widget;
			Window  window = this.Window;
			TabPage page   = new TabPage ();
			
			System.Diagnostics.Debug.Assert (this.window != null);
			System.Diagnostics.Debug.Assert (this.tab_book != null);
			
			widget.Dock   = DockStyle.Fill;
			widget.Parent = page;
			
			page.TabTitle = bundles[0].Name;
			
			this.tab_book.Items.Add (page);
			
			return panel;
		}
		
		protected void CreateWindow()
		{
			Drawing.Size size = Panels.StringEditPanel.DefaultSize;
			
			this.window = new Window ();
			this.window.ClientSize = size + new Drawing.Size (20, 60);
			this.window.CommandDispatcher = this.dispatcher;
			this.Window.Text = "Ressources textuelles";
			
			this.tool_bar = new HToolBar (this.Window.Root);
			this.tool_bar.Dock = DockStyle.Top;
			this.tool_bar.CommandDispatcher = this.dispatcher;
			
			this.tool_bar.Items.Add (IconButton.CreateSimple ("CreateStringBundle", "manifest:Epsitec.Common.Designer.Images.New.icon"));
			this.tool_bar.Items.Add (IconButton.CreateSimple ("OpenStringBundle()", "manifest:Epsitec.Common.Designer.Images.Open.icon"));
			this.tool_bar.Items.Add (IconButton.CreateSimple ("SaveStringBundle()", "manifest:Epsitec.Common.Designer.Images.Save.icon"));
			
			this.tab_book = new TabBook (this.Window.Root);
			this.tab_book.Dock = DockStyle.Fill;
			this.tab_book.DockMargins = new Drawing.Margins (8, 8, 8, 8);
			this.tab_book.HasCloseButton = true;
			this.tab_book.CloseClicked += new EventHandler (this.HandleTabBookCloseClicked);
		}
		
		
		[Command ("CreateStringBundle")]	void CommandCreateStringBundle(CommandDispatcher d, CommandEventArgs e)
		{
			if (e.CommandArgs.Length == 0)
			{
				Dialogs.BundleName dialog = new Dialogs.BundleName ("CreateStringBundle (\"{0}\", \"{1}\")", this.dispatcher);
				dialog.Owner = this.Window;
				dialog.Show ();
			}
			else if (e.CommandArgs.Length == 2)
			{
				string bundle_prefix = e.CommandArgs[0];
				string bundle_name   = e.CommandArgs[1];
				
				this.AttachNewBundle (bundle_prefix, bundle_name, "String");
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
				dialog.Owner = this.Window;
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
			if (e.CommandArgs.Length > 0)
			{
				this.ThrowInvalidOperationException (e, 0);
			}
			
			ResourceBundleCollection bundles = this.ActiveBundleCollection;
			
			if (bundles != null)
			{
				foreach (ResourceBundle bundle in bundles)
				{
					if (! bundle.IsEmpty)
					{
						Resources.SetBundle (bundle, ResourceSetMode.Write);
					}
				}
			}
		}
		
		
		private void HandleTabBookCloseClicked(object sender)
		{
			int index = this.tab_book.ActivePageIndex;
			
			if (index > -1)
			{
				//	Ferme la page active.
				
				Panels.StringEditPanel panel = this.panels[index] as Panels.StringEditPanel;
				string name = panel.Store.Name;
				
				this.bundles.Remove (name);
				this.panels.RemoveAt (index);
				this.tab_book.Items.RemoveAt (index);
			}
		}
		
		private void ActivateTabBookPage()
		{
			this.tab_book.ActivePageIndex = this.bundles.Count - 1;
		}
		
		
		private class BundleStringProvider : Support.Data.IPropertyProvider
		{
			public BundleStringProvider(StringEditController host)
			{
				this.host = host;
			}
			
			
			#region IPropertyProvider Members
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
			}

			public void SetProperty(string key, object value)
			{
				// TODO:  Add Resource.SetProperty implementation
			}
			#endregion
			
			private ResourceLevel				level = ResourceLevel.Default;
			private StringEditController		host;
		}
		
		protected System.Collections.Hashtable	bundles;
		protected System.Collections.ArrayList	panels;
		
		private BundleStringProvider			provider;
		
		protected Window						window;
		protected AbstractToolBar				tool_bar;
		protected TabBook						tab_book;
		protected Support.CommandDispatcher		dispatcher;
	}
}
