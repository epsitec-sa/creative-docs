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
			
			this.dispatcher.RegisterController (this);
		}
		
		
		public void AttachNewBundle(string full_id, string prefix, string type, ResourceLevel level, CultureInfo culture)
		{
			string name = Resources.ExtractName (full_id);
			ResourceBundle bundle = ResourceBundle.Create (name, prefix, level, culture);
			
			bundle.DefineType (type);
			
			this.AttachExistingBundle (prefix, name, level, culture);
			this.ActivateTabBookPage ();
		}
		
		public void AttachExistingBundle(string prefix, string name, ResourceLevel level, CultureInfo culture)
		{
			string full_name = Resources.MakeFullName (prefix, name);
			
			if (this.bundles.ContainsKey (full_name))
			{
				throw new System.InvalidOperationException (string.Format ("Cannot attach bundle {0} twice.", full_name));
			}
			
			string[] ids = Resources.GetBundleIds (full_name, null, ResourceLevel.All, culture);
			ResourceBundleCollection bundles = new ResourceBundleCollection (prefix, ids);
			
			this.bundles.Add (full_name, bundles);
			this.panels.Add (this.CreatePanel (bundles));
			
			this.ActivateTabBookPage ();
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
		
		
		public class Store : Support.Data.ITextArrayStore
		{
			public Store(StringEditController controller, ResourceBundleCollection bundles)
			{
				this.bundles    = bundles;
				this.controller = controller;
				
				this.bundles.FieldsChanged += new Epsitec.Common.Support.EventHandler (this.HandleBundleFieldsChanged);
				
				this.SetActive (ResourceLevel.Default, null);
			}
			
			
			#region ITextArrayStore Members
			public void InsertRows(int row, int num)
			{
				this.changing++;
				
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
				
				for (int i = 0; i < num; i++)
				{
					this.DefaultBundle.Remove (row);
				}
				
				this.changing--;
				this.OnStoreChanged ();
			}
			
			public void MoveRow(int row, int distance)
			{
				this.changing++;
				
				int row_a = row;
				int row_b = row + distance;
				
				int n = this.GetColumnCount ();
				
				for (int i = 0; i < n; i++)
				{
					string a = this.GetCellText (row_a, i);
					string b = this.GetCellText (row_b, i);
					
					this.SetCellText (row_a, i, b);
					this.SetCellText (row_b, i, a);
				}
				
				string about_a = this.DefaultBundle[row_a].About;
				string about_b = this.DefaultBundle[row_b].About;
				
				this.DefaultBundle[row_a].SetAbout (about_b);
				this.DefaultBundle[row_b].SetAbout (about_a);
				
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
						field = this.DefaultBundle[row];
						return TextLayout.ConvertToTaggedText (field.Name);
					case 1:
						name  = this.DefaultBundle[row].Name;
						field = this.ActiveBundle[name];
						
						if (field == null)
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
				
				ResourceBundle.Field field;
				string name;
				
				switch (column)
				{
					case 0:
						field = this.DefaultBundle[row];
						field.SetName (TextLayout.ConvertToSimpleText (value));
						break;
					case 1:
						name  = this.DefaultBundle[row].Name;
						field = this.ActiveBundle[name];
						
						if (field == null)
						{
							field = this.ActiveBundle.CreateField (ResourceFieldType.Data);
							field.SetName (name);
							field.SetStringValue (value);
							this.ActiveBundle.Insert (field);
						}
						else
						{
							field.SetStringValue (value);
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
				return (row >= 0) && (row <= this.GetRowCount ());
			}
			
			public bool CheckRemoveRows(int row, int num)
			{
				return (row >= 0) && (row < this.GetRowCount ()) && (row + num <= this.GetRowCount ());
			}
			
			public bool CheckMoveRow(int row, int distance)
			{
				int max = this.GetRowCount ();
				return (row >= 0) && (row < max) && (row + distance >= 0) && (row + distance < max);
			}
			
			
			public event Support.EventHandler	StoreChanged;
			#endregion
			
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
					return this.bundles[ResourceLevel.Default];
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
				this.active_bundle = this.bundles[level, culture];
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
			
			
			private ResourceBundleCollection	bundles;
			private ResourceBundle				active_bundle;
			private StringEditController		controller;
			private int							changing;
		}
		
		
		protected Panels.StringEditPanel CreatePanel(ResourceBundleCollection bundles)
		{
			Panels.StringEditPanel panel = new Panels.StringEditPanel (new Store (this, bundles));
			
			Widget  widget = panel.Widget;
			Window  window = this.Window;
			TabPage page   = new TabPage ();
			
			System.Diagnostics.Debug.Assert (this.window != null);
			System.Diagnostics.Debug.Assert (this.tab_book != null);
			
			window.Root.DockPadding = new Drawing.Margins (8, 8, 8, 8);
			
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
			
			this.tab_book = new TabBook ();
			this.tab_book.Dock = DockStyle.Fill;
			this.tab_book.Parent = this.window.Root;
			this.tab_book.HasCloseButton = true;
			this.tab_book.CloseClicked += new EventHandler (this.HandleTabBookCloseClicked);
		}
		
		
		[Command ("CreateStringBundle")]	void CommandCreateStringBundle(CommandDispatcher d, CommandEventArgs e)
		{
			if (e.CommandArgs.Length == 0)
			{
				Dialogs.BundleName dialog = new Dialogs.BundleName ("CreateStringBundle (\"{0}\", \"{1}\", {2}, {3})", this.dispatcher);
				dialog.Owner = this.Window;
				dialog.Show ();
			}
			else if (e.CommandArgs.Length == 4)
			{
				string bundle_prefix = e.CommandArgs[0];
				string bundle_id     = e.CommandArgs[1];
				string bundle_level  = e.CommandArgs[2];
				string culture_code  = e.CommandArgs[3];
				
				string      full_id = Resources.MakeFullName (bundle_prefix, bundle_id);
				ResourceLevel level = (ResourceLevel) System.Enum.Parse (typeof (ResourceLevel), bundle_level);
				CultureInfo culture = Resources.FindCultureInfo (culture_code);
				
				this.AttachNewBundle (full_id, bundle_prefix, "String", level, culture);
			}
			else
			{
				this.ThrowInvalidOperationException (e, 1);
			}
		}
		
		[Command ("OpenStringBundle")]		void CommandOpenStringBundle(CommandDispatcher d, CommandEventArgs e)
		{
			if (e.CommandArgs.Length == 0)
			{
				Dialogs.OpenExistingBundle dialog = new Dialogs.OpenExistingBundle ("OpenStringBundle (\"{0}\", {1}, \"{2}\")", this.dispatcher);
				dialog.SubBundleSpec.TypeFilter = "String";
				dialog.Owner = this.Window;
				dialog.UpdateListContents ();
				dialog.Show ();
			}
			else if (e.CommandArgs.Length == 3)
			{
				ResourceLevel  level   = (ResourceLevel) System.Enum.Parse (typeof (ResourceLevel), e.CommandArgs[1]);
				string         full_id = e.CommandArgs[0];
				string         prefix  = Resources.ExtractPrefix (full_id);
				string         name    = Resources.ExtractName (full_id);
				CultureInfo    culture = Resources.FindCultureInfo (e.CommandArgs[2]);
				ResourceBundle bundle  = Resources.GetBundle (e.CommandArgs[0], level, culture);
				
				this.AttachExistingBundle (prefix, name, level, culture);
			}
			else
			{
				this.ThrowInvalidOperationException (e, 3);
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
					Resources.SetBundle (bundle, ResourceSetMode.Write);
				}
			}
		}
		
		
		private void HandleTabBookCloseClicked(object sender)
		{
			ResourceBundleCollection bundles = this.ActiveBundleCollection;
			
			if (bundles != null)
			{
				//	Ferme la page active...
				
				this.tab_book.Items.RemoveAt (this.tab_book.ActivePageIndex);
				
				foreach (System.Collections.DictionaryEntry entry in this.bundles)
				{
					if (entry.Value == bundles)
					{
						this.bundles.Remove (entry.Key);
						break;
					}
				}
			}
		}
		
		private void ActivateTabBookPage()
		{
			this.tab_book.ActivePageIndex = this.bundles.Count - 1;
			this.tab_book.Invalidate ();
		}
		
		
		protected System.Collections.Hashtable	bundles;
		protected System.Collections.ArrayList	panels;
		protected Window						window;
		protected TabBook						tab_book;
		protected Support.CommandDispatcher		dispatcher;
	}
}
