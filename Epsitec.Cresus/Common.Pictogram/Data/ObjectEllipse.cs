using Epsitec.Common.Support;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe ObjectEllipse est la classe de l'objet graphique "ellipse".
	/// </summary>
	public class ObjectEllipse : AbstractObject
	{
		public ObjectEllipse()
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

			PropertyArc arc = new PropertyArc();
			arc.Type = PropertyType.Arc;
			arc.Changed += new EventHandler(this.HandleArcChanged);
			this.AddProperty(arc);
		}

		protected override AbstractObject CreateNewObject()
		{
			return new ObjectEllipse();
		}

		public override void Dispose()
		{
			if ( this.ExistProperty(4) )  this.PropertyArc(4).Changed -= new EventHandler(this.HandleArcChanged);
			base.Dispose();
		}


		public override string IconName
		{
			//	Nom de l'ic�ne.
			get { return @"file:images/ellipse.icon"; }
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

			if ( rank < 4 )
			{
				     if ( rank == 0 )  this.MoveCorner(pos, 0, 2,3, 1);
				else if ( rank == 1 )  this.MoveCorner(pos, 1, 3,2, 0);
				else if ( rank == 2 )  this.MoveCorner(pos, 2, 0,1, 3);
				else if ( rank == 3 )  this.MoveCorner(pos, 3, 1,0, 2);
				else                   this.Handle(rank).Position = pos;
			}
			else if ( rank == 4 )
			{
				this.PropertyArc(4).StartingAngle = this.ComputeArcHandle(pos);
			}
			else if ( rank == 5 )
			{
				this.PropertyArc(4).EndingAngle = this.ComputeArcHandle(pos);
			}
			this.UpdateArcHandle();
			this.dirtyBbox = true;
		}

		
		public override bool IsMoveHandlePropertyChanged(int rank)
		{
			//	Indique si le d�placement d'une poign�e doit se r�percuter sur les propri�t�s.
			if ( rank >= this.handles.Count )  // poign�e d'une propri�t� ?
			{
				return base.IsMoveHandlePropertyChanged(rank);
			}
			return ( rank >= 4 );
		}

		public override AbstractProperty MoveHandleProperty(int rank)
		{
			//	Retourne la propri�t� modifi�e en d�pla�ant une poign�e.
			if ( rank >= this.handles.Count )  // poign�e d'une propri�t� ?
			{
				return base.MoveHandleProperty(rank);
			}
			if ( rank >= 4 )  return this.PropertyArc(4);
			return null;
		}

		public override void CreateMouseDown(Drawing.Point pos, IconContext iconContext)
		{
			//	D�but de la cr�ation d'un objet.
			iconContext.ConstrainFixStarting(pos, ConstrainType.Line);
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

			//	Cr�e les 2 autres poign�es dans les coins oppos�s.
			Drawing.Point p1 = this.Handle(0).Position;
			Drawing.Point p2 = this.Handle(1).Position;
			this.HandleAdd(new Drawing.Point(p1.X, p2.Y), HandleType.Primary);  // rang = 2
			this.HandleAdd(new Drawing.Point(p2.X, p1.Y), HandleType.Primary);  // rang = 3

			this.UpdateArcHandle();
		}

		public override bool CreateIsExist(IconContext iconContext)
		{
			//	Indique si l'objet doit exister. Retourne false si l'objet ne peut
			//	pas exister et doit �tre d�truit.
			double len = Drawing.Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > this.minimalSize );
		}

		private void HandleArcChanged(object sender)
		{
			this.UpdateArcHandle();
		}

		protected void UpdateArcHandle()
		{
			//	Met � jour les poign�es pour l'arc.
			if ( this.handles.Count < 4 )  return;
			PropertyArc arc = this.PropertyArc(4);
			if ( arc.ArcType == ArcType.Full )
			{
				if ( this.handles.Count > 4 )
				{
					this.HandleDelete(5);
					this.HandleDelete(4);
				}
			}
			else
			{
				Drawing.Point p4 = this.ComputeArcHandle(arc.StartingAngle);
				Drawing.Point p5 = this.ComputeArcHandle(arc.EndingAngle);

				if ( this.handles.Count == 4 )
				{
					this.HandleAdd(p4, HandleType.Secondary);
					this.HandleAdd(p5, HandleType.Secondary);
				}
				else
				{
					this.Handle(4).Position = p4;
					this.Handle(5).Position = p5;
				}

				this.Handle(4).IsSelected = this.Handle(0).IsSelected;
				this.Handle(5).IsSelected = this.Handle(0).IsSelected;

				for ( int i=4 ; i<6 ; i++ )
				{
					this.GlobalHandleAdapt(i);
				}
			}
		}

	
		protected override void UpdateBoundingBox()
		{
			//	Met � jour le rectangle englobant l'objet.
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

		protected void ComputeGeometry(out Drawing.Point center, out double rx, out double ry, out double angle)
		{
			//	Calcule la g�om�trie de l'ellipse.
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

			center = new Drawing.Point((p1.X+p2.X+p3.X+p4.X)/4.0, (p1.Y+p2.Y+p3.Y+p4.Y)/4.0);
			rx = (Drawing.Point.Distance(p1,p4) + Drawing.Point.Distance(p2,p3))/4.0;
			ry = (Drawing.Point.Distance(p1,p2) + Drawing.Point.Distance(p3,p4))/4.0;
			angle = Drawing.Point.ComputeAngleDeg(p1,p4);
		}

		protected Drawing.Point ComputeArcHandle(double angle)
		{
			//	Calcule la position d'une poign�e pour l'arc.
			Drawing.Point center, p;
			double rx, ry, rot;
			this.ComputeGeometry(out center, out rx, out ry, out rot);

			if ( rx == 0.0 || ry == 0.0 )
			{
				return center;
			}
			else
			{
				p = center;
				p.X += rx;
				p = Drawing.Transform.RotatePointDeg(center, angle, p);
				p.Y = (p.Y-center.Y)*(ry/rx)+center.Y;
				return Drawing.Transform.RotatePointDeg(center, rot, p);
			}
		}

		protected double ComputeArcHandle(Drawing.Point pos)
		{
			//	Calcule l'angle d'apr�s la position de la souris.
			Drawing.Point center, p;
			double rx, ry, rot;
			this.ComputeGeometry(out center, out rx, out ry, out rot);

			if ( rx == 0.0 || ry == 0.0 )
			{
				return 0.0;
			}
			else
			{
				p = Drawing.Transform.RotatePointDeg(center, -rot, pos);
				p.Y = (p.Y-center.Y)/(ry/rx)+center.Y;
				double angle = Drawing.Point.ComputeAngleDeg(center, p);
				if ( angle < 0.0 )  angle += 360.0;  // 0..360
				return angle;
			}
		}

		protected Drawing.Path PathEllipse(IconContext iconContext, Drawing.Point p1, Drawing.Point p2, Drawing.Point p3, Drawing.Point p4)
		{
			//	Cr�e le chemin d'une ellipse inscrite dans un quadrilat�re.
#if true
			Drawing.Point center;
			double rx, ry, rot;
			this.ComputeGeometry(out center, out rx, out ry, out rot);

			double zoom = AbstractProperty.DefaultZoom(iconContext);
			Drawing.Path rightPath = new Drawing.Path();
			rightPath.DefaultZoom = zoom;

			PropertyArc arc = this.PropertyArc(4);
			double a1, a2;
			if ( arc.ArcType == ArcType.Full )
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
				rightPath.ArcDeg(center, rx, ry, a1, a2, true);
			}

			if ( arc.ArcType == ArcType.Close )
			{
				rightPath.Close();
			}
			if ( arc.ArcType == ArcType.Pie )
			{
				rightPath.LineTo(center);
				rightPath.Close();
			}

			Drawing.Path rotPath = new Drawing.Path();
			rotPath.DefaultZoom = zoom;
			Drawing.Transform rotate = new Drawing.Transform();
			rotate.RotateDeg(rot, center);
			rotPath.Append(rightPath, rotate, zoom);
			rightPath.Dispose();
			return rotPath;
#else
			Drawing.Point p12 = (p1+p2)/2;
			Drawing.Point p23 = (p2+p3)/2;
			Drawing.Point p34 = (p3+p4)/2;
			Drawing.Point p41 = (p4+p1)/2;

			Drawing.Path path = new Drawing.Path();

			if ( iconContext == null )
			{
				path.DefaultZoom = 10.0;
			}
			else
			{
				path.DefaultZoom = iconContext.ScaleX;
			}

			path.MoveTo(p12);
			path.CurveTo(Drawing.Point.Scale(p12, p2, 0.56), Drawing.Point.Scale(p23, p2, 0.56), p23);
			path.CurveTo(Drawing.Point.Scale(p23, p3, 0.56), Drawing.Point.Scale(p34, p3, 0.56), p34);
			path.CurveTo(Drawing.Point.Scale(p34, p4, 0.56), Drawing.Point.Scale(p41, p4, 0.56), p41);
			path.CurveTo(Drawing.Point.Scale(p41, p1, 0.56), Drawing.Point.Scale(p12, p1, 0.56), p12);
			path.Close();
			return path;
#endif
		}

		protected Drawing.Path PathBuild(IconContext iconContext)
		{
			//	Cr�e le chemin de l'objet.
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

			return this.PathEllipse(iconContext, p1, p2, p3, p4);
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
