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


		// Nom de l'icône.
		public override string IconName
		{
			get { return "manifest:Epsitec.App.DocumentEditor.Images.Circle.icon"; }
		}


		// Détecte si la souris est sur l'objet.
		public override bool Detect(Point pos)
		{
			if ( this.isHide )  return false;

			Point p1 = this.Handle(0).Position;
			Point p2 = this.Handle(1).Position;
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			double width = System.Math.Max(this.PropertyLineMode.Width/2, context.MinimalWidth);
			double radius = Point.Distance(p1, p2)+width;
			double dist = Point.Distance(p1, pos);

			if ( this.PropertyFillGradient.IsVisible() )
			{
				return ( dist <= radius );
			}
			else
			{
				return ( dist <= radius && dist >= radius-width*2 );
			}
		}


		// Début du déplacement une poignée.
		public override void MoveHandleStarting(int rank, Point pos, DrawingContext drawingContext)
		{
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
			this.TextInfoModifCircle();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		// Début de la création d'un objet.
		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			drawingContext.ConstrainFlush();
			drawingContext.ConstrainAddHV(pos);
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
			this.TextInfoModifCircle();
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
			drawingContext.MagnetClearStarting();
			this.isCreating = false;
			this.document.Modifier.TextInfoModif = "";

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

			if ( this.TotalHandle >= 2 )
			{
				this.InflateBoundingBox(this.Handle(1).Position, true);
			}
		}

		// Calcule la géométrie de l'ellipse.
		protected void ComputeGeometry(out Point center, out double radius, out double angle)
		{
			center = this.Handle(0).Position;
			Point p = this.Handle(1).Position;
			radius = Point.Distance(center, p);
			angle = Point.ComputeAngleDeg(center, p);
		}

		// Calcule la position d'une poignée pour l'arc.
		public Point ComputeArcHandle(double angle)
		{
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

		// Calcule l'angle d'après la position de la souris.
		public double ComputeArcHandle(Point pos)
		{
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

		// Crée le chemin d'un cercle.
		protected Path PathCircle(DrawingContext drawingContext, Point c, double rx, double ry)
		{
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

		// Crée le chemin de l'objet.
		protected Path PathBuild(DrawingContext drawingContext)
		{
			Point center = this.Handle(0).Position;
			double radius = Point.Distance(center, this.Handle(1).Position);
			return this.PathCircle(drawingContext, center, radius, radius);
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
		protected Circle(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		#endregion
	}
}
