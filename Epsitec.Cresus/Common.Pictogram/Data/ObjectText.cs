using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe ObjectText est la classe de l'objet graphique "texte simple".
	/// </summary>
	public class ObjectText : AbstractObject
	{
		public ObjectText()
		{
		}

		public override void CreateProperties()
		{
			PropertyColor lineColor = new PropertyColor();
			lineColor.Type = PropertyType.LineColor;
			this.AddProperty(lineColor);

			PropertyString textString = new PropertyString();
			textString.Type = PropertyType.TextString;
			this.AddProperty(textString);

			PropertyCombo textFontName = new PropertyCombo();
			textFontName.Type = PropertyType.TextFontName;
			this.AddProperty(textFontName);

			PropertyCombo textFontOptical = new PropertyCombo();
			textFontOptical.Type = PropertyType.TextFontOptical;
			this.AddProperty(textFontOptical);
		}

		protected override AbstractObject CreateNewObject()
		{
			return new ObjectText();
		}


		// Nom de l'icône.
		public override string IconName
		{
			get { return @"file:images/text1.icon"; }
		}


		// Détecte si la souris est sur l'objet.
		public override bool Detect(Drawing.Point pos)
		{
			Drawing.Point p1 = this.Handle(0).Position;
			Drawing.Point p2 = this.Handle(1).Position;
			return Drawing.Point.Detect(p1,p2, pos, this.lineWidth/this.scaleX);
		}


		// Début de la création d'un objet.
		public override void CreateMouseDown(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainFixStarting(pos);
			this.HandleAdd(pos, HandleType.Starting);
			this.HandleAdd(pos, HandleType.Primary);
			this.Handle(0).IsSelected = true;
			this.Handle(1).IsSelected = true;
		}

		// Déplacement pendant la création d'un objet.
		public override void CreateMouseMove(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);
			this.Handle(1).Position = pos;
			this.dirtyBbox = true;
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
			this.Deselect();
			double len = Drawing.Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > this.minimalSize );
		}


		// Met à jour le rectangle englobant l'objet.
		public override void UpdateBoundingBox()
		{
			Drawing.Point p1 = this.Handle(0).Position;
			Drawing.Point p2 = this.Handle(1).Position;
			this.bbox = new Epsitec.Common.Drawing.Rectangle(p1.X, p1.Y, p2.X-p1.X, p2.Y-p1.Y);
			this.bbox.Normalise();
		}

		// Dessine l'objet.
		public override void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext)
		{
			base.DrawGeometry(graphics, iconContext);

			if ( this.TotalHandle != 2 )  return;

			Drawing.Point p1 = this.Handle(0).Position;
			Drawing.Point p2 = this.Handle(1).Position;

			Drawing.Transform ot = graphics.SaveTransform();

			double angle = Drawing.Point.ComputeAngle(p1, p2);
			angle *= 180.0/System.Math.PI;  // radians -> degrés
			graphics.RotateTransform(angle, p1.X, p1.Y);

			string fn = "";  // font name
			switch ( this.PropertyCombo(2).Choice )
			{
				case 0:  fn="Tahoma";           break;
				case 1:  fn="Arial";            break;
				case 2:  fn="Courier New";      break;
				case 3:  fn="Times New Roman";  break;
			}
			string fo = "";  // font optical
			switch ( this.PropertyCombo(3).Choice )
			{
				case 0:  fo="Regular";       break;
				case 1:  fo="Bold";          break;
				case 2:  fo="Italic";        break;
				case 3:  fo="Bold Italic";   break;
			}
			Drawing.Font font = Drawing.Font.GetFont(fn, fo);
			double width = font.GetTextAdvance(this.PropertyString(1).String);
			if ( width != 0 )
			{
				double len = Drawing.Point.Distance(p1, p2)/width;
				graphics.AddText(p1.X, p1.Y, this.PropertyString(1).String, font, len);
				graphics.RenderSolid(iconContext.AdaptColor(this.PropertyColor(0).Color));
			}

			graphics.Transform = ot;

			if ( this.IsSelected() && iconContext.IsEditable )
			{
				double initialWidth = graphics.LineWidth;
				graphics.LineWidth = 1.0/iconContext.ScaleX;

				graphics.AddLine(p1, p2);
				graphics.RenderSolid(Drawing.Color.FromBrightness(0.6));

				graphics.LineWidth = initialWidth;
			}

			if ( this.IsHilite && iconContext.IsEditable )
			{
				Drawing.Path path = new Drawing.Path();
				path.MoveTo(p1);
				path.LineTo(p2);

				graphics.Rasterizer.AddOutline(path, this.lineWidth/this.scaleX);
				graphics.RenderSolid(iconContext.HiliteColor);
			}
		}


		protected double		lineWidth = 10;
	}
}
