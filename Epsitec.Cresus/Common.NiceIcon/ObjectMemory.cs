namespace Epsitec.Common.NiceIcon
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
			lineMode.BackgroundColor = Drawing.Color.FromBrightness(0.7);
			lineMode.Width = 5.0;
			lineMode.Cap   = Drawing.CapStyle.Round;
			lineMode.Join  = Drawing.JoinStyle.Round;
			this.AddProperty(lineMode);

			PropertyColor lineColor = new PropertyColor();
			lineColor.Type = PropertyType.LineColor;
			lineColor.Text = "Couleur trait";
			lineColor.BackgroundColor = Drawing.Color.FromBrightness(0.7);
			lineColor.Color = Drawing.Color.FromBrightness(0.0);
			this.AddProperty(lineColor);

			PropertyGradient fillGradient = new PropertyGradient();
			fillGradient.Type = PropertyType.FillGradient;
			fillGradient.Text = "Couleur interieure";
			fillGradient.BackgroundColor = Drawing.Color.FromBrightness(0.9);
			fillGradient.Fill   = Drawing.GradientFill.None;
			fillGradient.Color1 = Drawing.Color.FromBrightness(1.0);
			fillGradient.Color2 = Drawing.Color.FromBrightness(0.6);
			fillGradient.Angle  = 0.0;
			fillGradient.Cx     = 0.5;
			fillGradient.Cy     = 0.5;
			fillGradient.Repeat = 1;
			fillGradient.Middle = 0.0;
			fillGradient.Range  = 0.0;
			this.AddProperty(fillGradient);

			PropertyBool fillClose = new PropertyBool();
			fillClose.Type = PropertyType.PolyClose;
			fillClose.Text = "Contour ferme";
			fillClose.BackgroundColor = Drawing.Color.FromBrightness(0.8);
			fillClose.Bool = false;
			this.AddProperty(fillClose);

			PropertyDouble roundRect = new PropertyDouble();
			roundRect.Type = PropertyType.RoundRect;
			roundRect.Text = "Rayon des coins";
			roundRect.BackgroundColor = Drawing.Color.FromBrightness(0.8);
			roundRect.Value = 0;
			roundRect.MinRange = 0;
			roundRect.MaxRange = 20;
			roundRect.Step = 2;
			this.AddProperty(roundRect);

			PropertyDouble regularFaces = new PropertyDouble();
			regularFaces.Type = PropertyType.RegularFaces;
			regularFaces.Text = "Nombre de cotes";
			regularFaces.BackgroundColor = Drawing.Color.FromBrightness(0.8);
			regularFaces.Value = 6;
			regularFaces.MinRange = 3;
			regularFaces.MaxRange = 24;
			regularFaces.Step = 1;
			this.AddProperty(regularFaces);

			PropertyBool regularStar = new PropertyBool();
			regularStar.Type = PropertyType.RegularStar;
			regularStar.Text = "Etoile";
			regularStar.BackgroundColor = Drawing.Color.FromBrightness(0.8);
			regularStar.Bool = false;
			this.AddProperty(regularStar);

			PropertyDouble regularShape = new PropertyDouble();
			regularShape.Type = PropertyType.RegularShape;
			regularShape.Text = "Renfoncement";
			regularShape.BackgroundColor = Drawing.Color.FromBrightness(0.8);
			regularShape.Value = 50;
			regularShape.MinRange = 0;
			regularShape.MaxRange = 100;
			regularShape.Step = 5;
			this.AddProperty(regularShape);

			PropertyString textString = new PropertyString();
			textString.Type = PropertyType.TextString;
			textString.Text = "Texte";
			textString.BackgroundColor = Drawing.Color.FromBrightness(0.8);
			textString.String = "Abc";
			this.AddProperty(textString);

			PropertyList textFontName = new PropertyList();
			textFontName.Type = PropertyType.TextFontName;
			textFontName.Text = "Police";
			textFontName.BackgroundColor = Drawing.Color.FromBrightness(0.8);
			textFontName.Choice = 0;
			textFontName.Add("Tahoma");
			textFontName.Add("Arial");
			textFontName.Add("Courrier");
			textFontName.Add("Times");
			this.AddProperty(textFontName);

			PropertyList textFontOptical = new PropertyList();
			textFontOptical.Type = PropertyType.TextFontOptical;
			textFontOptical.Text = "Style";
			textFontOptical.BackgroundColor = Drawing.Color.FromBrightness(0.8);
			textFontOptical.Choice = 0;
			textFontOptical.Add("Normal");
			textFontOptical.Add("Gras");
			textFontOptical.Add("Italique");
			textFontOptical.Add("Gras italique");
			this.AddProperty(textFontOptical);
		}
	}
}
