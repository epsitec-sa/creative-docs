using System.Collections.Generic;
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


		public override string IconUri
		{
			//	Nom de l'icône.
			get { return Misc.Icon("ObjectGroup"); }
		}


		public override void MoveHandleStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Début du déplacement une poignée.
			this.InsertOpletGeometry();
			this.initialBBox = this.BoundingBox.Size;
		}

		public override void MoveHandleProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Déplace une poignée.
			if ( rank >= this.handles.Count )  // poignée d'une propriété ?
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

		protected void MoveHandleSoon(UndoableList objects, Selector selector)
		{
			//	Déplace tous les objets du groupe.
			foreach ( Objects.Abstract obj in this.document.Deep(this) )
			{
				obj.MoveGlobalStarting();
				obj.MoveGlobalProcess(selector);
			}
		}

		public override void AlignGrid(DrawingContext drawingContext)
		{
			//	Aligne l'objet sur la grille.
			//	On aligne les 4 coins du groupe en stretchant le contenu du groupe.
			for ( int i=0 ; i<4 ; i++ )
			{
				Point pos = this.Handle(i).Position;
				this.MoveHandleStarting(i, pos, drawingContext);
				drawingContext.SnapGridForce(ref pos);
				this.MoveHandleProcess(i, pos, drawingContext);
			}
		}

		
		public void UpdateDim(Drawing.Rectangle bbox)
		{
			//	Met à jour la bbox du groupe.
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

		public override Shape[] ShapesBuild(IPaintPort port, DrawingContext drawingContext, bool simplify)
		{
			//	Constuit les formes de l'objet.
			if ( this.handles.Count == 0 )  return null;

			Path pathCorners = null;

			if ( drawingContext != null &&
				 drawingContext.IsActive &&
				 !drawingContext.IsDimmed &&
				 !drawingContext.PreviewActive )
			{
				pathCorners = this.PathCorners();
			}

			int totalShapes = 1;
			if ( pathCorners != null )  totalShapes ++;

			Shape[] shapes = new Shape[totalShapes];
			int i = 0;
			
			if ( pathCorners != null )
			{
				shapes[i] = new Shape();
				shapes[i].Path = pathCorners;
				shapes[i].Type = Type.Stroke;
				i ++;
			}

			shapes[i] = new Shape();
			shapes[i].Path = this.PathRectangle();
			shapes[i].Type = Type.Surface;
			shapes[i].Aspect = Aspect.InvisibleBox;
			i ++;

			return shapes;
		}

		protected Path PathRectangle()
		{
			//	Crée le chemin d'un rectangle.
			Point p1 = this.Handle(0).Position;
			Point p2 = this.Handle(2).Position;
			Point p3 = this.Handle(1).Position;
			Point p4 = this.Handle(3).Position;

			Path path = new Path();
			path.MoveTo(p1);
			path.LineTo(p2);
			path.LineTo(p3);
			path.LineTo(p4);
			path.Close();

			return path;
		}

		protected Path PathCorners()
		{
			//	Crée le chemin des coins d'un rectangle.
			Point p1 = this.Handle(0).Position;
			Point p2 = this.Handle(2).Position;
			Point p3 = this.Handle(1).Position;
			Point p4 = this.Handle(3).Position;

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


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise l'objet.
			base.GetObjectData(info, context);
		}

		protected Group(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise l'objet.
		}
		#endregion

		
		protected Size				initialBBox;
		protected Size				minimalBBox = Size.Empty;
	}
}
