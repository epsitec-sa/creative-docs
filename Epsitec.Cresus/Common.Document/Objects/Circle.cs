using System.Collections.Generic;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe Circle est la classe de l'objet graphique "cercle".
	/// </summary>
	[System.Serializable()]
	public class Circle : Objects.Abstract
	{
		public Circle(Document document, Objects.Abstract model) : base(document, model)
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
			return new Circle(document, model);
		}

		public override void Dispose()
		{
			base.Dispose();
		}


		public override string IconName
		{
			//	Nom de l'ic�ne.
			get { return Misc.Icon("ObjectCircle"); }
		}


		public override void MoveHandleStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			//	D�but du d�placement une poign�e.
			base.MoveHandleStarting(rank, pos, drawingContext);

			if ( rank < this.handles.Count )  // poign�e de l'objet ?
			{
				drawingContext.ConstrainFlush();

				Handle handle = this.Handle(rank);
				if ( handle.PropertyType == Properties.Type.None )
				{
					if ( rank == 0 )  // centre ?
					{
						drawingContext.ConstrainAddHV(this.Handle(0).Position);
					}
					if ( rank == 1 )  // extr�mit� ?
					{
						drawingContext.ConstrainAddCenter(this.Handle(0).Position);
						drawingContext.ConstrainAddCircle(this.Handle(0).Position, this.Handle(1).Position);
					}
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
			//	D�place une poign�e.
			if ( rank >= 2 )  // poign�e d'une propri�t� ?
			{
				base.MoveHandleProcess(rank, pos, drawingContext);
				return;
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);

			if ( rank == 0 )  // centre ?
			{
				Point move = pos-this.Handle(rank).Position;
				this.Handle(0).Position = pos;
				this.Handle(1).Position += move;
			}
			if ( rank == 1 )  // extr�mit� ?
			{
				this.Handle(1).Position = pos;
			}

			this.HandlePropertiesUpdate();
			this.SetDirtyBbox();
			this.TextInfoModifCircle();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			//	D�but de la cr�ation d'un objet.
			drawingContext.ConstrainFlush();
			drawingContext.ConstrainAddHV(pos);
			this.HandleAdd(pos, HandleType.Primary);
			this.HandleAdd(pos, HandleType.Primary);
			this.isCreating = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			//	D�placement pendant la cr�ation d'un objet.
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);
			this.Handle(1).Position = pos;
			this.SetDirtyBbox();
			this.TextInfoModifCircle();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			//	Fin de la cr�ation d'un objet.
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);
			this.Handle(1).Position = pos;
			drawingContext.ConstrainDelStarting();
			drawingContext.MagnetClearStarting();
			this.isCreating = false;
			this.document.Modifier.TextInfoModif = "";

			this.HandlePropertiesCreate();
			this.HandlePropertiesUpdate();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override bool CreateIsExist(DrawingContext drawingContext)
		{
			//	Indique si l'objet doit exister. Retourne false si l'objet ne peut
			//	pas exister et doit �tre d�truit.
			double len = Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > drawingContext.MinimalSize );
		}

		
		public override Shape[] ShapesBuild(IPaintPort port, DrawingContext drawingContext, bool simplify)
		{
			//	Constuit les formes de l'objet.
			Path path = this.PathBuild(drawingContext);
			Shape[] shapes = new Shape[2];

			//	Forme de la surface.
			shapes[0] = new Shape();
			shapes[0].Path = path;
			shapes[0].SetPropertySurface(port, this.PropertyFillGradient);

			//	Forme du chemin.
			shapes[1] = new Shape();
			shapes[1].Path = path;
			shapes[1].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);

			return shapes;
		}

		protected void ComputeGeometry(out Point center, out double radius, out double angle)
		{
			//	Calcule la g�om�trie de l'ellipse.
			center = this.Handle(0).Position;
			Point p = this.Handle(1).Position;
			radius = Point.Distance(center, p);
			angle = Point.ComputeAngleDeg(center, p);
		}

		public Point ComputeArcHandle(double angle)
		{
			//	Calcule la position d'une poign�e pour l'arc.
			Point center, p;
			double radius, rot;
			this.ComputeGeometry(out center, out radius, out rot);

			if ( radius == 0.0 )
			{
				return center;
			}
			else
			{
				p = center;
				p.X += radius;
				p = Transform.RotatePointDeg(center, angle, p);
				return Transform.RotatePointDeg(center, rot, p);
			}
		}

		public double ComputeArcHandle(Point pos)
		{
			//	Calcule l'angle d'apr�s la position de la souris.
			Point center, p;
			double radius, rot;
			this.ComputeGeometry(out center, out radius, out rot);

			if ( radius == 0.0 )
			{
				return 0.0;
			}
			else
			{
				p = Transform.RotatePointDeg(center, -rot, pos);
				double angle = Point.ComputeAngleDeg(center, p);
				if ( angle < 0.0 )  angle += 360.0;  // 0..360
				return angle;
			}
		}

		protected Path PathCircle(DrawingContext drawingContext, Point c, double rx, double ry)
		{
			//	Cr�e le chemin d'un cercle.
			Point center;
			double radius, rot;
			this.ComputeGeometry(out center, out radius, out rot);

			Path path = new Path();
			path.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);

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
				path.ArcDeg(center, radius, radius, a1+rot, a2+rot, true);
			}

			if ( arc.ArcType == Properties.ArcType.Close )
			{
				path.Close();
			}
			if ( arc.ArcType == Properties.ArcType.Pie )
			{
				path.LineTo(center);
				path.Close();
			}

			return path;
		}

		protected Path PathBuild(DrawingContext drawingContext)
		{
			//	Cr�e le chemin de l'objet.
			Point center = this.Handle(0).Position;
			double radius = Point.Distance(center, this.Handle(1).Position);
			return this.PathCircle(drawingContext, center, radius, radius);
		}


		protected override Path GetPath()
		{
			//	Retourne le chemin g�om�trique de l'objet.
			return this.PathBuild(null);
		}


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	S�rialise l'objet.
			base.GetObjectData(info, context);
		}

		protected Circle(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui d�s�rialise l'objet.
		}
		#endregion
	}
}
