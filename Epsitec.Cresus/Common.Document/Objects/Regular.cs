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


		// Nom de l'icône.
		public override string IconName
		{
			get { return "manifest:Epsitec.App.DocumentEditor.Images.Regular.icon"; }
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
			if ( rank >= 2 )  // poignée d'une propriété ?
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
			if ( rank == 1 )  // extrémité ?
			{
				this.Handle(1).Position = pos;
			}

			this.HandlePropertiesUpdate();
			this.dirtyBbox = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		// Début de la création d'un objet.
		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			drawingContext.ConstrainFixStarting(pos);
			drawingContext.ConstrainFixType(ConstrainType.Normal);
			this.HandleAdd(pos, HandleType.Primary);
			this.HandleAdd(pos, HandleType.Primary);
			this.isCreating = true;
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
			this.isCreating = false;

			this.HandlePropertiesCreate();
			this.HandlePropertiesUpdate();
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
		}

		// Calcule les points pour les poignées de la propriété PropertyCorner.
		public void ComputeCorners(out Point a, out Point b)
		{
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

		// Calcule une droite de l'objet.
		protected bool ComputeLine(int i, out Point a, out Point b)
		{
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

		// Crée le chemin d'un polygone régulier.
		protected Path PathBuild(DrawingContext drawingContext)
		{
			Path path = new Path();
			path.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);;

			int total = this.PropertyRegular.NbFaces;
			Properties.Corner corner = this.PropertyCorner;

			if ( corner.CornerType == Properties.CornerType.Right )  // coins droits ?
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

		// Crée le chemin d'un coin.
		protected void PathCorner(Path path, Point p1, Point s, Point p2, Properties.Corner corner)
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

			if ( this.IsDrawDash(drawingContext) )
			{
				this.PropertyLineMode.DrawPathDash(graphics, drawingContext, path, this.PropertyLineColor);
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


		// Retourne le chemin géométrique de l'objet.
		public override Path GetPath(int rank)
		{
			if ( rank > 0 )  return null;
			return this.PathBuild(null);
		}


		#region Serialization
		// Sérialise l'objet.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		// Constructeur qui désérialise l'objet.
		protected Regular(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		#endregion
	}
}
