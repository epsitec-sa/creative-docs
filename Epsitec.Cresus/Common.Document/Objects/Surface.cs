using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe Surface est la classe de l'objet graphique surface 2d.
	/// </summary>
	[System.Serializable()]
	public class Surface : Objects.Abstract
	{
		public Surface(Document document, Objects.Abstract model) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);
		}

		protected override bool ExistingProperty(Properties.Type type)
		{
			if ( type == Properties.Type.Name )  return true;
			if ( type == Properties.Type.LineMode )  return true;
			if ( type == Properties.Type.LineColor )  return true;
			if ( type == Properties.Type.FillGradient )  return true;
			if ( type == Properties.Type.Surface )  return true;
			return false;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return new Surface(document, model);
		}

		public override void Dispose()
		{
			base.Dispose();
		}


		// Nom de l'ic�ne.
		public override string IconName
		{
			get { return "manifest:Epsitec.App.DocumentEditor.Images.Surface.icon"; }
		}


		// D�tecte si la souris est sur l'objet.
		public override bool Detect(Point pos)
		{
			if ( this.isHide )  return false;

			Drawing.Rectangle bbox = this.BoundingBox;
			if ( !bbox.Contains(pos) )  return false;

			Path surface, outline;
			this.PathBuild(null, out surface, out outline);

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			double width = System.Math.Max(this.PropertyLineMode.Width/2, context.MinimalWidth);
			if ( Geometry.DetectOutline(outline, width, pos) )  return true;
			
			if ( this.PropertyFillGradient.IsVisible() )
			{
				if ( Geometry.DetectSurface(surface, pos) )  return true;
			}
			
			return false;
		}


		// D�place une poign�e.
		public override void MoveHandleProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			if ( rank >= 4 )  // poign�e d'une propri�t� ?
			{
				base.MoveHandleProcess(rank, pos, drawingContext);
				return;
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.ConstrainSnapPos(ref pos);
			drawingContext.SnapGrid(ref pos);

				 if ( rank == 0 )  this.MoveCorner(pos, 0, 2,3, 1);
			else if ( rank == 1 )  this.MoveCorner(pos, 1, 3,2, 0);
			else if ( rank == 2 )  this.MoveCorner(pos, 2, 0,1, 3);
			else if ( rank == 3 )  this.MoveCorner(pos, 3, 1,0, 2);
			else                   this.Handle(rank).Position = pos;

			this.HandlePropertiesUpdate();
			this.dirtyBbox = true;
			this.TextInfoModifRect();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// D�but de la cr�ation d'un objet.
		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			drawingContext.ConstrainFixStarting(pos);
			drawingContext.ConstrainFixType(ConstrainType.Square);
			this.HandleAdd(pos, HandleType.Primary);  // rang = 0
			this.HandleAdd(pos, HandleType.Primary);  // rang = 1
			this.isCreating = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// D�placement pendant la cr�ation d'un objet.
		public override void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapGrid(ref pos);
			drawingContext.ConstrainSnapPos(ref pos);
			this.Handle(1).Position = pos;
			this.dirtyBbox = true;
			this.TextInfoModifRect();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Fin de la cr�ation d'un objet.
		public override void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);

			drawingContext.SnapGrid(ref pos);
			drawingContext.ConstrainSnapPos(ref pos);
			this.Handle(1).Position = pos;
			drawingContext.ConstrainDelStarting();
			this.isCreating = false;
			this.document.Modifier.TextInfoModif = "";

			// Cr�e les 2 autres poign�es dans les coins oppos�s.
			Point p1 = this.Handle(0).Position;
			Point p2 = this.Handle(1).Position;
			this.HandleAdd(new Point(p1.X, p2.Y), HandleType.Primary);  // rang = 2
			this.HandleAdd(new Point(p2.X, p1.Y), HandleType.Primary);  // rang = 3

			this.HandlePropertiesCreate();
			this.HandlePropertiesUpdate();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit �tre d�truit.
		public override bool CreateIsExist(DrawingContext drawingContext)
		{
			double len = Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > drawingContext.MinimalSize );
		}

		
		// Met � jour le rectangle englobant l'objet.
		protected override void UpdateBoundingBox()
		{
			if ( this.handles.Count < 2 )  return;

			Path surface, outline;
			this.PathBuild(null, out surface, out outline);

			Path[] paths = new Path[2];
			paths[0] = outline;
			paths[1] = surface;

			bool[] lineModes = new bool[2];
			lineModes[0] = true;
			lineModes[1] = false;

			bool[] lineColors = new bool[2];
			lineColors[0] = true;
			lineColors[1] = false;

			bool[] fillGradients = new bool[2];
			fillGradients[0] = false;
			fillGradients[1] = true;

			this.ComputeBoundingBox(paths, lineModes, lineColors, fillGradients);

			if ( this.TotalHandle >= 4 )
			{
				this.InflateBoundingBox(this.Handle(0).Position, false);
				this.InflateBoundingBox(this.Handle(1).Position, false);
				this.InflateBoundingBox(this.Handle(2).Position, false);
				this.InflateBoundingBox(this.Handle(3).Position, false);
			}
		}

		// Cr�e les chemins de l'objet.
		protected void PathBuild(DrawingContext drawingContext, out Path surface, out Path outline)
		{
			Point p1 = this.Handle(0).Position;
			Point p2 = new Point();
			Point p3 = this.Handle(1).Position;
			Point p4 = new Point();

			if ( this.handles.Count < 4 )
			{
				p2.X = p1.X;
				p2.Y = p3.Y;
				p4.X = p3.X;
				p4.Y = p1.Y;
			}
			else
			{
				p2 = this.Handle(2).Position;
				p4 = this.Handle(3).Position;
			}

			this.PathSurface(drawingContext, p1, p2, p3, p4, out surface, out outline);
		}

		// Cr�e le chemin d'une surface quelconque.
		protected void PathSurface(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4, out Path surface, out Path outline)
		{
			surface = null;
			outline = null;

			switch ( this.PropertySurface.SurfaceType )
			{
				case Properties.SurfaceType.ParallelT:
				case Properties.SurfaceType.ParallelB:
				case Properties.SurfaceType.ParallelL:
				case Properties.SurfaceType.ParallelR:
				case Properties.SurfaceType.QuadriL:
				case Properties.SurfaceType.QuadriP:
				case Properties.SurfaceType.QuadriC:
				case Properties.SurfaceType.QuadriX:
					surface = this.PathQuadri(drawingContext, p1, p2, p3, p4);
					outline = surface;
					break;

				case Properties.SurfaceType.TrapezeT:
					surface = this.PathTrapezeT(drawingContext, p1, p2, p3, p4);
					outline = surface;
					break;
				case Properties.SurfaceType.TrapezeB:
					surface = this.PathTrapezeB(drawingContext, p1, p2, p3, p4);
					outline = surface;
					break;
				case Properties.SurfaceType.TrapezeL:
					surface = this.PathTrapezeL(drawingContext, p1, p2, p3, p4);
					outline = surface;
					break;
				case Properties.SurfaceType.TrapezeR:
					surface = this.PathTrapezeR(drawingContext, p1, p2, p3, p4);
					outline = surface;
					break;

				case Properties.SurfaceType.Grid:
					surface = this.PathRectangle(drawingContext, p1, p2, p3, p4);
					outline = this.PathGrid(drawingContext, p1, p2, p3, p4);
					break;
				case Properties.SurfaceType.Pattern:
					surface = this.PathPattern(drawingContext, p1, p2, p3, p4);
					outline = this.PathGrid(drawingContext, p1, p2, p3, p4);
					break;

				case Properties.SurfaceType.Ring:
					surface = this.PathRing(drawingContext, p1, p2, p3, p4);
					outline = surface;
					break;
				case Properties.SurfaceType.SpiralCW:
					surface = this.PathSpiral(drawingContext, p1, p2, p3, p4);
					outline = surface;
					break;
				case Properties.SurfaceType.SpiralCCW:
					surface = this.PathSpiral(drawingContext, p2, p1, p4, p3);
					outline = surface;
					break;
			}
		}

		// Cr�e le chemin d'un quadrilat�re.
		protected Path PathQuadri(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
			Properties.Surface pf = this.PropertySurface;
			Point p12 = Point.Scale(p1,p2, pf.GetFactor(0));
			Point p23 = Point.Scale(p2,p3, pf.GetFactor(1));
			Point p34 = Point.Scale(p3,p4, pf.GetFactor(2));
			Point p41 = Point.Scale(p4,p1, pf.GetFactor(3));

			Path path = new Path();
			path.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);
			path.MoveTo(p12);
			path.LineTo(p23);
			path.LineTo(p34);
			path.LineTo(p41);
			path.Close();
			return path;
		}

		// Cr�e le chemin d'un trap�ze haut.
		protected Path PathTrapezeT(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
			Properties.Surface pf = this.PropertySurface;
			Point p23 = Point.Scale(p2,p3, pf.GetFactor(1));
			Point p32 = Point.Scale(p3,p2, pf.GetFactor(1));

			Path path = new Path();
			path.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);
			path.MoveTo(p1);
			path.LineTo(p23);
			path.LineTo(p32);
			path.LineTo(p4);
			path.Close();
			return path;
		}

		// Cr�e le chemin d'un trap�ze bas.
		protected Path PathTrapezeB(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
			Properties.Surface pf = this.PropertySurface;
			Point p14 = Point.Scale(p1,p4, pf.GetFactor(3));
			Point p41 = Point.Scale(p4,p1, pf.GetFactor(3));

			Path path = new Path();
			path.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);
			path.MoveTo(p14);
			path.LineTo(p2);
			path.LineTo(p3);
			path.LineTo(p41);
			path.Close();
			return path;
		}

		// Cr�e le chemin d'un trap�ze gauche.
		protected Path PathTrapezeL(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
			Properties.Surface pf = this.PropertySurface;
			Point p12 = Point.Scale(p1,p2, pf.GetFactor(0));
			Point p21 = Point.Scale(p2,p1, pf.GetFactor(0));

			Path path = new Path();
			path.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);
			path.MoveTo(p12);
			path.LineTo(p21);
			path.LineTo(p3);
			path.LineTo(p4);
			path.Close();
			return path;
		}

		// Cr�e le chemin d'un trap�ze droite.
		protected Path PathTrapezeR(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
			Properties.Surface pf = this.PropertySurface;
			Point p34 = Point.Scale(p3,p4, pf.GetFactor(2));
			Point p43 = Point.Scale(p4,p3, pf.GetFactor(2));

			Path path = new Path();
			path.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);
			path.MoveTo(p1);
			path.LineTo(p2);
			path.LineTo(p34);
			path.LineTo(p43);
			path.Close();
			return path;
		}

		// Cr�e le chemin d'une grille.
		protected Path PathGrid(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
			Properties.Surface pf = this.PropertySurface;
			int nx = pf.GetScalar(0);
			int ny = pf.GetScalar(1);

			Path path = new Path();
			path.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);

			double sx = 1.0/nx;
			for ( int i=0 ; i<=nx ; i++ )
			{
				double x = this.Expo(sx*i, pf.GetFactor(2));
				path.MoveTo(Point.Scale(p1,p4, x));
				path.LineTo(Point.Scale(p2,p3, x));
			}

			double sy = 1.0/ny;
			for ( int i=0 ; i<=ny ; i++ )
			{
				double y = this.Expo(sy*i, pf.GetFactor(3));
				path.MoveTo(Point.Scale(p1,p2, y));
				path.LineTo(Point.Scale(p4,p3, y));
			}

			return path;
		}

		// Cr�e le chemin d'un rectangle.
		protected Path PathRectangle(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
			Path path = new Path();
			path.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);

			path.MoveTo(p1);
			path.LineTo(p2);
			path.LineTo(p3);
			path.LineTo(p4);
			path.Close();

			return path;
		}

		// Cr�e le chemin d'un damier.
		protected Path PathPattern(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
			Properties.Surface pf = this.PropertySurface;
			int nx = pf.GetScalar(0);
			int ny = pf.GetScalar(1);

			Path path = new Path();
			path.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);

			double sx = 1.0/nx;
			double sy = 1.0/ny;
			for ( int y=0 ; y<ny ; y++ )
			{
				for ( int x=0 ; x<nx ; x++ )
				{
					if ( x%2 != y%2 )  continue;
					double x0 = this.Expo(sx*(x+0), pf.GetFactor(2));
					double x1 = this.Expo(sx*(x+1), pf.GetFactor(2));
					double y0 = this.Expo(sy*(y+0), pf.GetFactor(3));
					double y1 = this.Expo(sy*(y+1), pf.GetFactor(3));
					path.MoveTo(this.PointXY(p1,p2,p3,p4, x0,y0));
					path.LineTo(this.PointXY(p1,p2,p3,p4, x0,y1));
					path.LineTo(this.PointXY(p1,p2,p3,p4, x1,y1));
					path.LineTo(this.PointXY(p1,p2,p3,p4, x1,y0));
					path.Close();
				}
			}

			return path;
		}

		protected Point PointXY(Point p1, Point p2, Point p3, Point p4, double sx, double sy)
		{
			Point p14 = Point.Scale(p1,p4, sx);
			Point p23 = Point.Scale(p2,p3, sx);
			return Point.Scale(p14,p23, sy);
		}

		// Cr�e le chemin d'un anneau.
		protected Path PathRing(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
			Properties.Surface pf = this.PropertySurface;
			double r = pf.GetFactor(0);

			Point pp1 = p1;
			Point pp2 = p2;
			Point pp3 = p3;
			Point pp4 = p4;
			this.ScaleQuadri(r, ref pp1, ref pp2, ref pp3, ref pp4);

			Path path = new Path();
			path.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);
			this.PathEllipse(path, p1, p2, p3, p4);
			this.PathEllipse(path, pp4, pp3, pp2, pp1);
			return path;
		}

		// Cr�e le chemin d'une spirale.
		protected Path PathSpiral(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
			Properties.Surface pf = this.PropertySurface;
			int n = pf.GetScalar(0);

			Point p12 = Point.Scale(p1,p2, 0.5);
			Point p23 = Point.Scale(p2,p3, 0.5);
			Point p34 = Point.Scale(p3,p4, 0.5);
			Point p41 = Point.Scale(p4,p1, 0.5);
			Point center = (p1+p2+p3+p4)/4.0;

			Path path = new Path();
			path.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);

			Point current = p12;
			path.MoveTo(p12);

			double step = 1.0/(n*4+1);
			double r = 0.0;
			for ( int i=0 ; i<n ; i++ )
			{
				this.PathArcSpiral(path, center, p12, p2, p23, r, r+step);  r += step;
				this.PathArcSpiral(path, center, p23, p3, p34, r, r+step);  r += step;
				this.PathArcSpiral(path, center, p34, p4, p41, r, r+step);  r += step;
				this.PathArcSpiral(path, center, p41, p1, p12, r, r+step);  r += step;
			}

			return path;
		}

		// D�graisse un quadrilat�re quelconque.
		protected void ScaleQuadri(double r, ref Point p1, ref Point p2, ref Point p3, ref Point p4)
		{
			Point p12 = Point.Scale(p1,p2, r);
			Point p21 = Point.Scale(p2,p1, r);
			Point p34 = Point.Scale(p3,p4, r);
			Point p43 = Point.Scale(p4,p3, r);

			p1 = Point.Scale(p12,p43, r);
			p2 = Point.Scale(p21,p34, r);
			p3 = Point.Scale(p34,p21, r);
			p4 = Point.Scale(p43,p12, r);
		}

		// Ajoute le chemin d'une ellipse.
		protected void PathEllipse(Path path, Point p1, Point p2, Point p3, Point p4)
		{
			Point p12 = Point.Scale(p1,p2, 0.5);
			Point p23 = Point.Scale(p2,p3, 0.5);
			Point p34 = Point.Scale(p3,p4, 0.5);
			Point p41 = Point.Scale(p4,p1, 0.5);

			path.MoveTo(p12);
			path.ArcTo(p2, p23);
			path.ArcTo(p3, p34);
			path.ArcTo(p4, p41);
			path.ArcTo(p1, p12);
			path.Close();
		}

		// Ajoute le chemin d'un quart d'arc en spirale.
		protected void PathArcSpiral(Path path, Point center, Point current, Point corner, Point next, double r1, double r2)
		{
			double exp = this.PropertySurface.GetFactor(2);
			r1 = this.Expo(r1, exp);
			r2 = this.Expo(r2, exp);
			Point a = Point.Scale(current, center, r1);
			Point d = Point.Scale(corner, next, r1);
			Point b = Point.Scale(d, a, r2);
			Point c = Point.Scale(next, center, r2);
			path.ArcTo(b, c);
		}

		protected double Expo(double value, double factor)
		{
			if ( factor > 0.5 )
			{
				value = System.Math.Pow(value, factor*2.0);
			}

			if ( factor < 0.5 )
			{
				value = 1.0-System.Math.Pow(1.0-value, 2.0-factor*2.0);
			}

			return value;
		}


		// Dessine l'objet.
		public override void DrawGeometry(Graphics graphics, DrawingContext drawingContext)
		{
			base.DrawGeometry(graphics, drawingContext);

			if ( this.TotalHandle < 2 )  return;

			Path surface, outline;
			this.PathBuild(null, out surface, out outline);

			this.PropertyFillGradient.RenderSurface(graphics, drawingContext, surface, this.BoundingBoxThin);
			this.PropertyLineMode.DrawPath(graphics, drawingContext, outline, this.PropertyLineColor, this.BoundingBoxGeom);

			if ( this.IsHilite && drawingContext.IsActive )
			{
				if ( this.PropertyFillGradient.IsVisible() )
				{
					graphics.Rasterizer.AddSurface(surface);
					graphics.RenderSolid(drawingContext.HiliteSurfaceColor);
				}

				this.PropertyLineMode.AddOutline(graphics, outline, drawingContext.HiliteSize);
				graphics.RenderSolid(drawingContext.HiliteOutlineColor);
			}

			if ( this.IsDrawDash(drawingContext) )
			{
				this.PropertyLineMode.DrawPathDash(graphics, drawingContext, outline, this.PropertyLineColor);
			}
		}

		// Imprime l'objet.
		public override void PrintGeometry(Printing.PrintPort port, DrawingContext drawingContext)
		{
			base.PrintGeometry(port, drawingContext);

			if ( this.TotalHandle < 2 )  return;

			Path surface, outline;
			this.PathBuild(null, out surface, out outline);

			if ( this.PropertyFillGradient.PaintColor(port, drawingContext) )
			{
				port.PaintSurface(surface);
			}

			if ( this.PropertyLineColor.PaintColor(port, drawingContext) )
			{
				this.PropertyLineMode.PaintOutline(port, drawingContext, outline);
			}
		}


		// Retourne le chemin g�om�trique de l'objet.
		public override Path GetPath(int rank)
		{
			if ( rank > 0 )  return null;
			Path surface, outline;
			this.PathBuild(null, out surface, out outline);
			return outline;
		}


		#region Serialization
		// S�rialise l'objet.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		// Constructeur qui d�s�rialise l'objet.
		protected Surface(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		#endregion
	}
}
