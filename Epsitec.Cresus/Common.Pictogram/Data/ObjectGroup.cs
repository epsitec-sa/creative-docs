using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe ObjectGroup est la classe de l'objet graphique "groupe".
	/// </summary>
	public class ObjectGroup : AbstractObject
	{
		public ObjectGroup()
		{
			PropertyName name = new PropertyName();
			name.Type = PropertyType.Name;
			this.AddProperty(name);

			this.objects = new UndoList();
		}

		protected override AbstractObject CreateNewObject()
		{
			return new ObjectGroup();
		}


		// Nom de l'icône.
		public override string IconName
		{
			get { return @"file:images/objgroup.icon"; }
		}


		// Détecte si la souris est sur l'objet.
		public override bool Detect(Drawing.Point pos)
		{
			if ( this.isHide )  return false;

			InsideSurface inside = new InsideSurface(pos, 4);
			inside.AddLine(this.Handle(0).Position, this.Handle(2).Position);
			inside.AddLine(this.Handle(2).Position, this.Handle(1).Position);
			inside.AddLine(this.Handle(1).Position, this.Handle(3).Position);
			inside.AddLine(this.Handle(3).Position, this.Handle(0).Position);
			return inside.IsInside();
		}


		// Début du déplacement une poignée.
		public override void MoveHandleStarting(int rank, Drawing.Point pos, IconContext iconContext)
		{
			this.initialBBox = this.BoundingBox.Size;
		}

		// Déplace une poignée.
		public override void MoveHandleProcess(int rank, Drawing.Point pos, IconContext iconContext)
		{
			if ( rank >= this.handles.Count )  // poignée d'une propriété ?
			{
				base.MoveHandleProcess(rank, pos, iconContext);
				return;
			}

			iconContext.ConstrainSnapPos(ref pos);
			iconContext.SnapGrid(ref pos);

			GlobalModifierData initial = new GlobalModifierData();
			GlobalModifierData final = new GlobalModifierData();
			if ( rank == 0 )
			{
				initial.P1 = this.Handle(1).Position;
				initial.P2 = this.Handle(0).Position;
			}
			if ( rank == 1 )
			{
				initial.P1 = this.Handle(0).Position;
				initial.P2 = this.Handle(1).Position;
			}
			if ( rank == 2 )
			{
				initial.P1 = this.Handle(3).Position;
				initial.P2 = this.Handle(2).Position;
			}
			if ( rank == 3 )
			{
				initial.P1 = this.Handle(2).Position;
				initial.P2 = this.Handle(3).Position;
			}
			final.P1 = initial.P1;
			final.P2 = pos;
			this.MoveHandleSoon(this.objects, initial, final);

			if ( AbstractObject.IsRectangular(this.Handle(0).Position, this.Handle(1).Position, this.Handle(2).Position, this.Handle(3).Position) )
			{
				this.Handle(rank).Position = pos;

				if ( rank == 0 )
				{
					this.Handle(2).Position = Drawing.Point.Projection(this.Handle(2).Position, this.Handle(1).Position, pos);
					this.Handle(3).Position = Drawing.Point.Projection(this.Handle(3).Position, this.Handle(1).Position, pos);
				}
				if ( rank == 1 )
				{
					this.Handle(2).Position = Drawing.Point.Projection(this.Handle(2).Position, this.Handle(0).Position, pos);
					this.Handle(3).Position = Drawing.Point.Projection(this.Handle(3).Position, this.Handle(0).Position, pos);
				}
				if ( rank == 2 )
				{
					this.Handle(0).Position = Drawing.Point.Projection(this.Handle(0).Position, this.Handle(3).Position, pos);
					this.Handle(1).Position = Drawing.Point.Projection(this.Handle(1).Position, this.Handle(3).Position, pos);
				}
				if ( rank == 3 )
				{
					this.Handle(0).Position = Drawing.Point.Projection(this.Handle(0).Position, this.Handle(2).Position, pos);
					this.Handle(1).Position = Drawing.Point.Projection(this.Handle(1).Position, this.Handle(2).Position, pos);
				}
			}
			else
			{
				this.Handle(rank).Position = pos;
			}

			this.minimalBBox = this.initialBBox;
			this.dirtyBbox = true;
		}

		// Déplace tous les objets du groupe.
		protected void MoveHandleSoon(UndoList objects,
									  GlobalModifierData initial, GlobalModifierData final)
		{
			if ( objects != null && objects.Count != 0 )
			{
				foreach ( AbstractObject obj in objects )
				{
					obj.MoveGlobal(initial, final, true);

					if ( obj.Objects != null && obj.Objects.Count > 0 )
					{
						this.MoveHandleSoon(obj.Objects, initial, final);
					}
				}
			}
		}

		
		// Met à jour la bbox du groupe.
		public void UpdateDim(Drawing.Rectangle bbox)
		{
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
			this.bboxGeom = bbox;
			this.bboxFull = bbox;
			this.dirtyBbox = false;
		}

		// Met à jour le rectangle englobant l'objet.
		protected override void UpdateBoundingBox()
		{
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

				this.minimalBBox = Drawing.Size.Empty;
			}

			this.bboxFull = this.bboxGeom;
		}

		// Crée le chemin d'un rectangle.
		protected Drawing.Path PathRectangle(Drawing.Point p1, Drawing.Point p2, Drawing.Point p3, Drawing.Point p4)
		{
			Drawing.Path path = new Drawing.Path();
			path.MoveTo(p1);
			path.LineTo(p2);
			path.LineTo(p3);
			path.LineTo(p4);
			path.Close();
			return path;
		}

		// Crée le chemin des coins d'un rectangle.
		protected Drawing.Path PathCorners(Drawing.Point p1, Drawing.Point p2, Drawing.Point p3, Drawing.Point p4)
		{
			double d12 = Drawing.Point.Distance(p1, p2)*0.25;
			double d23 = Drawing.Point.Distance(p2, p3)*0.25;
			double d34 = Drawing.Point.Distance(p3, p4)*0.25;
			double d41 = Drawing.Point.Distance(p4, p1)*0.25;

			Drawing.Path path = new Drawing.Path();

			path.MoveTo(Drawing.Point.Move(p1, p4, d41));
			path.LineTo(p1);
			path.LineTo(Drawing.Point.Move(p1, p2, d12));

			path.MoveTo(Drawing.Point.Move(p2, p1, d12));
			path.LineTo(p2);
			path.LineTo(Drawing.Point.Move(p2, p3, d23));

			path.MoveTo(Drawing.Point.Move(p3, p2, d23));
			path.LineTo(p3);
			path.LineTo(Drawing.Point.Move(p3, p4, d34));

			path.MoveTo(Drawing.Point.Move(p4, p3, d34));
			path.LineTo(p4);
			path.LineTo(Drawing.Point.Move(p4, p1, d41));

			return path;
		}

		// Dessine l'objet.
		public override void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext, IconObjects iconObjects)
		{
			if ( base.IsFullHide(iconContext) )  return;
			base.DrawGeometry(graphics, iconContext, iconObjects);

			if ( this.TotalHandle < 2 )  return;
			if ( !iconContext.IsEditable || iconContext.IsDimmed )  return;

			Drawing.Point p1 = this.Handle(0).Position;
			Drawing.Point p2 = this.Handle(2).Position;
			Drawing.Point p3 = this.Handle(1).Position;
			Drawing.Point p4 = this.Handle(3).Position;
			Drawing.Path path = this.PathCorners(p1, p2, p3, p4);

			if ( !iconContext.PreviewActive )
			{
				Drawing.Color color = Drawing.Color.FromBrightness(0.7);
				if ( this.IsSelected() )  color = Drawing.Color.FromRGB(1,0,0);

				graphics.Rasterizer.AddOutline(path, 1.0/iconContext.ScaleX);
				graphics.RenderSolid(color);
			}

			if ( this.IsHilite )
			{
				path = this.PathRectangle(p1, p2, p3, p4);
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(iconContext.HiliteSurfaceColor);
			}
		}


		protected Drawing.Size				initialBBox;
		protected Drawing.Size				minimalBBox = Drawing.Size.Empty;
	}
}
