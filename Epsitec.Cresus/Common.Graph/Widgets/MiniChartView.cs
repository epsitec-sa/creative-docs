//	Copyright © 2009-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Graph.Widgets
{
	public class MiniChartView : ChartView
	{
		public MiniChartView()
		{
			this.LabelColor = Color.FromRgb (250/255.0, 255/255.0, 124/255.0); // Color.FromRgb (255/255.0, 254/255.0, 173/255.0)
			this.hiliteColor = Color.Empty;
		}


		public string Label
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public bool PaintPaperClip
		{
			get;
			set;
		}

		public bool PaintPaperStack
		{
			get;
			set;
		}

		public bool AutoCheckButton
		{
			get;
			set;
		}

		public Color LabelColor
		{
			get;
			set;
		}

		public Color HiliteColor
		{
			get
			{
				return this.hiliteColor;
			}
			set
			{
				if (this.hiliteColor != value)
				{
					this.hiliteColor = value;
					this.Invalidate ();
				}
			}
		}

		public bool DisplayValue
		{
			get;
			set;
		}

		public bool DisplaySample
		{
			get;
			set;
		}


		public void DefineIconButton(ButtonVisibility visibility, string iconUri, System.Action buttonClicked)
		{
			if (visibility != ButtonVisibility.Hide)
			{
				if (this.iconButton == null)
				{
					this.iconButtonVisibility = visibility;

					this.iconButton = new IconButton ()
					{
						Parent = this,
						Anchor = AnchorStyles.TopRight,
						Margins = new Margins (0, 0, 0, 0),
						PreferredWidth = 19,
						PreferredHeight = 19,
						IconUri = iconUri,
						AutoFocus = false
					};

					if ((visibility == ButtonVisibility.Show) ||
						(visibility == ButtonVisibility.ShowOnlyWhenEntered && this.IsEntered))
					{
						this.iconButton.Show ();
					}
					else
					{
						this.iconButton.Hide ();
					}

					if (buttonClicked != null)
					{
						this.iconButton.Clicked += (sender, e) => buttonClicked ();
					};
				}

				this.HideCheckButton ();
			}
			else
			{
				if (this.iconButton != null)
				{
					this.iconButton.Dispose ();
					this.iconButton = null;
				}
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var renderer  = this.Renderer;
			var scale     = this.Scale;
			var rectangle = Rectangle.Deflate (this.Client.Bounds, this.Padding);
			int seriesCount = renderer == null ? 0 : renderer.SeriesItems.Count;

			graphics.LineWidth = 1.0;
			graphics.LineJoin = JoinStyle.Miter;

			if ((this.PaintPaperStack) &&
				(seriesCount > 1))
			{
				var transform = graphics.Transform;
				graphics.RotateTransformDeg (3, rectangle.Center.X, rectangle.Center.Y);
				this.PaintBackSheet (graphics, rectangle);

				if (seriesCount > 2)
				{
					graphics.RotateTransformDeg (-3-4, rectangle.Center.X, rectangle.Center.Y);
					this.PaintBackSheet (graphics, rectangle);
				}

				graphics.Transform = transform;
			}

			this.PaintTopmostSheet (graphics, rectangle, this.TransformColor (this.hiliteColor.IsEmpty ? this.BackColor : this.hiliteColor));

			if ((renderer != null) &&
				(renderer.SeriesItems.Count > 0))
			{
				int valueCount = renderer.ValueCount;

				if ((valueCount > 1) ||
					((valueCount == 1) && (this.DisplayValue == false)))
				{
					MiniChartView.PaintGraph (graphics, renderer, rectangle, scale);
				}
				else if (valueCount == 1)
				{
					string value = string.Format ("{0:N}", (decimal) renderer.SeriesItems[0].Values[0].Value);
					MiniChartView.PaintValueAsText (graphics, Rectangle.Offset (rectangle, 0, System.Math.Round (this.Client.Size.Height / 3)), value);
				}

				if (this.DisplaySample)
                {
					renderer.FindStyle ("line-color").ApplyStyle (0, graphics);

					graphics.AddFilledRectangle (new Rectangle (rectangle.Right-12, rectangle.Top-12, 8, 8));
					graphics.RenderSolid ();
					graphics.AddRectangle (new Rectangle (rectangle.Right-12.5, rectangle.Top-12.5, 8, 8));
					graphics.RenderSolid (Color.FromBrightness (0.5));
				}

				MiniChartView.PaintTitle (graphics, rectangle, this.Title);
			}

			if (!string.IsNullOrEmpty (this.Label))
			{
				MiniChartView.PaintNote (graphics, rectangle, this.Label, this.TransformColor (this.LabelColor));
			}

			if (this.ActiveState != ActiveState.No)
			{
				MiniChartView.PaintCheckMark (graphics, rectangle, this.PaintState);
			}

			if (this.IsSelected)
			{
				graphics.LineWidth = 2.0;
				graphics.Color = this.TransformColor (Epsitec.Common.Widgets.Adorners.Factory.Active.ColorCaption);
				graphics.AddRectangle (Rectangle.Inflate (rectangle, new Margins (1, 1, 1, 1)));
				graphics.RenderSolid ();
			}

			if (this.PaintPaperClip)
			{
				MiniChartView.PaintPaperClipAdorner (graphics, rectangle);
			}
		}
		
		protected override void OnEntered(MessageEventArgs e)
		{
			if ((this.AutoCheckButton) &&
				(this.iconButton == null))
			{
				this.checkButton = new CheckButton ()
				{
					Parent = this,
					Anchor = AnchorStyles.TopRight,
					Margins = new Margins (0, 2, 2, 0),
					PreferredWidth = 15,
					PreferredHeight = 15,
					ActiveState = this.ActiveState
				};

				this.checkButton.ActiveStateChanged += sender => this.ActiveState = ((CheckButton) sender).ActiveState;
			}
			else if ((this.iconButtonVisibility == ButtonVisibility.ShowOnlyWhenEntered) &&
			         (this.iconButton != null))
			{
				this.iconButton.Show ();
			}

			base.OnEntered (e);
		}

		protected override void OnExited(MessageEventArgs e)
		{
			this.HideCheckButton ();

			if ((this.iconButtonVisibility == ButtonVisibility.ShowOnlyWhenEntered) &&
				(this.iconButton != null))
			{
				this.iconButton.Hide ();
			}

			base.OnExited (e);
		}

		protected override void OnActiveStateChanged()
		{
			if (this.checkButton != null)
			{
				this.checkButton.ActiveState = this.ActiveState;
			}

			base.OnActiveStateChanged ();
		}

		private Color TransformColor(Color color)
		{
			if (this.IsEnabled || color.IsEmpty)
			{
				return color;
			}
			else
			{
				var brightness = color.GetBrightness ();
				var alpha = color.A;

				return Color.FromAlphaRgb (alpha, brightness, brightness, brightness);
			}
		}
		
		private void HideCheckButton()
		{
			if (this.checkButton != null)
			{
				this.checkButton.Dispose ();
				this.checkButton = null;
			}
		}


		private static void PaintGraph(Graphics graphics, Epsitec.Common.Graph.Renderers.AbstractRenderer renderer, Rectangle rectangle, double scale)
		{
			var transform = graphics.Transform;
			graphics.ScaleTransform (scale, scale, 0, 0);

			Rectangle paint = rectangle;

			paint = Rectangle.Deflate (paint, new Margins (6, 6, 6, 20));
			graphics.AddFilledRectangle (Rectangle.Scale (paint, 1/scale));
			graphics.RenderSolid (Color.FromAlphaRgb (1.0, 1.0, 1.0, 1.0));

			var bounds = Rectangle.Deflate (paint, new Margins (4.5, 9, 9, 5));
            renderer.Render (graphics, Rectangle.Scale (paint, 1 / scale), Rectangle.Scale (bounds, 1 / scale));

			graphics.LineWidth = 1.0;
			graphics.LineJoin = JoinStyle.Miter;
			graphics.Transform = transform;
		}

		private static void PaintValueAsText(Graphics graphics, Rectangle rectangle, string text)
		{
			if (!string.IsNullOrEmpty (text))
			{
				Font   font     = Font.DefaultFont;
				double fontSize = 16.0;

			again:
				int length = FitText (rectangle.Width, font, fontSize, text);

				if (length < text.Length)
				{
					fontSize -= 1.0;
					goto again;
				}

				graphics.Color = Color.FromBrightness (0.0);
				graphics.PaintText (rectangle.X, rectangle.Y, rectangle.Width, 20, text, font, fontSize, ContentAlignment.MiddleCenter);
				graphics.RenderSolid ();
			}
		}

		private static void PaintTitle(Graphics graphics, Rectangle rectangle, string text)
		{
			if (!string.IsNullOrEmpty (text))
			{
				Font   font     = Font.GetFont ("Futura", "Condensed Medium") ?? Font.DefaultFont;
				double fontSize = 11.0;
				
				int length = FitText (rectangle.Width, font, fontSize, text);

				if (length < text.Length)
				{
					length = FitText (rectangle.Width, font, fontSize, ".." + text);
					text = text.Substring (0, length-2) + "..";
				}

				graphics.Color = Color.FromBrightness (0.0);
				graphics.PaintText (rectangle.X, rectangle.Y, rectangle.Width, 20, text, font, fontSize, ContentAlignment.MiddleCenter);
				graphics.RenderSolid ();
			}
		}

		private static int FitText(double width, Font font, double fontSize, string text)
		{
			double[] xPos;
			double xLength = width / fontSize;

			font.GetTextCharEndX (text, out xPos);

			for (int i = 0; i < xPos.Length; i++)
			{
				if (xPos[i] > xLength)
				{
					return i;
				}
			}

			return text.Length;
		}

		private void PaintBackSheet(Graphics graphics, Rectangle rectangle)
		{
			this.AddFrameSurface (graphics, Rectangle.Inflate (rectangle, 1, 1));
			graphics.RenderSolid (Color.FromAlphaRgb (0.2, 0.8, 0.8, 0.8));
			this.AddFrameSurface (graphics, rectangle);
			graphics.RenderSolid (Color.FromAlphaRgb (0.6, 0.8, 0.8, 0.8));
			this.AddFrameSurface (graphics, Rectangle.Deflate (rectangle, 1, 1));
			graphics.RenderSolid (Color.FromBrightness (1.0));
		}



		protected void AddFrameSurface(Graphics graphics, Rectangle rectangle)
		{
			using (Path path = new Path ())
			{
				this.AppendFramePath (path, rectangle);
				graphics.Rasterizer.AddSurface (path);
			}
		}

		protected void AddFrameOutline(Graphics graphics, Rectangle rectangle)
		{
			using (Path path = new Path ())
			{
				this.AppendFramePath (path, rectangle);
				graphics.Rasterizer.AddOutline (path);
			}
		}

		protected virtual void AppendFramePath(Path path, Rectangle rectangle)
		{
			path.AppendRectangle (rectangle);
		}

		private void PaintTopmostSheet(Graphics graphics, Rectangle rectangle, Color hiliteColor)
		{
			this.PaintTopmostSheetBackground (graphics, rectangle, hiliteColor);
			this.AddFrameOutline (graphics, Rectangle.Deflate (rectangle, 0.5, 0.5));
			graphics.RenderSolid (Color.FromBrightness (0.8));
		}

		protected virtual void PaintTopmostSheetBackground(Graphics graphics, Rectangle rectangle, Color hiliteColor)
		{
			Color sheetColor = hiliteColor.IsValid ? hiliteColor : Color.FromRgb (0.9, 0.9, 0.95);
			Color whiteColor = MiniChartView.GetLightColor (sheetColor);

			this.AddFrameSurface (graphics, rectangle);
			
			graphics.GradientRenderer.Fill = GradientFill.Y;
			graphics.GradientRenderer.SetColors (whiteColor, sheetColor);
			graphics.GradientRenderer.SetParameters (0, 100);
			graphics.GradientRenderer.Transform = Transform.Identity.Scale (1.0, rectangle.Height / 100).Translate (rectangle.BottomLeft).RotateDeg (10, rectangle.Center);
			graphics.RenderGradient ();
		}
		
		public static void PaintNote(Graphics graphics, Rectangle rectangle, string text, Color labelColor)
		{
			var label = new Rectangle (rectangle.Left + 2, rectangle.Top - 18 - 6, 48, 18);

			var transform = graphics.Transform;
			graphics.RotateTransformDeg (5, label.Center.X, label.Center.Y);

			MiniChartView.PaintShadow (graphics, label);
			graphics.AddFilledRectangle (label);

			Color color1 = labelColor;
			Color color2 = MiniChartView.GetLightColor (color1);
			
			graphics.GradientRenderer.Fill = GradientFill.Y;
			graphics.GradientRenderer.SetColors (color1, color2);
			graphics.GradientRenderer.SetParameters (0, 100);
			graphics.GradientRenderer.Transform = Transform.Identity.Scale (1.0, label.Height / 100.0).Translate (label.BottomLeft);
			graphics.RenderGradient ();

			Font font = Font.GetFont ("Futura", "Condensed Medium") ?? Font.DefaultFont;

			graphics.Color = Color.FromBrightness (0.0);
			graphics.PaintText (label.X, label.Y, label.Width, label.Height, text, font, 14.0, ContentAlignment.MiddleCenter);
			graphics.RenderSolid ();

			graphics.Transform = transform;
		}

		public static Color GetLightColor(Color color)
		{
			return Color.Mix (color, Color.FromBrightness (1), 0.75);
		}

		public static void PaintShadow(Graphics graphics, Rectangle rect)
		{
			rect = Rectangle.Deflate (rect, new Margins (2, 0, 2, 0));
			double[] alpha = new double[] { 0.4, 0.3, 0.2 };

			for (int i = 0; i < alpha.Length; i++)
			{
				rect = Rectangle.Inflate (rect, 1, 1);
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (Color.FromAlphaRgb (alpha[i], 0.8, 0.8, 0.8));
			}
		}

		private static void PaintCheckMark(Graphics graphics, Rectangle rectangle, WidgetPaintState state)
		{
			var rect = new Rectangle (rectangle.Right - 17-1, rectangle.Top - 17, 15, 15);
			
			graphics.Color = (state & WidgetPaintState.Enabled) != 0 ? Color.FromRgb (33.0/255.0, 161.0/255.0, 33.0/255.0)
				/**/												 : Color.FromRgb (198.0/255.0, 197.0/255.0, 201.0/255.0);
			

			if ((state & WidgetPaintState.ActiveYes) != 0)
			{
				rect = Rectangle.Deflate (rect, 1, 1);
				
				using (Drawing.Path path = new Drawing.Path ())
				{
					var center = rect.Center;
					
					path.MoveTo (center.X-rect.Width*0.1, center.Y-rect.Height*0.1);
					path.LineTo (center.X+rect.Width*0.3, center.Y+rect.Height*0.3);
					path.LineTo (center.X+rect.Width*0.3, center.Y+rect.Height*0.1);
					path.LineTo (center.X-rect.Width*0.1, center.Y-rect.Height*0.3);
					path.LineTo (center.X-rect.Width*0.3, center.Y-rect.Height*0.1);
					path.LineTo (center.X-rect.Width*0.3, center.Y+rect.Height*0.1);
					path.Close ();
					
					graphics.PaintSurface (path);
				}
			}
			else if ((state & WidgetPaintState.ActiveMaybe) != 0)
			{
				rect = Rectangle.Deflate (rect, 4, 4);

				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid ();
			}
		}

		private static void PaintPaperClipAdorner(Graphics graphics, Rectangle rectangle)
		{
			var image = Epsitec.Common.Support.ImageProvider.Instance.GetImage ("manifest:Epsitec.Common.Graph.Images.PaperClip.icon", Support.Resources.DefaultManager);
			graphics.PaintImage (image, new Rectangle (rectangle.X + 10, rectangle.Top - 34, 20, 40));
		}


		public static Color StickyNoteYellow = Color.FromRgb (1.0, 1.0, 0.8);

		private CheckButton checkButton;
		private IconButton iconButton;
		private ButtonVisibility iconButtonVisibility;
		private Color hiliteColor;
	}
}
