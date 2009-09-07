//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

		public bool AutoCheckButton
		{
			get;
			set;
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var renderer  = this.Renderer;
			var scale     = this.Scale;
			var rectangle = Rectangle.Deflate (this.Client.Bounds, this.Padding);
			int seriesCount = renderer == null ? 0 : renderer.SeriesItems.Count;

			graphics.LineWidth = 1.0;
			graphics.LineJoin = JoinStyle.Miter;

			if (seriesCount > 1)
			{
				var transform = graphics.Transform;
				graphics.RotateTransformDeg (3, rectangle.Center.X, rectangle.Center.Y);
				MiniChartView.PaintBackSheet (graphics, rectangle);

				if (seriesCount > 2)
				{
					graphics.RotateTransformDeg (-3-4, rectangle.Center.X, rectangle.Center.Y);
					MiniChartView.PaintBackSheet (graphics, rectangle);
				}

				graphics.Transform = transform;
			}

			MiniChartView.PaintTopmostSheet (graphics, rectangle);

			if ((renderer != null) &&
				(renderer.SeriesItems.Count > 0))
			{
				var transform = graphics.Transform;
				graphics.ScaleTransform (scale, scale, 0, 0);

				Rectangle paint = rectangle;

				paint = Rectangle.Deflate (paint, new Margins (6, 6, 6, 20));
				graphics.AddFilledRectangle (Rectangle.Scale (paint, 1/scale));
				graphics.RenderSolid (Color.FromAlphaRgb (1.0, 1.0, 1.0, 1.0));

				paint = Rectangle.Deflate (paint, new Margins (4.5, 9, 9, 5));
				renderer.Render (graphics, Rectangle.Scale (paint, 1/scale));

				graphics.LineWidth = 1.0;
				graphics.LineJoin = JoinStyle.Miter;
				graphics.Transform = transform;

				Font   font     = Font.GetFont ("Futura", "Condensed Medium");
				double fontSize = 11.0;

				graphics.Color = Color.FromBrightness (0.0);
				graphics.PaintText (rectangle.X, rectangle.Y, rectangle.Width, 20, this.Title, font, fontSize, ContentAlignment.MiddleCenter);
				graphics.RenderSolid ();
			}

			if (!string.IsNullOrEmpty (this.Label))
			{
				MiniChartView.PaintNote (graphics, rectangle, this.Label);
			}

			if (this.ActiveState != ActiveState.No)
			{
				MiniChartView.PaintCheckMark (graphics, rectangle, this.PaintState);
			}

			if (this.IsSelected)
			{
				graphics.LineWidth = 2.0;
				graphics.Color = Epsitec.Common.Widgets.Adorners.Factory.Active.ColorCaption;
				graphics.AddRectangle (Rectangle.Inflate (rectangle, new Margins (1, 1, 1, 1)));
				graphics.RenderSolid ();
			}

			if (seriesCount > 1)
			{
				MiniChartView.PaintPaperClip (graphics, rectangle);
			}
		}

		public void ShowIconButton(ButtonVisibility visibility, System.Action action, string iconName)
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
						IconName = iconName,
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

					if (action != null)
					{
						this.iconButton.Clicked += (sender, e) => action ();
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

		private void HideCheckButton()
		{
			if (this.checkButton != null)
			{
				this.checkButton.Dispose ();
				this.checkButton = null;
			}
		}

		protected override void OnActiveStateChanged()
		{
			if (this.checkButton != null)
			{
				this.checkButton.ActiveState = this.ActiveState;
			}

			base.OnActiveStateChanged ();
		}

		
		private static void PaintBackSheet(Graphics graphics, Rectangle rectangle)
		{
			graphics.AddFilledRectangle (Rectangle.Inflate (rectangle, 1, 1));
			graphics.RenderSolid (Color.FromAlphaRgb (0.2, 0.8, 0.8, 0.8));
			graphics.AddFilledRectangle (rectangle);
			graphics.RenderSolid (Color.FromAlphaRgb (0.6, 0.8, 0.8, 0.8));
			graphics.AddFilledRectangle (Rectangle.Deflate (rectangle, 1, 1));
			graphics.RenderSolid (Color.FromBrightness (1.0));
		}
		
		private static void PaintTopmostSheet(Graphics graphics, Rectangle rectangle)
		{
			graphics.AddFilledRectangle (rectangle);
			graphics.GradientRenderer.Fill = GradientFill.Y;
			graphics.GradientRenderer.SetColors (Color.FromBrightness (1.0), Color.FromRgb (0.9, 0.9, 0.95));
			graphics.GradientRenderer.SetParameters (0, 100);
			graphics.GradientRenderer.Transform = Transform.Identity.Scale (rectangle.Width/100, rectangle.Height/100).Translate (rectangle.Height/2, rectangle.Width/2).RotateDeg (10, rectangle.Center);
			graphics.RenderGradient ();

			graphics.AddRectangle (Rectangle.Deflate (rectangle, 0.5, 0.5));
			graphics.RenderSolid (Color.FromBrightness (0.8));
		}

		private static void PaintNote(Graphics graphics, Rectangle rectangle, string text)
		{
			var label = new Rectangle (6, rectangle.Top - 18 - 6, 48, 18);

			var transform = graphics.Transform;
			graphics.RotateTransformDeg (5, label.Center.X, label.Center.Y);

			MiniChartView.PaintShadow (graphics, label);
			graphics.AddFilledRectangle (label);

			graphics.GradientRenderer.Fill = GradientFill.Y;
			graphics.GradientRenderer.SetColors (Color.FromRgb (250/255.0, 255/255.0, 124/255.0), Color.FromRgb (255/255.0, 254/255.0, 173/255.0));
			graphics.GradientRenderer.SetParameters (0, 100);
			graphics.GradientRenderer.Transform = Transform.Identity.Scale (1.0, label.Height / 100.0).Translate (label.BottomLeft);
			graphics.RenderGradient ();

			graphics.Color = Color.FromBrightness (0.0);
			graphics.PaintText (label.X, label.Y, label.Width, label.Height, text, Font.GetFont ("Futura", "Condensed Medium"), 14.0, ContentAlignment.MiddleCenter);
			graphics.RenderSolid ();

			graphics.Transform = transform;
		}

		private static void PaintShadow(Graphics graphics, Rectangle rect)
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

		private static void PaintPaperClip(Graphics graphics, Rectangle rectangle)
		{
			var image = Epsitec.Common.Support.ImageProvider.Default.GetImage ("manifest:Epsitec.Common.Graph.Images.PaperClip.icon", Support.Resources.DefaultManager);
			graphics.PaintImage (image, new Rectangle (rectangle.X + 10, rectangle.Top - 34, 20, 40));
		}


		private CheckButton checkButton;
		private IconButton iconButton;
		private ButtonVisibility iconButtonVisibility;
	}
}
