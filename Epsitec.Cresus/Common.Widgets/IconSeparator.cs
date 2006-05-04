namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe IconSeparator permet de dessiner des séparations utiles
	/// pour remplir une ToolBar.
	/// </summary>
	public class IconSeparator : Widget
	{
		public IconSeparator()
		{
		}
		
		public IconSeparator(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		public IconSeparator(double breadth)
		{
			this.Breadth = breadth;
		}
		
		
		public override double DefaultWidth
		{
			//	Retourne la largeur standard d'une séparation.
			get
			{
				//?return this.isHorizontal ? this.breadth : 22;
				return this.breadth;
			}
		}

		public override double DefaultHeight
		{
			//	Retourne la hauteur standard d'une séparation.
			get
			{
				//?return this.isHorizontal ? 22 : this.breadth;
				return this.breadth;
			}
		}

		
		public bool IsHorizontal
		{
			get
			{
				return this.isHorizontal;
			}

			set
			{
				if ( this.isHorizontal != value )
				{
					this.isHorizontal = value;
				}
			}
		}
		
		public double Breadth
		{
			get
			{
				return this.breadth;
			}

			set
			{
				if ( this.breadth != value )
				{
					this.breadth = value;
					this.UpdateGeometry();
				}
			}
		}
		
		
		protected void UpdateGeometry()
		{
			Drawing.Rectangle bounds = new Drawing.Rectangle(0, 0, this.DefaultWidth, this.DefaultHeight);
			this.SetManualBounds(this.MapClientToParent(bounds));
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetPaintState       state = this.PaintState;
			
			if ( this.isHorizontal )
			{
				adorner.PaintSeparatorBackground(graphics, rect, state, Direction.Right, true);
			}
			else
			{
				adorner.PaintSeparatorBackground(graphics, rect, state, Direction.Down, true);
			}
		}

		
		protected double				breadth = 12;
		protected bool					isHorizontal = true;
	}
}
