namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe VRuler implémente la règle verticale.
	/// </summary>
	public class VRuler : AbstractRuler
	{
		public VRuler() : base(true)
		{
		}
		
		public VRuler(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public override double				DefaultWidth
		{
			get
			{
				return AbstractRuler.defaultBreadth;
			}
		}


		protected override void InvalidateBoxMarker()
		{
			if ( !this.markerVisible )  return;

			Drawing.Rectangle rect = this.Client.Bounds;
			double scale = (this.ending-this.starting)/rect.Height;
			double posy = (this.marker-this.starting)/scale;
			rect.Bottom = posy-4;
			rect.Top    = posy+4;

			if ( rect.IntersectsWith(this.Client.Bounds) )
			{
				this.invalidateBox.MergeWith(rect);
			}
		}


		protected void PaintGrad(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect = this.Client.Bounds;
			graphics.Align(ref rect);

			graphics.Color = adorner.ColorTextFieldBorder(this.IsEnabled);
			Drawing.Font font = Drawing.Font.GetFont("Tahoma", "Regular");

			double space = 3.0;
			double mul = 1.0;
			if ( this.ppm == 254.0 )
			{
				space = 1.0;
				mul = 2.54;
			}

			double scale = (this.ending-this.starting)/rect.Height;
			double step = System.Math.Pow(10.0, System.Math.Ceiling(System.Math.Log(scale*space, 10.0)))*mul;
			double grad = System.Math.Floor(this.starting/step)*step;

			Drawing.Transform ot = graphics.Transform;
			graphics.RotateTransformDeg(90.0, 0.0, 0.0);
			graphics.TranslateTransform(0, -rect.Width);

			graphics.SolidRenderer.Color = adorner.ColorText(this.PaintState);
			while ( grad < this.ending )
			{
				double posx = (grad-this.starting)/scale;
				int rank = (int) (System.Math.Floor(grad/step+0.5));

				if ( posx >= clipRect.Bottom-1.0 &&
					 posx <= clipRect.Top+1.0    )
				{
					double h = rect.Width;
					if ( rank%10 == 0 )  h *= 1.0;
					else if ( rank% 5 == 0 )  h *= 0.4;
					else                      h *= 0.2;
					graphics.AddLine(posx, 0, posx, h);
				}

				if ( rank%10 == 0 && posx <= clipRect.Top )
				{
					double value = grad/this.ppm;
					value *= 1000000.0;
					value = System.Math.Floor(value+0.5);  // arrondi à la 6ème décimale
					value /= 1000000.0;
					string text = value.ToString();

					double size = rect.Width*0.6;
					Drawing.Rectangle bounds = font.GetTextBounds(text);
					bounds.Scale(size);
					bounds.Offset(posx+2, 0);

					if ( bounds.Left < clipRect.Top && bounds.Right > clipRect.Bottom )
					{
						graphics.PaintText(posx+2, rect.Right-size, text, font, size);
					}
				}
				
				grad += step;
			}
			graphics.RenderSolid(adorner.ColorText(this.PaintState));
			graphics.Transform = ot;
		}

		protected void PaintMarker(Drawing.Graphics graphics)
		{
			if ( !this.markerVisible )  return;

			if ( this.marker < this.starting ||
				 this.marker > this.ending   )  return;

			IAdorner adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;
			graphics.Align(ref rect);

			double scale = (this.ending-this.starting)/rect.Height;
			double posy = (this.marker-this.starting)/scale;

			Drawing.Path path = new Drawing.Path();
			path.MoveTo(rect.Right-1, posy);
			path.LineTo(rect.Left, posy-4);
			path.LineTo(rect.Left, posy+4);
			path.Close();

			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(adorner.ColorCaption);

			graphics.Rasterizer.AddOutline(path);
			graphics.RenderSolid(adorner.ColorTextFieldBorder(this.IsEnabled));
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			
			Drawing.Rectangle rect = this.Client.Bounds;
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(adorner.ColorWindow);  // dessine le fond

			this.PaintGrad(graphics, clipRect);
			this.PaintMarker(graphics);

			rect.Deflate(0.5);
			graphics.AddRectangle(rect);  // dessine le cadre
			graphics.RenderSolid(adorner.ColorTextFieldBorder(this.IsEnabled));
		}
	}
}
