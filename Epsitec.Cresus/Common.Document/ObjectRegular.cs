using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe ObjectRegular est la classe de l'objet graphique "polygone r�gulier".
	/// </summary>
	[System.Serializable()]
	public class ObjectRegular : AbstractObject
	{
		public ObjectRegular(Document document, AbstractObject model) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);
		}

		protected override bool ExistingProperty(PropertyType type)
		{
			if ( type == PropertyType.Name )  return true;
			if ( type == PropertyType.LineMode )  return true;
			if ( type == PropertyType.LineColor )  return true;
			if ( type == PropertyType.FillGradient )  return true;
			if ( type == PropertyType.Regular )  return true;
			if ( type == PropertyType.Corner )  return true;
			return false;
		}

		protected override AbstractObject CreateNewObject(Document document, AbstractObject model)
		{
			return new ObjectRegular(document, model);
		}

		public override void Dispose()
		{
			base.Dispose();
		}


		// Nom de l'ic�ne.
		public override string IconName
		{
			get { return @"file:images/regular.icon"; }
		}


		// D�tecte si la souris est sur l'objet.
		public override bool Detect(Point pos)
		{
			if ( this.isHide )  return false;

			Rectangle bbox = this.BoundingBox;
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


		// D�place une poign�e.
		public override void MoveHandleProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			if ( rank >= 2 )  // poign�e d'une propri�t� ?
			{
				base.MoveHandleProcess(rank, pos, drawingContext);
				return;
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.ConstrainSnapPos(ref pos);
			drawingContext.SnapGrid(ref pos);

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

			this.HandlePropertiesUpdatePosition();
			this.dirtyBbox = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		// D�but de la cr�ation d'un objet.
		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			drawingContext.ConstrainFixStarting(pos);
			drawingContext.ConstrainFixType(ConstrainType.Normal);
			this.HandleAdd(pos, HandleType.Primary);
			this.HandleAdd(pos, HandleType.Primary);
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

			this.HandlePropertiesCreate();
			this.HandlePropertiesUpdatePosition();
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

			Path path = this.PathBuild(null);
			this.bboxThin = path.ComputeBounds();

			this.bboxGeom = this.bboxThin;
			this.bboxGeom.MergeWith(this.Handle(1).Position);
			this.PropertyLineMode.InflateBoundingBox(ref this.bboxGeom);

			this.bboxFull = this.bboxGeom;
			this.bboxGeom.MergeWith(this.PropertyFillGradient.BoundingBoxGeom(this.bboxThin));
			this.bboxFull.MergeWith(this.PropertyFillGradient.BoundingBoxFull(this.bboxThin));
			this.bboxFull.MergeWith(this.bboxGeom);
		}

		// Calcule les points pour les poign�es de la propri�t� PropertyCorner.
		public void ComputeCorners(out Point a, out Point b)
		{
			Point t;

			PropertyRegular pr = this.PropertyRegular;
			if ( pr.Star )  // �toile ?
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

		// Calcule une droite de l'objet.
		protected bool ComputeLine(int i, out Point a, out Point b)
		{
			int total = this.PropertyRegular.NbFaces;
			Point center = this.Handle(0).Position;
			Point corner = this.Handle(1).Position;

			a = new Point(0, 0);
			b = new Point(0, 0);

			if ( this.PropertyRegular.Star )  // �toile ?
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

		// Cr�e le chemin d'un polygone r�gulier.
		protected Path PathBuild(DrawingContext drawingContext)
		{
			Path path = new Path();
			path.DefaultZoom = AbstractProperty.DefaultZoom(drawingContext);;

			int total = this.PropertyRegular.NbFaces;
			PropertyCorner corner = this.PropertyCorner;

			if ( corner.CornerType == CornerType.Right )  // coins droits ?
			{
				Point a;
				Point b;

				if ( this.PropertyRegular.Star )  total *= 2;  // �toile ?

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

				if ( this.PropertyRegular.Star )  total *= 2;  // �toile ?

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

		// Cr�e le chemin d'un coin.
		protected void PathCorner(Path path, Point p1, Point s, Point p2, PropertyCorner corner)
		{
			double l1 = Point.Distance(p1, s);
			double l2 = Point.Distance(p2, s);
			double radius = System.Math.Min(corner.Radius, System.Math.Min(l1,l2)/2);
			Point c1 = Point.Move(s, p1, radius);
			Point c2 = Point.Move(s, p2, radius);
			if ( path.IsEmpty )  path.MoveTo(c1);
			else                 path.LineTo(c1);
			corner.PathCorner(path, c1,s,c2, radius);
		}

		// Dessine l'objet.
		public override void DrawGeometry(Graphics graphics, DrawingContext drawingContext)
		{
			base.DrawGeometry(graphics, drawingContext);

			if ( this.TotalHandle < 2 )  return;

			Path path = this.PathBuild(drawingContext);
			this.PropertyFillGradient.Render(graphics, drawingContext, path, this.BoundingBoxThin);

			this.PropertyLineMode.DrawPath(graphics, drawingContext, path, this.PropertyLineColor.Color);

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
		// S�rialise l'objet.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		// Constructeur qui d�s�rialise l'objet.
		protected ObjectRegular(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		#endregion
	}
}
