namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe AbstractToolBar impl�mente ce qui est commun � HToolBar et � VToolBar.
	/// </summary>
	public abstract class AbstractToolBar : Widget, Collections.IWidgetCollectionHost
	{
		public AbstractToolBar()
		{
			this.iconDockStyle = this.DefaultIconDockStyle;
			
			this.items = new Collections.WidgetCollection (this);
			this.items.AutoEmbedding = true;
			
			using (IconButton button = new IconButton ())
			{
				this.defaultButtonWidth  = button.DefaultWidth;
				this.defaultButtonHeight = button.DefaultHeight;
			}
		}
		
		
		public Collections.WidgetCollection		Items
		{
			get { return this.items; }
		}
		
		public abstract DockStyle			DefaultIconDockStyle
		{
			get;
		}
		
		public DockStyle					OppositeIconDockStyle
		{
			get
			{
				return AbstractToolBar.GetOpposite (this.DefaultIconDockStyle);
			}
		}
		
		
		public static DockStyle GetOpposite(DockStyle style)
		{
			switch (style)
			{
				case DockStyle.Left:	style = DockStyle.Right;	break;
				case DockStyle.Right:	style = DockStyle.Left;		break;
				case DockStyle.Top:		style = DockStyle.Bottom;	break;
				case DockStyle.Bottom:	style = DockStyle.Top;		break;
			}
			
			return style;
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
			//	Dessine la barre.
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			
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
		Collections.WidgetCollection Collections.IWidgetCollectionHost.GetWidgetCollection()
		{
			return this.Items;
		}
		
		public void NotifyInsertion(Widget widget)
		{
			if (widget.Dock == DockStyle.None)
			{
				widget.Dock = this.iconDockStyle;
			}
			
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
		
		private DockStyle					iconDockStyle;
		protected Direction					direction;
		protected Collections.WidgetCollection	items;
		
		protected double					defaultButtonWidth;
		protected double					defaultButtonHeight;
	}
}
