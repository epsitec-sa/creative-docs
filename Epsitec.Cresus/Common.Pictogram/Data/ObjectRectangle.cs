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

			PropertyDouble roundRect = new PropertyDouble();
			roundRect.Type = PropertyType.RoundRect;
			this.AddProperty(roundRect);
		}

		protected override AbstractObject CreateNewObject()
		{
			return new ObjectRectangle();
		}


		// Nom de l'icône.
		public override string IconName
		{
			get { return @"file:images/rectangle1.icon"; }
		}


		// Détecte si la souris est sur l'objet.
		public override bool Detect(Drawing.Point pos)
		{
			Drawing.Point p1 = this.Handle(0).Position;
			Drawing.Point p2 = this.Handle(1).Position;

			Drawing.Rectangle rect = new Epsitec.Common.Drawing.Rectangle();
			rect.Left   = System.Math.Min(p1.X, p2.X);
			rect.Right  = System.Math.Max(p1.X, p2.X);
			rect.Bottom = System.Math.Min(p1.Y, p2.Y);
			rect.Top    = System.Math.Max(p1.Y, p2.Y);
			double width = System.Math.Max(this.PropertyLine(0).Width/2, this.minimalWidth);

			if ( this.PropertyGradient(2).IsVisible() )
			{
				rect.Inflate(width, width);
				return rect.Contains(pos);
			}
			else
			{
				rect.Inflate(-width, -width);
				if ( rect.Contains(pos) )  return false;

				rect.Inflate(width*2, width*2);
				return rect.Contains(pos);
			}
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
			iconContext.ConstrainFixStarting(pos, ConstrainType.Square);
			this.HandleAdd(pos, HandleType.Primary);  // rang = 0
			this.HandleAdd(pos, HandleType.Primary);  // rang = 1
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

		
		// Crée le chemin d'un rectangle à coins arrondis.
		protected Drawing.Path PathRoundRectangle(Drawing.Rectangle rect, double radius)
		{
			double ox = System.Math.Min(rect.Left, rect.Right);
			double oy = System.Math.Min(rect.Bottom, rect.Top);
			double dx = System.Math.Abs(rect.Width);
			double dy = System.Math.Abs(rect.Height);

			Drawing.Path path = new Drawing.Path();
			radius = System.Math.Min(radius, System.Math.Min(dx,dy)/2);
			
			if ( radius == 0 )
			{
				path.MoveTo(ox, oy);
				path.LineTo(ox+dx, oy);
				path.LineTo(ox+dx, oy+dy);
				path.LineTo(ox, oy+dy);
				path.Close();
			}
			else
			{
				path.MoveTo (ox+radius, oy);
				path.LineTo (ox+dx-radius, oy);
				path.CurveTo(ox+dx, oy, ox+dx, oy+radius);
				path.LineTo (ox+dx, oy+dy-radius);
				path.CurveTo(ox+dx, oy+dy, ox+dx-radius, oy+dy);
				path.LineTo (ox+radius, oy+dy);
				path.CurveTo(ox, oy+dy, ox, oy+dy-radius);
				path.LineTo (ox, oy+radius);
				path.CurveTo(ox, oy, ox+radius, oy);
				path.Close();

			}
			return path;
		}

		// Dessine l'objet.
		public override void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext)
		{
			base.DrawGeometry(graphics, iconContext);

			if ( this.TotalHandle < 2 )  return;

			Drawing.Rectangle rect = new Drawing.Rectangle();
			rect.Left   = this.Handle(0).Position.X;
			rect.Bottom = this.Handle(0).Position.Y;
			rect.Right  = this.Handle(1).Position.X;
			rect.Top    = this.Handle(1).Position.Y;
			rect.Normalise();

			double radius = this.PropertyDouble(3).Value;
			Drawing.Path path = this.PathRoundRectangle(rect, radius);
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
