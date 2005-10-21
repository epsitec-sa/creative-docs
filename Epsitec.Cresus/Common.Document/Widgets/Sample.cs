using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe Sample est un widget affichant un échantillon d'une propriété.
	/// </summary>
	public class Sample : Widget
	{
		public Sample()
		{
		}

		public Sample(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		// Document associé.
		public Document Document
		{
			get
			{
				return this.document;
			}

			set
			{
				this.document = value;
			}
		}

		// Propriété représentée.
		public Properties.Abstract Property
		{
			get
			{
				return this.property;
			}

			set
			{
				this.property = value;
			}
		}

		// Affiche "..." au lieu de la croix si la propriété n'existe pas.
		public bool Dots
		{
			get
			{
				return this.dots;
			}

			set
			{
				this.dots = value;
			}
		}


		// Dessine l'échantillon.
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			if ( this.document == null )  return;

			IAdorner adorner = Common.Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;

			Color color = this.IsSelected ? adorner.ColorCaption : adorner.ColorTextBackground;
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(color);  // dessine le fond
			bool drawSelection = false;

			string text = "";
			if ( this.property != null )
			{
				text = this.property.SampleText;
			}

			if ( text != "" )
			{
				double size = rect.Height*0.5;
				double x = rect.Left+1.0;
				double y = rect.Bottom+rect.Height*0.3;
				graphics.Color = adorner.ColorText(this.State);
				graphics.PaintText(x, y, text, Font.DefaultFont, size);
			}
			else if ( this.property is Properties.Gradient )
			{
				this.PaintGradient(graphics, rect);
				drawSelection = true;
			}
			else if ( this.property is Properties.Corner )
			{
				this.PaintCorner(graphics, rect);
			}
			else if ( this.property is Properties.Arc )
			{
				this.PaintArc(graphics, rect);
			}
			else if ( this.property is Properties.Arrow )
			{
				this.PaintArrow(graphics, rect);
			}
			else if ( this.property is Properties.Font )
			{
				this.PaintFont(graphics, rect);
				drawSelection = true;
			}
			else if ( this.property == null )  // dessine une croix ?
			{
				if ( this.dots )
				{
					double size = rect.Height*0.8;
					double x = rect.Left+1.0;
					double y = rect.Bottom+rect.Height*0.3;
					graphics.Color = adorner.ColorText(this.State);
					graphics.PaintText(x, y, "...", Font.DefaultFont, size);
				}
				else
				{
					rect.Deflate(0.5);
					color = adorner.ColorBorder;
					color.A = 0.3;

					graphics.AddLine(rect.BottomLeft, rect.TopRight);
					graphics.RenderSolid(color);

					graphics.AddLine(rect.TopLeft, rect.BottomRight);
					graphics.RenderSolid(color);
				}
			}

			if ( this.IsSelected && drawSelection )
			{
				rect = this.Client.Bounds;

				rect.Deflate(0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(adorner.ColorCaption);

				rect.Deflate(1.0);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(adorner.ColorWindow);
			}
		}

		protected void PaintGradient(Graphics graphics, Rectangle rect)
		{
			double scale = 1.0/10.0;
			Transform initial = graphics.Transform;
			graphics.ScaleTransform(scale, scale, 0.0, 0.0);
			rect.Scale(1.0/scale);

			Objects.Rectangle obj = new Objects.Rectangle(null, null);
			obj.SurfaceAnchor.SetSurface(rect);

			Path path = new Path();
			path.AppendRectangle(rect);

			Shape shape = new Shape();
			shape.Path = path;
			shape.SetPropertySurface(graphics, this.property);

			Drawer drawer = new Drawer(null);
			drawer.DrawShapes(graphics, null, obj, shape);

			graphics.Transform = initial;

			obj.Dispose();
			path.Dispose();
		}

		protected void PaintCorner(Graphics graphics, Rectangle rect)
		{
			Properties.Corner corner = this.property as Properties.Corner;
			IAdorner adorner = Common.Widgets.Adorner.Factory.Active;

			Rectangle form = rect;
			form.Deflate(3.5);
			if ( form.Width > form.Height )
			{
				form.Width = form.Height;
			}
			else
			{
				form.Height = form.Width;
			}

			Path path = new Path();
			Point p0 = new Point(rect.Left, form.Top);
			Point p1 = form.TopLeft;
			Point c  = form.TopRight;
			Point p2 = form.BottomRight;
			Point p3 = new Point(form.Right, rect.Bottom);
			double radius = System.Math.Min(form.Width, form.Height);

			path.MoveTo(p0);
			path.LineTo(p1);

			if ( corner.CornerType == Properties.CornerType.Right )
			{
				path.LineTo(new Point(p2.X, p1.Y));
			}
			else
			{
				corner.PathCorner(path, p1, c, p2, radius);
			}

			path.LineTo(p3);

			graphics.Color = adorner.ColorText(this.State);
			graphics.PaintOutline(path);
			graphics.RenderSolid();
		}

		protected void PaintArc(Graphics graphics, Rectangle rect)
		{
			Properties.Arc arc = this.property as Properties.Arc;
			IAdorner adorner = Common.Widgets.Adorner.Factory.Active;

			Rectangle form = rect;
			form.Deflate(1.5);

			Path path = arc.PathEllipse(form);

			graphics.Color = adorner.ColorText(this.State);
			graphics.PaintOutline(path);
			graphics.RenderSolid();
		}

		protected void PaintArrow(Graphics graphics, Rectangle rect)
		{
			Properties.Arrow arrow = this.property as Properties.Arrow;
			IAdorner adorner = Common.Widgets.Adorner.Factory.Active;

			double scale = 1.0/10.0;
			Transform initialTransform = graphics.Transform;
			double initialWidth = graphics.LineWidth;
			graphics.ScaleTransform(scale, scale, 0.0, 0.0);
			rect.Scale(1.0/scale);

			Rectangle form = rect;
			form.Deflate(1.5);

			Path pathStart = new Path();
			Path pathEnd   = new Path();
			Path pathLine  = new Path();
			bool outlineStart, surfaceStart, outlineEnd, surfaceEnd;

			Point p1 = new Point(form.Left,  form.Center.Y);
			Point p2 = new Point(form.Right, form.Center.Y);
			double w = 1.0/scale;
			Point pp1 = arrow.PathExtremity(pathStart, 0, w, CapStyle.Square, p1,p2, false, out outlineStart, out surfaceStart);
			Point pp2 = arrow.PathExtremity(pathEnd,   1, w, CapStyle.Square, p2,p1, false, out outlineEnd,   out surfaceEnd);

			pathLine.MoveTo(pp1);
			pathLine.LineTo(pp2);

			graphics.Color = adorner.ColorText(this.State);
			graphics.LineWidth = w;
			if ( outlineStart )  graphics.PaintOutline(pathStart);
			if ( outlineEnd   )  graphics.PaintOutline(pathEnd);
			if ( surfaceStart )  graphics.PaintSurface(pathStart);
			if ( surfaceEnd   )  graphics.PaintSurface(pathEnd);
			graphics.PaintOutline(pathLine);
			graphics.RenderSolid();

			graphics.LineWidth = initialWidth;
			graphics.Transform = initialTransform;
		}

		protected void PaintFont(Graphics graphics, Rectangle rect)
		{
			Properties.Font font = this.property as Properties.Font;

			double size = rect.Height*0.5;
			double x = rect.Left+1.0;
			double y = rect.Bottom+rect.Height*0.3;

			Color textColor = font.FontColor.Basic;
			Color backColor = Color.FromBrightness(1.0);  // blanc
			double intensity = textColor.R + textColor.G + textColor.B;
			if ( intensity > 2.0 )  // couleur claire ?
			{
				backColor = Color.FromBrightness(0.0);  // noir
			}

			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(backColor);

			string text = font.FontName;
			graphics.Color = textColor;
			graphics.PaintText(x, y, text, Font.DefaultFont, size);
		}


		protected Document						document;
		protected Properties.Abstract			property;
		protected bool							dots = false;
	}
}
