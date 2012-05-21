using System.Collections.Generic;
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
			if ( type == Properties.Type.Frame )  return true;
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


		public override string IconUri
		{
			//	Nom de l'icône.
			get { return Misc.Icon("ObjectEllipse"); }
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
			this.HandleAdd(pos, HandleType.Primary);
			this.HandleAdd(pos, HandleType.Primary);
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

	
		public Point ComputeArcHandle(double angle)
		{
			//	Calcule la position d'une poignée pour l'arc.
			Stretcher stretcher = this.GetStretcher();
			Point pos = Transform.RotatePointDeg(angle, new Point(0.5, 0.0));
			return stretcher.Transform(pos);
		}

		public double ComputeArcHandle(Point pos)
		{
			//	Calcule l'angle d'après la position de la souris.
			Stretcher stretcher = this.GetStretcher();
			pos = stretcher.Reverse(pos);
			return Math.ClipAngleDeg(Point.ComputeAngleDeg(new Point(0.0, 0.0), pos));
		}

		protected Stretcher GetStretcher()
		{
			//	Donne le stretcher à utiliser pour l'ellipse.
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

			Stretcher stretcher = new Stretcher();
			stretcher.InitialRectangle = new Drawing.Rectangle(-0.5, 0.5, 1.0, -1.0);
			stretcher.FinalBottomLeft  = p1;
			stretcher.FinalTopLeft     = p2;
			stretcher.FinalTopRight    = p3;
			stretcher.FinalBottomRight = p4;

			return stretcher;
		}

		public override Shape[] ShapesBuild(IPaintPort port, DrawingContext drawingContext, bool simplify)
		{
			//	Constuit les formes de l'objet.
			var frame = this.PropertyFrame;

			Path path = this.PathBuild (drawingContext);
			var shapes = new List<Shape> ();
			var objectShapes = new List<Shape> ();

			//	Forme de la surface.
			{
				var shape = new Shape ();
				shape.Path = path;
				shape.SetPropertySurface (port, this.PropertyFillGradient);
				objectShapes.Add (shape);
			}

			//	Forme du chemin.
			{
				var shape = new Shape ();
				shape.Path = path;
				shape.SetPropertyStroke (port, this.PropertyLineMode, this.PropertyLineColor);
				objectShapes.Add (shape);
			}

			if (!simplify && (frame == null || frame.FrameType == Properties.FrameType.None))  // pas de cadre ?
			{
				shapes.AddRange (objectShapes);
			}
			else  // cadre ?
			{
				var polygons = Geometry.PathToPolygons (path);
				frame.AddShapes (this, shapes, objectShapes, port, drawingContext, polygons, null);
			}

			return shapes.ToArray ();
		}

		protected Path PathEllipse(DrawingContext drawingContext)
		{
			//	Crée le chemin d'une ellipse inscrite dans un quadrilatère.
			Stretcher stretcher = this.GetStretcher();
			Point center = new Point(0.0, 0.0);

			double zoom = Properties.Abstract.DefaultZoom(drawingContext);
			Path path = new Path();
			path.DefaultZoom = zoom;

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
				Geometry.ArcBezierDeg(path, stretcher, center, 0.5, 0.5, a1, a2, true, false);
			}

			if ( arc.ArcType == Properties.ArcType.Close )
			{
				path.Close();
			}
			if ( arc.ArcType == Properties.ArcType.Pie )
			{
				path.LineTo(stretcher.Transform(center));
				path.Close();
			}

			return path;
		}

		protected Path PathBuild(DrawingContext drawingContext)
		{
			//	Crée le chemin de l'objet.
			return this.PathEllipse(drawingContext);
		}


		protected override Path GetPath()
		{
			//	Retourne le chemin géométrique de l'objet.
			return this.PathBuild(null);
		}


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise l'objet.
			base.GetObjectData(info, context);
		}

		protected Ellipse(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise l'objet.
		}
		#endregion
	}
}
