using Epsitec.Common.Support;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe ObjectRectangle est la classe de l'objet graphique "rectangle".
	/// </summary>
	public class ObjectRectangle : AbstractObject
	{
		public ObjectRectangle()
		{
			PropertyName name = new PropertyName();
			name.Type = PropertyType.Name;
			this.AddProperty(name);

			PropertyLine lineMode = new PropertyLine();
			lineMode.Type = PropertyType.LineMode;
			this.AddProperty(lineMode);

			PropertyColor lineColor = new PropertyColor();
			lineColor.Type = PropertyType.LineColor;
			this.AddProperty(lineColor);

			PropertyGradient fillGradient = new PropertyGradient();
			fillGradient.Type = PropertyType.FillGradient;
			this.AddProperty(fillGradient);

			PropertyCorner corner = new PropertyCorner();
			corner.Type = PropertyType.Corner;
			corner.Changed += new EventHandler(this.HandleCornerChanged);
			this.AddProperty(corner);
		}

		protected override AbstractObject CreateNewObject()
		{
			return new ObjectRectangle();
		}

		public override void Dispose()
		{
			if ( this.ExistProperty(4) )  this.PropertyCorner(4).Changed -= new EventHandler(this.HandleCornerChanged);
			base.Dispose();
		}


		// Nom de l'icône.
		public override string IconName
		{
			get { return @"file:images/rectangle.icon"; }
		}


		// Détecte si la souris est sur l'objet.
		public override bool Detect(Drawing.Point pos)
		{
			if ( this.isHide )  return false;

			Drawing.Rectangle bbox = this.BoundingBox;
			if ( !bbox.Contains(pos) )  return false;

			Drawing.Path path = this.PathBuild(null);

			double width = System.Math.Max(this.PropertyLine(1).PatternWidth/2, this.minimalWidth);
			if ( AbstractObject.DetectOutline(path, width, pos) )  return true;
			
			if ( this.PropertyGradient(3).IsVisible() )
			{
				path.Close();
				if ( AbstractObject.DetectSurface(path, pos) )  return true;
			}
			return false;
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

			if ( rank < 4 )
			{
				     if ( rank == 0 )  this.MoveCorner(pos, 0, 2,3, 1);
				else if ( rank == 1 )  this.MoveCorner(pos, 1, 3,2, 0);
				else if ( rank == 2 )  this.MoveCorner(pos, 2, 0,1, 3);
				else if ( rank == 3 )  this.MoveCorner(pos, 3, 1,0, 2);
				else                   this.Handle(rank).Position = pos;

			}
			else if ( rank == 4 || rank == 5 )
			{
				this.PropertyCorner(4).Radius = Drawing.Point.Distance(this.Handle(0).Position, pos);
			}
			else if ( rank == 6 || rank == 7 )
			{
				this.PropertyCorner(4).Radius = Drawing.Point.Distance(this.Handle(2).Position, pos);
			}
			else if ( rank == 8 || rank == 9 )
			{
				this.PropertyCorner(4).Radius = Drawing.Point.Distance(this.Handle(1).Position, pos);
			}
			else if ( rank == 10 || rank == 11 )
			{
				this.PropertyCorner(4).Radius = Drawing.Point.Distance(this.Handle(3).Position, pos);
			}
			this.UpdateCornerHandle();
			this.dirtyBbox = true;
		}

		// Indique si le déplacement d'une poignée doit se répercuter sur les propriétés.
		public override bool IsMoveHandlePropertyChanged(int rank)
		{
			if ( rank >= this.handles.Count )  // poignée d'une propriété ?
			{
				return base.IsMoveHandlePropertyChanged(rank);
			}
			return ( rank >= 4 );
		}

		// Retourne la propriété modifiée en déplaçant une poignée.
		public override AbstractProperty MoveHandleProperty(int rank)
		{
			if ( rank >= this.handles.Count )  // poignée d'une propriété ?
			{
				return base.MoveHandleProperty(rank);
			}
			if ( rank >= 4 )  return this.PropertyCorner(4);
			return null;
		}

		// Début de la création d'un objet.
		public override void CreateMouseDown(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainFixStarting(pos, ConstrainType.Square);
			this.HandleAdd(pos, HandleType.Primary);  // rang = 0
			this.HandleAdd(pos, HandleType.Primary);  // rang = 1
		}

		// Déplacement pendant la création d'un objet.
		public override void CreateMouseMove(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);
			iconContext.SnapGrid(ref pos);
			this.Handle(1).Position = pos;
			this.dirtyBbox = true;
		}

		// Fin de la création d'un objet.
		public override void CreateMouseUp(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);
			iconContext.SnapGrid(ref pos);
			this.Handle(1).Position = pos;
			iconContext.ConstrainDelStarting();

			// Crée les 2 autres poignées dans les coins opposés.
			Drawing.Point p1 = this.Handle(0).Position;
			Drawing.Point p2 = this.Handle(1).Position;
			this.HandleAdd(new Drawing.Point(p1.X, p2.Y), HandleType.Primary);  // rang = 2
			this.HandleAdd(new Drawing.Point(p2.X, p1.Y), HandleType.Primary);  // rang = 3

			this.UpdateCornerHandle();
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateIsExist(IconContext iconContext)
		{
			double len = Drawing.Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > this.minimalSize );
		}

		private void HandleCornerChanged(object sender)
		{
			this.UpdateCornerHandle();
		}

		// Met à jour les poignées pour les coins.
		//	  2   7          8   1
		//	   o--o----------o--o
		//	   |                |
		//	 6 o                o 9
		//	   |                |
		//	   |                |
		//	   |                |
		//	 5 o                o 10
		//	   |                |
		//	   o--o----------o--o
		//	  0   4          11  3
		protected void UpdateCornerHandle()
		{
			if ( this.handles.Count < 4 )  return;
			PropertyCorner corner = this.PropertyCorner(4);
			if ( corner.CornerType == CornerType.Right )
			{
				if ( this.handles.Count > 4 )
				{
					this.HandleDelete(11);
					this.HandleDelete(10);
					this.HandleDelete(9);
					this.HandleDelete(8);
					this.HandleDelete(7);
					this.HandleDelete(6);
					this.HandleDelete(5);
					this.HandleDelete(4);
				}
			}
			else
			{
				Drawing.Point p4  = Drawing.Point.Move(this.Handle(0).Position, this.Handle(3).Position, corner.Radius);
				Drawing.Point p5  = Drawing.Point.Move(this.Handle(0).Position, this.Handle(2).Position, corner.Radius);
				Drawing.Point p6  = Drawing.Point.Move(this.Handle(2).Position, this.Handle(0).Position, corner.Radius);
				Drawing.Point p7  = Drawing.Point.Move(this.Handle(2).Position, this.Handle(1).Position, corner.Radius);
				Drawing.Point p8  = Drawing.Point.Move(this.Handle(1).Position, this.Handle(2).Position, corner.Radius);
				Drawing.Point p9  = Drawing.Point.Move(this.Handle(1).Position, this.Handle(3).Position, corner.Radius);
				Drawing.Point p10 = Drawing.Point.Move(this.Handle(3).Position, this.Handle(1).Position, corner.Radius);
				Drawing.Point p11 = Drawing.Point.Move(this.Handle(3).Position, this.Handle(0).Position, corner.Radius);

				if ( this.handles.Count == 4 )
				{
					this.HandleAdd(p4,  HandleType.Secondary);
					this.HandleAdd(p5,  HandleType.Secondary);
					this.HandleAdd(p6,  HandleType.Secondary);
					this.HandleAdd(p7,  HandleType.Secondary);
					this.HandleAdd(p8,  HandleType.Secondary);
					this.HandleAdd(p9,  HandleType.Secondary);
					this.HandleAdd(p10, HandleType.Secondary);
					this.HandleAdd(p11, HandleType.Secondary);
				}
				else
				{
					this.Handle(4).Position  = p4;
					this.Handle(5).Position  = p5;
					this.Handle(6).Position  = p6;
					this.Handle(7).Position  = p7;
					this.Handle(8).Position  = p8;
					this.Handle(9).Position  = p9;
					this.Handle(10).Position = p10;
					this.Handle(11).Position = p11;
				}

				this.Handle(4).IsSelected  = this.Handle(0).IsSelected && corner.Radius > 0;
				this.Handle(5).IsSelected  = this.Handle(0).IsSelected && corner.Radius > 0;
				this.Handle(6).IsSelected  = this.Handle(2).IsSelected && corner.Radius > 0;
				this.Handle(7).IsSelected  = this.Handle(2).IsSelected && corner.Radius > 0;
				this.Handle(8).IsSelected  = this.Handle(1).IsSelected && corner.Radius > 0;
				this.Handle(9).IsSelected  = this.Handle(1).IsSelected && corner.Radius > 0;
				this.Handle(10).IsSelected = this.Handle(3).IsSelected && corner.Radius > 0;
				this.Handle(11).IsSelected = this.Handle(3).IsSelected && corner.Radius > 0;

				for ( int i=4 ; i<12 ; i++ )
				{
					this.GlobalHandleAdapt(i);
				}
			}
		}

		
		// Met à jour le rectangle englobant l'objet.
		protected override void UpdateBoundingBox()
		{
			Drawing.Path path = this.PathBuild(null);
			this.bboxThin = AbstractObject.ComputeBoundingBox(path);

			this.bboxGeom = this.bboxThin;
			this.PropertyLine(1).InflateBoundingBox(ref this.bboxGeom);
			this.bboxGeom.MergeWith(this.PropertyGradient(3).BoundingBoxGeom(this.bboxThin));

			this.bboxFull = this.bboxGeom;
			if ( this.TotalHandle >= 4 )
			{
				this.bboxFull.MergeWith(this.Handle(0).Position);
				this.bboxFull.MergeWith(this.Handle(1).Position);
				this.bboxFull.MergeWith(this.Handle(2).Position);
				this.bboxFull.MergeWith(this.Handle(3).Position);
			}
			this.bboxFull.MergeWith(this.PropertyGradient(3).BoundingBoxFull(this.bboxThin));
		}

		// Crée le chemin de l'objet.
		protected Drawing.Path PathBuild(IconContext iconContext)
		{
			Drawing.Point p1 = this.Handle(0).Position;
			Drawing.Point p2 = new Drawing.Point();
			Drawing.Point p3 = this.Handle(1).Position;
			Drawing.Point p4 = new Drawing.Point();

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

			PropertyCorner corner = this.PropertyCorner(4);
			return this.PathCornerRectangle(iconContext, p1, p2, p3, p4, corner);
		}

		// Crée le chemin d'un rectangle à coins quelconques.
		protected Drawing.Path PathCornerRectangle(IconContext iconContext, Drawing.Point p1, Drawing.Point p2, Drawing.Point p3, Drawing.Point p4, PropertyCorner corner)
		{
			double d12 = Drawing.Point.Distance(p1, p2);
			double d23 = Drawing.Point.Distance(p2, p3);
			double d34 = Drawing.Point.Distance(p3, p4);
			double d41 = Drawing.Point.Distance(p4, p1);
			double min = System.Math.Min(System.Math.Min(d12, d23), System.Math.Min(d34, d41));
			double radius = System.Math.Min(corner.Radius, min/2);

			Drawing.Path path = new Drawing.Path();
			
			if ( iconContext == null )
			{
				path.DefaultZoom = 10.0;
			}
			else
			{
				path.DefaultZoom = iconContext.ScaleX;
			}

			if ( corner.CornerType == CornerType.Right || radius == 0 )
			{
				path.MoveTo(p1);
				path.LineTo(p2);
				path.LineTo(p3);
				path.LineTo(p4);
				path.Close();
			}
			else
			{
				Drawing.Point c1 = new Drawing.Point();
				Drawing.Point c2 = new Drawing.Point();

				c1 = Drawing.Point.Move(p1, p4, radius);
				c2 = Drawing.Point.Move(p1, p2, radius);
				path.MoveTo(c1);
				corner.PathCorner(path, c1, p1, c2, radius);

				c1 = Drawing.Point.Move(p2, p1, radius);
				c2 = Drawing.Point.Move(p2, p3, radius);
				path.LineTo(c1);
				corner.PathCorner(path, c1, p2, c2, radius);

				c1 = Drawing.Point.Move(p3, p2, radius);
				c2 = Drawing.Point.Move(p3, p4, radius);
				path.LineTo(c1);
				corner.PathCorner(path, c1, p3, c2, radius);

				c1 = Drawing.Point.Move(p4, p3, radius);
				c2 = Drawing.Point.Move(p4, p1, radius);
				path.LineTo(c1);
				corner.PathCorner(path, c1, p4, c2, radius);

				path.Close();
			}
			return path;
		}

		// Dessine l'objet.
		public override void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext, IconObjects iconObjects)
		{
			if ( base.IsFullHide(iconContext) )  return;
			base.DrawGeometry(graphics, iconContext, iconObjects);

			if ( this.TotalHandle < 2 )  return;

			Drawing.Path path = this.PathBuild(iconContext);
			this.PropertyGradient(3).Render(graphics, iconContext, path, this.BoundingBoxThin);

			this.PropertyLine(1).DrawPath(graphics, iconContext, iconObjects, path, this.PropertyColor(2).Color);

			if ( this.IsHilite && iconContext.IsEditable )
			{
				if ( this.PropertyGradient(3).IsVisible() )
				{
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(iconContext.HiliteSurfaceColor);
				}

				this.PropertyLine(1).AddOutline(graphics, path, iconContext.HiliteSize);
				graphics.RenderSolid(iconContext.HiliteOutlineColor);
			}
		}
	}
}
