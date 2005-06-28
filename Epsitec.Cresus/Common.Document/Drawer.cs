using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe Drawer contient le dessinateur universel.
	/// </summary>
	public class Drawer
	{
		public Drawer(Document document)
		{
			this.document = document;
		}

		// Dessine des formes dans n'importe quel IPaintPort.
		public void DrawShapes(IPaintPort port,
							   DrawingContext drawingContext,
							   Objects.Abstract obj,
							   params Shape[] shapes)
		{
			foreach ( Shape shape in shapes )
			{
				if ( shape == null || shape.Aspect == Aspect.InvisibleBox )  continue;

				if ( shape.Type == Type.Surface )
				{
					if ( !shape.IsVisible || shape.Path.IsEmpty )  continue;

					if ( port is Graphics )
					{
						this.DrawSurface(port as Graphics, drawingContext, shape, obj);
					}
					else if ( port is Printing.PrintPort )
					{
						this.DrawSurface(port as Printing.PrintPort, drawingContext, shape, obj);
					}
					else if ( port is PDF.Port )
					{
						this.DrawSurface(port as PDF.Port, drawingContext, shape, obj);
					}
				}

				if ( shape.Type == Type.Stroke )
				{
					if ( !shape.IsVisible || shape.Path.IsEmpty )  continue;

					if ( port is Graphics )
					{
						this.DrawStroke(port as Graphics, drawingContext, shape, obj);
					}
					else if ( port is Printing.PrintPort )
					{
						this.DrawStroke(port as Printing.PrintPort, drawingContext, shape, obj);
					}
					else if ( port is PDF.Port )
					{
						this.DrawStroke(port as PDF.Port, drawingContext, shape, obj);
					}
				}

				if ( shape.Type == Type.Text )
				{
					if ( !shape.IsVisible )  continue;

					shape.Object.DrawText(port, drawingContext);
				}

				if ( shape.Type == Type.Image )
				{
					if ( !shape.IsVisible )  continue;

					shape.Object.DrawImage(port, drawingContext);
				}
			}
		}

		// Dessine un traitillé simple (dash/gap) le long d'un chemin.
		public static void DrawPathDash(Graphics graphics, DrawingContext drawingContext, Path path,
										double width, double dash, double gap, Drawing.Color color)
		{
			if ( path.IsEmpty )  return;

			DashedPath dp = new DashedPath();
			dp.DefaultZoom = drawingContext.ScaleX;
			dp.Append(path);

			dash /= drawingContext.ScaleX;
			gap  /= drawingContext.ScaleX;
			if ( dash == 0.0 )  // juste un point ?
			{
				dash = 0.00001;
				gap -= dash;
			}
			dp.AddDash(dash, gap);

			using ( Path temp = dp.GenerateDashedPath() )
			{
				graphics.Rasterizer.AddOutline(temp, width/drawingContext.ScaleX, CapStyle.Square, JoinStyle.Round, 5.0);
				graphics.RenderSolid(color);
			}
		}

		// Détecte si la souris est sur une forme.
		public bool Detect(Point pos,
						   DrawingContext drawingContext,
						   params Shape[] shapes)
		{
			foreach ( Shape shape in shapes )
			{
				if ( shape == null ||
					 !shape.IsVisible ||
					 shape.Path == null ||
					 shape.Path.IsEmpty )
				{
					continue;
				}

				if ( shape.Type == Type.Surface )
				{
					if ( Geometry.DetectSurface(shape.Path, pos) )  return true;
				}

				if ( shape.Type == Type.Stroke )
				{
					double width = 0.0;
					if ( shape.PropertyStroke != null )  width = shape.PropertyStroke.Width/2;
					width = System.Math.Max(width, drawingContext.MinimalWidth);
					if ( Geometry.DetectOutline(shape.Path, width, pos) )  return true;
				}
			}

			return false;
		}

		// Détecte si la souris est sur le pourtour d'une forme.
		// Retourne le rang de la poignée de départ, ou -1
		public int DetectOutline(Point pos,
								 DrawingContext drawingContext,
								 params Shape[] shapes)
		{
			foreach ( Shape shape in shapes )
			{
				if ( shape == null ||
					 shape.Path == null ||
					 shape.Path.IsEmpty ||
					 shape.Aspect != Aspect.Normal )
				{
					continue;
				}

				if ( shape.Type == Type.Stroke )
				{
					double width = 0.0;
					if ( shape.PropertyStroke != null )  width = shape.PropertyStroke.Width/2;
					width = System.Math.Max(width, drawingContext.MinimalWidth);
					int rank = Geometry.DetectOutlineRank(shape.Path, width, pos);
					if ( rank != -1 )  return rank;
				}
			}

			return -1;
		}


		#region Graphics
		// Dessine une surface à l'écran ou dans un bitmap.
		protected void DrawSurface(Graphics port,
								   DrawingContext drawingContext,
								   Shape shape,
								   Objects.Abstract obj)
		{
			this.DrawShape(port, drawingContext, shape, obj);
		}

		// Dessine un chemin à l'écran ou dans un bitmap.
		protected void DrawStroke(Graphics port,
								  DrawingContext drawingContext,
								  Shape shape,
								  Objects.Abstract obj)
		{
			Properties.Line stroke = shape.PropertyStroke;

			if ( shape.Aspect != Aspect.Hilited    &&
				 shape.Aspect != Aspect.Additional &&
				 shape.Aspect != Aspect.Support    &&
				 ((stroke != null && stroke.Dash) || shape.Aspect == Aspect.OverDashed) )
			{
				DashedPath dp = new DashedPath();
				dp.DefaultZoom = drawingContext.ScaleX;
				dp.Append(shape.Path);

				if ( shape.Aspect == Aspect.OverDashed )
				{
					double pen = 0.00001;
					double gap = 4.0/drawingContext.ScaleX - pen;
					dp.AddDash(pen, gap);
				}
				else
				{
					for ( int i=0 ; i<Properties.Line.DashMax ; i++ )
					{
						if ( stroke.GetDashGap(i) == 0.0 )  continue;
						double pen, gap;
						stroke.GetPenGap(i, false, out pen, out gap);
						dp.AddDash(pen, gap);
					}
				}

				using ( Path temp = dp.GenerateDashedPath() )
				{
					Path initial = shape.Path;
					shape.Path = temp;
					this.DrawShape(port, drawingContext, shape, obj);
					shape.Path = initial;
				}
			}
			else	// trait continu ?
			{
				this.DrawShape(port, drawingContext, shape, obj);
			}
		}

		// Dessine une forme à l'écran ou dans un bitmap.
		protected void DrawShape(Graphics port,
								 DrawingContext drawingContext,
								 Shape shape,
								 Objects.Abstract obj)
		{
			SurfaceAnchor sa = obj.SurfaceAnchor;
			if ( sa.IsSurfaceZero && shape.Aspect != Aspect.Support )  return;
			sa.LineUse = (shape.Type == Type.Stroke);

			Properties.Gradient surface = shape.PropertySurface as Properties.Gradient;
			Properties.Line     stroke  = shape.PropertyStroke;

			if ( shape.PropertySurface is Properties.Font )
			{
				Properties.Font font = shape.PropertySurface as Properties.Font;
				surface = new Properties.Gradient(this.document, Properties.Type.FillGradient);
				surface.IsOnlyForCreation = true;  // pas de undo !
				surface.FillType = Properties.GradientFillType.None;
				surface.Color1 = font.FontColor;
			}

			if ( shape.Aspect == Aspect.OverDashed )
			{
				Drawing.Color color = Color.FromBrightness(0.0);
				if ( surface != null && surface.IsVisible )
				{
					if ( surface.Color1.A > 0.0 )
					{
						color = surface.Color1;
					}
					else
					{
						color = surface.Color2;
					}
				}
				surface = new Properties.Gradient(this.document, Properties.Type.FillGradient);
				surface.IsOnlyForCreation = true;  // pas de undo !
				surface.FillType = Properties.GradientFillType.None;
				surface.Color1 = color;
			}

			if ( shape.Aspect == Aspect.Support )
			{
				surface = new Properties.Gradient(this.document, Properties.Type.FillGradient);
				surface.IsOnlyForCreation = true;  // pas de undo !
				surface.FillType = Properties.GradientFillType.None;
				surface.Color1 = Color.FromBrightness(0.6);
			}

			if ( shape.Aspect == Aspect.Additional )
			{
				Color color = surface.Color1;
				color.A = 0.2;
				surface = new Properties.Gradient(this.document, Properties.Type.FillGradient);
				surface.IsOnlyForCreation = true;  // pas de undo !
				surface.FillType = Properties.GradientFillType.None;
				surface.Color1 = color;
			}

			if ( shape.Aspect == Aspect.Hilited )
			{
				surface = new Properties.Gradient(this.document, Properties.Type.FillGradient);
				surface.IsOnlyForCreation = true;  // pas de undo !
				surface.FillType = Properties.GradientFillType.None;
				surface.Color1 = (shape.Type == Type.Stroke) ? drawingContext.HiliteOutlineColor : drawingContext.HiliteSurfaceColor;
			}

			CapStyle  cap   = CapStyle.Round;
			JoinStyle join  = JoinStyle.Round;
			double    limit = 0.0;
			if ( shape.PropertyStroke != null )
			{
				cap   = shape.PropertyStroke.Cap;
				join  = shape.PropertyStroke.Join;
				limit = shape.PropertyStroke.Limit;
			}

			if ( surface != null &&
				 (surface.FillType == Properties.GradientFillType.Hatch   ||
				  surface.FillType == Properties.GradientFillType.Dots    ||
				  surface.FillType == Properties.GradientFillType.Squares ) )
			{
				if ( shape.Type == Type.Stroke )
				{
					Path path = new Path();
					path.Append(shape.Path, shape.PropertyStroke.Width, cap, join, limit, drawingContext.ScaleX);
					shape = new Shape();
					shape.Path = path;
					shape.Type = Type.Surface;
				}

				if ( surface.FillType == Properties.GradientFillType.Hatch )
				{
					this.RenderHatch(port, drawingContext, shape.Path, sa, surface);
				}
				if ( surface.FillType == Properties.GradientFillType.Dots )
				{
					this.RenderDots(port, drawingContext, shape.Path, sa, surface);
				}
				if ( surface.FillType == Properties.GradientFillType.Squares )
				{
					this.RenderSquares(port, drawingContext, shape.Path, sa, surface);
				}

				return;
			}

			Graphics mask = null;

			double lineWidth = 0.0;
			if ( shape.PropertyStroke != null      )  lineWidth += shape.PropertyStroke.Width;
			if ( shape.Aspect == Aspect.Hilited    )  lineWidth += drawingContext.HiliteSize;
			if ( shape.Aspect == Aspect.OverDashed )  lineWidth = 1.0/drawingContext.ScaleX;
			if ( shape.Aspect == Aspect.Support    )  lineWidth = 1.0/drawingContext.ScaleX;

			if ( surface != null && surface.Smooth > 0 )  // flou ?
			{
				double sx = 1;
				double sy = 1;

				if ( drawingContext != null )
				{
					sx = drawingContext.ScaleX;
					sy = drawingContext.ScaleY;
				}

				mask = port.CreateAlphaMask();

				int step = (int)(surface.Smooth*sx);
				if ( step > 20 )  step = 20;
				if ( drawingContext != null && !drawingContext.PreviewActive )  step /= 4;  // brouillon
				if ( step <  2 )  step =  2;
				for ( int i=0 ; i<step ; i++ )
				{
					double w = surface.Smooth-i*surface.Smooth/step;
					mask.Rasterizer.AddOutline(shape.Path, lineWidth+w*2, cap, join, limit);
					double intensity = (i+1.0)/step;
					mask.RenderSolid(Drawing.Color.FromBrightness(intensity));
				}
				if ( shape.Type == Type.Surface )
				{
					mask.Rasterizer.AddSurface(shape.Path);
				}
				mask.RenderSolid(Drawing.Color.FromBrightness(1.0));

				port.SolidRenderer.SetAlphaMask(mask.Pixmap, MaskComponent.R);

				Rectangle box = sa.BoundingBox;
				box.Inflate(surface.Smooth);
				port.AddFilledRectangle(box);
			}
			else
			{
				if ( shape.Type == Type.Surface )
				{
					port.Rasterizer.AddSurface(shape.Path);
				}
				else
				{
					port.Rasterizer.AddOutline(shape.Path, lineWidth, cap, join, limit);
				}
			}

			bool flat;
			if ( surface == null || surface.FillType == Properties.GradientFillType.None )
			{
				flat = true;
			}
			else if ( surface.FillType == Properties.GradientFillType.Linear )
			{
				flat = ( System.Math.Abs(surface.Sx) < 0.01 &&
						 System.Math.Abs(surface.Sy) < 0.01 );
			}
			else
			{
				flat = ( System.Math.Abs(surface.Sx) < 0.01 ||
						 System.Math.Abs(surface.Sy) < 0.01 );
			}

			if ( flat )  // uniforme ?
			{
				Drawing.Color c1 = (surface == null) ? Color.FromBrightness(0.6) : surface.Color1;
				if ( drawingContext != null && shape.Aspect != Aspect.Hilited )
				{
					c1 = drawingContext.AdaptColor(c1);
				}

				port.RenderSolid(c1);
			}
			else	// dégradé ?
			{
				Drawing.Color c1 = surface.Color1;
				Drawing.Color c2 = surface.Color2;
				if ( drawingContext != null )
				{
					c1 = drawingContext.AdaptColor(c1);
					c2 = drawingContext.AdaptColor(c2);
				}

				port.FillMode = FillMode.NonZero;

				if ( surface.Repeat == 1 && surface.Middle == 0.0 )
				{
					port.GradientRenderer.SetColors(c1, c2);
				}
				else
				{
					double[] r = new double[256];
					double[] g = new double[256];
					double[] b = new double[256];
					double[] a = new double[256];

					for ( int i=0 ; i<256 ; i++ )
					{
						double factor = this.GetFactor(surface, i/255.0);
						r[i] = c1.R + (c2.R-c1.R)*factor;
						g[i] = c1.G + (c2.G-c1.G)*factor;
						b[i] = c1.B + (c2.B-c1.B)*factor;
						a[i] = c1.A + (c2.A-c1.A)*factor;
					}

					port.GradientRenderer.SetColors(r, g, b, a);
				}

				Transform ot = port.GradientRenderer.Transform;
				Transform t = new Transform();

				Point center = sa.ToAbs(new Point(surface.Cx, surface.Cy));
				Point corner = sa.ToAbs(new Point(surface.Cx+surface.Sx, surface.Cy+surface.Sy));

				if ( surface.FillType == Properties.GradientFillType.Linear )
				{
					double a = Point.ComputeAngleDeg(center, corner)-90;
					double d = Point.Distance(center, corner);
					port.GradientRenderer.Fill = GradientFill.Y;
					port.GradientRenderer.SetParameters(-255, 255);
					t.RotateDeg(a);
					t.Scale(1.0, d/255);
					t.Translate(center);
				}
				else if ( surface.FillType == Properties.GradientFillType.Circle )
				{
					port.GradientRenderer.Fill = GradientFill.Circle;
					port.GradientRenderer.SetParameters(0, 255);
					t.RotateDeg(sa.Direction);
					t.Scale(sa.Width/255*surface.Sx, sa.Height/255*surface.Sy);
					t.Translate(center);
				}
				else if ( surface.FillType == Properties.GradientFillType.Diamond )
				{
					port.GradientRenderer.Fill = GradientFill.Diamond;
					port.GradientRenderer.SetParameters(0, 255);
					t.RotateDeg(sa.Direction);
					t.Scale(sa.Width/255*surface.Sx, sa.Height/255*surface.Sy);
					t.Translate(center);
				}
				else if ( surface.FillType == Properties.GradientFillType.Conic )
				{
					port.GradientRenderer.Fill = GradientFill.Conic;
					port.GradientRenderer.SetParameters(0, 250);
					t.RotateDeg(sa.Direction+surface.Angle-90.0);
					t.Scale(sa.Width/255*surface.Sx, sa.Height/255*surface.Sy);
					t.Translate(center);
				}

				port.GradientRenderer.Transform = t;
				port.RenderGradient();
				port.GradientRenderer.Transform = ot;
			}

			if ( surface != null && surface.Smooth > 0 )  // flou ?
			{
				port.SolidRenderer.SetAlphaMask(null, MaskComponent.None);
				mask.Dispose();
			}
		}

		// Calcule le facteur de progression dans la couleur [0..1].
		protected double GetFactor(Properties.Gradient surface, double progress)
		{
			if ( surface.Repeat > 1 )
			{
				int i = (int)(progress*surface.Repeat);
				progress = (progress*surface.Repeat)%1.0;
				if ( i%2 != 0 )  progress = 1.0-progress;
			}
			if ( surface.Middle != 0.0 )
			{
				if ( surface.Middle > 0.0 )
				{
					progress = 1.0-System.Math.Pow(1.0-progress, 1.0+surface.Middle);
				}
				else
				{
					progress = System.Math.Pow(progress, 1.0-surface.Middle);
				}
			}
			return progress;
		}

		// Effectue le rendu d'une zone quelconque hachurée.
		protected void RenderHatch(Graphics graphics, DrawingContext drawingContext, Path path, SurfaceAnchor sa, Properties.Gradient surface)
		{
			Drawing.Color initialColor = graphics.Color;

			Drawing.Color c1 = surface.Color1;
			Drawing.Color c2 = surface.Color2;
			if ( drawingContext != null )
			{
				c1 = drawingContext.AdaptColor(c1);
				c2 = drawingContext.AdaptColor(c2);
			}

			graphics.Color = c1;
			graphics.PaintSurface(path);  // dessine le fond

			for ( int i=0 ; i<Properties.Gradient.HatchMax ; i++ )
			{
				if ( surface.GetHatchWidth(i) == 0.0 || surface.GetHatchDistance(i) == 0.0 )  continue;

				Point p1, p2, p3, p4;
				double offset;
				this.MinRectMotif(surface, sa, i, out p1, out p2, out p3, out p4, out offset);
				double width = System.Math.Min(surface.GetHatchWidth(i), surface.GetHatchDistance(i));

				Path pathLines = new Path();
				double len = Point.Distance(p1, p2);
				double pos = offset;
				while ( pos < len )
				{
					Point pp1 = Point.Move(p1, p2, pos);
					Point pp2 = Point.Move(p1, p2, pos+width);
					Point pp3 = Point.Move(p3, p4, pos);
					Point pp4 = Point.Move(p3, p4, pos+width);

					pathLines.MoveTo(pp1);
					pathLines.LineTo(pp2);
					pathLines.LineTo(pp4);
					pathLines.LineTo(pp3);
					pathLines.Close();

					pos += surface.GetHatchDistance(i);
				}

				pathLines = Path.Combine(pathLines, path, PathOperation.And);

				graphics.Color = c2;
				graphics.PaintSurface(pathLines);
			}

			graphics.Color = initialColor;
		}

		// Effectue le rendu d'une zone quelconque de points.
		protected void RenderDots(Graphics graphics, DrawingContext drawingContext, Path path, SurfaceAnchor sa, Properties.Gradient surface)
		{
			Drawing.Color initialColor = graphics.Color;

			Drawing.Color c1 = surface.Color1;
			Drawing.Color c2 = surface.Color2;
			if ( drawingContext != null )
			{
				c1 = drawingContext.AdaptColor(c1);
				c2 = drawingContext.AdaptColor(c2);
			}

			graphics.Color = c1;
			graphics.PaintSurface(path);  // dessine le fond

			for ( int i=0 ; i<Properties.Gradient.HatchMax ; i++ )
			{
				if ( surface.GetHatchWidth(i) == 0.0 || surface.GetHatchDistance(i) == 0.0 )  continue;

				Point p1, p2, p3, p4;
				double offset;
				this.MinRectMotif(surface, sa, i, out p1, out p2, out p3, out p4, out offset);
				double width = System.Math.Min(surface.GetHatchWidth(i), surface.GetHatchDistance(i)/2);

				Path pathLines = new Path();
				double lenh = Point.Distance(p1, p2);
				double posh = offset;
				while ( posh < lenh )
				{
					Point pp1 = Point.Move(p1, p2, posh);
					Point pp3 = Point.Move(p3, p4, posh);

					double lenv = Point.Distance(p1, p3);
					double posv = 0.0;
					while ( posv < lenv )
					{
						Point center = Point.Move(pp1, pp3, posv);
						pathLines.AppendCircle(center, width);

						posv += surface.GetHatchDistance(i);
					}

					posh += surface.GetHatchDistance(i);
				}

				if ( !drawingContext.PreviewActive )
				{
					Path pathApprox = new Path();
					pathApprox.Append(pathLines, 0.01, 0.0);
					pathLines = pathApprox;
				}

				pathLines = Path.Combine(pathLines, path, PathOperation.And);

				graphics.Color = c2;
				graphics.PaintSurface(pathLines);
			}

			graphics.Color = initialColor;
		}

		// Effectue le rendu d'une zone quelconque de carrés.
		protected void RenderSquares(Graphics graphics, DrawingContext drawingContext, Path path, SurfaceAnchor sa, Properties.Gradient surface)
		{
			Drawing.Color initialColor = graphics.Color;

			Drawing.Color c1 = surface.Color1;
			Drawing.Color c2 = surface.Color2;
			if ( drawingContext != null )
			{
				c1 = drawingContext.AdaptColor(c1);
				c2 = drawingContext.AdaptColor(c2);
			}

			graphics.Color = c1;
			graphics.PaintSurface(path);  // dessine le fond

			for ( int i=0 ; i<Properties.Gradient.HatchMax ; i++ )
			{
				if ( surface.GetHatchWidth(i) == 0.0 || surface.GetHatchDistance(i) == 0.0 )  continue;

				Point p1, p2, p3, p4;
				double offset;
				this.MinRectMotif(surface, sa, i, out p1, out p2, out p3, out p4, out offset);
				double width = System.Math.Min(surface.GetHatchWidth(i), surface.GetHatchDistance(i)/2);

				Path pathLines = new Path();
				double lenh = Point.Distance(p1, p2);
				double posh = offset;
				while ( posh < lenh )
				{
					Point pp1 = Point.Move(p1, p2, posh);
					Point pp3 = Point.Move(p3, p4, posh);

					double lenv = Point.Distance(p1, p3);
					double posv = 0.0;
					while ( posv < lenv )
					{
						Point center = Point.Move(pp1, pp3, posv);
						center.X -= width;
						center.Y -= width;
						pathLines.AppendRectangle(center, new Size(width*2, width*2));

						posv += surface.GetHatchDistance(i);
					}

					posh += surface.GetHatchDistance(i);
				}

				pathLines = Path.Combine(pathLines, path, PathOperation.And);

				graphics.Color = c2;
				graphics.PaintSurface(pathLines);
			}

			graphics.Color = initialColor;
		}

		// Calcule le rectangle le plus petit possible qui sera rempli par le motif.
		// L'offset permet à deux objets proches d'avoir des hachures jointives.
		protected void MinRectMotif(Properties.Gradient surface, SurfaceAnchor sa, int i, out Point p1, out Point p2, out Point p3, out Point p4, out double offset)
		{
			Rectangle bbox = sa.BoundingBox;

			double b = Math.ClipAngleDeg(sa.Direction+surface.GetHatchAngle(i));
			double a = Math.DegToRad(b%90.0);

			double ha = System.Math.Cos(a)*bbox.Width;
			double hx = System.Math.Cos(a)*ha;
			double hy = System.Math.Sin(a)*ha;

			double va = System.Math.Cos(a)*bbox.Height;
			double vx = System.Math.Cos(a)*va;
			double vy = System.Math.Sin(a)*va;

			Point[] rect = new Point[4];
			rect[0] = new Point();
			rect[1] = new Point();
			rect[2] = new Point();
			rect[3] = new Point();

			rect[0].X = bbox.Right-hx;
			rect[0].Y = bbox.Bottom-hy;

			rect[1].X = bbox.Right+vy;
			rect[1].Y = bbox.Top-vx;

			rect[2].X = bbox.Left+hx;
			rect[2].Y = bbox.Top+hy;

			rect[3].X = bbox.Left-vy;
			rect[3].Y = bbox.Bottom+vx;

			int j = ((int) (b/90.0)) % 4;
			p1 = rect[(j+0)%4];
			p2 = rect[(j+1)%4];
			p4 = rect[(j+2)%4];
			p3 = rect[(j+3)%4];

			Point pp1 = Transform.RotatePointDeg(-(sa.Direction+surface.GetHatchAngle(i)), p1);
			offset = -(pp1.X+100000.0)%surface.GetHatchDistance(i);
		}
		#endregion

		
		#region Printer
		// Dessine une surface sur l'imprimante.
		protected void DrawSurface(Printing.PrintPort port,
								   DrawingContext drawingContext,
								   Shape shape,
								   Objects.Abstract obj)
		{
			if ( !this.PrinterSurface(port, drawingContext, shape) )  return;
			port.PaintSurface(shape.Path);
		}

		// Dessine un chemin sur l'imprimante.
		protected void DrawStroke(Printing.PrintPort port,
								  DrawingContext drawingContext,
								  Shape shape,
								  Objects.Abstract obj)
		{
			if ( !this.PrinterSurface(port, drawingContext, shape) )  return;

			Properties.Line stroke = shape.PropertyStroke;

			if ( stroke.Dash )  // traitillé ?
			{
				DashedPath dp = new DashedPath();
				dp.DefaultZoom = drawingContext.ScaleX;
				dp.Append(shape.Path);

				for ( int i=0 ; i<Properties.Line.DashMax ; i++ )
				{
					if ( stroke.GetDashGap(i) == 0.0 )  continue;
					double pen, gap;
					stroke.GetPenGap(i, true, out pen, out gap);
					dp.AddDash(pen, gap);
				}

				port.LineWidth = stroke.Width;
				port.LineCap = stroke.Cap;
				port.LineJoin = stroke.EffectiveJoin;
				port.LineMiterLimit = stroke.Limit;

				using ( Path temp = dp.GenerateDashedPath() )
				{
					port.PaintOutline(temp);
				}
			}
			else	// trait continu ?
			{
				port.LineWidth = stroke.Width;
				port.LineCap = stroke.Cap;
				port.LineJoin = stroke.EffectiveJoin;
				port.LineMiterLimit = stroke.Limit;
				port.PaintOutline(shape.Path);
			}
		}

		// Choix de la couleur de la surface.
		protected bool PrinterSurface(Printing.PrintPort port,
									  DrawingContext drawingContext,
									  Shape shape)
		{
			Color color = Color.Empty;
			if ( shape.PropertySurface is Properties.Gradient )
			{
				Properties.Gradient surface = shape.PropertySurface as Properties.Gradient;
				color = surface.Color1;
			}
			if ( shape.PropertySurface is Properties.Font )
			{
				Properties.Font font = shape.PropertySurface as Properties.Font;
				color = font.FontColor;
			}
			if ( color.IsEmpty || !color.IsOpaque )  return false;

			port.Color = drawingContext.AdaptColor(color);
			return true;
		}
		#endregion


		#region PDF
		// Dessine une surface dans un fichier PDF.
		protected void DrawSurface(PDF.Port port,
								   DrawingContext drawingContext,
								   Shape shape,
								   Objects.Abstract obj)
		{
			this.PDFSetSurface(port, drawingContext, obj, shape.PropertySurface);
			port.PaintSurface(shape.Path);
		}

		
		// Dessine un chemin dans un fichier PDF.
		protected void DrawStroke(PDF.Port port,
								  DrawingContext drawingContext,
								  Shape shape,
								  Objects.Abstract obj)
		{
			this.PDFSetSurface(port, drawingContext, obj, shape.PropertySurface);
			this.PDFSetStroke(port, drawingContext, obj, shape.PropertyStroke);
			port.PaintOutline(shape.Path);
		}

		// Initialise le port en fonction de la surface.
		protected void PDFSetSurface(PDF.Port port,
									 DrawingContext drawingContext,
									 Objects.Abstract obj,
									 Properties.Abstract surface)
		{
			if ( surface is Properties.Gradient )
			{
				Properties.Gradient gradient = surface as Properties.Gradient;

				if ( gradient.FillType == Properties.GradientFillType.Hatch )
				{
					Color color = drawingContext.AdaptColor(gradient.Color1);
					int pattern = port.SearchPattern(obj, gradient);
					port.SetColoredPattern(color, pattern);
				}
				else
				{
					port.Color = drawingContext.AdaptColor(gradient.Color1);
				}
			}

			if ( surface is Properties.Font )
			{
				Properties.Font font = surface as Properties.Font;
				port.Color = drawingContext.AdaptColor(font.FontColor);
			}
		}

		// Initialise le port en fonction du trait.
		protected void PDFSetStroke(PDF.Port port,
									DrawingContext drawingContext,
									Objects.Abstract obj,
									Properties.Line stroke)
		{
			if ( stroke.Dash )  // traitillé ?
			{
				port.SetLineDash(stroke.Width, stroke.GetDashPen(0), stroke.GetDashGap(0), stroke.GetDashPen(1), stroke.GetDashGap(1));
			}
			else	// trait continu ?
			{
				port.LineWidth = stroke.Width;
			}

			port.LineCap = stroke.Cap;
			port.LineJoin = stroke.Join;
			port.LineMiterLimit = stroke.Limit;
		}
		#endregion


		protected Document				document;
	}
}
