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
			return this.Client.Bounds;
		}


		// Dessine la couleur.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;

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
				graphics.RenderSolid(adorner.ColorBorder);

				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.color);

				rect.Deflate(0.5, 0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(adorner.ColorBorder);

				if ( (this.PaintState&WidgetState.Focused) != 0 )
				{
					Drawing.Color cFocus = this.Color;
					double h,s,v;
					cFocus.GetHSV(out h, out s, out v);
					if ( s < 0.2 )  // gris ou presque ?
					{
						cFocus = adorner.ColorCaption;
					}
					else
					{
						cFocus.R = 1.0-cFocus.R;
						cFocus.G = 1.0-cFocus.G;
						cFocus.B = 1.0-cFocus.B;  // couleur opposée
					}
					cFocus.A = 1.0;
					rect.Deflate(1, 1);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(cFocus);
				}
			}
			else
			{
				rect.Inflate(-0.5, -0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(adorner.ColorBorder);
			}
		}



		protected Drawing.Color				color;
		protected bool						possibleOrigin = false;
	}
}
