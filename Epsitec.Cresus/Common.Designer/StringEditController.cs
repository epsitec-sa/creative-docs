using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// La classe StringEditController gère l'édition d'un bundle de ressources
	/// textuelles.
	/// </summary>
	public class StringEditController
	{
		public StringEditController(CommandDispatcher dispatcher)
		{
			this.bundles    = new System.Collections.ArrayList ();
			this.panels     = new System.Collections.ArrayList ();
			this.dispatcher = dispatcher;
			
			this.dispatcher.RegisterController (this);
		}
		
		
		public void AttachNewBundle(string id, ResourceLevel level)
		{
			ResourceBundle bundle = ResourceBundle.Create (id, level);
			
			this.AttachExistingBundle (bundle);
		}
		
		public void AttachExistingBundle(ResourceBundle bundle)
		{
			if (this.bundles.Contains (bundle))
			{
				throw new System.InvalidOperationException (string.Format ("Cannot attach bundle {0} twice.", bundle.Name));
			}
			
			this.bundles.Add (bundle);
			this.panels.Add (this.CreatePanel (bundle));
		}
		
		
		public ResourceBundle					ActiveBundle
		{
			get
			{
				int active = this.tab_book.ActivePageIndex;
				
				if (active > -1)
				{
					return this.bundles[active] as ResourceBundle;
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
		
		
		protected class Store : Support.Data.ITextArrayStore
		{
			public Store(StringEditController controller, ResourceBundle bundle)
			{
				this.bundle     = bundle;
				this.controller = controller;
				
				this.bundle.FieldsChanged += new Epsitec.Common.Support.EventHandler (this.HandleBundleFieldsChanged);
			}
			
			
			private void HandleBundleFieldsChanged(object sender)
			{
				if (this.changing == 0)
				{
					if (this.StoreChanged != null)
					{
						this.StoreChanged (this);
					}
				}
			}
			
			
			#region ITextArrayStore Members
			public void InsertRows(int row, int num)
			{
				// TODO:  Add TextBundleArrayStore.InsertRows implementation
				
				if (row == this.CountBundleFields)
				{
					ResourceBundle.Field field = this.bundle.CreateEmptyField (ResourceFieldType.Data);
					this.bundle.Add (field);
				}
			}
			
			public void RemoveRows(int row, int num)
			{
				// TODO:  Add TextBundleArrayStore.RemoveRows implementation
			}
			
			public string GetCellText(int row, int column)
			{
				if (row == this.CountBundleFields)
				{
					return "";
				}
				
				ResourceBundle.Field field = this.bundle[row];
				
				switch (column)
				{
					case 0:
						return TextLayout.ConvertToTaggedText (field.Name);
					case 1:
						return field.AsString;
				}
				
				return null;
			}
			
			public void SetCellText(int row, int column, string value)
			{
				if (row == this.CountBundleFields)
				{
					switch (column)
					{
						case 0:
							this.col_0_cache = value;
							break;
						case 1:
							this.col_1_cache = value;
							if ((this.col_0_cache != "") &&
								(this.col_1_cache != ""))
							{
								this.InsertRows (row, 1);
								this.changing++;
								this.SetCellText (row, 0, this.col_0_cache);
								this.SetCellText (row, 1, this.col_1_cache);
								this.changing--;
							}
							break;
					}
				}
				else
				{
					this.changing++;
					
					ResourceBundle.Field field = this.bundle[row];
					
					switch (column)
					{
						case 0:
							field.SetName (TextLayout.ConvertToSimpleText (value));
							break;
						case 1:
							field.SetStringValue (value);
							break;
					}
					
					this.changing--;
				}
			}
			
			public int GetColumnCount()
			{
				return 2;
			}
			
			public int GetRowCount()
			{
				return this.CountBundleFields + 1;
			}
			
			public event Support.EventHandler	StoreChanged;
			#endregion
			
			public int							CountBundleFields
			{
				get
				{
					if (this.bundle != null)
					{
						return this.bundle.CountFields;
					}
					
					return 0;
				}
			}
			
			
			protected ResourceBundle			bundle;
			protected StringEditController		controller;
			protected string					col_0_cache;
			protected string					col_1_cache;
			protected int						changing;
		}
		
		
		protected Panels.StringEditPanel CreatePanel(ResourceBundle bundle)
		{
			Panels.StringEditPanel panel  = new Panels.StringEditPanel (new Store (this, bundle));
			
			Widget  widget = panel.Widget;
			Window  window = this.Window;
			TabPage page   = new TabPage ();
			
			System.Diagnostics.Debug.Assert (this.window != null);
			System.Diagnostics.Debug.Assert (this.tab_book != null);
			
			window.Root.DockMargins = new Drawing.Margins (8, 8, 8, 8);
			
			widget.Dock   = DockStyle.Fill;
			widget.Parent = page;
			
			string name = bundle.Name;
			
			switch (bundle.ResourceLevel)
			{
				case ResourceLevel.Default:
					break;
				case ResourceLevel.Localised:
					name = string.Format ("{0} ({1})", name, bundle.Culture.TwoLetterISOLanguageName);
					break;
				case ResourceLevel.Customised:
					name = string.Format ("{0} ({1})", name, "X");
					break;
			}
			
			page.TabTitle = name;
			
			this.tab_book.Items.Add (page);
			
			return panel;
		}
		
		protected void CreateWindow()
		{
			Drawing.Size size = Panels.StringEditPanel.DefaultSize;
			
			this.window = new Window ();
			this.window.ClientSize = size + new Drawing.Size (20, 60);
			this.window.CommandDispatcher = this.dispatcher;
			
			this.tab_book = new TabBook ();
			this.tab_book.Dock = DockStyle.Fill;
			this.tab_book.Parent = this.window.Root;
		}
		
		
		[Command ("CreateStringBundle")] void CommandCreateStringBundle(CommandDispatcher d, CommandEventArgs e)
		{
			if (e.CommandArgs.Length == 0)
			{
				Dialogs.BundleName dialog = new Dialogs.BundleName ("CreateStringBundle (\"{0}\")", this.dispatcher);
				dialog.Show ();
			}
			else if (e.CommandArgs.Length == 1)
			{
				this.AttachNewBundle (e.CommandArgs[0], ResourceLevel.Default);
				
				this.tab_book.ActivePageIndex = this.bundles.Count - 1;
				this.tab_book.Invalidate ();
			}
			else
			{
				throw new System.InvalidOperationException (string.Format ("Command {0} requires one argument.", e.CommandName));
			}
		}
		
		[Command ("OpenStringBundle")]  void CommandOpenStringBundle(CommandDispatcher d, CommandEventArgs e)
		{
			if (e.CommandArgs.Length == 0)
			{
				Dialogs.OpenExistingBundle dialog = new Dialogs.OpenExistingBundle ("OpenStringBundle (\"{0}\", {1})", this.dispatcher);
				dialog.Show ();
			}
			else if (e.CommandArgs.Length == 2)
			{
				ResourceLevel  level  = (ResourceLevel) System.Enum.Parse (typeof (ResourceLevel), e.CommandArgs[1]);
				ResourceBundle bundle = Resources.GetBundle (e.CommandArgs[0], level);
				this.AttachExistingBundle (bundle);
				
				this.tab_book.ActivePageIndex = this.bundles.Count - 1;
				this.tab_book.Invalidate ();
			}
			else
			{
				throw new System.InvalidOperationException (string.Format ("Command {0} requires one argument.", e.CommandName));
			}
		}
		
		
		protected System.Collections.ArrayList	bundles;
		protected System.Collections.ArrayList	panels;
		protected int							active = -1;
		protected Window						window;
		protected TabBook						tab_book;
		protected Support.CommandDispatcher		dispatcher;
	}
}
