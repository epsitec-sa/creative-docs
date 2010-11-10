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
			this.transform = Transform.Identity;
			this.stackColorModifier = new Stack<ColorModifierCallback> ();

			System.DateTime now = System.DateTime.Now.ToUniversalTime ();
			string timeStamp = string.Concat (now.ToShortDateString (), " ", now.ToShortTimeString (), " UTC");

			this.xDocument = new XDocument
			(
				new XDeclaration ("1.0", "utf-8", "yes"),
				new XComment ("Saved on " + timeStamp)
			);

			this.xRoot = new XElement ("root");
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
				return this.lineWidth;
			}
			set
			{
				if (this.lineWidth != value)
				{
					this.lineWidth = value;

					var xml = new XElement ("lineWidth");
					XmlPort.Add (xml, this.lineWidth);
					this.xRoot.Add (xml);
				}
			}
		}

		public JoinStyle LineJoin
		{
			get
			{
				return this.lineJoin;
			}
			set
			{
				if (this.lineJoin != value)
				{
					this.lineJoin = value;

					var xml = new XElement ("lineJoin");
					xml.Add (new XAttribute ("value", this.lineJoin));

					this.xRoot.Add (xml);
				}
			}
		}

		public CapStyle LineCap
		{
			get
			{
				return this.lineCap;
			}
			set
			{
				if (this.lineCap != value)
				{
					this.lineCap = value;

					var xml = new XElement ("lineCap");
					xml.Add (new XAttribute ("value", this.lineCap));
					this.xRoot.Add (xml);
				}
			}
		}

		public double LineMiterLimit
		{
			get
			{
				return this.lineMiterLimit;
			}
			set
			{
				if (this.lineMiterLimit != value)
				{
					this.lineMiterLimit = value;

					var xml = new XElement ("lineMiterLimit");
					XmlPort.Add (xml, this.lineMiterLimit);
					this.xRoot.Add (xml);
				}
			}
		}

		public RichColor RichColor
		{
			get
			{
				return RichColor.FromColor (this.originalColor);
			}
			set
			{
				this.originalColor = value.Basic;
				this.FinalColor = this.GetFinalColor (value.Basic);
			}
		}

		public Color Color
		{
			get
			{
				return this.originalColor;
			}
			set
			{
				this.originalColor = value;
				this.FinalColor = this.GetFinalColor (value);
			}
		}

		public RichColor FinalRichColor
		{
			get
			{
				return RichColor.FromColor (this.color);
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
				return this.color;
			}
			set
			{
				if (this.color != value)
				{
					this.color = value;

					var xml = new XElement ("color");
					XmlPort.Add (xml, this.color);
					this.xRoot.Add (xml);
				}
			}
		}

		public Transform Transform
		{
			get
			{
				return this.transform;
			}
			set
			{
				if (this.transform != value)
				{
					this.transform = value;

					var xml = new XElement ("transform");
					XmlPort.Add (xml, this.transform);
					this.xRoot.Add (xml);
				}
			}
		}

		public FillMode FillMode
		{
			get
			{
				return this.fillMode;
			}
			set
			{
				if (this.fillMode != value)
				{
					this.fillMode = value;

					var xml = new XElement ("fillMode");
					xml.Add (new XAttribute ("value", this.fillMode));
					this.xRoot.Add (xml);
				}
			}
		}

		public ImageFilter ImageFilter
		{
			get
			{
				return this.imageFilter;
			}
			set
			{
				if (this.imageFilter != value)
				{
					this.imageFilter = value;

					var xml = new XElement ("imageFilter");
					xml.Add (new XAttribute ("value", this.imageFilter));
					this.xRoot.Add (xml);
				}
			}
		}

		public Margins ImageCrop
		{
			get
			{
				return this.imageCrop;
			}
			set
			{
				if (this.imageCrop != value)
				{
					this.imageCrop = value;

					var xml = new XElement ("imageCrop");
					XmlPort.Add (xml, this.imageCrop);
					this.xRoot.Add (xml);
				}
			}
		}

		public Size ImageFinalSize
		{
			get
			{
				return this.imageFinalSize;
			}
			set
			{
				if (this.imageFinalSize != value)
				{
					this.imageFinalSize = value;

					var xml = new XElement ("imageFinalSize");
					XmlPort.Add (xml, this.imageFinalSize);
					this.xRoot.Add (xml);
				}
			}
		}

		public bool HasEmptyClippingRectangle
		{
			get
			{
				return this.clip.IsSurfaceZero;
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
			this.clip = Rectangle.Intersection (this.clip, rect);
		}

		public Rectangle SaveClippingRectangle()
		{
			return this.clip;
		}

		public void RestoreClippingRectangle(Rectangle rect)
		{
			this.clip = rect;
		}

		public void ResetClippingRectangle()
		{
			this.clip = Drawing.Rectangle.MaxValue;
		}

		public void Align(ref double x, ref double y)
		{
		}

		public void ScaleTransform(double sx, double sy, double cx, double cy)
		{
			Drawing.Transform scale = Drawing.Transform.CreateScaleTransform (sx, sy, cx, cy);
			this.Transform = scale.MultiplyBy (this.transform);
		}

		public void RotateTransformDeg(double angle, double cx, double cy)
		{
			Drawing.Transform rotation = Drawing.Transform.CreateRotationDegTransform (angle, cx, cy);
			this.Transform = rotation.MultiplyBy (this.transform);
		}

		public void RotateTransformRad(double angle, double cx, double cy)
		{
			Drawing.Transform rotation = Drawing.Transform.CreateRotationRadTransform (angle, cx, cy);
			this.Transform = rotation.MultiplyBy (this.transform);
		}

		public void TranslateTransform(double ox, double oy)
		{
			Drawing.Transform translation = Drawing.Transform.CreateTranslationTransform (ox, oy);
			this.Transform = translation.MultiplyBy (this.transform);
		}

		public void MergeTransform(Transform transform)
		{
			this.Transform = transform.MultiplyBy (this.transform);
		}

		public void PaintOutline(Path path)
		{
			var xml = new XElement ("paintOutline");
			XmlPort.Add (xml, path);
			this.xRoot.Add (xml);
		}

		public void PaintSurface(Path path)
		{
			var xml = new XElement ("paintSurface");
			XmlPort.Add (xml, path);
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
			var xml = new XElement ("paintText");
			XmlPort.Add (xml, new Point (x, y));
			XmlPort.Add (xml, text);
			XmlPort.Add (xml, font);
			XmlPort.Add (xml, size, "size");
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
			var xml = new XElement ("paintImage");
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
		}

		#endregion


		private static void Add(XElement xml, double value, string name="value")
		{
			xml.Add (new XAttribute (name, XmlPort.Truncate (value)));
		}

		private static void Add(XElement xml, string text, string name="text")
		{
			xml.Add (new XAttribute (name, text));
		}

		private static void Add(XElement xml, Point pos)
		{
			xml.Add (new XAttribute ("x", XmlPort.Truncate (pos.X)));
			xml.Add (new XAttribute ("y", XmlPort.Truncate (pos.Y)));
		}

		private static void Add(XElement xml, Size size)
		{
			xml.Add (new XAttribute ("width",  XmlPort.Truncate (size.Width)));
			xml.Add (new XAttribute ("height", XmlPort.Truncate (size.Height)));
		}

		private static void Add(XElement xml, Rectangle rect)
		{
			xml.Add (new XAttribute ("rect", rect.ToString ()));
		}

		private static void Add(XElement xml, Margins margins)
		{
			xml.Add (new XAttribute ("margins", margins.ToString ()));
		}

		private static void Add(XElement xml, Color color)
		{
			xml.Add (new XAttribute ("hexa", Color.ToHexa (color)));
			xml.Add (new XAttribute ("a", XmlPort.Truncate (color.A)));
		}

		private static void Add(XElement xml, Font font)
		{
			xml.Add (new XAttribute ("face",  font.FaceName));
			xml.Add (new XAttribute ("style", font.StyleName));
		}

		private static void Add(XElement xml, Transform transform)
		{
			xml.Add (new XAttribute ("matrix", transform.ToString ()));
		}

		private static void Add(XElement xml, Path path)
		{
			string s = path.ToString ().Replace ("\r\n", " ");
			xml.Add (new XAttribute ("path", s));
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

		public static double Truncate(double value, int numberOfDecimal=3)
		{
			//	Retourne un nombre tronqué à un certain nombre de décimales.
			double factor = System.Math.Pow (10, numberOfDecimal);
			return System.Math.Floor (value*factor) / factor;
		}


		private readonly Stack<ColorModifierCallback>	stackColorModifier;
		private readonly XDocument						xDocument;
		private readonly XElement						xRoot;

		private double									lineWidth      = 1.0;
		private JoinStyle								lineJoin       = JoinStyle.Miter;
		private CapStyle								lineCap        = CapStyle.Square;
		private double									lineMiterLimit = 4.0;
		private ImageFilter								imageFilter;
		private Margins									imageCrop;
		private Size									imageFinalSize;
		private Color									originalColor  = Color.FromRgb (0, 0, 0);
		private Color									color          = Color.FromRgb (0, 0, 0);
		private Rectangle								clip           = Rectangle.MaxValue;
		private Transform								transform      = Transform.Identity;
		private FillMode								fillMode       = FillMode.NonZero;
	}
}
