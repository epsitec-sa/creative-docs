namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ToolBar permet de réaliser des tool bars.
	/// </summary>
	public class ToolBar : Widget, Helpers.IWidgetCollectionHost
	{
		public ToolBar()
		{
			this.items = new Helpers.WidgetCollection(this);
			
			IconButton button = new IconButton();
			this.defaultButtonWidth = button.DefaultWidth;
			this.defaultButtonHeight = button.DefaultHeight;
			button.Dispose();

			double m = (this.DefaultHeight - this.defaultButtonHeight) / 2;
			
			this.DockMargins = new Drawing.Margins(m, m, m, m);
			
			this.colorControlLight      = Drawing.Color.FromName("ControlLight");
			this.colorControlLightLight = Drawing.Color.FromName("ControlLightLight");
			this.colorControlDark       = Drawing.Color.FromName("ControlDark");
			this.colorControlDarkDark   = Drawing.Color.FromName("ControlDarkDark");
		}
		
		public ToolBar(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public override double				DefaultHeight
		{
			// Retourne la hauteur standard d'une barre.
			get
			{
				return 28;
			}
		}

		public Helpers.WidgetCollection		Items
		{
			get { return this.items; }
		}

		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				Widget[] items = new Widget[this.items.Count];
				this.items.CopyTo (items, 0);
				this.items.Clear ();
				
				foreach (Widget item in items)
				{
					item.Dispose ();
				}
				
				this.items.Dispose();
				this.items = null;
			}
			
			base.Dispose(disposing);
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			// Dessine la barre.
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			adorner.PaintToolBackground(graphics, rect, WidgetState.None, Direction.None, Direction.None);
		}


		#region Interface IBundleSupport
		public override void RestoreFromBundle(Epsitec.Common.Support.ObjectBundler bundler, Epsitec.Common.Support.ResourceBundle bundle)
		{
			base.RestoreFromBundle (bundler, bundle);
			
			System.Collections.IList item_list = bundle.GetFieldBundleList ("items");
			
			if (item_list != null)
			{
				//	Notre bundle contient une liste de sous-bundles contenant les descriptions des
				//	items composant le menu.
				
				foreach (Support.ResourceBundle item_bundle in item_list)
				{
					Widget item = bundler.CreateFromBundle (item_bundle) as Widget;
					
					this.Items.Add (item);
				}
			}
		}
		#endregion
		
		#region IWidgetCollectionHost Members
		public void NotifyInsertion(Widget widget)
		{
			widget.Dock = DockStyle.Left;
			widget.SetEmbedder (this);
		}

		public void NotifyRemoval(Widget widget)
		{
			this.Children.Remove (widget);
		}
		#endregion
		
		protected Helpers.WidgetCollection	items;
		
		protected double					defaultButtonWidth;
		protected double					defaultButtonHeight;
		
		protected Drawing.Color				colorControlLight;
		protected Drawing.Color				colorControlLightLight;
		protected Drawing.Color				colorControlDark;
		protected Drawing.Color				colorControlDarkDark;
	}
}
