using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe Line est la classe de l'objet graphique "segment de ligne".
	/// </summary>
	[System.Serializable()]
	public class Line : Objects.Abstract
	{
		public Line(Document document, Objects.Abstract model) : this(document, model, false)
		{
		}

		public Line(Document document, Objects.Abstract model, bool floating) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, floating);
		}

		protected override bool ExistingProperty(Properties.Type type)
		{
			if ( type == Properties.Type.Name )  return true;
			if ( type == Properties.Type.LineMode )  return true;
			if ( type == Properties.Type.LineColor )  return true;
			if ( type == Properties.Type.Arrow )  return true;
			return false;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return new Line(document, model);
		}

		public override void Dispose()
		{
			base.Dispose();
		}


		// Nom de l'icône.
		public override string IconName
		{
			get { return "manifest:Epsitec.App.DocumentEditor.Images.Line.icon"; }
		}


		// Détecte si la souris est sur l'objet.
		public override bool Detect(Point pos)
		{
			if ( this.isHide )  return false;

			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine;
			this.PathBuild(null,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			double width = System.Math.Max(this.PropertyLineMode.Width/2, context.MinimalWidth);

			if (                 Geometry.DetectOutline(pathLine,  width, pos) )  return true;
			if ( outlineStart && Geometry.DetectOutline(pathStart, width, pos) )  return true;
			if ( outlineEnd   && Geometry.DetectOutline(pathEnd,   width, pos) )  return true;

			if ( surfaceStart && Geometry.DetectSurface(pathStart, pos) )  return true;
			if ( surfaceEnd   && Geometry.DetectSurface(pathEnd,   pos) )  return true;

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

			if ( rank == 0 )  // p1 ?
			{
				this.Handle(0).Position = pos;
			}
			else if ( rank == 1 )  // p2 ?
			{
				this.Handle(1).Position = pos;
			}
			this.HandlePropertiesUpdatePosition();
			this.dirtyBbox = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		// Déplace globalement l'objet.
		public override void MoveGlobalProcess(SelectorData initial, SelectorData final)
		{
			base.MoveGlobalProcess(initial, final);
			this.HandlePropertiesUpdatePosition();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		// Début de la création d'un objet.
		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			drawingContext.ConstrainFixStarting(pos);
			drawingContext.ConstrainFixType(ConstrainType.Normal);
			if ( this.handles.Count == 0 )
			{
				this.HandleAdd(pos, HandleType.Primary);
				this.HandleAdd(pos, HandleType.Primary);
			}
			else
			{
				this.Handle(0).Position = pos;
				this.Handle(1).Position = pos;
			}
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

			this.bboxThin = Drawing.Rectangle.FromCorners(this.Handle(0).Position, this.Handle(1).Position);

			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine;
			this.PathBuild(null,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

			Path[] paths = new Path[3];
			paths[0] = pathLine;
			paths[1] = pathStart;
			paths[2] = pathEnd;

			bool[] lineModes = new bool[3];
			lineModes[0] = true;
			lineModes[1] = outlineStart;
			lineModes[2] = outlineEnd;

			bool[] lineColors = new bool[3];
			lineColors[0] = true;
			lineColors[1] = surfaceStart;
			lineColors[2] = surfaceEnd;

			bool[] fillGradients = new bool[3];
			fillGradients[0] = false;
			fillGradients[1] = false;
			fillGradients[2] = false;

			this.ComputeBoundingBox(paths, lineModes, lineColors, fillGradients);
		}

		// Crée les 3 chemins de l'objet.
		protected void PathBuild(DrawingContext drawingContext,
								 out Path pathStart, out bool outlineStart, out bool surfaceStart,
								 out Path pathEnd,   out bool outlineEnd,   out bool surfaceEnd,
								 out Path pathLine)
		{
			pathStart = new Path();
			pathEnd   = new Path();
			pathLine  = new Path();

			double zoom = Properties.Abstract.DefaultZoom(drawingContext);
			pathStart.DefaultZoom = zoom;
			pathEnd.DefaultZoom = zoom;
			pathLine.DefaultZoom = zoom;

			Point p1 = this.Handle(0).Position;
			Point p2 = this.Handle(1).Position;
			double w = this.PropertyLineMode.Width;
			CapStyle cap = this.PropertyLineMode.Cap;
			Point pp1 = this.PropertyArrow.PathExtremity(pathStart, 0, w,cap, p1,p2, out outlineStart, out surfaceStart);
			Point pp2 = this.PropertyArrow.PathExtremity(pathEnd,   1, w,cap, p2,p1, out outlineEnd,   out surfaceEnd);

			pathLine.MoveTo(pp1);
			pathLine.LineTo(pp2);
		}

		// Dessine l'objet.
		public override void DrawGeometry(Graphics graphics, DrawingContext drawingContext)
		{
			base.DrawGeometry(graphics, drawingContext);

			if ( this.TotalHandle < 2 )  return;

			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine;
			this.PathBuild(drawingContext,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

			if ( outlineStart )
			{
				this.PropertyLineMode.DrawPath(graphics, drawingContext, pathStart, this.PropertyLineColor, this.BoundingBoxGeom);
			}
			if ( surfaceStart )
			{
				this.PropertyLineColor.RenderSurface(graphics, drawingContext, pathStart, this.BoundingBoxThin);
			}

			if ( outlineEnd )
			{
				this.PropertyLineMode.DrawPath(graphics, drawingContext, pathEnd, this.PropertyLineColor, this.BoundingBoxGeom);
			}
			if ( surfaceEnd )
			{
				this.PropertyLineColor.RenderSurface(graphics, drawingContext, pathEnd, this.BoundingBoxThin);
			}

			this.PropertyLineMode.DrawPath(graphics, drawingContext, pathLine, this.PropertyLineColor, this.BoundingBoxGeom);

			if ( this.IsHilite && drawingContext.IsActive )
			{
				if ( outlineStart )
				{
					this.PropertyLineMode.AddOutline(graphics, pathStart, drawingContext.HiliteSize);
					graphics.RenderSolid(drawingContext.HiliteOutlineColor);
				}
				if ( surfaceStart )
				{
					graphics.Rasterizer.AddSurface(pathStart);
					graphics.RenderSolid(drawingContext.HiliteOutlineColor);
				}

				if ( outlineEnd )
				{
					this.PropertyLineMode.AddOutline(graphics, pathEnd, drawingContext.HiliteSize);
					graphics.RenderSolid(drawingContext.HiliteOutlineColor);
				}
				if ( surfaceEnd )
				{
					graphics.Rasterizer.AddSurface(pathEnd);
					graphics.RenderSolid(drawingContext.HiliteOutlineColor);
				}

				this.PropertyLineMode.AddOutline(graphics, pathLine, drawingContext.HiliteSize);
				graphics.RenderSolid(drawingContext.HiliteOutlineColor);
			}
		}

		// Imprime l'objet.
		public override void PrintGeometry(Printing.PrintPort port, DrawingContext drawingContext)
		{
			base.PrintGeometry(port, drawingContext);

			if ( this.TotalHandle < 2 )  return;

			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine;
			this.PathBuild(drawingContext,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

			if ( this.PropertyLineColor.PaintColor(port, drawingContext) )
			{
				if ( outlineStart )
				{
					this.PropertyLineMode.PaintOutline(port, drawingContext, pathStart);
				}
				if ( surfaceStart )
				{
					port.PaintSurface(pathStart);
				}

				if ( outlineEnd )
				{
					this.PropertyLineMode.PaintOutline(port, drawingContext, pathEnd);
				}
				if ( surfaceEnd )
				{
					port.PaintSurface(pathEnd);
				}

				this.PropertyLineMode.PaintOutline(port, drawingContext, pathLine);
			}
		}


		#region Serialization
		// Sérialise l'objet.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		// Constructeur qui désérialise l'objet.
		protected Line(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		#endregion
	}
}
