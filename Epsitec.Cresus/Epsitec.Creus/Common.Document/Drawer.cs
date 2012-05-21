using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe Drawer contient le dessinateur universel, pour dessiner sur
	/// l'écran ou dans un bitmap avec AGG, pour imprimer ou pour exporter en PDF.
	/// </summary>
	public class Drawer
	{
		public enum DrawShapesMode
		{
			All,			// dessine tout
			NoText,			// ne dessine pas le texte
			OnlyText,		// ne dessine que le texte
		}


		public Drawer(Document document)
		{
			this.document = document;
		}

		public bool DrawShapes(IPaintPort port,
							   DrawingContext drawingContext,
							   Objects.Abstract obj,
							   DrawShapesMode mode,
							   params Shape[] shapes)
		{
			//	Dessine des formes dans n'importe quel IPaintPort.
			//	Retourne true si un texte a été sauté en mode NoText.
			FillMode iMode = port.FillMode;
			bool skipText = false;

#if false
			for ( int i=0 ; i<shapes.Length ; i++ )
			{
				Shape shape = shapes[i];
				if ( shape == null )  continue;

				shape.IsLinkWithNext = false;

				if ( i < shapes.Length-1 )
				{
					Shape next = shapes[i+1];
					if ( next == null )  continue;

					if ( shape.PropertySurface == next.PropertySurface &&
						 shape.Aspect == next.Aspect )
					{
						shape.IsLinkWithNext = true;
					}
				}
			}
#endif

			foreach ( Shape shape in shapes )
			{
				if ( shape == null ||
					 shape.Aspect == Aspect.InvisibleBox ||
					 shape.Aspect == Aspect.OnlyDetect )  continue;

				port.FillMode = shape.FillMode;

				if ( shape.Type == Type.Surface && mode != DrawShapesMode.OnlyText )
				{
					if ( !shape.IsVisible || shape.Path.IsEmpty )  continue;

					obj.SurfaceAnchor.LineUse = false;

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

				if ( shape.Type == Type.Stroke && mode != DrawShapesMode.OnlyText )
				{
					if ( !shape.IsVisible || shape.Path.IsEmpty )  continue;

					obj.SurfaceAnchor.LineUse = true;

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
					if ( mode == DrawShapesMode.NoText )
					{
						skipText = true;
					}
					else
					{
						if ( !shape.IsVisible && mode != DrawShapesMode.OnlyText )  continue;
						shape.Object.DrawText(port, drawingContext);
					}
				}

				if ( shape.Type == Type.Image && mode != DrawShapesMode.OnlyText )
				{
					if (!shape.IsVisible)
					{
						continue;
					}

					if ((drawingContext.DrawImageFilter != null) &&
						(!drawingContext.DrawImageFilter (new DrawingContext.DrawImageFilterInfo (shape.Object, "image"))))
					{
						continue;
					}

					shape.Object.DrawImage(port, drawingContext);
				}
			}

			port.FillMode = iMode;
			return skipText;
		}

		public static void DrawPathDash(Graphics graphics, DrawingContext drawingContext, Path path,
										double width, double dash, double gap, Drawing.Color color)
		{
			//	Dessine un traitillé simple (dash/gap) le long d'un chemin.
			Drawer.DrawPathDash(graphics, drawingContext.ScaleX, path, width, dash, gap, color);
		}

		public static void DrawPathDash(Graphics graphics, double scale, Path path,
										double width, double dash, double gap, Drawing.Color color)
		{
			//	Dessine un traitillé simple (dash/gap) le long d'un chemin.
			if (path.IsEmpty)
			{
				return;
			}

			DashedPath dp = new DashedPath();
			dp.DefaultZoom = scale;
			dp.Append(path);

			dash /= scale;
			gap  /= scale;
			if ( dash == 0.0 )  // juste un point ?
			{
				dash = 0.00001;
				gap -= dash;
			}
			dp.AddDash(dash, gap);

			using ( Path temp = dp.GenerateDashedPath() )
			{
				graphics.Rasterizer.AddOutline(temp, width/scale, CapStyle.Square, JoinStyle.Round, 5.0);
				graphics.RenderSolid(color);
			}
		}

		public bool Detect(Point pos,
						   DrawingContext drawingContext,
						   params Shape[] shapes)
		{
			//	Détecte si la souris est sur une forme.
			foreach ( Shape shape in shapes )
			{
				if ( shape == null ||
					 shape.Path == null ||
					 shape.Path.IsEmpty )
				{
					continue;
				}

				if ( shape.Type == Type.Surface )
				{
					if ( !shape.IsVisible )  continue;
					if ( Geometry.DetectSurface(shape.Path, pos) )  return true;
				}

				if ( shape.Type == Type.Stroke )
				{
					if ( !shape.IsVisible && shape.IsMisc )  continue;
					double width = 0.0;
					if ( shape.PropertyStroke != null )  width = shape.PropertyStroke.Width/2;
					width = System.Math.Max(width, drawingContext.MinimalWidth);
					if ( Geometry.DetectOutline(shape.Path, width, pos) )  return true;
				}
			}

			return false;
		}

		public int DetectOutline(Point pos,
								 DrawingContext drawingContext,
								 params Shape[] shapes)
		{
			//	Détecte si la souris est sur le pourtour d'une forme.
			//	Retourne le rang de la poignée de départ, ou -1
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
		protected void DrawSurface(Graphics port,
								   DrawingContext drawingContext,
								   Shape shape,
								   Objects.Abstract obj)
		{
			//	Dessine une surface à l'écran ou dans un bitmap.
			this.DrawShape(port, drawingContext, shape, obj);
		}

		protected void DrawStroke(Graphics port,
								  DrawingContext drawingContext,
								  Shape shape,
								  Objects.Abstract obj)
		{
			//	Dessine un chemin à l'écran ou dans un bitmap.
			Properties.Line stroke = shape.PropertyStroke;

			if ( shape.Aspect != Aspect.Hilited    &&
				 shape.Aspect != Aspect.Additional &&
				 shape.Aspect != Aspect.Support    &&
				 ((stroke != null && stroke.Dash) || shape.Aspect == Aspect.OverDashed) )
			{
				double scaleX = (drawingContext == null) ? 1 : drawingContext.ScaleX;
				DashedPath dp = new DashedPath();
				dp.DefaultZoom = scaleX;
				dp.Append(shape.Path);

				if ( shape.Aspect == Aspect.OverDashed )
				{
					double pen = 0.00001;
					double gap = 4.0/scaleX - pen;
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

		protected void DrawShape(Graphics port,
								 DrawingContext drawingContext,
								 Shape shape,
								 Objects.Abstract obj)
		{
			//	Dessine une forme à l'écran ou dans un bitmap.
			SurfaceAnchor sa = obj.SurfaceAnchor;
			if ( sa.IsSurfaceZero && shape.Aspect != Aspect.Support )  return;

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
				RichColor color = RichColor.FromBrightness(0.0);
				if ( surface != null && surface.IsVisible(port) )
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
				surface.Color1 = RichColor.FromBrightness(0.6);
			}

			if ( shape.Aspect == Aspect.Additional )
			{
				RichColor color = surface.Color1;
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
				//?surface.Color1 = (shape.Type == Type.Stroke) ? drawingContext.HiliteOutlineColor : drawingContext.HiliteSurfaceColor;
				surface.Color1 = new RichColor(drawingContext.HiliteSurfaceColor);
				if ( shape.PropertySurface != null &&
					 shape.PropertySurface.Type == Properties.Type.LineColor )
				{
					surface.Color1 = new RichColor(drawingContext.HiliteOutlineColor);
				}
			}

			CapStyle  cap   = CapStyle.Round;
			JoinStyle join  = JoinStyle.Round;
			double    limit = 5.0;
			if ( shape.PropertyStroke != null )
			{
				cap   = shape.PropertyStroke.Cap;
				join  = shape.PropertyStroke.EffectiveJoin;
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

				Drawer.RenderMotif(port, drawingContext, null, shape.Path, sa, surface);
				return;
			}

			Graphics mask = null;
			double lineWidth = 0.0;

			if ( shape.PropertyStroke != null )
			{
				if ( shape.Type != Type.Stroke || shape.PropertySurface.IsVisible(port) )
				{
					lineWidth += shape.PropertyStroke.Width;
				}
			}

			if ( shape.Aspect == Aspect.Hilited )
			{
				lineWidth += drawingContext.HiliteSize;
			}

			if ( shape.Aspect == Aspect.OverDashed )
			{
				lineWidth = 1.0/drawingContext.ScaleX;
			}

			if ( shape.Aspect == Aspect.Support )
			{
				lineWidth = 1.0/drawingContext.ScaleX;
			}

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
				if ( drawingContext != null && drawingContext.IsBitmap )  step *= 2;  // qualité supérieure
				if ( step <  2 )  step =  2;
				for ( int i=0 ; i<step ; i++ )
				{
					double w = surface.Smooth-i*surface.Smooth/step;
					mask.Rasterizer.AddOutline(shape.Path, lineWidth+w*2.0, cap, join, limit);
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
				Color c1 = (surface == null) ? Color.FromBrightness(0.6) : surface.Color1.Basic;
				if ( drawingContext != null && shape.Aspect != Aspect.Hilited )
				{
					c1 = port.GetFinalColor(c1);
				}

				if ( !shape.IsLinkWithNext )
				{
					port.FinalRenderSolid(c1);
				}
			}
			else	// dégradé ?
			{
				Color c1 = surface.Color1.Basic;
				Color c2 = surface.Color2.Basic;
				if ( drawingContext != null )
				{
					c1 = port.GetFinalColor(c1);
					c2 = port.GetFinalColor(c2);
				}

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
						double factor = surface.GetProgressColorFactor(i/255.0);
						r[i] = c1.R + (c2.R-c1.R)*factor;
						g[i] = c1.G + (c2.G-c1.G)*factor;
						b[i] = c1.B + (c2.B-c1.B)*factor;
						a[i] = c1.A + (c2.A-c1.A)*factor;
					}

					port.GradientRenderer.SetColors(r, g, b, a);
				}

				Transform ot = port.GradientRenderer.Transform;
				Transform t = Transform.Identity;

				Point center = sa.ToAbs(new Point(surface.Cx, surface.Cy));
				Point corner = sa.ToAbs(new Point(surface.Cx+surface.Sx, surface.Cy+surface.Sy));

				if ( surface.FillType == Properties.GradientFillType.Linear )
				{
					double a = Point.ComputeAngleDeg(center, corner)-90.0;
					double d = Point.Distance(center, corner);
					port.GradientRenderer.Fill = GradientFill.Y;
					port.GradientRenderer.SetParameters(-255, 255);
					t = t.RotateDeg(a);
					t = t.Scale(1.0, d/255);
					t = t.Translate(center);
				}
				else if ( surface.FillType == Properties.GradientFillType.Circle )
				{
					port.GradientRenderer.Fill = GradientFill.Circle;
					port.GradientRenderer.SetParameters(0, 255);
					t = t.RotateDeg(sa.Direction);
					t = t.Scale(sa.Width/255*surface.Sx, sa.Height/255*surface.Sy);
					t = t.Translate(center);
				}
				else if ( surface.FillType == Properties.GradientFillType.Diamond )
				{
					port.GradientRenderer.Fill = GradientFill.Diamond;
					port.GradientRenderer.SetParameters(0, 255);
					t = t.RotateDeg(sa.Direction);
					t = t.Scale(sa.Width/255*surface.Sx, sa.Height/255*surface.Sy);
					t = t.Translate(center);
				}
				else if ( surface.FillType == Properties.GradientFillType.Conic )
				{
					port.GradientRenderer.Fill = GradientFill.Conic;
					port.GradientRenderer.SetParameters(0, 250);
					t = t.Scale(sa.Width/255*surface.Sx, sa.Height/255*surface.Sy);
					t = t.Translate(center);
					t = t.RotateDeg(sa.Direction+surface.Angle, center);
					t = t.MultiplyByPostfix(Transform.CreateRotationDegTransform(-90.0));
				}

				if ( !shape.IsLinkWithNext )
				{
					port.GradientRenderer.Transform = t;
					port.RenderGradient();
					port.GradientRenderer.Transform = ot;
				}
			}

			if ( surface != null && surface.Smooth > 0 )  // flou ?
			{
				port.SolidRenderer.SetAlphaMask(null, MaskComponent.None);
				mask.Dispose();
			}
		}

		public static void RenderMotif(IPaintPort port, DrawingContext drawingContext, Objects.Abstract obj, Path path, SurfaceAnchor sa, Properties.Gradient surface)
		{
			//	Effectue le rendu d'une zone quelconque contenant un motif.
			switch ( surface.FillType )
			{
				case Properties.GradientFillType.Hatch:
					Drawer.RenderHatch(port, drawingContext, obj, path, sa, surface);
					break;

				case Properties.GradientFillType.Dots:
					Drawer.RenderDots(port, drawingContext, obj, path, sa, surface);
					break;

				case Properties.GradientFillType.Squares:
					Drawer.RenderSquares(port, drawingContext, obj, path, sa, surface);
					break;
			}
		}

		protected static void RenderHatch(IPaintPort port, DrawingContext drawingContext, Objects.Abstract obj, Path path, SurfaceAnchor sa, Properties.Gradient surface)
		{
			//	Effectue le rendu d'une zone quelconque hachurée.
			Drawing.RichColor initialColor = port.RichColor;

			RichColor c1 = surface.Color1;
			RichColor c2 = surface.Color2;
			if ( drawingContext != null )
			{
				c1 = port.GetFinalColor(c1);
				c2 = port.GetFinalColor(c2);
			}

			Drawer.RenderMotifColor(port, drawingContext, obj, surface, c1, true);
			port.PaintSurface(path);  // dessine le fond

			for ( int i=0 ; i<Properties.Gradient.HatchMax ; i++ )
			{
				if ( surface.GetHatchWidth(i) == 0.0 || surface.GetHatchDistance(i) == 0.0 )  continue;

				Point p1, p2, p3, p4;
				double offset;
				Drawer.MinRectMotif(surface, sa, i, out p1, out p2, out p3, out p4, out offset);
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

				Drawer.RenderMotifColor(port, drawingContext, obj, surface, c2, false);
				port.PaintSurface(pathLines);
			}

			port.RichColor = initialColor;
		}

		protected static void RenderDots(IPaintPort port, DrawingContext drawingContext, Objects.Abstract obj, Path path, SurfaceAnchor sa, Properties.Gradient surface)
		{
			//	Effectue le rendu d'une zone quelconque de points.
			Drawing.RichColor initialColor = port.RichColor;

			RichColor c1 = surface.Color1;
			RichColor c2 = surface.Color2;
			if ( drawingContext != null )
			{
				c1 = port.GetFinalColor(c1);
				c2 = port.GetFinalColor(c2);
			}

			Drawer.RenderMotifColor(port, drawingContext, obj, surface, c1, true);
			port.PaintSurface(path);  // dessine le fond

			for ( int i=0 ; i<Properties.Gradient.HatchMax ; i++ )
			{
				if ( surface.GetHatchWidth(i) == 0.0 || surface.GetHatchDistance(i) == 0.0 )  continue;

				Point p1, p2, p3, p4;
				double offset;
				Drawer.MinRectMotif(surface, sa, i, out p1, out p2, out p3, out p4, out offset);
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

				if ( drawingContext != null && !drawingContext.PreviewActive )
				{
					Path pathApprox = new Path();
					pathApprox.Append(pathLines, 0.01, 0.0);
					pathLines = pathApprox;
				}

				pathLines = Path.Combine(pathLines, path, PathOperation.And);

				Drawer.RenderMotifColor(port, drawingContext, obj, surface, c2, false);
				port.PaintSurface(pathLines);
			}

			port.RichColor = initialColor;
		}

		protected static void RenderSquares(IPaintPort port, DrawingContext drawingContext, Objects.Abstract obj, Path path, SurfaceAnchor sa, Properties.Gradient surface)
		{
			//	Effectue le rendu d'une zone quelconque de carrés.
			Drawing.RichColor initialColor = port.RichColor;

			RichColor c1 = surface.Color1;
			RichColor c2 = surface.Color2;
			if ( drawingContext != null )
			{
				c1 = port.GetFinalColor(c1);
				c2 = port.GetFinalColor(c2);
			}

			Drawer.RenderMotifColor(port, drawingContext, obj, surface, c1, true);
			port.PaintSurface(path);  // dessine le fond

			for ( int i=0 ; i<Properties.Gradient.HatchMax ; i++ )
			{
				if ( surface.GetHatchWidth(i) == 0.0 || surface.GetHatchDistance(i) == 0.0 )  continue;

				Point p1, p2, p3, p4;
				double offset;
				Drawer.MinRectMotif(surface, sa, i, out p1, out p2, out p3, out p4, out offset);
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

				Drawer.RenderMotifColor(port, drawingContext, obj, surface, c2, false);
				port.PaintSurface(pathLines);
			}

			port.RichColor = initialColor;
		}

		protected static void RenderMotifColor(IPaintPort port, DrawingContext drawingContext, Objects.Abstract obj, Properties.Gradient surface, RichColor color, bool firstColor)
		{
			if ( obj != null && port is PDF.Port )
			{
				PDF.Type type = surface.TypeComplexSurfacePDF(port);
				if ( type == PDF.Type.TransparencyPattern )
				{
					PDF.Port pdfPort = port as PDF.Port;

					int id = pdfPort.SearchComplexSurface(obj, surface);
					pdfPort.SetColoredComplexSurface(color, id, firstColor ? PDF.PdfComplexSurfaceType.ExtGStateP1 : PDF.PdfComplexSurfaceType.ExtGStateP2);
					return;
				}
			}

			port.RichColor = color;
		}

		protected static void MinRectMotif(Properties.Gradient surface, SurfaceAnchor sa, int i, out Point p1, out Point p2, out Point p3, out Point p4, out double offset)
		{
			//	Calcule le rectangle le plus petit possible qui sera rempli par le motif.
			//	L'offset permet à deux objets proches d'avoir des hachures jointives.
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
		protected void DrawSurface(Printing.PrintPort port,
								   DrawingContext drawingContext,
								   Shape shape,
								   Objects.Abstract obj)
		{
			//	Dessine une surface sur l'imprimante.
			if ( !this.PrinterSurface(port, drawingContext, shape) )  return;
			port.PaintSurface(shape.Path);
		}

		protected void DrawStroke(Printing.PrintPort port,
								  DrawingContext drawingContext,
								  Shape shape,
								  Objects.Abstract obj)
		{
			//	Dessine un chemin sur l'imprimante.
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

		protected bool PrinterSurface(Printing.PrintPort port,
									  DrawingContext drawingContext,
									  Shape shape)
		{
			//	Choix de la couleur de la surface.
			Color color = Color.Empty;
			if ( shape.PropertySurface is Properties.Gradient )
			{
				Properties.Gradient surface = shape.PropertySurface as Properties.Gradient;
				color = surface.Color1.Basic;
			}
			if ( shape.PropertySurface is Properties.Font )
			{
				Properties.Font font = shape.PropertySurface as Properties.Font;
				color = font.FontColor.Basic;
			}
			if ( color.IsEmpty || !color.IsOpaque )  return false;

			port.Color = color;
			return true;
		}
		#endregion


		#region PDF
		protected void DrawSurface(PDF.Port port,
								   DrawingContext drawingContext,
								   Shape shape,
								   Objects.Abstract obj)
		{
			//	Dessine une surface dans un fichier PDF.
			if ( shape.PropertySurface == null )  return;
			this.PDFSetSurface(port, drawingContext, obj, shape.PropertySurface);
			port.PaintSurface(shape.Path);
		}

		
		protected void DrawStroke(PDF.Port port,
								  DrawingContext drawingContext,
								  Shape shape,
								  Objects.Abstract obj)
		{
			//	Dessine un chemin dans un fichier PDF.
			if ( shape.PropertySurface == null || shape.PropertyStroke == null )  return;

			Properties.Gradient gradient = shape.PropertySurface as Properties.Gradient;
			PDF.Type type = gradient.TypeComplexSurfacePDF(port);
			bool isSmooth = gradient.IsSmoothSurfacePDF(port);

			if ( type == PDF.Type.OpaqueGradient       ||
				 type == PDF.Type.TransparencyGradient ||
				 type == PDF.Type.OpaquePattern        ||
				 type == PDF.Type.TransparencyPattern  ||
				 isSmooth                              )
			{
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

					using ( Path temp = dp.GenerateDashedPath() )
					{
						Path pathSurface = new Path();
						pathSurface.Append(temp, stroke.Width, stroke.Cap, stroke.EffectiveJoin, stroke.Limit, drawingContext.ScaleX);

						this.PDFSetSurface(port, drawingContext, obj, shape.PropertySurface);
						port.PaintSurface(pathSurface);
					}
				}
				else
				{
					Path pathSurface = new Path();
					pathSurface.Append(shape.Path, stroke.Width, stroke.Cap, stroke.EffectiveJoin, stroke.Limit, drawingContext.ScaleX);

					this.PDFSetSurface(port, drawingContext, obj, shape.PropertySurface);
					port.PaintSurface(pathSurface);
				}
			}
			else
			{
				this.PDFSetSurface(port, drawingContext, obj, shape.PropertySurface);
				this.PDFSetStroke(port, drawingContext, obj, shape.PropertyStroke);
				port.PaintOutline(shape.Path);
			}
		}

		protected void PDFSetSurface(PDF.Port port,
									 DrawingContext drawingContext,
									 Objects.Abstract obj,
									 Properties.Abstract surface)
		{
			//	Initialise le port en fonction de la surface.
			PDF.Type type = surface.TypeComplexSurfacePDF(port);
			bool isSmooth = surface.IsSmoothSurfacePDF(port);

			RichColor color = RichColor.Empty;
			if ( surface is Properties.Gradient )
			{
				Properties.Gradient gradient = surface as Properties.Gradient;
				color = port.GetFinalColor(gradient.Color1);
			}
			if ( surface is Properties.Font )
			{
				Properties.Font font = surface as Properties.Font;
				color = port.GetFinalColor(font.FontColor);
			}

			if ( type == PDF.Type.None && !isSmooth )
			{
				port.RichColor = color;
			}
			else
			{
				int id = port.SearchComplexSurface(obj, surface);
				port.SetColoredComplexSurface(color, id);
			}
		}

		protected void PDFSetStroke(PDF.Port port,
									DrawingContext drawingContext,
									Objects.Abstract obj,
									Properties.Line stroke)
		{
			//	Initialise le port en fonction du trait.
			if ( stroke.Dash )  // traitillé ?
			{
				port.SetLineDash(stroke.Width, stroke.GetDashPen(0), stroke.GetDashGap(0), stroke.GetDashPen(1), stroke.GetDashGap(1), stroke.GetDashPen(2), stroke.GetDashGap(2));
			}
			else	// trait continu ?
			{
				port.LineWidth = stroke.Width;
			}

			port.LineCap = stroke.Cap;
			port.LineJoin = stroke.EffectiveJoin;
			port.LineMiterLimit = stroke.Limit;
		}
		#endregion


		protected Document				document;
	}
}
