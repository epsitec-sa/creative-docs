using Epsitec.Common.Support;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe ObjectRegular est la classe de l'objet graphique "polygone r�gulier".
	/// </summary>
	public class ObjectRegular : AbstractObject
	{
		public ObjectRegular()
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

			PropertyRegular regular = new PropertyRegular();
			regular.Type = PropertyType.Regular;
			regular.Changed += new EventHandler(this.HandleRegularChanged);
			this.AddProperty(regular);

			PropertyCorner corner = new PropertyCorner();
			corner.Type = PropertyType.Corner;
			this.AddProperty(corner);
		}

		protected override AbstractObject CreateNewObject()
		{
			return new ObjectRegular();
		}

		public override void Dispose()
		{
			if ( this.ExistProperty(4) )  this.PropertyRegular(4).Changed -= new EventHandler(this.HandleRegularChanged);
			base.Dispose();
		}


		public override string IconName
		{
			//	Nom de l'ic�ne.
			get { return @"file:images/regular.icon"; }
		}


		public override bool Detect(Drawing.Point pos)
		{
			//	D�tecte si la souris est sur l'objet.
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

		public override bool Detect(Drawing.Rectangle rect, bool all)
		{
			//	D�tecte si l'objet est dans un rectangle.
			if ( this.isHide )  return false;

			Drawing.Rectangle fullBbox = this.BoundingBox;
			double width = System.Math.Max(this.PropertyLine(1).PatternWidth/2, this.minimalWidth);
			fullBbox.Inflate(width, width);
			return rect.Contains(fullBbox);
		}


		public override void MoveHandleProcess(int rank, Drawing.Point pos, IconContext iconContext)
		{
			//	D�place une poign�e.
			if ( rank >= this.handles.Count )  // poign�e d'une propri�t� ?
			{
				base.MoveHandleProcess(rank, pos, iconContext);
				return;
			}

			iconContext.ConstrainSnapPos(ref pos);
			iconContext.SnapGrid(ref pos);

			if ( rank == 0 )  // centre ?
			{
				Drawing.Point move = pos-this.Handle(rank).Position;
				this.Handle(0).Position = pos;
				this.Handle(1).Position += move;
			}
			else if ( rank == 1 )  // extr�mit� ?
			{
				this.Handle(1).Position = pos;
			}
			else if ( rank == 2 )  // renfoncement de l'�toile ?
			{
				double d1 = Drawing.Point.Distance(this.Handle(1).Position, this.Handle(0).Position);
				double d2 = Drawing.Point.Distance(this.Handle(1).Position, pos);
				this.PropertyRegular(4).Deep = d2/d1;
			}
			this.UpdateRegularHandle();
			this.dirtyBbox = true;
		}

		public override bool IsMoveHandlePropertyChanged(int rank)
		{
			//	Indique si le d�placement d'une poign�e doit se r�percuter sur les propri�t�s.
			if ( rank >= this.handles.Count )  // poign�e d'une propri�t� ?
			{
				return base.IsMoveHandlePropertyChanged(rank);
			}
			return ( rank >= 2 );
		}

		public override AbstractProperty MoveHandleProperty(int rank)
		{
			//	Retourne la propri�t� modifi�e en d�pla�ant une poign�e.
			if ( rank >= this.handles.Count )  // poign�e d'une propri�t� ?
			{
				return base.MoveHandleProperty(rank);
			}
			if ( rank >= 2 )  return this.PropertyRegular(4);
			return null;
		}


		public override void CreateMouseDown(Drawing.Point pos, IconContext iconContext)
		{
			//	D�but de la cr�ation d'un objet.
			iconContext.ConstrainFixStarting(pos);
			this.HandleAdd(pos, HandleType.Primary);
			this.HandleAdd(pos, HandleType.Primary);
		}

		public override void CreateMouseMove(Drawing.Point pos, IconContext iconContext)
		{
			//	D�placement pendant la cr�ation d'un objet.
			iconContext.ConstrainSnapPos(ref pos);
			iconContext.SnapGrid(ref pos);
			this.Handle(1).Position = pos;
			this.dirtyBbox = true;
		}

		public override void CreateMouseUp(Drawing.Point pos, IconContext iconContext)
		{
			//	Fin de la cr�ation d'un objet.
			iconContext.ConstrainSnapPos(ref pos);
			iconContext.SnapGrid(ref pos);
			this.Handle(1).Position = pos;
			iconContext.ConstrainDelStarting();
			this.UpdateRegularHandle();
		}

		public override bool CreateIsExist(IconContext iconContext)
		{
			//	Indique si l'objet doit exister. Retourne false si l'objet ne peut
			//	pas exister et doit �tre d�truit.
			double len = Drawing.Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > this.minimalSize );
		}


		private void HandleRegularChanged(object sender)
		{
			this.UpdateRegularHandle();
		}

		protected void UpdateRegularHandle()
		{
			//	Met � jour la poign�e pour la profondeur de l'�toile.
			if ( this.handles.Count < 2 )  return;
			PropertyRegular reg = this.PropertyRegular(4);
			if ( !reg.Star )  // polygone ?
			{
				if ( this.handles.Count > 2 )
				{
					this.HandleDelete(2);
				}
			}
			else	// �toile ?
			{
				Drawing.Point pos = Drawing.Point.Scale(this.Handle(1).Position, this.Handle(0).Position, reg.Deep);

				if ( this.handles.Count == 2 )
				{
					this.HandleAdd(pos, HandleType.Secondary);
				}
				else
				{
					this.Handle(2).Position = pos;
				}
				this.Handle(2).IsSelected = this.Handle(1).IsSelected;
				this.GlobalHandleAdapt(2);
			}
		}

		
		protected override void UpdateBoundingBox()
		{
			//	Met � jour le rectangle englobant l'objet.
			Drawing.Path path = this.PathBuild(null);
			this.bboxThin = path.ComputeBounds();

			this.bboxGeom = this.bboxThin;
			this.bboxGeom.MergeWith(this.Handle(1).Position);
			this.PropertyLine(1).InflateBoundingBox(ref this.bboxGeom);

			this.bboxFull = this.bboxGeom;
			this.bboxGeom.MergeWith(this.PropertyGradient(3).BoundingBoxGeom(this.bboxThin));
			this.bboxFull.MergeWith(this.PropertyGradient(3).BoundingBoxFull(this.bboxThin));
			this.bboxFull.MergeWith(this.bboxGeom);
		}

		protected bool ComputeLine(int i, out Drawing.Point a, out Drawing.Point b)
		{
			//	Calcule une droite de l'objet.
			int total = this.PropertyRegular(4).NbFaces;
			Drawing.Point center = this.Handle(0).Position;
			Drawing.Point corner = this.Handle(1).Position;

			a = new Drawing.Point(0, 0);
			b = new Drawing.Point(0, 0);

			if ( this.PropertyRegular(4).Star )  // �toile ?
			{
				if ( i >= total*2 )  return false;

				Drawing.Point star = center + (corner-center)*(1-this.PropertyRegular(4).Deep);
				a = Drawing.Transform.RotatePointDeg(center, 360.0*(i+0)/(total*2), (i%2==0) ? corner : star);
				b = Drawing.Transform.RotatePointDeg(center, 360.0*(i+1)/(total*2), (i%2==0) ? star : corner);
				return true;
			}
			else	// polygone ?
			{
				if ( i >= total )  return false;

				a = Drawing.Transform.RotatePointDeg(center, 360.0*(i+0)/total, corner);
				b = Drawing.Transform.RotatePointDeg(center, 360.0*(i+1)/total, corner);
				return true;
			}
		}

		protected Drawing.Path PathBuild(IconContext iconContext)
		{
			//	Cr�e le chemin d'un polygone r�gulier.
			Drawing.Path path = new Drawing.Path();
			path.DefaultZoom = AbstractProperty.DefaultZoom(iconContext);;

			int total = this.PropertyRegular(4).NbFaces;
			PropertyCorner corner = this.PropertyCorner(5);

			if ( corner.CornerType == CornerType.Right )  // coins droits ?
			{
				Drawing.Point a;
				Drawing.Point b;

				if ( this.PropertyRegular(4).Star )  total *= 2;  // �toile ?

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
				Drawing.Point p1;
				Drawing.Point s;
				Drawing.Point p2;

				if ( this.PropertyRegular(4).Star )  total *= 2;  // �toile ?

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

		protected void PathCorner(Drawing.Path path, Drawing.Point p1, Drawing.Point s, Drawing.Point p2, PropertyCorner corner)
		{
			//	Cr�e le chemin d'un coin.
			double l1 = Drawing.Point.Distance(p1, s);
			double l2 = Drawing.Point.Distance(p2, s);
			double radius = System.Math.Min(corner.Radius, System.Math.Min(l1,l2)/2);
			Drawing.Point c1 = Drawing.Point.Move(s, p1, radius);
			Drawing.Point c2 = Drawing.Point.Move(s, p2, radius);
			if ( path.IsEmpty )  path.MoveTo(c1);
			else                 path.LineTo(c1);
			corner.PathCorner(path, c1,s,c2, radius);
		}

		public override void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext, IconObjects iconObjects)
		{
			//	Dessine l'objet.
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

		public override void PrintGeometry(Printing.PrintPort port, IconContext iconContext, IconObjects iconObjects)
		{
			//	Imprime l'objet.
			base.PrintGeometry(port, iconContext, iconObjects);

			if ( this.TotalHandle < 2 )  return;

			Drawing.Path path = this.PathBuild(iconContext);

			if ( this.PropertyGradient(3).PaintColor(port, iconContext) )
			{
				port.PaintSurface(path);
			}

			if ( this.PropertyColor(2).PaintColor(port, iconContext) )
			{
				this.PropertyLine(1).PaintOutline(port, iconContext, path);
			}
		}
	}
}
