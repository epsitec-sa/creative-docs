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


		// Nom de l'icône.
		public override string IconName
		{
			get { return @"file:images/ellipse.icon"; }
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
			if ( rank >= 4 )  return this.PropertyArc(4);
			return null;
		}

		// Début de la création d'un objet.
		public override void CreateMouseDown(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainFixStarting(pos, ConstrainType.Line);
			this.HandleAdd(pos, HandleType.Primary);
			this.HandleAdd(pos, HandleType.Primary);
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

			this.UpdateArcHandle();
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateIsExist(IconContext iconContext)
		{
			double len = Drawing.Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > this.minimalSize );
		}

		private void HandleArcChanged(object sender)
		{
			this.UpdateArcHandle();
		}

		// Met à jour les poignées pour l'arc.
		protected void UpdateArcHandle()
		{
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

		// Calcule la géométrie de l'ellipse.
		protected void ComputeGeometry(out Drawing.Point center, out double rx, out double ry, out double angle)
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

			center = new Drawing.Point((p1.X+p2.X+p3.X+p4.X)/4.0, (p1.Y+p2.Y+p3.Y+p4.Y)/4.0);
			rx = (Drawing.Point.Distance(p1,p4) + Drawing.Point.Distance(p2,p3))/4.0;
			ry = (Drawing.Point.Distance(p1,p2) + Drawing.Point.Distance(p3,p4))/4.0;
			angle = Drawing.Point.ComputeAngleDeg(p1,p4);
		}

		// Calcule la position d'une poignée pour l'arc.
		protected Drawing.Point ComputeArcHandle(double angle)
		{
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

		// Calcule l'angle d'après la position de la souris.
		protected double ComputeArcHandle(Drawing.Point pos)
		{
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

		// Crée le chemin d'une ellipse inscrite dans un quadrilatère.
		protected Drawing.Path PathEllipse(IconContext iconContext, Drawing.Point p1, Drawing.Point p2, Drawing.Point p3, Drawing.Point p4)
		{
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

			return this.PathEllipse(iconContext, p1, p2, p3, p4);
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
