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
			lineMode.Width = 0.1;
			lineMode.Cap   = Drawing.CapStyle.Round;
			lineMode.Join  = Drawing.JoinStyle.Round;
			this.AddProperty(lineMode);

			PropertyColor lineColor = new PropertyColor();
			lineColor.Type = PropertyType.LineColor;
			lineColor.Color = Drawing.Color.FromBrightness(0.0);
			this.AddProperty(lineColor);

			PropertyArrow arrow = new PropertyArrow();
			arrow.Type = PropertyType.Arrow;
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
			fillClose.Bool = false;
			this.AddProperty(fillClose);

			PropertyCorner corner = new PropertyCorner();
			corner.Type = PropertyType.Corner;
			corner.CornerType = CornerType.Right;
			corner.Radius = 2.0;
			corner.Effect1 = 0.5;
			corner.Effect2 = 0.5;
			this.AddProperty(corner);

			PropertyRegular regular = new PropertyRegular();
			regular.Type = PropertyType.Regular;
			regular.NbFaces = 6;
			regular.Star = false;
			regular.Deep = 0.5;
			this.AddProperty(regular);

			PropertyArc arc = new PropertyArc();
			arc.Type = PropertyType.Arc;
			arc.ArcType       = ArcType.Full;
			arc.StartingAngle =  90.0;
			arc.EndingAngle   = 360.0;
			this.AddProperty(arc);

			PropertyColor backColor = new PropertyColor();
			backColor.Type = PropertyType.BackColor;
			backColor.Color = Drawing.Color.FromBrightness(1.0);
			this.AddProperty(backColor);

			PropertyFont textFont = new PropertyFont();
			textFont.Type = PropertyType.TextFont;
			textFont.FontName    = "Tahoma";
			textFont.FontOptical = "Regular";
			textFont.FontSize    = 1.0;
			textFont.FontColor   = Drawing.Color.FromBrightness(0);
			this.AddProperty(textFont);

			PropertyJustif textJustif = new PropertyJustif();
			textJustif.Type = PropertyType.TextJustif;
			textJustif.Horizontal  = JustifHorizontal.Left;
			textJustif.Vertical    = JustifVertical.Top;
			textJustif.Orientation = JustifOrientation.LeftToRight;
			textJustif.MarginH     = 0.2;
			textJustif.MarginV     = 0.1;
			textJustif.OffsetV     = 0.0;
			this.AddProperty(textJustif);

			PropertyTextLine textLine = new PropertyTextLine();
			textLine.Type = PropertyType.TextLine;
			textLine.Horizontal = JustifHorizontal.Left;
			textLine.Offset     = 0.0;
			textLine.Add        = 0.0;
			this.AddProperty(textLine);

			PropertyImage image = new PropertyImage();
			image.Type = PropertyType.Image;
			image.Filename = "";
			image.MirrorH = false;
			image.MirrorV = false;
			image.Homo = true;
			this.AddProperty(image);
		}

		protected override AbstractObject CreateNewObject()
		{
			return new ObjectMemory();
		}
	}
}
