namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe ObjectMemory est un objet caché qui collectionne toutes les propriétés.
	/// C'est également cet objet qui détermine les valeurs initiales des propriétés.
	/// </summary>
	public class ObjectMemory : AbstractObject
	{
		public ObjectMemory()
		{
			PropertyLine lineMode = new PropertyLine();
			lineMode.Type = PropertyType.LineMode;
			lineMode.Text = "Epaisseur trait";
			lineMode.BackgroundIntensity = 0.85;
			lineMode.Width = 1.0;
			lineMode.Cap   = Drawing.CapStyle.Round;
			lineMode.Join  = Drawing.JoinStyle.Round;
			this.AddProperty(lineMode);

			PropertyColor lineColor = new PropertyColor();
			lineColor.Type = PropertyType.LineColor;
			lineColor.Text = "Couleur trait";
			lineColor.BackgroundIntensity = 0.85;
			lineColor.Color = Drawing.Color.FromBrightness(0.0);
			this.AddProperty(lineColor);

			PropertyArrow arrow = new PropertyArrow();
			arrow.Type = PropertyType.Arrow;
			arrow.Text = "Extrémités";
			arrow.BackgroundIntensity = 0.85;
			arrow.ArrowType1 = ArrowType.Right;
			arrow.ArrowType2 = ArrowType.Right;
			arrow.Length1  = 2.0;
			arrow.Effect11 = 0.5;
			arrow.Effect12 = 0.5;
			arrow.Length2  = 2.0;
			arrow.Effect21 = 0.5;
			arrow.Effect22 = 0.5;
			this.AddProperty(arrow);

			PropertyGradient fillGradient = new PropertyGradient();
			fillGradient.Type = PropertyType.FillGradient;
			fillGradient.Text = "Couleur intérieure";
			fillGradient.BackgroundIntensity = 0.95;
			fillGradient.Fill   = GradientFill.None;
			fillGradient.Color1 = Drawing.Color.FromBrightness(1.0);
			fillGradient.Color2 = Drawing.Color.FromBrightness(0.5);
			fillGradient.Angle  = 0.0;
			fillGradient.Cx     = 0.5;
			fillGradient.Cy     = 0.5;
			fillGradient.Sx     = 1.0;
			fillGradient.Sy     = 1.0;
			fillGradient.Repeat = 1;
			fillGradient.Middle = 0.0;
			fillGradient.Smooth = 0.0;
			this.AddProperty(fillGradient);

#if false
			PropertyShadow shadow = new PropertyShadow();
			shadow.Type = PropertyType.Shadow;
			shadow.Text = "Ombre";
			shadow.BackgroundColor = Drawing.Color.FromBrightness(0.9);
			shadow.Color  = Drawing.Color.FromARGB(0.0, 0.5, 0.5, 0.5);
			shadow.Radius =  2.0;
			shadow.Ox     =  1.0;
			shadow.Oy     = -1.0;
			this.AddProperty(shadow);
#endif

			PropertyBool fillClose = new PropertyBool();
			fillClose.Type = PropertyType.PolyClose;
			fillClose.Text = "Contour fermé";
			fillClose.BackgroundIntensity = 0.90;
			fillClose.Bool = false;
			this.AddProperty(fillClose);

			PropertyCorner corner = new PropertyCorner();
			corner.Type = PropertyType.Corner;
			corner.Text = "Coins";
			corner.BackgroundIntensity = 0.90;
			corner.CornerType = CornerType.Right;
			corner.Radius = 2.0;
			corner.Effect1 = 0.5;
			corner.Effect2 = 0.5;
			this.AddProperty(corner);

			PropertyRegular regular = new PropertyRegular();
			regular.Type = PropertyType.Regular;
			regular.Text = "Nombre de côtés";
			regular.BackgroundIntensity = 0.90;
			regular.NbFaces = 6;
			regular.Star = false;
			regular.Deep = 0.5;
			this.AddProperty(regular);

			PropertyString textString = new PropertyString();
			textString.Type = PropertyType.TextString;
			textString.Text = "Texte";
			textString.BackgroundIntensity = 0.80;
			textString.String = "Abc";
			this.AddProperty(textString);

			PropertyCombo textFontName = new PropertyCombo();
			textFontName.Type = PropertyType.TextFontName;
			textFontName.Text = "Police";
			textFontName.BackgroundIntensity = 0.80;
			textFontName.Choice = 0;
			textFontName.Add("Tahoma");
			textFontName.Add("Arial");
			textFontName.Add("Courrier");
			textFontName.Add("Times");
			this.AddProperty(textFontName);

			PropertyCombo textFontOptical = new PropertyCombo();
			textFontOptical.Type = PropertyType.TextFontOptical;
			textFontOptical.Text = "Style";
			textFontOptical.BackgroundIntensity = 0.80;
			textFontOptical.Choice = 0;
			textFontOptical.Add("Normal");
			textFontOptical.Add("Gras");
			textFontOptical.Add("Italique");
			textFontOptical.Add("Gras italique");
			this.AddProperty(textFontOptical);
		}

		protected override AbstractObject CreateNewObject()
		{
			return new ObjectMemory();
		}
	}
}
