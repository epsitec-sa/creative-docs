using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe Rectangle est la classe de l'objet graphique "rectangle".
	/// </summary>
	[System.Serializable()]
	public class Rectangle : Objects.Abstract
	{
		public Rectangle(Document document, Objects.Abstract model) : base(document, model)
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
			if ( type == Properties.Type.Corner )  return true;
			return false;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return new Rectangle(document, model);
		}

		public override void Dispose()
		{
			base.Dispose();
		}


		// Nom de l'ic�ne.
		public override string IconName
		{
			get { return "manifest:Epsitec.App.DocumentEditor.Images.Rectangle.icon"; }
		}


		// D�tecte si la souris est sur l'objet.
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
				this.InflateBoundingBox(this.Handle(0).Position, false);
				this.InflateBoundingBox(this.Handle(1).Position, false);
				this.InflateBoundingBox(this.Handle(2).Position, false);
				this.InflateBoundingBox(this.Handle(3).Position, false);
			}
		}

		// Cr�e le chemin de l'objet.
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

			Properties.Corner corner = this.PropertyCorner;
			return this.PathCornerRectangle(drawingContext, p1, p2, p3, p4, corner);
		}

		// Cr�e le chemin d'un rectangle � coins quelconques.
		protected Path PathCornerRectangle(DrawingContext drawingContext, Point p1, Point p2, Point p3, Point p4, Properties.Corner corner)
		{
			double d12 = Point.Distance(p1, p2);
			double d23 = Point.Distance(p2, p3);
			double d34 = Point.Distance(p3, p4);
			double d41 = Point.Distance(p4, p1);
			double min = System.Math.Min(System.Math.Min(d12, d23), System.Math.Min(d34, d41));
			double radius = System.Math.Min(corner.Radius, min/2);

			Path path = new Path();
			path.DefaultZoom = Properties.Abstract.DefaultZoom(drawingContext);

			if ( corner.CornerType == Properties.CornerType.Right || radius == 0 )
			{
				path.MoveTo(p1);
				path.LineTo(p2);
				path.LineTo(p3);
				path.LineTo(p4);
				path.Close();
			}
			else
			{
				Point c1 = new Point();
				Point c2 = new Point();

				c1 = Point.Move(p1, p4, radius);
				c2 = Point.Move(p1, p2, radius);
				path.MoveTo(c1);
				corner.PathCorner(path, c1, p1, c2, radius);

				c1 = Point.Move(p2, p1, radius);
				c2 = Point.Move(p2, p3, radius);
				path.LineTo(c1);
				corner.PathCorner(path, c1, p2, c2, radius);

				c1 = Point.Move(p3, p2, radius);
				c2 = Point.Move(p3, p4, radius);
				path.LineTo(c1);
				corner.PathCorner(path, c1, p3, c2, radius);

				c1 = Point.Move(p4, p3, radius);
				c2 = Point.Move(p4, p1, radius);
				path.LineTo(c1);
				corner.PathCorner(path, c1, p4, c2, radius);

				path.Close();
			}
			return path;
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


		// Retourne le chemin g�om�trique de l'objet.
		public override Path GetPath(int rank)
		{
			if ( rank > 0 )  return null;
			return this.PathBuild(null);
		}


		#region Serialization
		// S�rialise l'objet.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		// Constructeur qui d�s�rialise l'objet.
		protected Rectangle(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		#endregion
	}
}
