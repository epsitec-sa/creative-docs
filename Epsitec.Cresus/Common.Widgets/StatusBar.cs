namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe StatusBar représente une barre de statuts en bas d'une fenêtre.
	/// </summary>
	public class StatusBar : Widget, Helpers.IWidgetCollectionHost
	{
		public StatusBar()
		{
			this.items = new Helpers.WidgetCollection(this);

			this.DockPadding = new Drawing.Margins(1, 1, 2, 1);
		}
		
		public StatusBar(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		public override double DefaultHeight
		{
			// Retourne la hauteur standard.
			get
			{
				return this.DefaultFontHeight+6;
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
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetState       state = this.PaintState;
			adorner.PaintStatusBackground(graphics, rect, state);
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
			widget.Dock = DockStyle.Left;
			widget.AutoFocus = false;
			widget.SetEmbedder(this);
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
	}
}
