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
		}

		public override void CreateProperties()
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

			PropertyGradient fillGradient = new PropertyGradient();
			fillGradient.Type = PropertyType.FillGradient;
			fillGradient.Text = "Couleur interieure";
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
			fillClose.Text = "Contour ferme";
			fillClose.BackgroundIntensity = 0.90;
			fillClose.Bool = false;
			this.AddProperty(fillClose);

			PropertyDouble roundRect = new PropertyDouble();
			roundRect.Type = PropertyType.RoundRect;
			roundRect.Text = "Rayon des coins";
			roundRect.BackgroundIntensity = 0.90;
			roundRect.Value = 0;
			roundRect.MinRange = 0;
			roundRect.MaxRange = 10;
			roundRect.Step = 1;
			this.AddProperty(roundRect);

			PropertyDouble regularFaces = new PropertyDouble();
			regularFaces.Type = PropertyType.RegularFaces;
			regularFaces.Text = "Nombre de cotes";
			regularFaces.BackgroundIntensity = 0.90;
			regularFaces.Value = 6;
			regularFaces.MinRange = 3;
			regularFaces.MaxRange = 24;
			regularFaces.Step = 1;
			this.AddProperty(regularFaces);

			PropertyBool regularStar = new PropertyBool();
			regularStar.Type = PropertyType.RegularStar;
			regularStar.Text = "Etoile";
			regularStar.BackgroundIntensity = 0.90;
			regularStar.Bool = false;
			this.AddProperty(regularStar);

			PropertyDouble regularShape = new PropertyDouble();
			regularShape.Type = PropertyType.RegularShape;
			regularShape.Text = "Renfoncement";
			regularShape.BackgroundIntensity = 0.90;
			regularShape.Value = 50;
			regularShape.MinRange = 0;
			regularShape.MaxRange = 100;
			regularShape.Step = 5;
			this.AddProperty(regularShape);

			PropertyString textString = new PropertyString();
			textString.Type = PropertyType.TextString;
			textString.Text = "Texte";
			textString.BackgroundIntensity = 0.90;
			textString.String = "Abc";
			this.AddProperty(textString);

			PropertyList textFontName = new PropertyList();
			textFontName.Type = PropertyType.TextFontName;
			textFontName.Text = "Police";
			textFontName.BackgroundIntensity = 0.90;
			textFontName.Choice = 0;
			textFontName.Add("Tahoma");
			textFontName.Add("Arial");
			textFontName.Add("Courrier");
			textFontName.Add("Times");
			this.AddProperty(textFontName);

			PropertyList textFontOptical = new PropertyList();
			textFontOptical.Type = PropertyType.TextFontOptical;
			textFontOptical.Text = "Style";
			textFontOptical.BackgroundIntensity = 0.90;
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
