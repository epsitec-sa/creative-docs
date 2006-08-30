using System.Collections.Generic;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe Regular est la classe de l'objet graphique "polygone régulier".
	/// </summary>
	[System.Serializable()]
	public class Regular : Objects.Abstract
	{
		public Regular(Document document, Objects.Abstract model) : base(document, model)
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
			if ( type == Properties.Type.Regular )  return true;
			if ( type == Properties.Type.Corner )  return true;
			return false;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return new Regular(document, model);
		}

		public override void Dispose()
		{
			base.Dispose();
		}


		public override string IconName
		{
			//	Nom de l'icône.
			get { return Misc.Icon("ObjectRegular"); }
		}


		public override void MoveHandleStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Début du déplacement une poignée.
			base.MoveHandleStarting(rank, pos, drawingContext);

			if ( rank < this.handles.Count )  // poignée de l'objet ?
			{
				drawingContext.ConstrainFlush();

				Handle handle = this.Handle(rank);
				if ( handle.PropertyType == Properties.Type.None )
				{
					if ( rank == 0 )  // centre ?
					{
						drawingContext.ConstrainAddHV(this.Handle(0).Position);
					}
					if ( rank == 1 )  // extrémité ?
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
			//	Déplace une poignée.
			if ( rank >= 2 )  // poignée d'une propriété ?
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
			if ( rank == 1 )  // extrémité ?
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
			//	Début de la création d'un objet.
			drawingContext.ConstrainFlush();
			drawingContext.ConstrainAddHV(pos);
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
			this.TextInfoModifCircle();
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


		public override Shape[] ShapesBuild(IPaintPort port, DrawingContext drawingContext, bool simplify)
		{
			//	Constuit les formes de l'objet.
			Path path = this.PathBuild(drawingContext, simplify);
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

		public void ComputeCorners(out Point a, out Point b)
		{
			//	Calcule les points pour les poignées de la propriété PropertyCorner.
			Point t;

			Properties.Regular pr = this.PropertyRegular;
			if ( pr.Star )  // étoile ?
			{
				this.ComputeLine(0, out t, out a);
				this.ComputeLine(pr.NbFaces*2-1, out b, out t);
			}
			else	// polygone ?
			{
				this.ComputeLine(0, out t, out a);
				this.ComputeLine(pr.NbFaces-1, out b, out t);
			}
		}

		protected bool ComputeLine(int i, out Point a, out Point b)
		{
			//	Calcule une droite de l'objet.
			int total = this.PropertyRegular.NbFaces;
			Point center = this.Handle(0).Position;
			Point corner = this.Handle(1).Position;

			a = new Point(0, 0);
			b = new Point(0, 0);

			if ( this.PropertyRegular.Star )  // étoile ?
			{
				if ( i >= total*2 )  return false;

				Point star = center + (corner-center)*(1-this.PropertyRegular.Deep);
				a = Transform.RotatePointDeg(center, 360.0*(i+0)/(total*2), (i%2==0) ? corner : star);
				b = Transform.RotatePointDeg(center, 360.0*(i+1)/(total*2), (i%2==0) ? star : corner);
				return true;
			}
			else	// polygone ?
			{
				if ( i >= total )  return false;

				a = Transform.RotatePointDeg(center, 360.0*(i+0)/total, corner);
				b = Transform.RotatePointDeg(center, 360.0*(i+1)/total, corner);
				return true;
			}
		}

		protected Path PathBuild(DrawingContext drawingContext, bool simplify)
		{
			//	Crée le chemin d'un polygone régulier.
			Path path = new Path();
			path.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);;

			int total = this.PropertyRegular.NbFaces;
			Properties.Corner corner = this.PropertyCorner;

			if ( corner.CornerType == Properties.CornerType.Right || simplify )  // coins droits ?
			{
				Point a;
				Point b;

				if ( this.PropertyRegular.Star )  total *= 2;  // étoile ?

				for ( int i=0 ; i<total ; i++ )
				{
					this.ComputeLine(i, out a, out b);
					if ( i == 0 )  path.MoveTo(a);
					else           path.LineTo(a);
				}
				path.Close();
			}
			else	// coins quelconques ?
			{
				Point p1;
				Point s;
				Point p2;

				if ( this.PropertyRegular.Star )  total *= 2;  // étoile ?

				for ( int i=0 ; i<total ; i++ )
				{
					int prev = i-1;  if ( prev < 0 )  prev = total-1;
					this.ComputeLine(prev, out p1, out s);
					this.ComputeLine(i, out s, out p2);
					this.PathCorner(path, p1,s,p2, corner);
				}
				path.Close();
			}

			return path;
		}

		protected void PathCorner(Path path, Point p1, Point s, Point p2, Properties.Corner corner)
		{
			//	Crée le chemin d'un coin.
			double l1 = Point.Distance(p1, s);
			double l2 = Point.Distance(p2, s);
			double radius = System.Math.Min(corner.Radius, System.Math.Min(l1,l2)/2);
			Point c1 = Point.Move(s, p1, radius);
			Point c2 = Point.Move(s, p2, radius);
			if ( path.IsEmpty )  path.MoveTo(c1);
			else                 path.LineTo(c1);
			corner.PathCorner(path, c1,s,c2, radius);
		}


		public override Path GetMagnetPath()
		{
			//	Retourne le chemin géométrique de l'objet pour les constructions
			//	magnétiques.
			return this.PathBuild(null, true);
		}

		protected override Path GetPath()
		{
			//	Retourne le chemin géométrique de l'objet.
			return this.PathBuild(null, false);
		}


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise l'objet.
			base.GetObjectData(info, context);
		}

		protected Regular(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise l'objet.
		}
		#endregion
	}
}
