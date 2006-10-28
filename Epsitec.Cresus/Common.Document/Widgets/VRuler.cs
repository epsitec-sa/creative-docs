using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Document.Widgets
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

		static VRuler()
		{
			Types.DependencyPropertyMetadata metadataDx = Visual.PreferredWidthProperty.DefaultMetadata.Clone ();

			metadataDx.DefineDefaultValue (AbstractRuler.defaultBreadth);
			
			Common.Widgets.Visual.PreferredWidthProperty.OverrideMetadata(typeof(VRuler), metadataDx);
		}


		protected override void InvalidateBoxMarker()
		{
			if ( !this.markerVisible )  return;

			Rectangle rect = this.Client.Bounds;
			double posy = this.DocumentToScreen(this.marker);
			rect.Bottom = posy-4;
			rect.Top    = posy+4;

			if ( rect.IntersectsWith(this.Client.Bounds) )
			{
				this.invalidateBox.MergeWith(rect);
			}
		}


		protected void PaintGrad(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			Rectangle rect = this.Client.Bounds;
			graphics.Align(ref rect);

			graphics.Color = adorner.ColorTextFieldBorder(this.IsEnabled);
			Font font = Font.GetFont("Tahoma", "Regular");

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

			Transform ot = graphics.Transform;
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
					Rectangle bounds = font.GetTextBounds(text);
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

		protected void PaintMarker(Graphics graphics)
		{
			if ( !this.markerVisible )  return;

			if ( this.marker < this.starting ||
				 this.marker > this.ending   )  return;

			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Rectangle rect = this.Client.Bounds;
			graphics.Align(ref rect);

			double posy = this.DocumentToScreen(this.marker);

			Path path = new Path();
			path.MoveTo(rect.Right-1, posy);
			path.LineTo(rect.Left, posy-4);
			path.LineTo(rect.Left, posy+4);
			path.Close();

			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(adorner.ColorCaption);

			graphics.Rasterizer.AddOutline(path);
			graphics.RenderSolid(adorner.ColorTextFieldBorder(this.IsEnabled));
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			
			Rectangle rect = this.Client.Bounds;
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.ColorBackground);  // dessine le fond

			if ( this.edited )
			{
				Rectangle zone = rect;
				zone.Bottom = this.DocumentToScreen(this.limitLow);
				zone.Top    = this.DocumentToScreen(this.limitHigh);
				graphics.AddFilledRectangle(zone);
				graphics.RenderSolid(this.ColorBackgroundEdited);
			}

			this.PaintGrad(graphics, clipRect);
			this.PaintMarker(graphics);

			rect.Deflate(0.5);
			graphics.AddRectangle(rect);  // dessine le cadre
			graphics.RenderSolid(adorner.ColorTextFieldBorder(this.IsEnabled));
		}


		protected double DocumentToScreen(double value)
		{
			//	Conversion d'une position dans le document en position en pixel dans l'écran.
			double scale = (this.ending-this.starting)/this.Client.Bounds.Height;
			return (value-this.starting)/scale;
		}

		protected double ScreenToDocument(double value)
		{
			//	Conversion d'une position en pixel dans l'écran en position dans le document.
			double scale = (this.ending-this.starting)/this.Client.Bounds.Height;
			return value*scale + this.starting;
		}
	}
}
