using System.Collections.Generic;
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
			if ( type == Properties.Type.Frame )  return true;
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


		public override string IconUri
		{
			//	Nom de l'icône.
			get { return Misc.Icon("ObjectSurface"); }
		}


		public override void MoveHandleStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Début du déplacement d'une poignée.
			base.MoveHandleStarting(rank, pos, drawingContext);

			if ( rank < this.handles.Count )  // poignée de l'objet ?
			{
				drawingContext.ConstrainClear();

				Handle handle = this.Handle(rank);
				if ( handle.PropertyType == Properties.Type.None )
				{
						 if ( rank == 0 )  drawingContext.ConstrainAddRect(this.Handle(0).Position, this.Handle(1).Position, this.Handle(2).Position, this.Handle(3).Position, false, -1);
					else if ( rank == 1 )  drawingContext.ConstrainAddRect(this.Handle(1).Position, this.Handle(0).Position, this.Handle(3).Position, this.Handle(2).Position, false, -1);
					else if ( rank == 2 )  drawingContext.ConstrainAddRect(this.Handle(2).Position, this.Handle(3).Position, this.Handle(0).Position, this.Handle(1).Position, false, -1);
					else if ( rank == 3 )  drawingContext.ConstrainAddRect(this.Handle(3).Position, this.Handle(2).Position, this.Handle(1).Position, this.Handle(0).Position, false, -1);
				}
				else
				{
					Properties.Abstract property = this.Property(handle.PropertyType);
					property.MoveHandleStarting(this, handle.PropertyRank, pos, drawingContext);
				}

				drawingContext.MagnetClearStarting();
			}
		}

		public override void MoveHandleProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Déplace une poignée.
			if ( rank >= 4 )  // poignée d'une propriété ?
			{
				base.MoveHandleProcess(rank, pos, drawingContext);
				return;
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);

				 if ( rank == 0 )  this.MoveCorner(pos, 0, 2,3, 1);
			else if ( rank == 1 )  this.MoveCorner(pos, 1, 3,2, 0);
			else if ( rank == 2 )  this.MoveCorner(pos, 2, 0,1, 3);
			else if ( rank == 3 )  this.MoveCorner(pos, 3, 1,0, 2);
			else                   this.Handle(rank).Position = pos;

			this.HandlePropertiesUpdate();
			this.SetDirtyBbox();
			this.TextInfoModifRect();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			//	Début de la création d'un objet.
			drawingContext.ConstrainClear();
			drawingContext.ConstrainAddHomo(pos, false, -1);
			this.HandleAdd(pos, HandleType.Primary);  // rang = 0
			this.HandleAdd(pos, HandleType.Primary);  // rang = 1
			this.isCreating = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			//	Déplacement pendant la création d'un objet.
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);
			this.Handle(1).Position = pos;
			this.SetDirtyBbox();
			this.TextInfoModifRect();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			//	Fin de la création d'un objet.
			this.document.Notifier.NotifyArea(this.BoundingBox);

			drawingContext.SnapPos(ref pos);
			this.Handle(1).Position = pos;
			drawingContext.ConstrainDelStarting();
			drawingContext.MagnetClearStarting();
			this.isCreating = false;
			this.document.Modifier.TextInfoModif = "";

			//	Crée les 2 autres poignées dans les coins opposés.
			Point p1 = this.Handle(0).Position;
			Point p2 = this.Handle(1).Position;
			this.HandleAdd(new Point(p1.X, p2.Y), HandleType.Primary);  // rang = 2
			this.HandleAdd(new Point(p2.X, p1.Y), HandleType.Primary);  // rang = 3

			this.HandlePropertiesCreate();
			this.HandlePropertiesUpdate();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override bool CreateIsExist(DrawingContext drawingContext)
		{
			//	Indique si l'objet doit exister. Retourne false si l'objet ne peut
			//	pas exister et doit être détruit.
			double len = Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > drawingContext.MinimalSize );
		}

		public override void Reset()
		{
			//	Remet l'objet droit et d'équerre.
			if (this.handles.Count >= 4)
			{
				Drawing.Rectangle box = this.BoundingBoxThin;
				this.Handle(0).Position = box.BottomLeft;
				this.Handle(1).Position = box.TopRight;
				this.Handle(2).Position = box.TopLeft;
				this.Handle(3).Position = box.BottomRight;
			}
		}

		
		public override Shape[] ShapesBuild(IPaintPort port, DrawingContext drawingContext, bool simplify)
		{
			//	Constuit les formes de l'objet.
			var frame = this.PropertyFrame;

			Path surface, outline, box;
			this.PathBuild (null, out surface, out outline, out box);

			var shapes = new List<Shape> ();
			var objectShapes = new List<Shape> ();

			Properties.SurfaceType type = this.PropertySurface.SurfaceType;

			//	Forme de la surface.
			{
				var shape = new Shape ();
				shape.Path = surface;
				shape.SetPropertySurface (port, this.PropertyFillGradient);

				if (type == Properties.SurfaceType.SpiralCCW ||
					type == Properties.SurfaceType.SpiralCW)
				{
					shape.FillMode = FillMode.NonZero;
				}

				objectShapes.Add (shape);
			}

			//	Forme du chemin.
			{
				var shape = new Shape ();
				shape.Path = outline;
				shape.SetPropertyStroke (port, this.PropertyLineMode, this.PropertyLineColor);
				objectShapes.Add (shape);
			}

			if (!simplify && (frame == null || frame.FrameType == Properties.FrameType.None))  // pas de cadre ?
			{
				shapes.AddRange (objectShapes);
			}
			else  // cadre ?
			{
				frame.AddShapes (shapes, objectShapes, port, drawingContext, Geometry.PathToPoints (box), this.PropertyCorner);
			}

			return shapes.ToArray ();
		}

		protected void PathBuild(DrawingContext drawingContext, out Path surface, out Path outline, out Path box)
		{
			//	Crée les chemins de l'objet.
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

			this.PathSurface(drawingContext, p1, p2, p3, p4, out surface, out outline, out box);
		}

		protected void PathSurface(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4, out Path surface, out Path outline, out Path box)
		{
			//	Crée le chemin d'une surface quelconque.
			surface = null;
			outline = null;
			box     = null;

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
					box     = surface;
					break;

				case Properties.SurfaceType.TrapezeT:
					surface = this.PathTrapezeT(drawingContext, p1, p2, p3, p4);
					outline = surface;
					box     = surface;
					break;
				case Properties.SurfaceType.TrapezeB:
					surface = this.PathTrapezeB(drawingContext, p1, p2, p3, p4);
					outline = surface;
					box     = surface;
					break;
				case Properties.SurfaceType.TrapezeL:
					surface = this.PathTrapezeL(drawingContext, p1, p2, p3, p4);
					outline = surface;
					box     = surface;
					break;
				case Properties.SurfaceType.TrapezeR:
					surface = this.PathTrapezeR(drawingContext, p1, p2, p3, p4);
					outline = surface;
					box     = surface;
					break;

				case Properties.SurfaceType.Grid:
					surface = this.PathRectangle(drawingContext, p1, p2, p3, p4);
					outline = this.PathGrid(drawingContext, p1, p2, p3, p4);
					box     = surface;
					break;
				case Properties.SurfaceType.Pattern:
					surface = this.PathPattern(drawingContext, p1, p2, p3, p4);
					outline = this.PathGrid(drawingContext, p1, p2, p3, p4);
					box     = this.PathRectangle (drawingContext, p1, p2, p3, p4);
					break;

				case Properties.SurfaceType.Ring:
					surface = this.PathRing(drawingContext, p1, p2, p3, p4);
					outline = surface;
					box     = surface;
					break;
				case Properties.SurfaceType.SpiralCW:
					surface = this.PathSpiral(drawingContext, p1, p2, p3, p4);
					outline = surface;
					box     = surface;
					break;
				case Properties.SurfaceType.SpiralCCW:
					surface = this.PathSpiral(drawingContext, p2, p1, p4, p3);
					outline = surface;
					box     = surface;
					break;
			}
		}

		protected Path PathQuadri(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
			//	Crée le chemin d'un quadrilatère.
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

		protected Path PathTrapezeT(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
			//	Crée le chemin d'un trapèze haut.
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

		protected Path PathTrapezeB(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
			//	Crée le chemin d'un trapèze bas.
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

		protected Path PathTrapezeL(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
			//	Crée le chemin d'un trapèze gauche.
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

		protected Path PathTrapezeR(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
			//	Crée le chemin d'un trapèze droite.
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

		protected Path PathGrid(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
			//	Crée le chemin d'une grille.
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

		protected Path PathRectangle(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
			//	Crée le chemin d'un rectangle.
			Path path = new Path();
			path.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);

			path.MoveTo(p1);
			path.LineTo(p2);
			path.LineTo(p3);
			path.LineTo(p4);
			path.Close();

			return path;
		}

		protected Path PathPattern(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
			//	Crée le chemin d'un damier.
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

		protected Path PathRing(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
			//	Crée le chemin d'un anneau.
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

		protected Path PathSpiral(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
			//	Crée le chemin d'une spirale.
#if false  // système utilisé dans les version <= 1.0.19
			Properties.Surface pf = this.PropertySurface;
			int n = pf.GetScalar(0);
			double step = 1.0/(n*4+1);

			Point p12 = Point.Scale(p1,p2, 0.5);
			Point p23 = Point.Scale(p2,p3, 0.5);
			Point p34 = Point.Scale(p3,p4, 0.5);
			Point p41 = Point.Scale(p4,p1, 0.5);
			Point center = (p1+p2+p3+p4)/4.0;

			Path path = new Path();
			path.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);

			path.MoveTo(p12);

			double r = 0.0;
			for ( int i=0 ; i<n ; i++ )
			{
				this.PathArcSpiral(path, center, p12, p2, p23, r, r+step);  r += step;
				this.PathArcSpiral(path, center, p23, p3, p34, r, r+step);  r += step;
				this.PathArcSpiral(path, center, p34, p4, p41, r, r+step);  r += step;
				this.PathArcSpiral(path, center, p41, p1, p12, r, r+step);  r += step;
			}

			return path;
#endif
#if false  // spirales d'Archimèdes de référence (avec des droites)
			Properties.Surface pf = this.PropertySurface;
			int n = pf.GetScalar(0);
			double exp = pf.GetFactor(2);

			Path path = new Path();
			path.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);

			path.MoveTo(this.PlacePointToQuadri(new Point(0.0, 1.0), p1, p2, p3, p4));

			for ( double angle=5.0 ; angle<=360.0*n ; angle+=5.0 )
			{
				Point p = Transform.RotatePointDeg(-angle, new Point(0.0, 1.0));
				double d = angle/(360.0*n);
				d = this.Expo(d, exp);
				p = Point.Move(p, new Point(0.0, 0.0), d);
				p = this.PlacePointToQuadri(p, p1, p2, p3, p4);
				path.LineTo(p);
			}

			return path;
#endif
#if true  // nouveau système qui approxime les spirales d'Archimèdes
			Properties.Surface pf = this.PropertySurface;
			int n = pf.GetScalar(0);
			double exp = pf.GetFactor(2);
			double impact = pf.GetFactor(3);

			Path path = new Path();
			path.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);

			path.MoveTo(this.PlacePointToQuadri(new Point(0.0, 1.0), p1, p2, p3, p4));

			double angle = 0.0;
			do
			{
				angle += 45.0;
				Point a = Transform.RotatePointDeg(-angle, new Point(0.0, 1.0));
				double r = angle/(360.0*n);
				r = this.Expo(r, exp);
				r = (1.0-r*impact)*1.4142 + 1.0/(40.0*n);  // 40.0 = empyrique !
				a = Point.Move(new Point(0.0, 0.0), a, r);
				a = this.PlacePointToQuadri(a, p1, p2, p3, p4);

				angle += 45.0;
				Point b = Transform.RotatePointDeg(-angle, new Point(0.0, 1.0));
				r = angle/(360.0*n);
				r = this.Expo(r, exp);
				r = 1.0-r*impact;
				b = Point.Move(new Point(0.0, 0.0), b, r);
				b = this.PlacePointToQuadri(b, p1, p2, p3, p4);

				path.ArcTo(a, b);
			}
			while ( angle < 360.0*n );

			return path;
#endif
		}

		protected void ScaleQuadri(double r, ref Point p1, ref Point p2, ref Point p3, ref Point p4)
		{
			//	Dégraisse un quadrilatère quelconque.
			Point p12 = Point.Scale(p1,p2, r);
			Point p21 = Point.Scale(p2,p1, r);
			Point p34 = Point.Scale(p3,p4, r);
			Point p43 = Point.Scale(p4,p3, r);

			p1 = Point.Scale(p12,p43, r);
			p2 = Point.Scale(p21,p34, r);
			p3 = Point.Scale(p34,p21, r);
			p4 = Point.Scale(p43,p12, r);
		}

		protected void PathEllipse(Path path, Point p1, Point p2, Point p3, Point p4)
		{
			//	Ajoute le chemin d'une ellipse.
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

		protected void PathArcSpiral(Path path, Point center, Point current, Point corner, Point next, double r1, double r2)
		{
			//	Ajoute le chemin d'un quart d'arc en spirale.
			double exp = this.PropertySurface.GetFactor(2);
			r1 = this.Expo(r1, exp);
			r2 = this.Expo(r2, exp);
			Point a = Point.Scale(current, center, r1);
			Point d = Point.Scale(corner, next, r1);
			Point b = Point.Scale(d, a, r2);
			Point c = Point.Scale(next, center, r2);
			path.ArcTo(b, c);
		}

		protected Point PlacePointToQuadri(Point p, Point p1, Point p2, Point p3, Point p4)
		{
			//	Place un point du système de coordonnées -1..1 dans le système défini
			//	par le quadrilatère p1;p2;p3;p4.
			p.X = p.X*0.5 + 0.5;
			p.Y = p.Y*0.5 + 0.5;  // 0..1

			Point p12 = Point.Scale(p1, p2, p.X);
			Point p43 = Point.Scale(p4, p3, p.X);
			return Point.Scale(p43, p12, p.Y);
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


		protected override Path GetPath()
		{
			//	Retourne le chemin géométrique de l'objet.
			Path surface, outline, box;
			this.PathBuild (null, out surface, out outline, out box);
			return outline;
		}


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise l'objet.
			base.GetObjectData(info, context);
		}

		protected Surface(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise l'objet.
		}
		#endregion
	}
}
