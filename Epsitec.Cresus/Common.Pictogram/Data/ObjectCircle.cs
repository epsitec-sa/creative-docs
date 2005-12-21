using Epsitec.Common.Support;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe ObjectCircle est la classe de l'objet graphique "cercle".
	/// </summary>
	public class ObjectCircle : AbstractObject
	{
		public ObjectCircle()
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
			return new ObjectCircle();
		}

		public override void Dispose()
		{
			if ( this.ExistProperty(4) )  this.PropertyArc(4).Changed -= new EventHandler(this.HandleArcChanged);
			base.Dispose();
		}


		public override string IconName
		{
			//	Nom de l'icône.
			get { return @"file:images/circle.icon"; }
		}


		public override bool Detect(Drawing.Point pos)
		{
			//	Détecte si la souris est sur l'objet.
			if ( this.isHide )  return false;

			Drawing.Point p1 = this.Handle(0).Position;
			Drawing.Point p2 = this.Handle(1).Position;
			double width = System.Math.Max(this.PropertyLine(1).PatternWidth/2, this.minimalWidth);
			double radius = Drawing.Point.Distance(p1, p2)+width;
			double dist = Drawing.Point.Distance(p1, pos);

			if ( this.PropertyGradient(3).IsVisible() )
			{
				return ( dist <= radius );
			}
			else
			{
				return ( dist <= radius && dist >= radius-width*2 );
			}
		}


		public override void MoveHandleProcess(int rank, Drawing.Point pos, IconContext iconContext)
		{
			//	Déplace une poignée.
			if ( rank >= this.handles.Count )  // poignée d'une propriété ?
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
			else if ( rank == 1 )  // extrémité ?
			{
				this.Handle(1).Position = pos;
			}
			else if ( rank == 2 )
			{
				this.PropertyArc(4).StartingAngle = this.ComputeArcHandle(pos);
			}
			else if ( rank == 3 )
			{
				this.PropertyArc(4).EndingAngle = this.ComputeArcHandle(pos);
			}
			this.UpdateArcHandle();
			this.dirtyBbox = true;
		}


		public override bool IsMoveHandlePropertyChanged(int rank)
		{
			//	Indique si le déplacement d'une poignée doit se répercuter sur les propriétés.
			if ( rank >= this.handles.Count )  // poignée d'une propriété ?
			{
				return base.IsMoveHandlePropertyChanged(rank);
			}
			return ( rank >= 2 );
		}

		public override AbstractProperty MoveHandleProperty(int rank)
		{
			//	Retourne la propriété modifiée en déplaçant une poignée.
			if ( rank >= this.handles.Count )  // poignée d'une propriété ?
			{
				return base.MoveHandleProperty(rank);
			}
			if ( rank >= 2 )  return this.PropertyArc(4);
			return null;
		}


		public override void CreateMouseDown(Drawing.Point pos, IconContext iconContext)
		{
			//	Début de la création d'un objet.
			iconContext.ConstrainFixStarting(pos, ConstrainType.Line);
			this.HandleAdd(pos, HandleType.Primary);
			this.HandleAdd(pos, HandleType.Primary);
		}

		public override void CreateMouseMove(Drawing.Point pos, IconContext iconContext)
		{
			//	Déplacement pendant la création d'un objet.
			iconContext.ConstrainSnapPos(ref pos);
			iconContext.SnapGrid(ref pos);
			this.Handle(1).Position = pos;
			this.dirtyBbox = true;
		}

		public override void CreateMouseUp(Drawing.Point pos, IconContext iconContext)
		{
			//	Fin de la création d'un objet.
			iconContext.ConstrainSnapPos(ref pos);
			iconContext.SnapGrid(ref pos);
			this.Handle(1).Position = pos;
			iconContext.ConstrainDelStarting();

			this.UpdateArcHandle();
		}

		public override bool CreateIsExist(IconContext iconContext)
		{
			//	Indique si l'objet doit exister. Retourne false si l'objet ne peut
			//	pas exister et doit être détruit.
			double len = Drawing.Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > this.minimalSize );
		}

		private void HandleArcChanged(object sender)
		{
			this.UpdateArcHandle();
		}

		protected void UpdateArcHandle()
		{
			//	Met à jour les poignées pour l'arc.
			if ( this.handles.Count < 2 )  return;
			PropertyArc arc = this.PropertyArc(4);
			if ( arc.ArcType == ArcType.Full )
			{
				if ( this.handles.Count > 2 )
				{
					this.HandleDelete(3);
					this.HandleDelete(2);
				}
			}
			else
			{
				Drawing.Point p2 = this.ComputeArcHandle(arc.StartingAngle);
				Drawing.Point p3 = this.ComputeArcHandle(arc.EndingAngle);

				if ( this.handles.Count == 2 )
				{
					this.HandleAdd(p2, HandleType.Secondary);
					this.HandleAdd(p3, HandleType.Secondary);
				}
				else
				{
					this.Handle(2).Position = p2;
					this.Handle(3).Position = p3;
				}

				this.Handle(2).IsSelected = this.Handle(0).IsSelected;
				this.Handle(3).IsSelected = this.Handle(0).IsSelected;

				for ( int i=2 ; i<4 ; i++ )
				{
					this.GlobalHandleAdapt(i);
				}
			}
		}

		
		protected override void UpdateBoundingBox()
		{
			//	Met à jour le rectangle englobant l'objet.
			Drawing.Path path = this.PathBuild(null);
			this.bboxThin = path.ComputeBounds();

			this.bboxGeom = this.bboxThin;
			this.PropertyLine(1).InflateBoundingBox(ref this.bboxGeom);

			this.bboxFull = this.bboxGeom;
			if ( this.TotalHandle >= 2 )
			{
				this.bboxFull.MergeWith(this.Handle(1).Position);
			}
			this.bboxGeom.MergeWith(this.PropertyGradient(3).BoundingBoxGeom(this.bboxThin));
			this.bboxFull.MergeWith(this.PropertyGradient(3).BoundingBoxFull(this.bboxThin));
			this.bboxFull.MergeWith(this.bboxGeom);
		}

		protected void ComputeGeometry(out Drawing.Point center, out double radius, out double angle)
		{
			//	Calcule la géométrie de l'ellipse.
			center = this.Handle(0).Position;
			Drawing.Point p = this.Handle(1).Position;
			radius = Drawing.Point.Distance(center, p);
			angle = Drawing.Point.ComputeAngleDeg(center, p);
		}

		protected Drawing.Point ComputeArcHandle(double angle)
		{
			//	Calcule la position d'une poignée pour l'arc.
			Drawing.Point center, p;
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
				p = Drawing.Transform.RotatePointDeg(center, angle, p);
				return Drawing.Transform.RotatePointDeg(center, rot, p);
			}
		}

		protected double ComputeArcHandle(Drawing.Point pos)
		{
			//	Calcule l'angle d'après la position de la souris.
			Drawing.Point center, p;
			double radius, rot;
			this.ComputeGeometry(out center, out radius, out rot);

			if ( radius == 0.0 )
			{
				return 0.0;
			}
			else
			{
				p = Drawing.Transform.RotatePointDeg(center, -rot, pos);
				double angle = Drawing.Point.ComputeAngleDeg(center, p);
				if ( angle < 0.0 )  angle += 360.0;  // 0..360
				return angle;
			}
		}

		protected Drawing.Path PathCircle(IconContext iconContext, Drawing.Point c, double rx, double ry)
		{
			//	Crée le chemin d'un cercle.
			Drawing.Point center;
			double radius, rot;
			this.ComputeGeometry(out center, out radius, out rot);

			Drawing.Path path = new Drawing.Path();
			path.DefaultZoom = AbstractProperty.DefaultZoom(iconContext);

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
				path.ArcDeg(center, radius, radius, a1+rot, a2+rot, true);
			}

			if ( arc.ArcType == ArcType.Close )
			{
				path.Close();
			}
			if ( arc.ArcType == ArcType.Pie )
			{
				path.LineTo(center);
				path.Close();
			}

			return path;
		}

		protected Drawing.Path PathBuild(IconContext iconContext)
		{
			//	Crée le chemin de l'objet.
			Drawing.Point center = this.Handle(0).Position;
			double radius = Drawing.Point.Distance(center, this.Handle(1).Position);
			return this.PathCircle(iconContext, center, radius, radius);
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
