using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe Group est la classe de l'objet graphique "groupe".
	/// </summary>
	[System.Serializable()]
	public class Group : Objects.Abstract
	{
		public Group(Document document, Objects.Abstract model) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);
			this.objects = new UndoableList(this.document, UndoableListType.ObjectsInsideDocument);
		}

		protected override bool ExistingProperty(Properties.Type type)
		{
			if ( type == Properties.Type.Name )  return true;
			return false;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return new Group(document, model);
		}


		// Nom de l'ic�ne.
		public override string IconName
		{
			get { return "manifest:Epsitec.App.DocumentEditor.Images.ObjGroup.icon"; }
		}


		// D�tecte si la souris est sur l'objet.
		public override bool Detect(Point pos)
		{
			if ( this.isHide )  return false;

			InsideSurface inside = new InsideSurface(pos, 4);
			inside.AddLine(this.Handle(0).Position, this.Handle(2).Position);
			inside.AddLine(this.Handle(2).Position, this.Handle(1).Position);
			inside.AddLine(this.Handle(1).Position, this.Handle(3).Position);
			inside.AddLine(this.Handle(3).Position, this.Handle(0).Position);
			return inside.IsInside();
		}


		// D�but du d�placement une poign�e.
		public override void MoveHandleStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			this.InsertOpletGeometry();
			this.initialBBox = this.BoundingBox.Size;
		}

		// D�place une poign�e.
		public override void MoveHandleProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			if ( rank >= this.handles.Count )  // poign�e d'une propri�t� ?
			{
				base.MoveHandleProcess(rank, pos, drawingContext);
				return;
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);

			Selector selector = new Selector(this.document);
			if ( rank == 0 )
			{
				selector.FixStarting(this.Handle(1).Position);
				selector.FixEnding(this.Handle(0).Position);
			}
			if ( rank == 1 )
			{
				selector.FixStarting(this.Handle(0).Position);
				selector.FixEnding(this.Handle(1).Position);
			}
			if ( rank == 2 )
			{
				selector.FixStarting(this.Handle(3).Position);
				selector.FixEnding(this.Handle(2).Position);
			}
			if ( rank == 3 )
			{
				selector.FixStarting(this.Handle(2).Position);
				selector.FixEnding(this.Handle(3).Position);
			}
			selector.FinalToInitialData();
			selector.FixEnding(pos);
			this.MoveHandleSoon(this.objects, selector);

			if ( Geometry.IsRectangular(this.Handle(0).Position, this.Handle(1).Position, this.Handle(2).Position, this.Handle(3).Position) )
			{
				this.Handle(rank).Position = pos;

				if ( rank == 0 )
				{
					this.Handle(2).Position = Point.Projection(this.Handle(2).Position, this.Handle(1).Position, pos);
					this.Handle(3).Position = Point.Projection(this.Handle(3).Position, this.Handle(1).Position, pos);
				}
				if ( rank == 1 )
				{
					this.Handle(2).Position = Point.Projection(this.Handle(2).Position, this.Handle(0).Position, pos);
					this.Handle(3).Position = Point.Projection(this.Handle(3).Position, this.Handle(0).Position, pos);
				}
				if ( rank == 2 )
				{
					this.Handle(0).Position = Point.Projection(this.Handle(0).Position, this.Handle(3).Position, pos);
					this.Handle(1).Position = Point.Projection(this.Handle(1).Position, this.Handle(3).Position, pos);
				}
				if ( rank == 3 )
				{
					this.Handle(0).Position = Point.Projection(this.Handle(0).Position, this.Handle(2).Position, pos);
					this.Handle(1).Position = Point.Projection(this.Handle(1).Position, this.Handle(2).Position, pos);
				}
			}
			else
			{
				this.Handle(rank).Position = pos;
			}

			this.minimalBBox = this.initialBBox;
			this.SetDirtyBbox();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// D�place tous les objets du groupe.
		protected void MoveHandleSoon(UndoableList objects, Selector selector)
		{
			foreach ( Objects.Abstract obj in this.document.Deep(this) )
			{
				obj.MoveGlobalStarting();
				obj.MoveGlobalProcess(selector);
			}
		}

		// Aligne l'objet sur la grille.
		// On aligne les 4 coins du groupe en stretchant le contenu du groupe.
		public override void AlignGrid(DrawingContext drawingContext)
		{
			for ( int i=0 ; i<4 ; i++ )
			{
				Point pos = this.Handle(i).Position;
				this.MoveHandleStarting(i, pos, drawingContext);
				drawingContext.SnapGridForce(ref pos);
				this.MoveHandleProcess(i, pos, drawingContext);
			}
		}

		
		// Met � jour la bbox du groupe.
		public void UpdateDim(Drawing.Rectangle bbox)
		{
			this.InsertOpletGeometry();

			if ( this.handles.Count == 0 )
			{
				this.HandleAdd(bbox.BottomLeft,  HandleType.Primary);
				this.HandleAdd(bbox.TopRight,    HandleType.Primary);
				this.HandleAdd(bbox.TopLeft,     HandleType.Primary);
				this.HandleAdd(bbox.BottomRight, HandleType.Primary);
			}
			else
			{
				this.Handle(0).Position = bbox.BottomLeft;
				this.Handle(1).Position = bbox.TopRight;
				this.Handle(2).Position = bbox.TopLeft;
				this.Handle(3).Position = bbox.BottomRight;
			}
			this.bboxThin = bbox;
			this.bboxGeom = bbox;
			this.bboxFull = bbox;
			this.dirtyBbox = false;
		}

		// Met � jour le rectangle englobant l'objet.
		protected override void UpdateBoundingBox()
		{
			if ( this.handles.Count < 4 )  return;

			this.bboxGeom = Drawing.Rectangle.Empty;
			this.bboxGeom.MergeWith(this.Handle(0).Position);
			this.bboxGeom.MergeWith(this.Handle(1).Position);
			this.bboxGeom.MergeWith(this.Handle(2).Position);
			this.bboxGeom.MergeWith(this.Handle(3).Position);

			if ( !this.minimalBBox.IsEmpty )
			{
				double dx = this.minimalBBox.Width-this.bboxGeom.Width;
				if ( dx > 0 )
				{
					this.bboxGeom.Left  -= dx/2;
					this.bboxGeom.Right += dx/2;
				}

				double dy = this.minimalBBox.Height-this.bboxGeom.Height;
				if ( dy > 0 )
				{
					this.bboxGeom.Bottom -= dy/2;
					this.bboxGeom.Top    += dy/2;
				}

				this.minimalBBox = Size.Empty;
			}

			this.bboxFull = this.bboxGeom;
			this.bboxThin = this.bboxGeom;
		}

		// Cr�e le chemin d'un rectangle.
		protected Path PathRectangle(Point p1, Point p2, Point p3, Point p4)
		{
			Path path = new Path();
			path.MoveTo(p1);
			path.LineTo(p2);
			path.LineTo(p3);
			path.LineTo(p4);
			path.Close();
			return path;
		}

		// Cr�e le chemin des coins d'un rectangle.
		protected Path PathCorners(Point p1, Point p2, Point p3, Point p4)
		{
			double d12 = Point.Distance(p1, p2)*0.25;
			double d23 = Point.Distance(p2, p3)*0.25;
			double d34 = Point.Distance(p3, p4)*0.25;
			double d41 = Point.Distance(p4, p1)*0.25;

			Path path = new Path();

			path.MoveTo(Point.Move(p1, p4, d41));
			path.LineTo(p1);
			path.LineTo(Point.Move(p1, p2, d12));

			path.MoveTo(Point.Move(p2, p1, d12));
			path.LineTo(p2);
			path.LineTo(Point.Move(p2, p3, d23));

			path.MoveTo(Point.Move(p3, p2, d23));
			path.LineTo(p3);
			path.LineTo(Point.Move(p3, p4, d34));

			path.MoveTo(Point.Move(p4, p3, d34));
			path.LineTo(p4);
			path.LineTo(Point.Move(p4, p1, d41));

			return path;
		}

		// Dessine l'objet.
		public override void DrawGeometry(Graphics graphics, DrawingContext drawingContext)
		{
			base.DrawGeometry(graphics, drawingContext);

			if ( this.TotalHandle < 2 )  return;
			if ( !drawingContext.IsActive || drawingContext.IsDimmed )  return;

			Point p1 = this.Handle(0).Position;
			Point p2 = this.Handle(2).Position;
			Point p3 = this.Handle(1).Position;
			Point p4 = this.Handle(3).Position;
			Path path = this.PathCorners(p1, p2, p3, p4);

			if ( !drawingContext.PreviewActive )
			{
				Color color = Color.FromBrightness(0.7);
				if ( this.IsSelected )  color = Color.FromRGB(1,0,0);

				graphics.Rasterizer.AddOutline(path, 1.0/drawingContext.ScaleX);
				graphics.RenderSolid(color);
			}

			if ( this.IsHilite )
			{
				path = this.PathRectangle(p1, p2, p3, p4);
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(drawingContext.HiliteSurfaceColor);
			}
		}


		#region Serialization
		// S�rialise l'objet.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		// Constructeur qui d�s�rialise l'objet.
		protected Group(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		#endregion

		
		protected Size				initialBBox;
		protected Size				minimalBBox = Size.Empty;
	}
}
