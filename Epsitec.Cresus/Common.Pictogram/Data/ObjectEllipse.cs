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
		}

		protected override AbstractObject CreateNewObject()
		{
			return new ObjectEllipse();
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

			Drawing.Path path = this.PathBuild();

			double width = System.Math.Max(this.PropertyLine(1).Width/2, this.minimalWidth);
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

			     if ( rank == 0 )  this.MoveCorner(pos, 0, 2,3, 1);
			else if ( rank == 1 )  this.MoveCorner(pos, 1, 3,2, 0);
			else if ( rank == 2 )  this.MoveCorner(pos, 2, 0,1, 3);
			else if ( rank == 3 )  this.MoveCorner(pos, 3, 1,0, 2);
			else                   this.Handle(rank).Position = pos;

			this.dirtyBbox = true;
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
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateIsExist(IconContext iconContext)
		{
			double len = Drawing.Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > this.minimalSize );
		}

	
		// Met à jour le rectangle englobant l'objet.
		protected override void UpdateBoundingBox()
		{
			Drawing.Path path = this.PathBuild();
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

		// Crée le chemin d'une ellipse inscrite dans un quadrilatère.
		protected Drawing.Path PathEllipse(Drawing.Point p1, Drawing.Point p2, Drawing.Point p3, Drawing.Point p4)
		{
			Drawing.Point p12 = (p1+p2)/2;
			Drawing.Point p23 = (p2+p3)/2;
			Drawing.Point p34 = (p3+p4)/2;
			Drawing.Point p41 = (p4+p1)/2;

			Drawing.Path path = new Drawing.Path();
			path.MoveTo(p12);
			path.CurveTo(Drawing.Point.Scale(p12, p2, 0.56), Drawing.Point.Scale(p23, p2, 0.56), p23);
			path.CurveTo(Drawing.Point.Scale(p23, p3, 0.56), Drawing.Point.Scale(p34, p3, 0.56), p34);
			path.CurveTo(Drawing.Point.Scale(p34, p4, 0.56), Drawing.Point.Scale(p41, p4, 0.56), p41);
			path.CurveTo(Drawing.Point.Scale(p41, p1, 0.56), Drawing.Point.Scale(p12, p1, 0.56), p12);
			path.Close();
			return path;
		}

		// Crée le chemin de l'objet.
		protected Drawing.Path PathBuild()
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

			return this.PathEllipse(p1, p2, p3, p4);
		}

		// Dessine l'objet.
		public override void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext, IconObjects iconObjects)
		{
			if ( base.IsFullHide(iconContext) )  return;
			base.DrawGeometry(graphics, iconContext, iconObjects);

			if ( this.TotalHandle < 2 )  return;

			Drawing.Path path = this.PathBuild();
			this.PropertyGradient(3).Render(graphics, iconContext, path, this.BoundingBoxThin);

			this.PropertyLine(1).DrawPath(graphics, iconContext, iconObjects, path, this.PropertyColor(2).Color);

			if ( this.IsHilite && iconContext.IsEditable )
			{
				if ( this.PropertyGradient(3).IsVisible() )
				{
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(iconContext.HiliteSurfaceColor);
				}

				graphics.Rasterizer.AddOutline(path, this.PropertyLine(1).Width+iconContext.HiliteSize, this.PropertyLine(1).Cap, this.PropertyLine(1).Join, this.PropertyLine(1).Limit);
				graphics.RenderSolid(iconContext.HiliteOutlineColor);
			}
		}
	}
}
