namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe HToolBar permet de réaliser des tool bars horizontales.
	/// </summary>
	public class HToolBar : Widget, Helpers.IWidgetCollectionHost
	{
		public HToolBar()
		{
			this.items = new Helpers.WidgetCollection(this);
			
			IconButton button = new IconButton();
			this.defaultButtonWidth = button.DefaultWidth;
			this.defaultButtonHeight = button.DefaultHeight;
			button.Dispose();

			double m = (this.DefaultHeight-this.defaultButtonHeight)/2;
			this.DockMargins = new Drawing.Margins(m, m, m, m);
		}
		
		public HToolBar(Widget embedder) : this()
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
				this.items.CopyTo(items, 0);
				this.items.Clear();
				
				foreach ( Widget item in items )
				{
					item.Dispose();
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

			Drawing.Rectangle rect  = this.Client.Bounds;
			adorner.PaintToolBackground(graphics, rect, WidgetState.None, Direction.Up);
		}


		#region Interface IBundleSupport
		public override void RestoreFromBundle(Epsitec.Common.Support.ObjectBundler bundler, Epsitec.Common.Support.ResourceBundle bundle)
		{
			base.RestoreFromBundle (bundler, bundle);
			
			Support.ResourceBundle.FieldList item_list = bundle["items"].AsList;
			
			if (item_list != null)
			{
				//	Notre bundle contient une liste de sous-bundles contenant les descriptions des
				//	items composant le menu.
				
				foreach (Support.ResourceBundle.Field field in item_list)
				{
					Support.ResourceBundle item_bundle = field.AsBundle;
					Widget                 item_widget = bundler.CreateFromBundle (item_bundle) as Widget;
					
					this.Items.Add (item_widget);
				}
			}
		}
		#endregion
		
		#region IWidgetCollectionHost Members
		Helpers.WidgetCollection Helpers.IWidgetCollectionHost.GetWidgetCollection()
		{
			return this.Items;
		}
		
		public void NotifyInsertion(Widget widget)
		{
			widget.Dock = DockStyle.Left;
			widget.SetEmbedder (this);
		}

		public void NotifyRemoval(Widget widget)
		{
			this.Children.Remove (widget);
		}
		
		public void NotifyPostRemoval(Widget widget)
		{
		}
		#endregion
		
		protected Helpers.WidgetCollection	items;
		
		protected double					defaultButtonWidth;
		protected double					defaultButtonHeight;
	}
}
