namespace Epsitec.Common.Widgets
{
	using BundleAttribute = Epsitec.Common.Support.BundleAttribute;
	
	/// <summary>
	/// La classe ColorSample permet de représenter une couleur rgb.
	/// </summary>
	public class ColorSample : AbstractButton
	{
		public ColorSample()
		{
			this.colorBlack   = Drawing.Color.FromName("WindowFrame");
			this.colorWhite   = Drawing.Color.FromName("Window");
		}
		
		public ColorSample(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		// Couleur.
		public Drawing.Color Color
		{
			get
			{
				return this.color;
			}

			set
			{
				if ( this.color != value )
				{
					this.color = value;
					this.Invalidate();
				}
			}
		}


		// Possibilité d'utiliser ce widget comme origine des couleurs.
		public bool PossibleOrigin
		{
			get
			{
				return this.possibleOrigin;
			}

			set
			{
				this.possibleOrigin = value;
			}
		}


		public override Drawing.Rectangle GetShapeBounds()
		{
			if ( this.possibleOrigin )
			{
				return new Drawing.Rectangle(-5, -5, this.Client.Width+10, this.Client.Height+10);
			}
			return new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
		}


		// Dessine la couleur.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);

			if ( this.possibleOrigin && this.ActiveState == WidgetState.ActiveYes )
			{
				Drawing.Rectangle r = rect;
				r.Inflate(5, 5);
				r.Bottom ++;
				graphics.AddFilledRectangle(r);
				graphics.RenderSolid(adorner.ColorCaption);
			}

			if ( this.IsEnabled )
			{
				graphics.AddLine(rect.Left+0.5, rect.Bottom+0.5, rect.Right-0.5, rect.Top-0.5);
				graphics.AddLine(rect.Left+0.5, rect.Top-0.5, rect.Right-0.5, rect.Bottom+0.5);
				graphics.RenderSolid(this.colorBlack);

				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.color);

				rect.Inflate(-0.5, -0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.colorBlack);
			}
			else
			{
				rect.Inflate(-0.5, -0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(adorner.ColorBorder);
			}
		}



		protected Drawing.Color				colorBlack;
		protected Drawing.Color				colorWhite;
		protected Drawing.Color				color;
		protected bool						possibleOrigin = false;
	}
}
