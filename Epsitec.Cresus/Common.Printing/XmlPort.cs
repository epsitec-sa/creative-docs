//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Epsitec.Common.Printing
{
	public sealed class XmlPort : IPaintPort
	{
		public XmlPort()
		{
			this.stackColorModifier = new Stack<ColorModifierCallback> ();

			System.DateTime now = System.DateTime.Now.ToUniversalTime ();
			string timeStamp = string.Concat (now.ToShortDateString (), " ", now.ToShortTimeString (), " UTC");

			this.xDocument = new XDocument
			(
				new XDeclaration ("1.0", "utf-8", "yes"),
				new XComment ("Saved on " + timeStamp)
			);

			this.xRoot = new XElement ("g");
			this.xDocument.Add (this.xRoot);
		}

		public string XmlSource
		{
			get
			{
				//?return this.xDocument.ToString (SaveOptions.DisableFormatting);
				return this.xDocument.ToString (SaveOptions.None);
			}
		}


		#region IPaintPort Members

		public double LineWidth
		{
			get
			{
				return this.currentState.lineWidth;
			}
			set
			{
				this.currentState.lineWidth = value;
			}
		}

		public JoinStyle LineJoin
		{
			get
			{
				return this.currentState.lineJoin;
			}
			set
			{
				this.currentState.lineJoin = value;
			}
		}

		public CapStyle LineCap
		{
			get
			{
				return this.currentState.lineCap;
			}
			set
			{
				this.currentState.lineCap = value;
			}
		}

		public double LineMiterLimit
		{
			get
			{
				return this.currentState.lineMiterLimit;
			}
			set
			{
				this.currentState.lineMiterLimit = value;
			}
		}

		public RichColor RichColor
		{
			get
			{
				return RichColor.FromColor (this.currentState.originalColor);
			}
			set
			{
				this.currentState.originalColor = value.Basic;
				this.FinalColor = this.GetFinalColor (value.Basic);
			}
		}

		public Color Color
		{
			get
			{
				return this.currentState.originalColor;
			}
			set
			{
				this.currentState.originalColor = value;
				this.FinalColor = this.GetFinalColor (value);
			}
		}

		public RichColor FinalRichColor
		{
			get
			{
				return RichColor.FromColor (this.currentState.color);
			}
			set
			{
				this.FinalColor = value.Basic;
			}
		}

		public Color FinalColor
		{
			get
			{
				return this.currentState.color;
			}
			set
			{
				this.currentState.color = value;
			}
		}

		public Transform Transform
		{
			get
			{
				return this.currentState.transform;
			}
			set
			{
				this.currentState.transform = value;
			}
		}

		public FillMode FillMode
		{
			get
			{
				return this.currentState.fillMode;
			}
			set
			{
				this.currentState.fillMode = value;
			}
		}

		public ImageFilter ImageFilter
		{
			get
			{
				return this.currentState.imageFilter;
			}
			set
			{
				this.currentState.imageFilter = value;
			}
		}

		public Margins ImageCrop
		{
			get
			{
				return this.currentState.imageCrop;
			}
			set
			{
				this.currentState.imageCrop = value;
			}
		}

		public Size ImageFinalSize
		{
			get
			{
				return this.currentState.imageFinalSize;
			}
			set
			{
				this.currentState.imageFinalSize = value;
			}
		}

		public bool HasEmptyClippingRectangle
		{
			get
			{
				return this.currentState.clip.IsSurfaceZero;
			}
		}

		public void PushColorModifier(ColorModifierCallback method)
		{
			this.stackColorModifier.Push (method);
		}

		public ColorModifierCallback PopColorModifier()
		{
			return this.stackColorModifier.Pop ();
		}

		public RichColor GetFinalColor(RichColor color)
		{
			foreach (ColorModifierCallback method in this.stackColorModifier)
			{
				color = method (color);
			}

			return color;
		}

		public Color GetFinalColor(Color color)
		{
			if (this.stackColorModifier.Count == 0)
			{
				return color;
			}

			RichColor rich = RichColor.FromColor (color);
			foreach (ColorModifierCallback method in this.stackColorModifier)
			{
				rich = method (rich);
			}

			return rich.Basic;
		}

		public void SetClippingRectangle(Rectangle rect)
		{
			this.currentState.clip = Rectangle.Intersection (this.currentState.clip, rect);
		}

		public Rectangle SaveClippingRectangle()
		{
			return this.currentState.clip;
		}

		public void RestoreClippingRectangle(Rectangle rect)
		{
			this.currentState.clip = rect;
		}

		public void ResetClippingRectangle()
		{
			this.currentState.clip = Drawing.Rectangle.MaxValue;
		}

		public void Align(ref double x, ref double y)
		{
		}

		public void ScaleTransform(double sx, double sy, double cx, double cy)
		{
			Drawing.Transform scale = Drawing.Transform.CreateScaleTransform (sx, sy, cx, cy);
			this.Transform = scale.MultiplyBy (this.currentState.transform);
		}

		public void RotateTransformDeg(double angle, double cx, double cy)
		{
			Drawing.Transform rotation = Drawing.Transform.CreateRotationDegTransform (angle, cx, cy);
			this.Transform = rotation.MultiplyBy (this.currentState.transform);
		}

		public void RotateTransformRad(double angle, double cx, double cy)
		{
			Drawing.Transform rotation = Drawing.Transform.CreateRotationRadTransform (angle, cx, cy);
			this.Transform = rotation.MultiplyBy (this.currentState.transform);
		}

		public void TranslateTransform(double ox, double oy)
		{
			Drawing.Transform translation = Drawing.Transform.CreateTranslationTransform (ox, oy);
			this.Transform = translation.MultiplyBy (this.currentState.transform);
		}

		public void MergeTransform(Transform transform)
		{
			this.Transform = transform.MultiplyBy (this.currentState.transform);
		}

		public void PaintOutline(Path path)
		{
			var xml = new XElement ("outline");

			xml.Add (new XAttribute ("path", XmlPort.Serialize (path)));
			this.UpdateGraphicState (xml);

			this.xRoot.Add (xml);
		}

		public void PaintSurface(Path path)
		{
			var xml = new XElement ("surface");

			xml.Add (new XAttribute ("path", XmlPort.Serialize (path)));
			this.UpdateGraphicState (xml);

			this.xRoot.Add (xml);
		}

		public void PaintGlyphs(Font font, double size, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy)
		{
		}

		public double PaintText(double x, double y, string text, Font font, double size)
		{
			return this.PaintText (x, y, text, font, size, null);
		}

		public double PaintText(double x, double y, string text, Font font, double size, FontClassInfo[] infos)
		{
			this.currentState.fontFace  = font.FaceName;
			this.currentState.fontStyle = font.StyleName;
			this.currentState.fontSize  = size;

			var xml = new XElement ("text", text);

			xml.Add (new XAttribute ("x", XmlPort.Truncate (x)));
			xml.Add (new XAttribute ("y", XmlPort.Truncate (y)));
			this.UpdateGraphicState (xml);

			this.xRoot.Add (xml);

			return 0;
		}

		public void PaintImage(Image bitmap, Rectangle fill)
		{
			this.PaintImage (bitmap, fill.Left, fill.Bottom, fill.Width, fill.Height, 0, 0, bitmap.Width, bitmap.Height);
		}

		public void PaintImage(Image bitmap, double fillX, double fillY, double fillWidth, double fillHeight)
		{
			this.PaintImage (bitmap, fillX, fillY, fillWidth, fillHeight, 0, 0, bitmap.Width, bitmap.Height);
		}

		public void PaintImage(Image bitmap, Rectangle fill, Point imageOrigin)
		{
			this.PaintImage (bitmap, fill.Left, fill.Bottom, fill.Width, fill.Height, imageOrigin.X, imageOrigin.Y, fill.Width, fill.Height);
		}

		public void PaintImage(Image bitmap, Rectangle fill, Rectangle imageRect)
		{
			this.PaintImage (bitmap, fill.Left, fill.Bottom, fill.Width, fill.Height, imageRect.Left, imageRect.Bottom, imageRect.Width, imageRect.Height);
		}

		public void PaintImage(Image bitmap, double fillX, double fillY, double fillWidth, double fillHeight, double imageOriginX, double imageOriginY)
		{
			this.PaintImage (bitmap, fillX, fillY, fillWidth, fillHeight, imageOriginX, imageOriginY, fillWidth, fillHeight);
		}

		public void PaintImage(Image bitmap, double fillX, double fillY, double fillWidth, double fillHeight, double imageOriginX, double imageOriginY, double imageWidth, double imageHeight)
		{
#if false
			var xml = new XElement ("image");

			XmlPort.Add (xml, fillX, "fillX");
			XmlPort.Add (xml, fillY, "fillY");
			XmlPort.Add (xml, fillWidth, "fillWidth");
			XmlPort.Add (xml, fillHeight, "fillHeight");
			XmlPort.Add (xml, imageOriginX, "imageOriginX");
			XmlPort.Add (xml, imageOriginY, "imageOriginY");
			XmlPort.Add (xml, imageWidth, "imageWidth");
			XmlPort.Add (xml, imageHeight, "imageHeight");

			XmlPort.Add (xml, bitmap);
			this.xRoot.Add (xml);
#endif
		}

		#endregion


		private void UpdateGraphicState(XElement xml)
		{
			if (this.lastState.lineWidth != this.currentState.lineWidth)
			{
				this.lastState.lineWidth = this.currentState.lineWidth;

				xml.Add (new XAttribute ("lineWidth", XmlPort.Truncate (this.lastState.lineWidth)));
			}

			if (this.lastState.lineJoin != this.currentState.lineJoin)
			{
				this.lastState.lineJoin = this.currentState.lineJoin;

				xml.Add (new XAttribute ("lineJoin", this.lastState.lineJoin));
			}

			if (this.lastState.lineCap != this.currentState.lineCap)
			{
				this.lastState.lineCap = this.currentState.lineCap;

				xml.Add (new XAttribute ("lineCap", this.lastState.lineCap));
			}

			if (this.lastState.lineMiterLimit != this.currentState.lineMiterLimit)
			{
				this.lastState.lineMiterLimit = this.currentState.lineMiterLimit;

				xml.Add (new XAttribute ("lineMiterLimit", XmlPort.Truncate (this.lastState.lineMiterLimit)));
			}

			if (this.lastState.imageFilter != this.currentState.imageFilter)
			{
				this.lastState.imageFilter = this.currentState.imageFilter;
			}

			if (this.lastState.imageCrop != this.currentState.imageCrop)
			{
				this.lastState.imageCrop = this.currentState.imageCrop;
			}

			if (this.lastState.imageFinalSize != this.currentState.imageFinalSize)
			{
				this.lastState.imageFinalSize = this.currentState.imageFinalSize;
			}

			if (this.lastState.color != this.currentState.color)
			{
				this.lastState.color = this.currentState.color;

				xml.Add (new XAttribute ("color", XmlPort.Serialize (this.lastState.color)));
			}

			if (this.lastState.clip != this.currentState.clip)
			{
				this.lastState.clip = this.currentState.clip;
			}

			if (this.lastState.transform != this.currentState.transform)
			{
				this.lastState.transform = this.currentState.transform;
			}

			if (this.lastState.fillMode != this.currentState.fillMode)
			{
				this.lastState.fillMode = this.currentState.fillMode;
			}

			if (this.lastState.fontFace != this.currentState.fontFace)
			{
				this.lastState.fontFace = this.currentState.fontFace;

				xml.Add (new XAttribute ("fontFamily", this.lastState.fontFace));
			}

			if (this.lastState.fontStyle != this.currentState.fontStyle)
			{
				this.lastState.fontStyle = this.currentState.fontStyle;

				xml.Add (new XAttribute ("fontStyle", this.lastState.fontStyle));
			}

			if (this.lastState.fontSize != this.currentState.fontSize)
			{
				this.lastState.fontSize = this.currentState.fontSize;

				xml.Add (new XAttribute ("fontSize", XmlPort.Truncate (this.lastState.fontSize)));
			}
		}


		private static void Add(XElement xml, Image image)
		{
			xml.Add (new XAttribute ("width",  XmlPort.Truncate (image.Width)));
			xml.Add (new XAttribute ("height", XmlPort.Truncate (image.Height)));

			int dx = image.BitmapImage.PixelWidth * 4;
			int dy = image.BitmapImage.PixelHeight;

			byte[] bytes = image.BitmapImage.GetRawBitmapBytes ();
			System.Diagnostics.Debug.Assert (bytes.Length == dx*dy);

			for (int y = 0; y < dy; y++)
			{
				var builder = new System.Text.StringBuilder ();

				for (int x = 0; x < dx; x++)
				{
					builder.Append (string.Format ("{0:x2}", bytes[dx*y+x]));
				}

				xml.Add (new XAttribute (string.Format ("row{0}", y.ToString (System.Globalization.CultureInfo.InvariantCulture)), builder.ToString ()));
			}
		}


		private static string Serialize(Color color)
		{
			return string.Concat ("#", Color.ToHexa (color));
		}

		private Color DeserializeColor(string value)
		{
			if (value.StartsWith ("#"))
			{
				return Color.FromHexa (value.Substring (1));
			}
			else
			{
				return Color.Empty;
			}
		}


		private static string Serialize(Path path)
		{
			return path.Serialize ();
		}

		private Path DeserializePath(string value)
		{
			return Path.FromDeserialize (value);
		}


		private static double Truncate(double value, int numberOfDecimal=2)
		{
			//	Retourne un nombre tronqué à un certain nombre de décimales.
			double factor = System.Math.Pow (10, numberOfDecimal);
			return System.Math.Floor (value*factor) / factor;
		}


		private readonly Stack<ColorModifierCallback>	stackColorModifier;
		private readonly XDocument						xDocument;
		private readonly XElement						xRoot;

		private GraphicState							currentState = new GraphicState ();
		private GraphicState							lastState    = new GraphicState ();


		private class GraphicState
		{
			public double								lineWidth      = 1.0;
			public JoinStyle							lineJoin       = JoinStyle.Miter;
			public CapStyle								lineCap        = CapStyle.Square;
			public double								lineMiterLimit = 4.0;
			public ImageFilter							imageFilter;
			public Margins								imageCrop;
			public Size									imageFinalSize;
			public Color								originalColor  = Color.FromRgb (0, 0, 0);
			public Color								color          = Color.FromRgb (0, 0, 0);
			public Rectangle							clip           = Rectangle.MaxValue;
			public Transform							transform      = Transform.Identity;
			public FillMode								fillMode       = FillMode.NonZero;
			public string								fontFace       = "Arial";
			public string								fontStyle      = "Regular";
			public double								fontSize       = 10.0;
		}
	}
}
