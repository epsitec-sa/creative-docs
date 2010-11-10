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

			this.xRoot = new XElement ("Root");
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
					xml.Add (new XAttribute ("value", this.lineWidth));

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
					xml.Add (new XAttribute ("value", this.lineMiterLimit));

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
					xml.Add (new XAttribute ("a", this.color.A));
					xml.Add (new XAttribute ("r", this.color.R));
					xml.Add (new XAttribute ("g", this.color.G));
					xml.Add (new XAttribute ("b", this.color.B));

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
					xml.Add (new XAttribute ("xx", this.transform.XX));
					xml.Add (new XAttribute ("xy", this.transform.XY));
					xml.Add (new XAttribute ("yx", this.transform.YX));
					xml.Add (new XAttribute ("yy", this.transform.YY));
					xml.Add (new XAttribute ("tx", this.transform.TX));
					xml.Add (new XAttribute ("ty", this.transform.TY));

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

					this.AddMargins ("imageCrop", this.imageCrop);
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
					xml.Add (new XAttribute ("width",  this.imageFinalSize.Width));
					xml.Add (new XAttribute ("height", this.imageFinalSize.Height));

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
		}

		public void PaintSurface(Path path)
		{
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
			var xml = new XElement ("text");

			xml.Add (new XAttribute ("x", x));
			xml.Add (new XAttribute ("y", y));
			xml.Add (new XAttribute ("text", text));
			//?xml.Add (new XAttribute ("font", font));
			xml.Add (new XAttribute ("size", size));

			this.xRoot.Add (xml);

			return 0;
		}

		public void PaintImage(Image bitmap, Rectangle fill)
		{
		}

		public void PaintImage(Image bitmap, double fillX, double fillY, double fillWidth, double fillHeight)
		{
		}

		public void PaintImage(Image bitmap, Rectangle fill, Point imageOrigin)
		{
		}

		public void PaintImage(Image bitmap, Rectangle fill, Rectangle imageRect)
		{
		}

		public void PaintImage(Image bitmap, double fillX, double fillY, double fillWidth, double fillHeight, double imageOriginX, double imageOriginY)
		{
		}

		public void PaintImage(Image bitmap, double fillX, double fillY, double fillWidth, double fillHeight, double imageOriginX, double imageOriginY, double imageWidth, double imageHeight)
		{
		}

		#endregion


		private void AddMargins(string name, Margins margins)
		{
			var xml = new XElement (name);

			xml.Add (new XAttribute ("left",   margins.Left));
			xml.Add (new XAttribute ("right",  margins.Right));
			xml.Add (new XAttribute ("bottom", margins.Bottom));
			xml.Add (new XAttribute ("top",    margins.Top));

			this.xRoot.Add (xml);
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
