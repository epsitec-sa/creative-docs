//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Drawing.Serializers;
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

			this.pathSerializer      = new PathSerializer (2);
			this.transformSerializer = new TransformSerializer (2);

			this.currentState = new GraphicState ();
			this.lastState    = new GraphicState ();
		}

		public string XmlSource
		{
			get
			{
				//?return this.xDocument.ToString (SaveOptions.DisableFormatting);
				return this.xDocument.ToString (SaveOptions.None);
			}
		}


		#region Deserialisation
		public void Deserialize(string xmlSource, IPaintPort dstPort)
		{
			this.UpdateGraphicState (dstPort, updateAll: true);

			XDocument doc = XDocument.Parse (xmlSource, LoadOptions.None);
			XElement root = doc.Element ("g");

			foreach (var element in root.Elements ())
			{
				this.DeserializeGraphicState (element);  // this.currentState <- selon xml
				this.UpdateGraphicState (dstPort);

				if (element.Name == "surface")
				{
					string value = (string) element.Attribute ("path");
					var path = XmlPort.DeserializePath (value);
					dstPort.PaintSurface (path);
				}
				else if (element.Name == "outline")
				{
					string value = (string) element.Attribute ("path");
					var path = XmlPort.DeserializePath (value);
					dstPort.PaintOutline (path);
				}
				else if (element.Name == "text")
				{
					double x    = (double) element.Attribute ("x");
					double y    = (double) element.Attribute ("y");
					string text = (string) element.Attribute ("text");

					dstPort.PaintText (x, y, text, this.currentFont, this.currentState.fontSize);
				}
				else if (element.Name == "image")
				{
					string id           = (string) element.Attribute ("id");
					double fillX        = (double) element.Attribute ("fill-x");
					double fillY        = (double) element.Attribute ("fill-y");
					double fillWidth    = (double) element.Attribute ("fill-width");
					double fillHeight   = (double) element.Attribute ("fill-height");
					double imageOriginX = (double) element.Attribute ("image-originX");
					double imageOriginY = (double) element.Attribute ("image-originY");
					double imageWidth   = (double) element.Attribute ("image-width");
					double imageHeight  = (double) element.Attribute ("image-height");

					Image bitmap = null;  // TODO: Retrouver l'image à partir de 'id' !

					if (bitmap != null)
					{
						dstPort.PaintImage (bitmap, fillX, fillY, fillWidth, fillHeight, imageOriginX, imageOriginY, imageWidth, imageHeight);
					}
				}
				else
				{
					throw new System.ArgumentException ("Invalid serialized data");
				}
			}
		}

		private void DeserializeGraphicState(XElement element)
		{
			XAttribute attribute;

			attribute = element.Attribute ("line-width");
			if (attribute != null)
			{
				this.currentState.lineWidth = (double) attribute;
			}

			attribute = element.Attribute ("line-join");
			if (attribute != null)
			{
				//?this.currentState.lineJoin = (JoinStyle) attribute;
			}

			attribute = element.Attribute ("line-cap");
			if (attribute != null)
			{
				//?this.currentState.lineCap = (CapStyle) attribute;
			}

			attribute = element.Attribute ("line-miter-limit");
			if (attribute != null)
			{
				this.currentState.lineMiterLimit = (double) attribute;
			}

			attribute = element.Attribute ("color");
			if (attribute != null)
			{
				this.currentState.color = XmlPort.DeserializeColor ((string) attribute);
			}

			attribute = element.Attribute ("transform");
			if (attribute != null)
			{
				this.currentState.transform = XmlPort.DeserializeTransform ((string) attribute);
			}

			attribute = element.Attribute ("fill-mode");
			if (attribute != null)
			{
				//?this.currentState.fillMode = (FillMode) attribute;
			}

			attribute = element.Attribute ("font-face");
			if (attribute != null)
			{
				this.currentState.fontFace = (string) attribute;
			}

			attribute = element.Attribute ("font-style");
			if (attribute != null)
			{
				this.currentState.fontStyle = (string) attribute;
			}

			attribute = element.Attribute ("font-size");
			if (attribute != null)
			{
				this.currentState.fontSize = (double) attribute;
			}

		}

		private void UpdateGraphicState(IPaintPort dstPort, bool updateAll=false)
		{
			if (this.lastState.lineWidth != this.currentState.lineWidth || updateAll)
			{
				this.lastState.lineWidth = this.currentState.lineWidth;

				dstPort.LineWidth = this.lastState.lineWidth;
			}

			if (this.lastState.lineJoin != this.currentState.lineJoin || updateAll)
			{
				this.lastState.lineJoin = this.currentState.lineJoin;

				dstPort.LineJoin = this.lastState.lineJoin;
			}

			if (this.lastState.lineCap != this.currentState.lineCap || updateAll)
			{
				this.lastState.lineCap = this.currentState.lineCap;

				dstPort.LineCap = this.lastState.lineCap;
			}

			if (this.lastState.lineMiterLimit != this.currentState.lineMiterLimit || updateAll)
			{
				this.lastState.lineMiterLimit = this.currentState.lineMiterLimit;

				dstPort.LineMiterLimit = this.lastState.lineMiterLimit;
			}

			if (this.lastState.imageFilter != this.currentState.imageFilter || updateAll)
			{
				this.lastState.imageFilter = this.currentState.imageFilter;

				dstPort.ImageFilter = this.lastState.imageFilter;
			}

			if (this.lastState.imageCrop != this.currentState.imageCrop || updateAll)
			{
				this.lastState.imageCrop = this.currentState.imageCrop;

				dstPort.ImageCrop = this.lastState.imageCrop;
			}

			if (this.lastState.imageFinalSize != this.currentState.imageFinalSize || updateAll)
			{
				this.lastState.imageFinalSize = this.currentState.imageFinalSize;

				dstPort.ImageFinalSize = this.lastState.imageFinalSize;
			}

			if (this.lastState.color != this.currentState.color || updateAll)
			{
				this.lastState.color = this.currentState.color;

				dstPort.Color = this.lastState.color;
			}

			if (this.lastState.clip != this.currentState.clip || updateAll)
			{
				this.lastState.clip = this.currentState.clip;

				dstPort.SetClippingRectangle (this.lastState.clip);
			}

			if (this.lastState.transform != this.currentState.transform || updateAll)
			{
				this.lastState.transform = this.currentState.transform;

				dstPort.Transform = this.lastState.transform;
			}

			if (this.lastState.fillMode != this.currentState.fillMode || updateAll)
			{
				this.lastState.fillMode = this.currentState.fillMode;

				dstPort.FillMode = this.lastState.fillMode;
			}

			if (this.lastState.fontFace  != this.currentState.fontFace  ||
				this.lastState.fontStyle != this.currentState.fontStyle || updateAll)
			{
				this.lastState.fontFace  = this.currentState.fontFace;
				this.lastState.fontStyle = this.currentState.fontStyle;

				this.currentFont = Font.GetFont (this.lastState.fontFace, this.lastState.fontStyle);
			}
		}
		#endregion


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

			xml.Add (new XAttribute ("path", this.Serialize (path)));
			this.UpdateGraphicState (xml);

			this.xRoot.Add (xml);
		}

		public void PaintSurface(Path path)
		{
			var xml = new XElement ("surface");

			xml.Add (new XAttribute ("path", this.Serialize (path)));
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

			xml.Add (new XAttribute ("x", this.Truncate (x)));
			xml.Add (new XAttribute ("y", this.Truncate (y)));
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
			var xml = new XElement ("image");

			//	L'image doit avoir un identificateur, pour permettre la désérialisation !
			xml.Add (new XAttribute ("id",            bitmap.Id));
			xml.Add (new XAttribute ("fill-x",        this.Truncate (fillX)));
			xml.Add (new XAttribute ("fill-y",        this.Truncate (fillY)));
			xml.Add (new XAttribute ("fill-width",    this.Truncate (fillWidth)));
			xml.Add (new XAttribute ("fill-height",   this.Truncate (fillHeight)));
			xml.Add (new XAttribute ("image-originX", this.Truncate (imageOriginX)));
			xml.Add (new XAttribute ("image-originY", this.Truncate (imageOriginY)));
			xml.Add (new XAttribute ("image-width",   this.Truncate (imageWidth)));
			xml.Add (new XAttribute ("image-height",  this.Truncate (imageHeight)));
			this.UpdateGraphicState (xml);

			this.xRoot.Add (xml);
		}

		#endregion


		private void UpdateGraphicState(XElement xml)
		{
			if (this.lastState.lineWidth != this.currentState.lineWidth)
			{
				this.lastState.lineWidth = this.currentState.lineWidth;

				xml.Add (new XAttribute ("line-width", this.Truncate (this.lastState.lineWidth)));
			}

			if (this.lastState.lineJoin != this.currentState.lineJoin)
			{
				this.lastState.lineJoin = this.currentState.lineJoin;

				xml.Add (new XAttribute ("line-join", this.lastState.lineJoin));
			}

			if (this.lastState.lineCap != this.currentState.lineCap)
			{
				this.lastState.lineCap = this.currentState.lineCap;

				xml.Add (new XAttribute ("line-cap", this.lastState.lineCap));
			}

			if (this.lastState.lineMiterLimit != this.currentState.lineMiterLimit)
			{
				this.lastState.lineMiterLimit = this.currentState.lineMiterLimit;

				xml.Add (new XAttribute ("line-miter-limit", this.Truncate (this.lastState.lineMiterLimit)));
			}

			if (this.lastState.imageFilter != this.currentState.imageFilter)
			{
				this.lastState.imageFilter = this.currentState.imageFilter;

				xml.Add (new XAttribute ("image-filter", this.lastState.imageFilter));
			}

			if (this.lastState.imageCrop != this.currentState.imageCrop)
			{
				this.lastState.imageCrop = this.currentState.imageCrop;

				xml.Add (new XAttribute ("image-crop", this.lastState.imageCrop));
			}

			if (this.lastState.imageFinalSize != this.currentState.imageFinalSize)
			{
				this.lastState.imageFinalSize = this.currentState.imageFinalSize;

				xml.Add (new XAttribute ("image-final-size-width",  this.Truncate (this.lastState.imageFinalSize.Width)));
				xml.Add (new XAttribute ("image-final-size-height", this.Truncate (this.lastState.imageFinalSize.Height)));
			}

			if (this.lastState.color != this.currentState.color)
			{
				this.lastState.color = this.currentState.color;

				xml.Add (new XAttribute ("color", XmlPort.Serialize (this.lastState.color)));
			}

			if (this.lastState.clip != this.currentState.clip)
			{
				this.lastState.clip = this.currentState.clip;

				xml.Add (new XAttribute ("clip-left",   this.Truncate (this.lastState.clip.Left)));
				xml.Add (new XAttribute ("clip-bottom", this.Truncate (this.lastState.clip.Bottom)));
				xml.Add (new XAttribute ("clip-width",  this.Truncate (this.lastState.clip.Width)));
				xml.Add (new XAttribute ("clip-height", this.Truncate (this.lastState.clip.Height)));
			}

			if (this.lastState.transform != this.currentState.transform)
			{
				this.lastState.transform = this.currentState.transform;

				xml.Add (new XAttribute ("transform", this.Serialize (this.lastState.transform)));
			}

			if (this.lastState.fillMode != this.currentState.fillMode)
			{
				this.lastState.fillMode = this.currentState.fillMode;

				xml.Add (new XAttribute ("fill-mode", this.lastState.fillMode));
			}

			if (this.lastState.fontFace != this.currentState.fontFace)
			{
				this.lastState.fontFace = this.currentState.fontFace;

				xml.Add (new XAttribute ("font-face", this.lastState.fontFace));
			}

			if (this.lastState.fontStyle != this.currentState.fontStyle)
			{
				this.lastState.fontStyle = this.currentState.fontStyle;

				xml.Add (new XAttribute ("font-style", this.lastState.fontStyle));
			}

			if (this.lastState.fontSize != this.currentState.fontSize)
			{
				this.lastState.fontSize = this.currentState.fontSize;

				xml.Add (new XAttribute ("font-size", this.Truncate (this.lastState.fontSize)));
			}
		}


		private static string Serialize(Color color)
		{
			return string.Concat ("#", Color.ToHexa (color));
		}

		private static Color DeserializeColor(string value)
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


		private string Serialize(Path path)
		{
			return this.pathSerializer.Serialize (path);
		}

		private static Path DeserializePath(string value)
		{
			return PathSerializer.Parse (value);
		}


		private string Serialize(Transform transform)
		{
			return this.transformSerializer.Serialize (transform);
		}

		private static Transform DeserializeTransform(string value)
		{
			return TransformSerializer.Parse (value);
		}


		private string Truncate(double value)
		{
			return this.pathSerializer.Serialize (value);
		}


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

	
		private readonly Stack<ColorModifierCallback>	stackColorModifier;
		private readonly XDocument						xDocument;
		private readonly XElement						xRoot;
		private readonly PathSerializer					pathSerializer;
		private readonly TransformSerializer			transformSerializer;
		private readonly GraphicState					currentState;
		private readonly GraphicState					lastState;

		private Font									currentFont;
	}
}
