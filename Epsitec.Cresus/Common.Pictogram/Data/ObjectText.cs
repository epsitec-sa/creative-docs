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
			get { return @"file:images/text.icon"; }
		}


		// Détecte si la souris est sur l'objet.
		public override bool Detect(Drawing.Point pos)
		{
			if ( this.isHide )  return false;
			Drawing.Rectangle bbox = this.BoundingBoxGeom;  // màj p1..p4
			if ( !bbox.Contains(pos) )  return false;
			InsideSurface inside = new InsideSurface(pos, 4);
			inside.AddLine(this.p1, this.p2);
			inside.AddLine(this.p2, this.p3);
			inside.AddLine(this.p3, this.p4);
			inside.AddLine(this.p4, this.p1);
			return inside.IsInside();
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
		protected override void UpdateBoundingBox()
		{
			this.bboxThin = Drawing.Rectangle.Empty;
			this.bboxGeom = Drawing.Rectangle.Empty;
			this.bboxFull = Drawing.Rectangle.Empty;

			Drawing.Point p1 = this.Handle(0).Position;
			Drawing.Point p2 = this.Handle(1).Position;

			Drawing.Font font = this.GetFont();
			double width = font.GetTextAdvance(this.PropertyString(1).String);
			if ( width == 0 )  return;
			double len = Drawing.Point.Distance(p1, p2)/width;

			Drawing.Rectangle rect = font.GetTextBounds(this.PropertyString(1).String);
			rect.Scale(len);
			rect.Offset(p1);

			double angle = Drawing.Point.ComputeAngle(p1, p2);
			this.p1 = Drawing.Transform.RotatePoint(p1, angle, rect.BottomLeft);
			this.p2 = Drawing.Transform.RotatePoint(p1, angle, rect.TopLeft);
			this.p3 = Drawing.Transform.RotatePoint(p1, angle, rect.TopRight);
			this.p4 = Drawing.Transform.RotatePoint(p1, angle, rect.BottomRight);

			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			bbox.MergeWith(this.p1);
			bbox.MergeWith(this.p2);
			bbox.MergeWith(this.p3);
			bbox.MergeWith(this.p4);

			this.bboxThin = bbox;
			this.bboxGeom = bbox;
			this.bboxFull = bbox;
		}

		// Retourne la fonte à utiliser.
		protected Drawing.Font GetFont()
		{
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

			return Drawing.Font.GetFont(fn, fo);
		}

		// Dessine l'objet.
		public override void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext)
		{
			if ( this.isHide )  return;
			base.DrawGeometry(graphics, iconContext);

			if ( this.TotalHandle != 2 )  return;

			Drawing.Point p1 = this.Handle(0).Position;
			Drawing.Point p2 = this.Handle(1).Position;

			Drawing.Transform ot = graphics.SaveTransform();

			double angle = Drawing.Point.ComputeAngle(p1, p2);
			angle *= 180.0/System.Math.PI;  // radians -> degrés
			graphics.RotateTransform(angle, p1.X, p1.Y);

			Drawing.Font font = this.GetFont();
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
				Drawing.Rectangle bbox = this.BoundingBoxGeom;  // màj p1..p4
				Drawing.Path path = new Epsitec.Common.Drawing.Path();
				path.MoveTo(this.p1);
				path.LineTo(this.p2);
				path.LineTo(this.p3);
				path.LineTo(this.p4);
				path.Close();
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(iconContext.HiliteOutlineColor);
			}
		}


		protected Drawing.Point		p1;
		protected Drawing.Point		p2;
		protected Drawing.Point		p3;
		protected Drawing.Point		p4;
	}
}
