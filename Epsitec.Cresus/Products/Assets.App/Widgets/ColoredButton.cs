//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class ColoredButton : AbstractButton
	{
		public ColoredButton()
		{
			this.textLayout = new TextLayout ();

			this.NormalColor = ColorManager.ToolbarBackgroundColor;
			this.HoverColor  = ColorManager.HoverColor;
		}


		public bool								SameColorWhenDisable;

		public Color							NormalColor
		{
			get
			{
				return this.normalColor;
			}
			set
			{
				if (this.normalColor != value)
				{
					this.normalColor = value;
					this.BackColor = this.GetColor (false);
				}
			}
		}

		public Color							SelectedColor
		{
			get
			{
				return this.selectedColor;
			}
			set
			{
				if (this.selectedColor != value)
				{
					this.selectedColor = value;
					this.BackColor = this.GetColor (false);
				}
			}
		}

		public Color							HoverColor
		{
			get
			{
				return this.hoverColor;
			}
			set
			{
				if (this.hoverColor != value)
				{
					this.hoverColor = value;
					this.BackColor = this.GetColor (false);
				}
			}
		}

		public bool								Hatch
		{
			get
			{
				return this.hatch;
			}
			set
			{
				if (this.hatch != value)
				{
					this.hatch = value;
					this.Invalidate ();
				}
			}
		}

		public int								HorizontalMargins = ColoredButton.horizontalMargins;


		protected override void OnActiveStateChanged()
		{
			this.BackColor = this.GetColor (false);
			base.OnActiveStateChanged ();
		}

		protected override void OnEntered(MessageEventArgs e)
		{
			this.BackColor = this.GetColor (true);
			base.OnEntered (e);
		}

		protected override void OnExited(MessageEventArgs e)
		{
			this.BackColor = this.GetColor (false);
			base.OnExited (e);
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var rect = new Rectangle (0, 0, this.ActualWidth, this.ActualHeight);

			//	Dessine le fond coloré.
			if (this.hatch)
			{
				var reference = this.MapParentToClient (rect.BottomLeft);
				PaintHatch.Paint (graphics, rect, reference, 0.2);
			}
			else
			{
				var color = this.BackColor;

				if (!this.Enable && !this.SameColorWhenDisable)
				{
					color = color.Delta (0.1);  // plus clair si disable
				}

				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (color);
			}

			if ((this.GetPaintState () & WidgetPaintState.Focused) != 0)
			{
				ColoredButton.DrawFocusedRectangle (graphics, rect);
			}

			//	Dessine le texte.
			rect.Deflate (this.HorizontalMargins, 0);  // en cas d'alignement à gauche, comme un TextField !

			this.textLayout.Text            = this.Text;
			this.textLayout.DefaultFont     = Font.DefaultFont;
			this.textLayout.DefaultFontSize = Font.DefaultFontSize;
			this.textLayout.LayoutSize      = rect.Size;
			this.textLayout.BreakMode       = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
			this.textLayout.Alignment       = this.ContentAlignment;

			double brightness = this.Enable ? 0.0 : 0.7;  // gris clair si disable
			this.textLayout.Paint (rect.BottomLeft, graphics, rect, Color.FromBrightness (brightness), GlyphPaintStyle.Normal);
		}

		private static void DrawFocusedRectangle(Graphics graphics, Rectangle rect)
		{
			//	Dessine un rectangle pointillé pour indiquer qu'on a le focus.
			rect.Deflate (2.5);
			using (var path = new Path (rect))
			{
				graphics.PaintDashedOutline (path, 1, 0, 2, CapStyle.Square, Color.FromBrightness (0.4));
			}
		}


		private Color GetColor(bool hover)
		{
			if (hover && this.hoverColor.IsValid)
			{
				return this.hoverColor;
			}
			else
			{
				if (this.ActiveState == ActiveState.Yes)
				{
					return this.selectedColor;
				}
				else
				{
					return this.normalColor;
				}
			}
		}


		public const int horizontalMargins = 3;

		private readonly TextLayout				textLayout;

		private Color							normalColor;
		private Color							selectedColor;
		private Color							hoverColor;
		private bool							hatch;
	}
}
