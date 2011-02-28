//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// La classe Graphics encapsule le contexte graphique utilisé pour peindre.
	/// </summary>
	public class Graphics : System.IDisposable, IPaintPort
	{
		static Graphics()
		{
			Agg.Library.Initialize ();
		}
		
		
		public Graphics()
		{
			this.ResetLineStyle ();
			
			this.pixmap     = new Pixmap ();
			this.rasterizer = new Common.Drawing.Rasterizer ();
			this.transform  = Transform.Identity;

			this.colorModifierStack = new Stack<ColorModifierCallback> ();
			
			this.solidRenderer    = new Common.Drawing.Renderers.Solid ();
			this.imageRenderer    = new Common.Drawing.Renderers.Image (this);
			this.gradientRenderer = new Common.Drawing.Renderers.Gradient (this);
			this.smoothRenderer   = new Common.Drawing.Renderers.Smooth (this);
			
			this.rasterizer.Gamma = 1.2;
		}

		public double							LineWidth
		{
			get { return this.lineWidth; }
			set { this.lineWidth = value; }
		}
		
		public JoinStyle						LineJoin
		{
			get { return this.lineJoin; }
			set { this.lineJoin = value; }
		}
		
		public CapStyle							LineCap
		{
			get { return this.lineCap; }
			set { this.lineCap = value; }
		}
		
		public double							LineMiterLimit
		{
			get { return this.lineMiterLimit; }
			set { this.lineMiterLimit = value; }
		}

		public Rectangle						ClipBounds
		{
			get
			{
				return this.SaveClippingRectangle ();
			}
		}

		public Point							ClipOffset
		{
			get
			{
				return new Point (this.clipOx, this.clipOy);
			}
			set
			{
				this.clipOx = value.X;
				this.clipOy = value.Y;
			}
		}
		
		public RichColor						RichColor
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
		
		public Color							Color
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

		public RichColor						FinalRichColor
		{
			get
			{
				return RichColor.FromColor(this.FinalColor);
			}
			set
			{
				this.FinalColor = value.Basic;
			}
		}
		
		public Color							FinalColor
		{
			get
			{
				return this.SolidRenderer.Color;
			}
			set
			{
				this.SolidRenderer.Color = value;
			}
		}

		public ImageFilter						ImageFilter
		{
			get
			{
				return this.imageFilter;
			}
			set
			{
				this.imageFilter = value;
			}
		}
		
		public Margins							ImageCrop
		{
			get
			{
				return this.imageCrop;
			}
			set
			{
				this.imageCrop = value;
			}
		}
		
		public Size								ImageFinalSize
		{
			get
			{
				return this.imageFinalSize;
			}
			set
			{
				this.imageFinalSize = value;
			}
		}
		
		
		public FillMode							FillMode
		{
			get
			{
				return this.Rasterizer.FillMode;
			}
			set
			{
				this.Rasterizer.FillMode = value;
			}
		}

		public Drawing.AbstractRasterizer		Rasterizer
		{
			get
			{
				return this.rasterizer;
			}
		}
		
		public Drawing.Transform				Transform
		{
			get
			{
				return this.transform;
			}
			set
			{
				this.transform = value;
				this.UpdateTransform ();
			}
		}
		
		public Drawing.Pixmap					Pixmap
		{
			get { return this.pixmap; }
		}
		
		public Renderers.Solid					SolidRenderer
		{
			get { return this.solidRenderer; }
		}
		
		public Renderers.Image					ImageRenderer
		{
			get { return this.imageRenderer; }
		}
		
		public Renderers.Gradient				GradientRenderer
		{
			get { return this.gradientRenderer; }
		}
		
		public Renderers.Smooth					SmoothRenderer
		{
			get { return this.smoothRenderer; }
		}
		
		
		public void ReplaceRasterizer(AbstractRasterizer rasterizer)
		{
			this.rasterizer = rasterizer;
			this.rasterizer.Transform = this.transform;
		}
		
		
		public void RenderSolid()
		{
			this.rasterizer.Render (this.solidRenderer);
		}
		
		public void RenderSolid(Color color)
		{
			this.Color = color;
			this.rasterizer.Render (this.solidRenderer);
		}
		
		public void FinalRenderSolid(Color color)
		{
			this.FinalColor = color;
			this.rasterizer.Render (this.solidRenderer);
		}
		
		public void RenderImage()
		{
			this.rasterizer.Render (this.imageRenderer);
		}
		
		public void RenderGradient()
		{
			this.rasterizer.Render (this.gradientRenderer);
		}
		
		public void ResetLineStyle()
		{
			this.LineWidth      = 1.0;
			this.LineJoin       = JoinStyle.Miter;
			this.LineCap        = CapStyle.Square;
			this.LineMiterLimit = 4.0;
		}
		
		
		public void AllocatePixmap()
		{
			this.pixmap.AllocatePixmap (new System.Drawing.Size (8, 8));
			
			this.solidRenderer.Pixmap    = this.pixmap;
			this.imageRenderer.Pixmap    = this.pixmap;
			this.gradientRenderer.Pixmap = this.pixmap;
			this.smoothRenderer.Pixmap   = this.pixmap;
		}
		
		public Epsitec.Common.Drawing.Graphics CreateAlphaMask()
		{
			Graphics mask = new Graphics ();
			
			mask.SetPixmapSize (this.pixmap.Size.Width, this.pixmap.Size.Height);
			mask.SolidRenderer.ClearAlphaRgb (0, 0, 0, 0);
			mask.Transform = this.Transform;
			
			return mask;
		}
		
		
		public void PushColorModifier(ColorModifierCallback method)
		{
			this.colorModifierStack.Push (method);
		}

		public ColorModifierCallback PopColorModifier()
		{
			return this.colorModifierStack.Pop () as ColorModifierCallback;
		}

		
		public RichColor GetFinalColor(RichColor color)
		{
			foreach (ColorModifierCallback method in this.colorModifierStack)
			{
				color = method (color);
			}
			
			return color;
		}
		
		public Color GetFinalColor(Color color)
		{
			if (this.colorModifierStack.Count == 0)
			{
				return color;
			}
			
			RichColor rich = RichColor.FromColor (color);
			
			foreach (ColorModifierCallback method in this.colorModifierStack)
			{
				rich = method (rich);
			}
			
			return rich.Basic;
		}

		
		public void PaintOutline(Path path)
		{
			this.Rasterizer.AddOutline (path, this.lineWidth, this.lineCap, this.lineJoin, this.lineMiterLimit);
			this.RenderSolid ();
		}
		
		public void PaintSurface(Path path)
		{
			this.Rasterizer.AddSurface (path);
			this.RenderSolid ();
		}

		public void PaintDashedOutline(Path path, double width, double dash, double gap, CapStyle capStyle, Color color)
		{
			//	Dessine un traitillé simple (dash/gap) le long d'un chemin.
			if (path.IsEmpty)
			{
				return;
			}

			using (var dp = new DashedPath ())
			{
				dp.Append (path);

				if (dash == 0.0)  // juste un point ?
				{
					dash = 0.00001;
					gap -= dash;
				}
				dp.AddDash (dash, gap);

				using (Path temp = dp.GenerateDashedPath ())
				{
					this.Rasterizer.AddOutline (temp, width, capStyle, JoinStyle.Round, 5.0);
					this.RenderSolid (color);
				}
			}
		}

		
		public void PaintGlyphs(Font font, double size, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy)
		{
			for (int i = 0; i < glyphs.Length; i++)
			{
				this.Rasterizer.AddGlyph (font, glyphs[i], x[i], y[i], size, sx == null ? 1 : sx[i], sy == null ? 1 : sy[i]);
			}
		}
		
		public bool PaintCachedGlyphs(Font font, double scale, ushort[] glyphs, double[] x, double[] y, double[] sx, Color color)
		{
			if (this.transform.OnlyScaleOrTranslate)
			{
				double xx = this.transform.XX;
				double yy = this.transform.YY;
				double tx = this.transform.TX;
				double ty = this.transform.TY;

				double scaleX = scale * xx;
				double scaleY = scale * yy;

				if ((scaleX < 2) || (scaleX > 25) ||
					(scaleY < 2) || (scaleY > 25))
				{
					return false;
				}

				font.PaintPixelGlyphs (this.pixmap, scale, glyphs, x, y, sx, this.GetFinalColor (color), xx, yy, tx, ty);

				return true;
			}
			else
			{
				return false;
			}
		}
		
		
		
		public void   PaintText(double x, double y, double width, double height, string text, Font font, double size, ContentAlignment align)
		{
			double textWidth  = font.GetTextAdvance (text) * size;
			double textHeight = (font.Ascender - font.Descender) * size;
			
			switch (align)
			{
				case ContentAlignment.BottomLeft:
				case ContentAlignment.BottomCenter:
				case ContentAlignment.BottomRight:
					y = y - font.Descender * size;
					break;
				
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.MiddleRight:
					y = y + (height - textHeight) / 2 - font.Descender * size;
					break;
				
				case ContentAlignment.TopLeft:
				case ContentAlignment.TopCenter:
				case ContentAlignment.TopRight:
					y = y + height - textHeight - font.Descender * size;
					break;
			}
			
			switch (align)
			{
				case ContentAlignment.BottomLeft:
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.TopLeft:
					break;
				
				case ContentAlignment.BottomCenter:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.TopCenter:
					x = x + (width - textWidth) / 2;
					break;
				
				case ContentAlignment.BottomRight:
				case ContentAlignment.MiddleRight:
				case ContentAlignment.TopRight:
					x = x + width - textWidth;
					break;
			}
			
			this.PaintText (x, y, text, font, size);
		}
		
		public double PaintText(double x, double y, string text, Font font, double size)
		{
			if (this.transform.OnlyTranslate && ! font.IsSynthetic)
			{
				x += this.transform.TX;
				y += this.transform.TY;
				
				return font.PaintPixelCache (this.pixmap, text, size, x, y, this.SolidRenderer.Color);
			}
			else
			{
				double advance = this.AddText (x, y, text, font, size);
				this.RenderSolid ();
				return advance;
			}
		}
		
		public double PaintText(double x, double y, string text, Font font, double size, FontClassInfo[] infos)
		{
			//	TODO: déplacer ce code dans la librairie AGG; faire en sorte que ça marche aussi
			//	si ClassId != ClassId.Space...
			
			for (int i = 0; i < infos.Length; i++)
			{
				if ((infos[i].Scale != 1.00) &&
					(infos[i].GlyphClass == GlyphClass.Space))
				{
					string[] texts = text.Split (new char[] { ' ', (char) 160 });
					double spaceW = font.GetCharAdvance (' ') * size * infos[i].Scale;
					double totalW = 0;
					
					for (int j = 0; j < texts.Length; j++)
					{
						double w = this.PaintText (x, y, texts[j], font, size) + spaceW;
						
						totalW += w;
						x       += w;
					}
					
					return totalW - spaceW;
				}
			}
			
			return this.PaintText (x, y, text, font, size);
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
			if (bitmap == null)
			{
				return;
			}
			double ix1 = imageOriginX;
			double iy1 = imageOriginY;
			double ix2 = imageOriginX + imageWidth;
			double iy2 = imageOriginY + imageHeight;
			
			double idx = bitmap.Width;
			double idy = bitmap.Height;
			
			if (ix1 >= idx) return;
			if (iy1 >= idy) return;
			
			if (ix1 < 0)			//	Clipping à gauche.
			{
				fillX     -= ix1;
				fillWidth += ix1;
				ix1 = 0;
			}
			
			if (iy1 < 0)			//	Clipping en bas
			{
				fillY      -= iy1;
				fillHeight += iy1;
				iy1 = 0;
			}
			
			if (ix2 > idx)			//	Clipping à droite
			{
				fillWidth -= ix2 - idx;
				ix2 = idx;
			}
			
			if (iy2 > idy)			//	Clipping en haut
			{
				fillHeight -= iy2 - idy;
				iy2 = idy;
			}

			Transform transform = Transform.Identity;
			
#if true
			Point vectorOo = new Point (0, 0); vectorOo = this.ApplyTransformDirect (vectorOo);
			Point vectorOx = new Point (1, 0); vectorOx = this.ApplyTransformDirect (vectorOx) - vectorOo;
			Point vectorOy = new Point (0, 1); vectorOy = this.ApplyTransformDirect (vectorOy) - vectorOo;

			double adjust = this.imageFilter.Mode == ImageFilteringMode.Bilinear ? 1 : 0;
			
			double fixX = System.Math.Sqrt (vectorOx.X * vectorOx.X + vectorOx.Y * vectorOx.Y);
			double fixY = System.Math.Sqrt (vectorOy.X * vectorOy.X + vectorOy.Y * vectorOy.Y);

			double sx = (ix2-ix1 <= 1) ? (Graphics.AlmostInfinite) : ((fillWidth  > adjust/fixX) ? (fillWidth-adjust/fixX)  / (ix2-ix1-adjust) : 1.0);
			double sy = (iy2-iy1 <= 1) ? (Graphics.AlmostInfinite) : ((fillHeight > adjust/fixY) ? (fillHeight-adjust/fixY) / (iy2-iy1-adjust) : 1.0);
#else
			double sx = (ix2-ix1 < 1) ? (Graphics.AlmostInfinite) : ((fillWidth > 1)  ? (fill_width)  / (ix2-ix1) : 1.0);
			double sy = (iy2-iy1 < 1) ? (Graphics.AlmostInfinite) : ((fillHeight > 1) ? (fill_height) / (iy2-iy1) : 1.0);
#endif
			
			Drawing.Bitmap bmi = bitmap.BitmapImage;
			
			sx *= bitmap.Width  / bmi.PixelWidth;
			sy *= bitmap.Height / bmi.PixelHeight;
			
			transform = transform.Translate (-ix1, -iy1);
			transform = transform.Scale (sx, sy);
			transform = transform.Translate (fillX, fillY);
			
			this.AddFilledRectangle (fillX, fillY, fillWidth, fillHeight);
			this.ImageRenderer.BitmapImage = bitmap;
			this.ImageRenderer.Transform = transform;

			this.ImageRenderer.SelectAdvancedFilter (this.imageFilter.Mode, this.imageFilter.Radius);
			this.RenderImage ();
			this.ImageRenderer.BitmapImage = null;
		}

		public void DrawVerticalGradient(Rectangle rect, Color bottomColor, Color topColor)
		{
			this.FillMode = FillMode.NonZero;
			this.GradientRenderer.Fill = GradientFill.Y;
			this.GradientRenderer.SetColors (bottomColor, topColor);
			this.GradientRenderer.SetParameters (-100, 100);

			Transform ot = this.GradientRenderer.Transform;
			Transform t = Transform.Identity;
			Point center = rect.Center;
			t = t.Scale (rect.Width/100/2, rect.Height/100/2);
			t = t.Translate (center);
			this.GradientRenderer.Transform = t;
			this.RenderGradient ();
			this.GradientRenderer.Transform = ot;
		}


		public void PaintHorizontalGradient(Rectangle rect, Color leftColor, Color rightColor)
		{
			this.FillMode = FillMode.NonZero;
			this.GradientRenderer.Fill = GradientFill.X;
			this.GradientRenderer.SetParameters (-100, 100);
			this.GradientRenderer.SetColors (leftColor, rightColor);
			
			Transform ot = this.GradientRenderer.Transform;
			Transform t = Transform.Identity;
			Point center = rect.Center;
			t = t.Scale (rect.Width/100/2, rect.Height/100/2);
			t = t.Translate (center);
			this.GradientRenderer.Transform = t;
			this.RenderGradient ();  // dégradé de gauche à droite
			this.GradientRenderer.Transform = ot;
		}

		public void PaintCircularGradient(Rectangle rect, Color borderColor, Color centerColor)
		{
			Transform ot = this.GradientRenderer.Transform;

			this.GradientRenderer.Fill = GradientFill.Circle;
			this.GradientRenderer.SetParameters (0, 100);
			this.GradientRenderer.SetColors (borderColor, centerColor);

			Transform t = Transform.Identity;
			Point center = rect.Center;
			t = t.Scale (rect.Width/100/2, rect.Height/100/2);
			t = t.Translate (center);
			this.GradientRenderer.Transform = t;
			this.RenderGradient ();  // dégradé circulaire
			this.GradientRenderer.Transform = ot;
		}




		public void AddLine(Point p1, Point p2)
		{
			this.AddLine (p1.X, p1.Y, p2.X, p2.Y);
		}
		
		public void AddRectangle(Point p, Size s)
		{
			this.AddRectangle (p.X, p.Y, s.Width, s.Height);
		}
		
		public void AddRectangle(Rectangle rect)
		{
			this.AddRectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}
		
		public void AddCircle(Point p, double r)
		{
			this.AddCircle (p.X, p.Y, r, r);
		}
		
		public void AddCircle(Point p, double rx, double ry)
		{
			this.AddCircle (p.X, p.Y, rx, ry);
		}
		
		public void AddCircle(double x, double y, double r)
		{
			this.AddCircle (x, y, r, r);
		}
		
		public void AddFilledRectangle(Point p, Size s)
		{
			this.AddFilledRectangle (p.X, p.Y, s.Width, s.Height);
		}
		
		public void AddFilledRectangle(Rectangle rect)
		{
			this.AddFilledRectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}
		
		public void AddFilledCircle(Point p, double r)
		{
			this.AddFilledCircle (p.X, p.Y, r, r);
		}
		
		public void AddFilledCircle(Point p, double rx, double ry)
		{
			this.AddFilledCircle (p.X, p.Y, rx, ry);
		}
		
		public void AddFilledCircle(double x, double y, double r)
		{
			this.AddFilledCircle (x, y, r, r);
		}
		
		public void AddText(double x, double y, double width, double height, string text, Font font, double size, ContentAlignment align)
		{
			double textWidth  = font.GetTextAdvance (text) * size;
			double textHeight = (font.Ascender - font.Descender) * size;
			
			switch (align)
			{
				case ContentAlignment.BottomLeft:
				case ContentAlignment.BottomCenter:
				case ContentAlignment.BottomRight:
					y = y - font.Descender * size;
					break;
				
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.MiddleRight:
					y = y + (height - textHeight) / 2 - font.Descender * size;
					break;
				
				case ContentAlignment.TopLeft:
				case ContentAlignment.TopCenter:
				case ContentAlignment.TopRight:
					y = y + height - textHeight - font.Descender * size;
					break;
			}
			
			switch (align)
			{
				case ContentAlignment.BottomLeft:
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.TopLeft:
					break;
				
				case ContentAlignment.BottomCenter:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.TopCenter:
					x = x + (width - textWidth) / 2;
					break;
				
				case ContentAlignment.BottomRight:
				case ContentAlignment.MiddleRight:
				case ContentAlignment.TopRight:
					x = x + width - textWidth;
					break;
			}
			
			this.AddText (x, y, text, font, size);
		}
		
		
		public void   AddLine(double x1, double y1, double x2, double y2)
		{
			using (Path path = new Path ())
			{
				path.MoveTo (x1, y1);
				path.LineTo (x2, y2);
				
				this.rasterizer.AddOutline (path, this.lineWidth, this.lineCap, this.lineJoin, this.lineMiterLimit);
			}
		}
		
		public void   AddRectangle(double x, double y, double width, double height)
		{
			using (Path path = new Path ())
			{
				path.MoveTo (x, y);
				path.LineTo (x+width, y);
				path.LineTo (x+width, y+height);
				path.LineTo (x, y+height);
				path.Close ();
				
				this.rasterizer.AddOutline (path, this.lineWidth, this.lineCap, this.lineJoin, this.lineMiterLimit);
			}
		}
		
		public void   AddCircle(double cx, double cy, double rx, double ry)
		{
			using (Path path = Path.CreateCircle (cx, cy, rx, ry))
			{
				this.rasterizer.AddOutline (path, this.lineWidth, this.lineCap, this.lineJoin, this.lineMiterLimit);
			}
		}
		
		public double AddText(double x, double y, string text, Font font, double size)
		{
			return this.rasterizer.AddText (font, text, x, y, size);
		}
		
		public void   AddFilledRectangle(double x, double y, double width, double height)
		{
			using (Path path = new Path ())
			{
				path.MoveTo (x, y);
				path.LineTo (x+width, y);
				path.LineTo (x+width, y+height);
				path.LineTo (x, y+height);
				path.Close ();
				
				this.rasterizer.AddSurface (path);
			}
		}
		
		public void   AddFilledCircle(double cx, double cy, double rx, double ry)
		{
			using (Path path = Path.CreateCircle (cx, cy, rx, ry))
			{
				this.rasterizer.AddSurface (path);
			}
		}
		
		public void Align(ref double x, ref double y)
		{
			this.transform.TransformDirect (ref x, ref y);
			x = System.Math.Floor (x + 0.5);
			y = System.Math.Floor (y + 0.5);
			this.transform.TransformInverse (ref x, ref y);
		}

		public Rectangle Align(Rectangle rect)
		{
			this.Align (ref rect);
			return rect;
		}
		
		public void Align(ref Drawing.Rectangle rect)
		{
			double x1 = rect.Left;
			double y1 = rect.Bottom;
			double x2 = rect.Right;
			double y2 = rect.Top;
			
			this.transform.TransformDirect (ref x1, ref y1);
			this.transform.TransformDirect (ref x2, ref y2);
			
			x1 = System.Math.Floor (x1 + 0.5);
			y1 = System.Math.Floor (y1 + 0.5);
			x2 = System.Math.Floor (x2 + 0.5);
			y2 = System.Math.Floor (y2 + 0.5);
			
			this.transform.TransformInverse (ref x1, ref y1);
			this.transform.TransformInverse (ref x2, ref y2);
			
			rect = new Rectangle (x1, y1, x2-x1, y2-y1);
		}
		
		
		public void Align(ref Drawing.Point p)
		{
			double x = p.X;
			double y = p.Y;
			
			this.Align (ref x, ref y);
			
			p.X = x;
			p.Y = y;
		}
		
		
		public void ScaleTransform(double sx, double sy, double cx, double cy)
		{
			this.transform = this.transform.MultiplyByPostfix (Drawing.Transform.CreateScaleTransform (sx, sy, cx, cy));
			this.UpdateTransform ();
		}
		
		public void RotateTransformDeg(double angle, double cx, double cy)
		{
			this.transform = this.transform.MultiplyByPostfix (Drawing.Transform.CreateRotationDegTransform (angle, cx, cy));
			this.UpdateTransform ();
		}
		
		public void RotateTransformRad(double angle, double cx, double cy)
		{
			this.transform = this.transform.MultiplyByPostfix (Drawing.Transform.CreateRotationRadTransform (angle, cx, cy));
			this.UpdateTransform ();
		}
		
		public void TranslateTransform(double ox, double oy)
		{
			this.transform = this.transform.MultiplyByPostfix (Drawing.Transform.CreateTranslationTransform (ox, oy));
			this.UpdateTransform ();
		}
		
		public void MergeTransform(Transform transform)
		{
			this.transform = this.transform.MultiplyByPostfix (transform);
			this.UpdateTransform ();
		}

		public Point ApplyTransformDirect(Point pt)
		{
			return this.transform.TransformDirect (pt);
		}
		
		public Point ApplyTransformInverse(Point pt)
		{
			return this.transform.TransformInverse (pt);
		}
		
		protected void UpdateTransform()
		{
			this.rasterizer.Transform = this.transform;
			
			//	Lorsque la matrice de transformation change, il faut aussi mettre à jour les
			//	transformations des renderers qui en ont...
			
			Transform tImage    = this.imageRenderer.Transform;
			Transform tGradient = this.gradientRenderer.Transform;
			
			this.imageRenderer.Transform    = tImage;
			this.gradientRenderer.Transform = tGradient;
		}
		
		
		private void SetClippingRectangle(double x, double y, double width, double height)
		{
			double x1 = x + this.clipOx;
			double y1 = y + this.clipOy;
			double x2 = x + width + this.clipOx;
			double y2 = y + height + this.clipOy;
			
			if (this.hasClipRect)
			{
				x1 = System.Math.Max (x1, this.clipX1);
				x2 = System.Math.Min (x2, this.clipX2);
				y1 = System.Math.Max (y1, this.clipY1);
				y2 = System.Math.Min (y2, this.clipY2);
			}
			else
			{
				this.hasClipRect = true;
			}
			
			this.clipX1 = x1;
			this.clipY1 = y1;
			this.clipX2 = x2;
			this.clipY2 = y2;
			
			this.rasterizer.SetClipBox (x1, y1, x2, y2);
			this.pixmap.EmptyClipping ();
			this.pixmap.AddClipBox (x1, y1, x2, y2);
		}
		
		public void SetClippingRectangles(Drawing.Rectangle[] rectangles)
		{
			if (rectangles.Length == 0)
			{
				return;
			}
			
			Drawing.Rectangle clip = this.SaveClippingRectangle ();
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			
			this.pixmap.EmptyClipping ();
			
			for (int i = 0; i < rectangles.Length; i++)
			{
				Drawing.Rectangle rect = Drawing.Rectangle.Intersection (rectangles[i], clip);
				
				if (!rect.IsEmpty)
				{
					this.pixmap.AddClipBox (rect.Left + this.clipOx, rect.Bottom + this.clipOy, rect.Right + this.clipOx, rect.Top + this.clipOy);
					bbox = Drawing.Rectangle.Union (bbox, rect);
				}
			}
			
			this.hasClipRect = true;
			this.rasterizer.SetClipBox (bbox.Left, bbox.Bottom, bbox.Right, bbox.Top);
		}
		
		public void SetClippingRectangles(System.Collections.ICollection rectangles)
		{
			if ((rectangles == null) ||
				(rectangles.Count == 0))
			{
				return;
			}
			
			Drawing.Rectangle clip = this.SaveClippingRectangle ();
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			
			this.pixmap.EmptyClipping ();
			
			foreach (Drawing.Rectangle rectangle in rectangles)
			{
				Drawing.Rectangle rect = Drawing.Rectangle.Intersection (rectangle, clip);
				
				if (!rect.IsEmpty)
				{
					this.pixmap.AddClipBox (rect.Left + this.clipOx, rect.Bottom + this.clipOy, rect.Right + this.clipOx, rect.Top + this.clipOy);
					bbox = Drawing.Rectangle.Union (bbox, rect);
				}
			}
			
			this.hasClipRect = true;
			this.rasterizer.SetClipBox (bbox.Left, bbox.Bottom, bbox.Right, bbox.Top);
		}
		
		
		public Drawing.Rectangle SaveClippingRectangle()
		{
			if (this.hasClipRect)
			{
				return new Drawing.Rectangle (this.clipX1, this.clipY1, this.clipX2-this.clipX1, this.clipY2-this.clipY1);
			}
			
			return Drawing.Rectangle.MaxValue;
		}
		
		public void RestoreClippingRectangle(Drawing.Rectangle rect)
		{
			if (rect == Drawing.Rectangle.MaxValue)
			{
				this.ResetClippingRectangle ();
			}
			else
			{
				this.clipX1 = rect.Left;
				this.clipY1 = rect.Bottom;
				this.clipX2 = rect.Right;
				this.clipY2 = rect.Top;
				
				this.hasClipRect = true;
				
				this.rasterizer.SetClipBox (this.clipX1, this.clipY1, this.clipX2, this.clipY2);
				this.pixmap.EmptyClipping ();
				this.pixmap.AddClipBox (this.clipX1, this.clipY1, this.clipX2, this.clipY2);
			}
		}
		
		public void ResetClippingRectangle()
		{
			this.rasterizer.ResetClipBox ();
			this.pixmap.InfiniteClipping ();
			this.hasClipRect = false;
		}
		
		
		public bool HasEmptyClippingRectangle
		{
			get
			{
				if (this.hasClipRect)
				{
					if ((this.clipX1 >= this.clipX2) ||
					(this.clipY1 >= this.clipY2))
					{
						return true;
					}
				}

				return false;
			}
		}
		
		
		public void SetClippingRectangle(Rectangle rect)
		{
			this.SetClippingRectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}
		
		
		public bool SetPixmapSize(int width, int height)
		{
			if ((this.pixmap.Size.Width == width) &&
				(this.pixmap.Size.Height == height))
			{
				return false;
			}
			
			this.pixmap.Size = new System.Drawing.Size (width, height);
			
			this.solidRenderer.Pixmap    = null;
			this.imageRenderer.Pixmap	 = null;
			this.gradientRenderer.Pixmap = null;
			this.smoothRenderer.Pixmap   = null;
			
			this.solidRenderer.Pixmap    = this.pixmap;
			this.imageRenderer.Pixmap    = this.pixmap;
			this.gradientRenderer.Pixmap = this.pixmap;
			this.smoothRenderer.Pixmap   = this.pixmap;
			
			return true;
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.pixmap != null)
				{
					this.pixmap.Dispose ();
				}
				if (this.rasterizer != null)
				{
					this.rasterizer.Dispose ();
				}
				if (this.solidRenderer != null)
				{
					this.solidRenderer.Pixmap = null;
					this.solidRenderer.Dispose ();
				}
				if (this.imageRenderer != null)
				{
					this.imageRenderer.Pixmap = null;
					this.imageRenderer.Dispose ();
				}
				if (this.gradientRenderer != null)
				{
					this.gradientRenderer.Pixmap = null;
					this.gradientRenderer.Dispose ();
				}
				if (this.smoothRenderer != null)
				{
					this.smoothRenderer.Pixmap = null;
					this.smoothRenderer.Dispose ();
				}
				
				this.pixmap            = null;
				this.rasterizer        = null;
				this.solidRenderer    = null;
				this.imageRenderer    = null;
				this.gradientRenderer = null;
				this.smoothRenderer   = null;
			}
		}

		
		private const double				AlmostInfinite = 1000000000.0;
		private ImageFilter					imageFilter = new ImageFilter(ImageFilteringMode.Bilinear);
		private Margins						imageCrop;
		private Size						imageFinalSize;
		
		private double						lineWidth;
		private JoinStyle					lineJoin;
		private CapStyle					lineCap;
		private double						lineMiterLimit;
		
		private Pixmap						pixmap;
		private AbstractRasterizer			rasterizer;
		private Transform					transform;
		
		private Renderers.Solid				solidRenderer;
		private Renderers.Image				imageRenderer;
		private Renderers.Gradient			gradientRenderer;
		private Renderers.Smooth			smoothRenderer;
		
		private double						clipX1, clipY1, clipX2, clipY2;
		private double						clipOx, clipOy;
		private bool						hasClipRect;

		private Color						originalColor;
		private Stack<ColorModifierCallback>	colorModifierStack;
	}
}
