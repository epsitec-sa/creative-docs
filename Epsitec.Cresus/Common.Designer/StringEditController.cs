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
			}
			
			
			#region ITextArrayStore Members
			public void InsertRows(int row, int num)
			{
				// TODO:  Add TextBundleArrayStore.InsertRows implementation
			}
			
			public void RemoveRows(int row, int num)
			{
				// TODO:  Add TextBundleArrayStore.RemoveRows implementation
			}
			
			public string GetCellText(int row, int column)
			{
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
			}
			
			public int GetColumnCount()
			{
				return 2;
			}
			
			public int GetRowCount()
			{
				return this.CountBundleFields + 1;
			}
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
		}
		
		
		protected Panels.StringEditPanel CreatePanel(ResourceBundle bundle)
		{
			Panels.StringEditPanel panel  = new Panels.StringEditPanel (new Store (this, bundle));
			
			Widget  widget = panel.Widget;
			Window  window = this.Window;
			TabPage page   = new TabPage ();
			
			System.Diagnostics.Debug.Assert (this.window != null);
			System.Diagnostics.Debug.Assert (this.tab_book != null);
			
			widget.Dock   = DockStyle.Fill;
			widget.Parent = page;
			
			page.TabTitle = bundle.Name;
			
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
			if (e.CommandArgs.Length != 1)
			{
				throw new System.InvalidOperationException (string.Format ("Command {0} requires one argument.", e.CommandName));
			}
			
			this.AttachNewBundle (e.CommandArgs[0], ResourceLevel.Default);
			
			this.tab_book.ActivePageIndex = this.bundles.Count - 1;
			this.tab_book.Invalidate ();
		}
		
		
		protected System.Collections.ArrayList	bundles;
		protected System.Collections.ArrayList	panels;
		protected int							active = -1;
		protected Window						window;
		protected TabBook						tab_book;
		protected Support.CommandDispatcher		dispatcher;
	}
}
