namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe AbstractToolBar implémente ce qui est commun à HToolBar et à VToolBar.
	/// </summary>
	public abstract class AbstractToolBar : Widget, Helpers.IWidgetCollectionHost
	{
		public AbstractToolBar()
		{
			this.items = new Helpers.WidgetCollection (this);
			
			using (IconButton button = new IconButton ())
			{
				this.defaultButtonWidth  = button.DefaultWidth;
				this.defaultButtonHeight = button.DefaultHeight;
			}
		}
		
		
		public Helpers.WidgetCollection		Items
		{
			get { return this.items; }
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Widget[] items = new Widget[this.items.Count];
				
				this.items.CopyTo (items, 0);
				this.items.Clear ();
				
				for (int i = 0; i < items.Length; i++)
				{
					items[i].Dispose();
				}
				
				this.items.Dispose ();
				this.items = null;
			}
			
			base.Dispose (disposing);
		}
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			// Dessine la barre.
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			
			Drawing.Rectangle rect  = this.Client.Bounds;
			adorner.PaintToolBackground(graphics, rect, WidgetState.None, this.direction);
		}
		
		
		#region Interface IBundleSupport
		public override void RestoreFromBundle(Epsitec.Common.Support.ObjectBundler bundler, Epsitec.Common.Support.ResourceBundle bundle)
		{
			base.RestoreFromBundle (bundler, bundle);
			this.items.RestoreFromBundle ("items", bundler, bundle);
		}
		
		public override void SerializeToBundle(Support.ObjectBundler bundler, Support.ResourceBundle bundle)
		{
			base.SerializeToBundle (bundler, bundle);
			this.items.SerializeToBundle ("items", bundler, bundle);
		}
		#endregion
		
		#region IWidgetCollectionHost Members
		Helpers.WidgetCollection Helpers.IWidgetCollectionHost.GetWidgetCollection()
		{
			return this.Items;
		}
		
		public void NotifyInsertion(Widget widget)
		{
			widget.Dock = this.iconDockStyle;
			widget.AutoFocus = false;
			widget.SetEmbedder (this);
			this.OnItemsChanged ();
		}

		public void NotifyRemoval(Widget widget)
		{
			this.Children.Remove (widget);
		}
		
		public void NotifyPostRemoval(Widget widget)
		{
			this.OnItemsChanged ();
		}
		#endregion
		
		protected virtual void OnItemsChanged()
		{
			if (this.ItemsChanged != null)
			{
				this.ItemsChanged (this);
			}
		}
		
		
		public event Support.EventHandler	ItemsChanged;
		
		protected DockStyle					iconDockStyle;
		protected Direction					direction;
		protected Helpers.WidgetCollection	items;
		
		protected double					defaultButtonWidth;
		protected double					defaultButtonHeight;
	}
}
