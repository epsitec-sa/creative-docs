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
		}

		public override void CreateProperties()
		{
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
			get { return @"file:images/ellipse1.icon"; }
		}


		// Détecte si la souris est sur l'objet.
		public override bool Detect(Drawing.Point pos)
		{
			Drawing.Point p1 = this.Handle(0).Position;
			Drawing.Point p2 = this.Handle(1).Position;
			Drawing.Point center = new Drawing.Point();
			center.X = (p1.X+p2.X)/2;
			center.Y = (p1.Y+p2.Y)/2;
			double rx = System.Math.Abs(p1.X-center.X);
			double ry = System.Math.Abs(p1.Y-center.Y);
			double width = System.Math.Max(this.PropertyLine(0).Width/2, this.minimalWidth);
			Drawing.Point s1,s2;
			for ( int i=0 ; i<4 ; i++ )
			{
				this.ComputeBezier(i, center, rx, ry, out p1, out s1, out s2, out p2);
				if ( Drawing.Point.Detect(p1,s1,s2,p2, pos, width) )  return true;
			}

			if ( !this.PropertyGradient(2).IsVisible() )  return false;
			InsideSurface surf = new InsideSurface(pos, 4*InsideSurface.bezierStep);
			for ( int i=0 ; i<4 ; i++ )
			{
				this.ComputeBezier(i, center, rx, ry, out p1, out s1, out s2, out p2);
				surf.AddBezier(p1, s1, s2, p2);
			}
			return surf.IsInside();
		}

		// Détecte si l'objet est dans un rectangle.
		public override bool Detect(Drawing.Rectangle rect)
		{
			Drawing.Rectangle fullBbox = this.bbox;
			double width = System.Math.Max(this.PropertyLine(0).Width/2, this.minimalWidth);
			fullBbox.Inflate(width, width);
			return rect.Contains(fullBbox);
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

			Drawing.Point p1 = this.Handle(0).Position;
			Drawing.Point p2 = this.Handle(1).Position;

			if ( rank == 2 )
			{
				this.Handle(0).Position = new Drawing.Point(pos.X, p1.Y);
				this.Handle(1).Position = new Drawing.Point(p2.X, pos.Y);
			}
			else if ( rank == 3 )
			{
				this.Handle(0).Position = new Drawing.Point(p1.X, pos.Y);
				this.Handle(1).Position = new Drawing.Point(pos.X, p2.Y);
			}
			else
			{
				this.Handle(rank).Position = pos;
			}

			p1 = this.Handle(0).Position;
			p2 = this.Handle(1).Position;
			this.Handle(2).Position = new Drawing.Point(p1.X, p2.Y);
			this.Handle(3).Position = new Drawing.Point(p2.X, p1.Y);
		}

		// Déplace tout l'objet.
		public override void MoveAll(Drawing.Point move)
		{
			this.Handle(0).Position += move;
			this.Handle(1).Position += move;

			Drawing.Point p1 = this.Handle(0).Position;
			Drawing.Point p2 = this.Handle(1).Position;
			this.Handle(2).Position = new Drawing.Point(p1.X, p2.Y);
			this.Handle(3).Position = new Drawing.Point(p2.X, p1.Y);
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
			this.Handle(1).Position = pos;
		}

		// Fin de la création d'un objet.
		public override void CreateMouseUp(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);
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

	
		// Calcule une courbe de Bézier de l'objet.
		protected bool ComputeBezier(int i, Drawing.Point c, double rx, double ry,
									 out Drawing.Point p1, out Drawing.Point s1, out Drawing.Point s2, out Drawing.Point p2)
		{
			p1 = new Drawing.Point(0, 0);
			s1 = new Drawing.Point(0, 0);
			s2 = new Drawing.Point(0, 0);
			p2 = new Drawing.Point(0, 0);

			if ( i == 0 )
			{
				p1.X = c.X-rx;       p1.Y = c.Y;
				s1.X = c.X-rx;       s1.Y = c.Y+ry*0.56;
				s2.X = c.X-rx*0.56;  s2.Y = c.Y+ry;
				p2.X = c.X;          p2.Y = c.Y+ry;
				return true;
			}

			if ( i == 1 )
			{
				p1.X = c.X;          p1.Y = c.Y+ry;
				s1.X = c.X+rx*0.56;  s1.Y = c.Y+ry;
				s2.X = c.X+rx;       s2.Y = c.Y+ry*0.56;
				p2.X = c.X+rx;       p2.Y = c.Y;
				return true;
			}

			if ( i == 2 )
			{
				p1.X = c.X+rx;       p1.Y = c.Y;
				s1.X = c.X+rx;       s1.Y = c.Y-ry*0.56;
				s2.X = c.X+rx*0.56;  s2.Y = c.Y-ry;
				p2.X = c.X;          p2.Y = c.Y-ry;
				return true;
			}

			if ( i == 3 )
			{
				p1.X = c.X;          p1.Y = c.Y-ry;
				s1.X = c.X-rx*0.56;  s1.Y = c.Y-ry;
				s2.X = c.X-rx;       s2.Y = c.Y-ry*0.56;
				p2.X = c.X-rx;       p2.Y = c.Y;
				return true;
			}

			return false;
		}

		// Crée le chemin d'un cercle.
		protected Drawing.Path PathCircle(Drawing.Point c, double rx, double ry)
		{
			Drawing.Path path = new Drawing.Path();
			Drawing.Point p1,s1,s2,p2;
			for ( int i=0 ; i<4 ; i++ )
			{
				this.ComputeBezier(i, c, rx, ry, out p1, out s1, out s2, out p2);
				if ( i == 0 )  path.MoveTo(p1);
				path.CurveTo(s1, s2, p2);
			}
			path.Close();
			return path;
		}

		// Dessine l'objet.
		public override void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext)
		{
			base.DrawGeometry(graphics, iconContext);

			if ( this.TotalHandle < 2 )  return;

			Drawing.Point p1 = this.Handle(0).Position;
			Drawing.Point p2 = this.Handle(1).Position;
			Drawing.Point center = new Drawing.Point();
			center.X = (p1.X+p2.X)/2;
			center.Y = (p1.Y+p2.Y)/2;
			double rx = System.Math.Abs(p1.X-center.X);
			double ry = System.Math.Abs(p1.Y-center.Y);
			Drawing.Path path = this.PathCircle(center, rx, ry);
			this.bbox = path.ComputeBounds();
			this.PropertyGradient(2).Render(graphics, iconContext, path, this.bbox);

			graphics.Rasterizer.AddOutline(path, this.PropertyLine(0).Width, this.PropertyLine(0).Cap, this.PropertyLine(0).Join);
			graphics.RenderSolid(iconContext.AdaptColor(this.PropertyColor(1).Color));

			if ( this.IsHilite && iconContext.IsEditable )
			{
				graphics.Rasterizer.AddOutline(path, this.PropertyLine(0).Width+iconContext.HiliteSize, this.PropertyLine(0).Cap, this.PropertyLine(0).Join);
				graphics.RenderSolid(iconContext.HiliteColor);
			}
		}
	}
}
