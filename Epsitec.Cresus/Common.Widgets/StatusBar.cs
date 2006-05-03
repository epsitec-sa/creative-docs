namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe StatusBar représente une barre de statuts en bas d'une fenêtre.
	/// </summary>
	public class StatusBar : Widget, Collections.IWidgetCollectionHost
	{
		public StatusBar()
		{
			this.items = new Collections.WidgetCollection(this);
			this.items.AutoEmbedding = true;

			this.Padding = new Drawing.Margins(1, 1, 2, 1);
		}
		
		public StatusBar(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				return this.DefaultFontHeight+6;
			}
		}

		public Collections.WidgetCollection		Items
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
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetState       state = this.PaintState;
			adorner.PaintStatusBackground(graphics, rect, state);
		}

		
		#region IWidgetCollectionHost Members
		Collections.WidgetCollection Collections.IWidgetCollectionHost.GetWidgetCollection()
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
		
		protected Collections.WidgetCollection	items;
	}
}
