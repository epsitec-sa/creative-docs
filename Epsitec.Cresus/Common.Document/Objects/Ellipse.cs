using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe Ellipse est la classe de l'objet graphique "ellipse".
	/// </summary>
	[System.Serializable()]
	public class Ellipse : Objects.Abstract
	{
		public Ellipse(Document document, Objects.Abstract model) : base(document, model)
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
			if ( type == Properties.Type.Arc )  return true;
			return false;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return new Ellipse(document, model);
		}

		public override void Dispose()
		{
			base.Dispose();
		}


		// Nom de l'icône.
		public override string IconName
		{
			get { return @"file:images/ellipse.icon"; }
		}


		// Détecte si la souris est sur l'objet.
		public override bool Detect(Point pos)
		{
			if ( this.isHide )  return false;

			Drawing.Rectangle bbox = this.BoundingBox;
			if ( !bbox.Contains(pos) )  return false;

			Path path = this.PathBuild(null);

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			double width = System.Math.Max(this.PropertyLineMode.Width/2, context.MinimalWidth);
			if ( Geometry.DetectOutline(path, width, pos) )  return true;
			
			if ( this.PropertyFillGradient.IsVisible() )
			{
				path.Close();
				if ( Geometry.DetectSurface(path, pos) )  return true;
			}
			return false;
		}


		// Déplace une poignée.
		public override void MoveHandleProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			if ( rank >= 4 )  // poignée d'une propriété ?
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

			this.HandlePropertiesUpdatePosition();
			this.dirtyBbox = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		
		// Début de la création d'un objet.
		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			drawingContext.ConstrainFixStarting(pos);
			drawingContext.ConstrainFixType(ConstrainType.Square);
			this.HandleAdd(pos, HandleType.Primary);
			this.HandleAdd(pos, HandleType.Primary);
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Déplacement pendant la création d'un objet.
		public override void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapGrid(ref pos);
			drawingContext.ConstrainSnapPos(ref pos);
			this.Handle(1).Position = pos;
			this.dirtyBbox = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Fin de la création d'un objet.
		public override void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);

			drawingContext.SnapGrid(ref pos);
			drawingContext.ConstrainSnapPos(ref pos);
			this.Handle(1).Position = pos;
			drawingContext.ConstrainDelStarting();

			// Crée les 2 autres poignées dans les coins opposés.
			Point p1 = this.Handle(0).Position;
			Point p2 = this.Handle(1).Position;
			this.HandleAdd(new Point(p1.X, p2.Y), HandleType.Primary);  // rang = 2
			this.HandleAdd(new Point(p2.X, p1.Y), HandleType.Primary);  // rang = 3

			this.HandlePropertiesCreate();
			this.HandlePropertiesUpdatePosition();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateIsExist(DrawingContext drawingContext)
		{
			double len = Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > drawingContext.MinimalSize );
		}

	
		// Met à jour le rectangle englobant l'objet.
		protected override void UpdateBoundingBox()
		{
			if ( this.handles.Count < 2 )  return;

			Path path = this.PathBuild(null);
			
			Path[] paths = new Path[1];
			paths[0] = path;

			bool[] lineModes = new bool[1];
			lineModes[0] = true;

			bool[] lineColors = new bool[1];
			lineColors[0] = true;

			bool[] fillGradients = new bool[1];
			fillGradients[0] = true;

			this.ComputeBoundingBox(paths, lineModes, lineColors, fillGradients);

			if ( this.TotalHandle >= 4 )
			{
				this.InflateBoundingBox(this.Handle(0).Position, true);
				this.InflateBoundingBox(this.Handle(1).Position, true);
				this.InflateBoundingBox(this.Handle(2).Position, true);
				this.InflateBoundingBox(this.Handle(3).Position, true);
			}
		}

		// Calcule la géométrie de l'ellipse.
		protected void ComputeGeometry(out Point center, out double rx, out double ry, out double angle)
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

			center = new Point((p1.X+p2.X+p3.X+p4.X)/4.0, (p1.Y+p2.Y+p3.Y+p4.Y)/4.0);
			rx = (Point.Distance(p1,p4) + Point.Distance(p2,p3))/4.0;
			ry = (Point.Distance(p1,p2) + Point.Distance(p3,p4))/4.0;
			angle = Point.ComputeAngleDeg(p1,p4);
		}

		// Calcule la position d'une poignée pour l'arc.
		public Point ComputeArcHandle(double angle)
		{
			Point center, p;
			double rx, ry, rot;
			this.ComputeGeometry(out center, out rx, out ry, out rot);

			if ( rx == 0.0 || ry == 0.0 )
			{
				return center;
			}
			else
			{
				p = center;
				p.X += rx;
				p = Transform.RotatePointDeg(center, angle, p);
				p.Y = (p.Y-center.Y)*(ry/rx)+center.Y;
				return Transform.RotatePointDeg(center, rot, p);
			}
		}

		// Calcule l'angle d'après la position de la souris.
		public double ComputeArcHandle(Point pos)
		{
			Point center, p;
			double rx, ry, rot;
			this.ComputeGeometry(out center, out rx, out ry, out rot);

			if ( rx == 0.0 || ry == 0.0 )
			{
				return 0.0;
			}
			else
			{
				p = Transform.RotatePointDeg(center, -rot, pos);
				p.Y = (p.Y-center.Y)/(ry/rx)+center.Y;
				double angle = Point.ComputeAngleDeg(center, p);
				if ( angle < 0.0 )  angle += 360.0;  // 0..360
				return angle;
			}
		}

		// Crée le chemin d'une ellipse inscrite dans un quadrilatère.
		protected Path PathEllipse(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
#if true
			Point center;
			double rx, ry, rot;
			this.ComputeGeometry(out center, out rx, out ry, out rot);

			double zoom = Properties.Abstract.DefaultZoom(drawingContext);
			Path rightPath = new Path();
			rightPath.DefaultZoom = zoom;

			Properties.Arc arc = this.PropertyArc;
			double a1, a2;
			if ( arc.ArcType == Properties.ArcType.Full )
			{
				a1 =   0.0;
				a2 = 360.0;
			}
			else
			{
				a1 = arc.StartingAngle;
				a2 = arc.EndingAngle;
			}
			if ( a1 != a2 )
			{
				rightPath.ArcDeg(center, rx, ry, a1, a2, true);
			}

			if ( arc.ArcType == Properties.ArcType.Close )
			{
				rightPath.Close();
			}
			if ( arc.ArcType == Properties.ArcType.Pie )
			{
				rightPath.LineTo(center);
				rightPath.Close();
			}

			Path rotPath = new Path();
			rotPath.DefaultZoom = zoom;
			Transform rotate = new Transform();
			rotate.RotateDeg(rot, center);
			rotPath.Append(rightPath, rotate, zoom);
			rightPath.Dispose();
			return rotPath;
#else
			Point p12 = (p1+p2)/2;
			Point p23 = (p2+p3)/2;
			Point p34 = (p3+p4)/2;
			Point p41 = (p4+p1)/2;

			Path path = new Path();

			if ( drawingContext == null )
			{
				path.DefaultZoom = 10.0;
			}
			else
			{
				path.DefaultZoom = drawingContext.ScaleX;
			}

			path.MoveTo(p12);
			path.CurveTo(Point.Scale(p12, p2, 0.56), Point.Scale(p23, p2, 0.56), p23);
			path.CurveTo(Point.Scale(p23, p3, 0.56), Point.Scale(p34, p3, 0.56), p34);
			path.CurveTo(Point.Scale(p34, p4, 0.56), Point.Scale(p41, p4, 0.56), p41);
			path.CurveTo(Point.Scale(p41, p1, 0.56), Point.Scale(p12, p1, 0.56), p12);
			path.Close();
			return path;
#endif
		}

		// Crée le chemin de l'objet.
		protected Path PathBuild(DrawingContext drawingContext)
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

			return this.PathEllipse(drawingContext, p1, p2, p3, p4);
		}

		// Dessine l'objet.
		public override void DrawGeometry(Graphics graphics, DrawingContext drawingContext)
		{
			base.DrawGeometry(graphics, drawingContext);

			if ( this.TotalHandle < 2 )  return;

			Path path = this.PathBuild(drawingContext);
			this.PropertyFillGradient.RenderSurface(graphics, drawingContext, path, this.BoundingBoxThin);

			this.PropertyLineMode.DrawPath(graphics, drawingContext, path, this.PropertyLineColor, this.BoundingBoxGeom);

			if ( this.IsHilite && drawingContext.IsActive )
			{
				if ( this.PropertyFillGradient.IsVisible() )
				{
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(drawingContext.HiliteSurfaceColor);
				}

				this.PropertyLineMode.AddOutline(graphics, path, drawingContext.HiliteSize);
				graphics.RenderSolid(drawingContext.HiliteOutlineColor);
			}
		}

		// Imprime l'objet.
		public override void PrintGeometry(Printing.PrintPort port, DrawingContext drawingContext)
		{
			base.PrintGeometry(port, drawingContext);

			if ( this.TotalHandle < 2 )  return;

			Path path = this.PathBuild(drawingContext);

			if ( this.PropertyFillGradient.PaintColor(port, drawingContext) )
			{
				port.PaintSurface(path);
			}

			if ( this.PropertyLineColor.PaintColor(port, drawingContext) )
			{
				this.PropertyLineMode.PaintOutline(port, drawingContext, path);
			}
		}


		#region Serialization
		// Sérialise l'objet.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		// Constructeur qui désérialise l'objet.
		protected Ellipse(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		#endregion
	}
}
