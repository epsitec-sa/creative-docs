using System.Xml.Serialization;

namespace Epsitec.Common.NiceIcon
{
	/// <summary>
	/// La classe ObjectRegular est la classe de l'objet graphique "polygone régulier".
	/// </summary>
	public class ObjectRegular : AbstractObject
	{
		public ObjectRegular()
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

			PropertyDouble regularFaces = new PropertyDouble();
			regularFaces.Type = PropertyType.RegularFaces;
			this.AddProperty(regularFaces);

			PropertyBool regularStar = new PropertyBool();
			regularStar.Type = PropertyType.RegularStar;
			this.AddProperty(regularStar);

			PropertyDouble regularShape = new PropertyDouble();
			regularShape.Type = PropertyType.RegularShape;
			this.AddProperty(regularShape);
		}


		// Nom de l'icône.
		public override string IconName
		{
			get { return @"manifest:Epsitec.Common.NiceIcon/Images.regular.png"; }
		}


		// Détecte si la souris est sur l'objet.
		public override bool Detect(Drawing.Point pos)
		{
			int i=0;
			Drawing.Point a;
			Drawing.Point b;
			double width = System.Math.Max(this.PropertyLine(0).Width/2, this.minimalWidth);
			while ( this.ComputeLine(i++, out a, out b) )
			{
				if ( Widgets.Math.Detect(a,b, pos, width) )  return true;
			}

			if ( !this.PropertyGradient(2).IsVisible() )  return false;
			InsideSurface surf = new InsideSurface(pos, i);
			i = 0;
			while ( this.ComputeLine(i++, out a, out b) )
			{
				surf.AddLine(a,b);
			}
			return surf.IsInside();
		}

		// Détecte si l'objet est dans un rectangle.
		public override bool Detect(Drawing.Rectangle rect)
		{
			Drawing.Rectangle fullBbox = new Drawing.Rectangle();
			fullBbox = this.bbox;
			double width = System.Math.Max(this.PropertyLine(0).Width/2, this.minimalWidth);
			fullBbox.Inflate(width, width);
			return rect.Contains(fullBbox);
		}


		// Début de la création d'un objet.
		public override void CreateMouseDown(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainFixStarting(pos);
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
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateIsExist(IconContext iconContext)
		{
			double len = Widgets.Math.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > this.minimalSize );
		}


		// Calcule une droite de l'objet.
		protected bool ComputeLine(int i, out Drawing.Point a, out Drawing.Point b)
		{
			int total = (int)this.PropertyDouble(3).Value;
			Drawing.Point center = this.Handle(0).Position;
			Drawing.Point corner = this.Handle(1).Position;

			a = new Drawing.Point(0, 0);
			b = new Drawing.Point(0, 0);

			if ( this.PropertyBool(4).Bool )  // étoile ?
			{
				if ( i >= total*2 )  return false;

				Drawing.Point star = new Drawing.Point();
				star.X = center.X + (corner.X-center.X)*(1-this.PropertyDouble(5).Value/100);
				star.Y = center.Y + (corner.Y-center.Y)*(1-this.PropertyDouble(5).Value/100);

				a = Widgets.Math.RotatePoint(center, System.Math.PI*2*(i+0)/(total*2), (i%2==0) ? corner : star);
				b = Widgets.Math.RotatePoint(center, System.Math.PI*2*(i+1)/(total*2), (i%2==0) ? star : corner);
				return true;
			}
			else	// polygone ?
			{
				if ( i >= total )  return false;

				a = Widgets.Math.RotatePoint(center, System.Math.PI*2*(i+0)/total, corner);
				b = Widgets.Math.RotatePoint(center, System.Math.PI*2*(i+1)/total, corner);
				return true;
			}
		}

		// Crée le chemin d'un polygone régulier.
		protected Drawing.Path PathRegular()
		{
			Drawing.Path path = new Drawing.Path();
			int total = (int)this.PropertyDouble(3).Value;

			Drawing.Point a;
			Drawing.Point b;

			if ( this.PropertyBool(4).Bool )  // étoile ?
			{
				for ( int i=0 ; i<total ; i++ )
				{
					this.ComputeLine(i*2, out a, out b);
					if ( i == 0 )  path.MoveTo(a);
					else           path.LineTo(a);

					this.ComputeLine(i*2+1, out a, out b);
					path.LineTo(a);
				}
				path.Close();
			}
			else	// polygone ?
			{
				for ( int i=0 ; i<total ; i++ )
				{
					this.ComputeLine(i, out a, out b);
					if ( i == 0 )  path.MoveTo(a);
					else           path.LineTo(a);
				}
				path.Close();
			}

			return path;
		}

		// Dessine l'objet.
		public override void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext)
		{
			base.DrawGeometry(graphics, iconContext);

			if ( this.TotalHandle != 2 )  return;

			Drawing.Path path = this.PathRegular();
			graphics.Rasterizer.AddSurface(path);
			this.bbox = path.ComputeBounds();
			this.PropertyGradient(2).Render(graphics, iconContext, bbox);

			graphics.Rasterizer.AddOutline(path, this.PropertyLine(0).Width, this.PropertyLine(0).Cap, this.PropertyLine(0).Join);
			graphics.RenderSolid(iconContext.AdaptColor(this.PropertyColor(1).Color));

			if ( this.IsHilite && iconContext.IsEditable )
			{
				graphics.Rasterizer.AddOutline(path, this.PropertyLine(0).Width+iconContext.HiliteSize, this.PropertyLine(0).Cap, this.PropertyLine(0).Join);
				graphics.RenderSolid(iconContext.HiliteColor);
			}
		}


		protected Drawing.Rectangle			bbox;
	}
}
