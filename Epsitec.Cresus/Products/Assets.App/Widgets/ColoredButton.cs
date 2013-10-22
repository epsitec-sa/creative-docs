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
		}


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
			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (this.BackColor);

			//	Dessine le texte.
			this.textLayout.Text            = this.Text;
			this.textLayout.DefaultFont     = Font.DefaultFont;
			this.textLayout.DefaultFontSize = Font.DefaultFontSize;
			this.textLayout.LayoutSize      = rect.Size;
			this.textLayout.BreakMode       = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
			this.textLayout.Alignment       = this.ContentAlignment;

			this.textLayout.Paint (rect.BottomLeft, graphics, rect, Color.FromBrightness (0), GlyphPaintStyle.Normal);
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


		private readonly TextLayout				textLayout;

		private Color							normalColor;
		private Color							selectedColor;
		private Color							hoverColor;
	}
}
