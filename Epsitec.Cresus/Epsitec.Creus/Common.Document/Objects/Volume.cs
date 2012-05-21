using System.Collections.Generic;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe Volume est la classe de l'objet graphique volume 3d.
	/// </summary>
	[System.Serializable()]
	public class Volume : Objects.Abstract
	{
		public Volume(Document document, Objects.Abstract model) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);
		}

		protected override bool ExistingProperty(Properties.Type type)
		{
			if ( type == Properties.Type.Name )  return true;
			if ( type == Properties.Type.LineMode )  return true;
			if ( type == Properties.Type.LineColor )  return true;
			if ( type == Properties.Type.Volume )  return true;
			if ( type == Properties.Type.FillGradientVT )  return true;
			if ( type == Properties.Type.FillGradientVL )  return true;
			if ( type == Properties.Type.FillGradientVR )  return true;
			return false;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return new Volume(document, model);
		}

		public override void Dispose()
		{
			base.Dispose();
		}


		public override string IconUri
		{
			//	Nom de l'icône.
			get { return Misc.Icon("ObjectVolume"); }
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
			Drawing.Rectangle rect = Drawing.Rectangle.FromPoints(this.Handle(0).Position, this.Handle(1).Position);
			Point p1 = rect.BottomLeft;
			Point p2 = rect.TopRight;
			this.Handle(0).Position = p1;
			this.Handle(1).Position = p2;
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
			Paths paths = this.PathBuild(drawingContext);
			Shape[] shapes = new Shape[paths.Count*2];

			for ( int i=0 ; i<paths.Count ; i++ )
			{
				Path path = paths.Get(i);
				if ( path != null )
				{
					shapes[i*2+0] = new Shape();
					shapes[i*2+0].Path = path;
					shapes[i*2+0].SetPropertySurface(port, paths.PropertySurface(i));

					shapes[i*2+1] = new Shape();
					shapes[i*2+1].Path = path;
					shapes[i*2+1].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
				}
			}

			return shapes;
		}

		protected Paths PathBuild(DrawingContext drawingContext)
		{
			//	Crée les chemins de l'objet.
			Point p1 = this.Handle(0).Position;
			Point p2 = new Point();
			Point p3 = this.Handle(1).Position;
			Point p4 = new Point();

			if ( this.handles.Count < 4 )
			{
				Drawing.Rectangle rect = Drawing.Rectangle.FromPoints(p1, p3);
				p1 = rect.BottomLeft;
				p2 = rect.TopLeft;
				p3 = rect.TopRight;
				p4 = rect.BottomRight;
			}
			else
			{
				p2 = this.Handle(2).Position;
				p4 = this.Handle(3).Position;
			}

			return this.PathVolume(drawingContext, p1, p2, p3, p4);
		}

		protected Paths PathVolume(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
			//	Crée les chemins d'un volume quelconque.
			Paths paths = new Paths(this);

			switch ( this.PropertyVolume.VolumeType )
			{
				case Properties.VolumeType.BoxClose:
					this.PathBox(ref paths, drawingContext, p1, p2, p3, p4);
					break;

				case Properties.VolumeType.BoxOpen:
					this.PathBox(ref paths, drawingContext, p1, p2, p3, p4);
					paths.Top = null;
					break;

				case Properties.VolumeType.Pyramid:
					this.PathPyramid(ref paths, drawingContext, p1, p2, p3, p4);
					break;

				case Properties.VolumeType.Cylinder:
					this.PathCylinder(ref paths, drawingContext, p1, p2, p3, p4);
					break;
			}

			return paths;
		}

		protected void PathBox(ref Paths paths, DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
			//	Crée les chemins d'une boîte.
			Properties.Volume pf = this.PropertyVolume;
			Point p14 = Point.Scale(p1,p4, pf.Rapport);
			Point p23 = Point.Scale(p3,p2, pf.Rapport);

			double a = pf.AngleLeft*System.Math.PI/180.0;
			double b = pf.AngleRight*System.Math.PI/180.0;

			double d114 = Point.Distance(p1,p14);
			double d414 = Point.Distance(p4,p14);
			Point p12b = Point.Move(p1,p2, d114*System.Math.Tan(a));
			Point p34b = Point.Move(p4,p3, d414*System.Math.Tan(b));

			double d223 = Point.Distance(p2,p23);
			double d323 = Point.Distance(p3,p23);
			Point p34t = Point.Move(p3,p4, d323*System.Math.Tan(a));
			Point p12t = Point.Move(p2,p1, d223*System.Math.Tan(b));

			Point v1 = p23-p12t;
			Point v2 = p34b-p14;
			Point v = (v1+v2)/2;
			Point c = p34t-v;
			Point d = p12b+v;

			paths.Top = new Path();
			paths.Top.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);
			paths.Top.MoveTo(p12t);
			paths.Top.LineTo(p23);
			paths.Top.LineTo(p34t);
			paths.Top.LineTo(c);
			paths.Top.Close();

			paths.Bottom = new Path();
			paths.Bottom.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);
			paths.Bottom.MoveTo(p12b);
			paths.Bottom.LineTo(d);
			paths.Bottom.LineTo(p34b);
			paths.Bottom.LineTo(p14);
			paths.Bottom.Close();

			paths.FrontLeft = new Path();
			paths.FrontLeft.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);
			paths.FrontLeft.MoveTo(p12b);
			paths.FrontLeft.LineTo(p12t);
			paths.FrontLeft.LineTo(c);
			paths.FrontLeft.LineTo(p14);
			paths.FrontLeft.Close();

			paths.BackLeft = new Path();
			paths.BackLeft.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);
			paths.BackLeft.MoveTo(d);
			paths.BackLeft.LineTo(p23);
			paths.BackLeft.LineTo(p34t);
			paths.BackLeft.LineTo(p34b);
			paths.BackLeft.Close();

			paths.FrontRight = new Path();
			paths.FrontRight.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);
			paths.FrontRight.MoveTo(p14);
			paths.FrontRight.LineTo(c);
			paths.FrontRight.LineTo(p34t);
			paths.FrontRight.LineTo(p34b);
			paths.FrontRight.Close();

			paths.BackRight = new Path();
			paths.BackRight.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);
			paths.BackRight.MoveTo(p12b);
			paths.BackRight.LineTo(p12t);
			paths.BackRight.LineTo(p23);
			paths.BackRight.LineTo(d);
			paths.BackRight.Close();
		}

		protected void PathPyramid(ref Paths paths, DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
			//	Crée les chemins d'une pyramide.
			Properties.Volume pf = this.PropertyVolume;
			Point p14 = Point.Scale(p1,p4, pf.Rapport);
			Point p23 = Point.Scale(p3,p2, 0.5);

			double a = pf.AngleLeft*System.Math.PI/180.0;
			double b = pf.AngleRight*System.Math.PI/180.0;

			double d114 = Point.Distance(p1,p14);
			double d414 = Point.Distance(p4,p14);
			Point p12b = Point.Move(p1,p2, d114*System.Math.Tan(a));
			Point p34b = Point.Move(p4,p3, d414*System.Math.Tan(b));

			Point v = p34b-p14;
			Point d = p12b+v;

			paths.FrontLeft = new Path();
			paths.FrontLeft.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);
			paths.FrontLeft.MoveTo(p12b);
			paths.FrontLeft.LineTo(p23);
			paths.FrontLeft.LineTo(p14);
			paths.FrontLeft.Close();

			paths.BackLeft = new Path();
			paths.BackLeft.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);
			paths.BackLeft.MoveTo(d);
			paths.BackLeft.LineTo(p23);
			paths.BackLeft.LineTo(p34b);
			paths.BackLeft.Close();

			paths.FrontRight = new Path();
			paths.FrontRight.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);
			paths.FrontRight.MoveTo(p14);
			paths.FrontRight.LineTo(p23);
			paths.FrontRight.LineTo(p34b);
			paths.FrontRight.Close();

			paths.BackRight = new Path();
			paths.BackRight.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);
			paths.BackRight.MoveTo(p12b);
			paths.BackRight.LineTo(p23);
			paths.BackRight.LineTo(d);
			paths.BackRight.Close();

			paths.Bottom = new Path();
			paths.Bottom.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);
			paths.Bottom.MoveTo(p12b);
			paths.Bottom.LineTo(d);
			paths.Bottom.LineTo(p34b);
			paths.Bottom.LineTo(p14);
			paths.Bottom.Close();
		}

		protected void PathCylinder(ref Paths paths, DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4)
		{
			//	Crée les chemins d'un cylindre.
			double d12 = Point.Distance(p1, p2);
			double d23 = Point.Distance(p2, p3);
			double d34 = Point.Distance(p3, p4);
			double d41 = Point.Distance(p4, p1);
			double min = System.Math.Min(d12, d34);

			Properties.Volume pf = this.PropertyVolume;
			double h = d23*pf.Rapport/2.0;
			h = System.Math.Min(h, min/2);
			Point p1b = Point.Move(p1,p2,h);
			Point p2b = Point.Move(p2,p1,h);
			Point p3b = Point.Move(p3,p4,h);
			Point p4b = Point.Move(p4,p3,h);
			Point p23 = Point.Scale(p2,p3,0.5);
			Point p14 = Point.Scale(p1,p4,0.5);

			Point p2bb = Point.Move(p2,p1,h*2);
			Point p3bb = Point.Move(p3,p4,h*2);
			Point p23b = Point.Scale(p2bb,p3bb,0.5);

			Point p1bb = Point.Move(p1,p2,h*2);
			Point p4bb = Point.Move(p4,p3,h*2);
			Point p14b = Point.Scale(p1bb,p4bb,0.5);

			paths.FrontLeft = new Path();
			paths.FrontLeft.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);
			paths.FrontLeft.MoveTo(p1b);
			paths.FrontLeft.LineTo(p2b);
			paths.FrontLeft.ArcTo(p2bb, p23b);
			paths.FrontLeft.ArcTo(p3bb, p3b);
			paths.FrontLeft.LineTo(p4b);
			paths.FrontLeft.ArcTo(p4, p14);
			paths.FrontLeft.ArcTo(p1, p1b);
			paths.FrontLeft.Close();

			paths.BackRight = new Path();
			paths.BackRight.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);
			paths.BackRight.MoveTo(p1b);
			paths.BackRight.LineTo(p2b);
			paths.BackRight.ArcTo(p2, p23);
			paths.BackRight.ArcTo(p3, p3b);
			paths.BackRight.LineTo(p4b);
			paths.BackRight.ArcTo(p4bb, p14b);
			paths.BackRight.ArcTo(p1bb, p1b);
			paths.BackRight.Close();

			paths.Top = new Path();
			paths.Top.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);
			paths.Top.MoveTo(p2b);
			paths.Top.ArcTo(p2, p23);
			paths.Top.ArcTo(p3, p3b);
			paths.Top.ArcTo(p3bb, p23b);
			paths.Top.ArcTo(p2bb, p2b);
			paths.Top.Close();

			paths.Bottom = new Path();
			paths.Bottom.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);
			paths.Bottom.MoveTo(p1b);
			paths.Bottom.ArcTo(p1bb, p14b);
			paths.Bottom.ArcTo(p4bb, p4b);
			paths.Bottom.ArcTo(p4, p14);
			paths.Bottom.ArcTo(p1, p1b);
			paths.Bottom.Close();
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


		protected override Path GetPath()
		{
			//	Retourne le chemin géométrique de l'objet.
			Paths paths = this.PathBuild(null);
			Path path = new Path();
			for ( int i=0 ; i<paths.Count ; i++ )
			{
				Path p = paths.Get(i);
				if ( p == null )  continue;
				path.Append(p, 0.0);
			}
			return path;
		}


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise l'objet.
			base.GetObjectData(info, context);
		}

		protected Volume(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise l'objet.
		}
		#endregion


		//	Gestion des chemins.
		protected class Paths
		{
			public Paths(Objects.Abstract obj)
			{
				this.obj = obj;
			}

			public int Count
			{
				get { return 6; }
			}

			public Path Get(int index)
			{
				switch ( index )
				{
					case 0:  return this.Bottom;
					case 1:  return this.BackLeft;
					case 2:  return this.BackRight;
					case 3:  return this.Top;
					case 4:  return this.FrontLeft;
					case 5:  return this.FrontRight;
				}
				return null;
			}

			public Properties.Gradient PropertySurface(int index)
			{
				if ( this.Get(index) == null )  return null;
				switch ( index )
				{
					case 0:  return this.obj.PropertyFillGradientVT;
					case 1:  return this.obj.PropertyFillGradientVL;
					case 2:  return this.obj.PropertyFillGradientVR;
					case 3:  return this.obj.PropertyFillGradientVT;
					case 4:  return this.obj.PropertyFillGradientVL;
					case 5:  return this.obj.PropertyFillGradientVR;
				}
				return null;
			}

			public Path						Bottom;
			public Path						BackLeft;
			public Path						BackRight;
			public Path						Top;
			public Path						FrontLeft;
			public Path						FrontRight;
			protected Objects.Abstract		obj;
		}
	}
}
