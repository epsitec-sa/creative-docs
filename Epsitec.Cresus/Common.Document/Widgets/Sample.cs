using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe Sample est un widget affichant un échantillon d'une propriété graphique.
	/// </summary>
	public class Sample : AbstractSample
	{
		public Sample() : base()
		{
		}

		public Sample(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public Properties.Aggregate Aggregate
		{
			//	Style représenté.
			get
			{
				return this.aggregate;
			}

			set
			{
				this.aggregate = value;
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine l'échantillon.
			if ( this.document == null )  return;

			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;
			Drawing.Rectangle box;

			if ( this.aggregate == null || this.aggregate.Styles.Count == 0 )  // dessine une croix ?
			{
				rect.Deflate(0.5);
				Color color = Color.FromAlphaColor(0.3, adorner.ColorBorder);

				graphics.AddLine(rect.BottomLeft, rect.TopRight);
				graphics.RenderSolid(color);

				graphics.AddLine(rect.TopLeft, rect.BottomRight);
				graphics.RenderSolid(color);
			}
			else
			{
				Properties.Gradient fillColor      = this.aggregate.Property(Properties.Type.FillGradient,   this.isDeep) as Properties.Gradient;
				Properties.Gradient fillColorVT    = this.aggregate.Property(Properties.Type.FillGradientVT, this.isDeep) as Properties.Gradient;
				Properties.Gradient fillColorVL    = this.aggregate.Property(Properties.Type.FillGradientVL, this.isDeep) as Properties.Gradient;
				Properties.Gradient fillColorVR    = this.aggregate.Property(Properties.Type.FillGradientVR, this.isDeep) as Properties.Gradient;
				Properties.Gradient lineColor      = this.aggregate.Property(Properties.Type.LineColor,      this.isDeep) as Properties.Gradient;
				Properties.Line     lineMode       = this.aggregate.Property(Properties.Type.LineMode,       this.isDeep) as Properties.Line;
				Properties.Line     lineDimension  = this.aggregate.Property(Properties.Type.LineDimension,  this.isDeep) as Properties.Line;
				Properties.Corner   corner         = this.aggregate.Property(Properties.Type.Corner,         this.isDeep) as Properties.Corner;
				Properties.Arc      arc            = this.aggregate.Property(Properties.Type.Arc,            this.isDeep) as Properties.Arc;
				Properties.Arrow    arrow          = this.aggregate.Property(Properties.Type.Arrow,          this.isDeep) as Properties.Arrow;
				Properties.Arrow    arrowDimension = this.aggregate.Property(Properties.Type.DimensionArrow, this.isDeep) as Properties.Arrow;
				Properties.Font     font           = this.aggregate.Property(Properties.Type.TextFont,       this.isDeep) as Properties.Font;

				int total = 0;
				if ( fillColor      != null || lineColor != null || lineMode != null )  total ++;
				if ( fillColorVT    != null )  total ++;
				if ( fillColorVL    != null )  total ++;
				if ( fillColorVR    != null )  total ++;
				if ( lineDimension  != null )  total ++;
				if ( corner         != null )  total ++;
				if ( arc            != null )  total ++;
				if ( arrow          != null )  total ++;
				if ( arrowDimension != null )  total ++;
				if ( font           != null )  total ++;

				System.Collections.ArrayList texts = new System.Collections.ArrayList();
				UndoableList styles = this.aggregate.Styles;
				foreach ( Properties.Abstract property in styles )
				{
					string text = property.SampleText;
					if ( text != null && text != "" )
					{
						texts.Add(text);
					}
				}
				total += (texts.Count+3)/4;  // total: nombre de places nécessaire

				int places = System.Math.Max((int)(rect.Width/rect.Height), 1);  // places disponibles

				double dim = rect.Height;  // dimension d'une place (une place est carrée)
				bool small = false;
				if ( total > places )
				{
					dim /= 2.0;  // il y aura 4 fois plus de places disponibles
					small = true;
				}

				int rank = 0;

				if ( fillColor != null || lineColor != null || lineMode != null )
				{
					box = this.GetPlace(rect, dim, rank++);
					this.PaintSurface(graphics, box, fillColor, lineColor, lineMode);
				}

				if ( fillColorVT != null )
				{
					box = this.GetPlace(rect, dim, rank++);
					this.PaintSurface(graphics, box, fillColorVT, null, null);
				}

				if ( fillColorVL != null )
				{
					box = this.GetPlace(rect, dim, rank++);
					this.PaintSurface(graphics, box, fillColorVL, null, null);
				}

				if ( fillColorVR != null )
				{
					box = this.GetPlace(rect, dim, rank++);
					this.PaintSurface(graphics, box, fillColorVR, null, null);
				}

				if ( lineDimension != null )
				{
					box = this.GetPlace(rect, dim, rank++);
					this.PaintSurface(graphics, box, null, lineColor, lineDimension);
				}

				if ( corner != null )
				{
					box = this.GetPlace(rect, dim, rank++);
					this.PaintCorner(graphics, box, corner);
				}

				if ( arc != null )
				{
					box = this.GetPlace(rect, dim, rank++);
					this.PaintArc(graphics, box, arc);
				}

				if ( arrow != null )
				{
					box = this.GetPlace(rect, dim, rank++);
					this.PaintArrow(graphics, box, arrow);
				}

				if ( arrowDimension != null )
				{
					box = this.GetPlace(rect, dim, rank++);
					this.PaintArrow(graphics, box, arrowDimension);
				}

				if ( font != null )
				{
					box = this.GetPlace(rect, dim, rank++);
					this.PaintFont(graphics, box, font);
				}

				if ( small )
				{
					dim *= 2.0;
					rank = (rank+3)/4;
				}

				rank *= 4;  // 4 sous-places par place
				foreach ( string text in texts )
				{
					box = this.GetSubPlace(rect, dim, rank++);
					if ( box.IsEmpty )  break;

					Drawing.Rectangle iClip = graphics.SaveClippingRectangle();
					graphics.SetClippingRectangle(this.MapClientToRoot(box));

					double size = box.Height*0.9;
					double x = box.Left+1.0;
					double y = box.Bottom+box.Height*0.1;
					graphics.Color = adorner.ColorText(this.PaintState);
					graphics.PaintText(x, y, text, Font.DefaultFont, size);

					graphics.RestoreClippingRectangle(iClip);
				}
			}
		}

		protected Drawing.Rectangle GetSubPlace(Drawing.Rectangle rect, double dim, int rank)
		{
			Drawing.Rectangle box = this.GetPlace(rect, dim, rank/4);
			if ( box.IsEmpty )  return box;

			rank %= 4;  // sous-rang 0..3
			double h = dim/4;
			box.Top -= rank*h;
			box.Bottom = box.Top-h;
			return box;
		}

		protected Drawing.Rectangle GetPlace(Drawing.Rectangle rect, double dim, int rank)
		{
			//	Retourne le rectangle d'une place.
			int dx = (int) (rect.Width/dim);
			int dy = (int) (rect.Height/dim);

			if ( rank >= dx*dy )  return Drawing.Rectangle.Empty;

			int x = rank/dy;
			int y = rank%dy;

			return new Drawing.Rectangle(rect.Left+x*dim, rect.Top-(y+1)*dim, dim, dim);
		}

		protected void PaintSurface(Graphics graphics, Rectangle rect, Properties.Gradient fillColor, Properties.Gradient lineColor, Properties.Line lineMode)
		{
			if ( rect.IsEmpty )  return;

			Drawing.Rectangle iClip = graphics.SaveClippingRectangle();
			graphics.SetClippingRectangle(this.MapClientToRoot(rect));

			rect.Deflate(rect.Height*0.15);

			if ( fillColor != null )  // dessine une croix "x" dans le fond ?
			{
				rect.Deflate(1);
				graphics.LineWidth = 1;
				graphics.AddLine(rect.BottomLeft, rect.TopRight);
				graphics.AddLine(rect.BottomRight, rect.TopLeft);
				graphics.RenderSolid(Color.FromBrightness(0));
				rect.Inflate(1);
			}

			double scale = 1.0/10.0;
			Transform initial = graphics.Transform;
			graphics.ScaleTransform(scale, scale, 0.0, 0.0);
			rect.Scale(1.0/scale);

			Objects.Rectangle obj = new Objects.Rectangle(null, null);
			obj.SurfaceAnchor.SetSurface(rect);

			Path path = new Path();
			if ( fillColor == null )
			{
				path.MoveTo(rect.BottomLeft);
				path.LineTo(rect.TopRight);
			}
			else
			{
				path.AppendRectangle(rect);
			}

			Drawer drawer = new Drawer(null);
			
			if ( fillColor != null )
			{
				Shape shape = new Shape();
				shape.Path = path;
				shape.SetPropertySurface(graphics, fillColor);
				drawer.DrawShapes(graphics, null, obj, Drawer.DrawShapesMode.All, shape);
			}

			if (lineColor != null)
			{
				Shape shape = new Shape();
				shape.Path = path;
				if (lineMode == null)
				{
					lineMode = new Properties.Line(this.document, Properties.Type.LineMode);
				}
				shape.SetPropertyStroke(graphics, lineMode, lineColor);
				drawer.DrawShapes(graphics, null, obj, Drawer.DrawShapesMode.All, shape);
			}

			graphics.Transform = initial;

			obj.Dispose();
			path.Dispose();

			graphics.RestoreClippingRectangle(iClip);
		}

		protected void PaintCorner(Graphics graphics, Rectangle rect, Properties.Corner corner)
		{
			if ( rect.IsEmpty )  return;

			rect.Deflate(rect.Height*0.15);
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

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

			graphics.Color = adorner.ColorText(this.PaintState);
			graphics.PaintOutline(path);
			graphics.RenderSolid();
		}

		protected void PaintArc(Graphics graphics, Rectangle rect, Properties.Arc arc)
		{
			if ( rect.IsEmpty )  return;

			rect.Deflate(rect.Height*0.15);
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			Rectangle form = rect;
			form.Deflate(1.5);

			Path path = arc.PathEllipse(form);

			graphics.Color = adorner.ColorText(this.PaintState);
			graphics.PaintOutline(path);
			graphics.RenderSolid();
		}

		protected void PaintArrow(Graphics graphics, Rectangle rect, Properties.Arrow arrow)
		{
			if ( rect.IsEmpty )  return;

			Drawing.Rectangle iClip = graphics.SaveClippingRectangle();
			graphics.SetClippingRectangle(this.MapClientToRoot(rect));

			rect.Deflate(rect.Height*0.15);
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

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

			Point p1 = form.BottomLeft;
			Point p2 = form.TopRight;
			double w = 1.0/scale;
			Point pp1 = arrow.PathExtremity(pathStart, 0, w, CapStyle.Square, p1,p2, false, out outlineStart, out surfaceStart);
			Point pp2 = arrow.PathExtremity(pathEnd,   1, w, CapStyle.Square, p2,p1, false, out outlineEnd,   out surfaceEnd);

			pathLine.MoveTo(pp1);
			pathLine.LineTo(pp2);

			graphics.Color = adorner.ColorText(this.PaintState);
			graphics.LineWidth = w;
			if ( outlineStart )  graphics.PaintOutline(pathStart);
			if ( outlineEnd   )  graphics.PaintOutline(pathEnd);
			if ( surfaceStart )  graphics.PaintSurface(pathStart);
			if ( surfaceEnd   )  graphics.PaintSurface(pathEnd);
			graphics.PaintOutline(pathLine);
			graphics.RenderSolid();

			graphics.LineWidth = initialWidth;
			graphics.Transform = initialTransform;

			graphics.RestoreClippingRectangle(iClip);
		}

		protected void PaintFont(Graphics graphics, Rectangle rect, Properties.Font font)
		{
			if ( rect.IsEmpty )  return;

			Drawing.Rectangle iClip = graphics.SaveClippingRectangle();
			graphics.SetClippingRectangle(this.MapClientToRoot(rect));

			OpenType.FontIdentity id = Misc.DefaultFontIdentityStyle(font.FontName);
			double size = rect.Height*0.6;
			double ox = rect.Left;
			double oy = rect.Bottom+rect.Height*0.3;
			Path path = Common.Widgets.Helpers.FontPreviewer.GetPathAbc(id, ox, oy, size);

			graphics.Color = font.FontColor.Basic;
			graphics.PaintSurface(path);

			graphics.RestoreClippingRectangle(iClip);
		}


		protected Properties.Aggregate			aggregate;
	}
}
