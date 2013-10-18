﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class ColoredButton : AbstractButton
	{
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

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (this.BackColor);

			graphics.AddText (rect.Left, rect.Bottom, rect.Width, rect.Height, this.Text, Font.DefaultFont, Font.DefaultFontSize, this.ContentAlignment);
			graphics.RenderSolid (Color.FromBrightness (0));
		}


		private Color GetColor(bool hover)
		{
			if (hover)
			{
				return this.HoverColor;
			}
			else
			{
				if (this.ActiveState == ActiveState.Yes)
				{
					return this.SelectedColor;
				}
				else
				{
					return this.NormalColor;
				}
			}
		}


		private Color							normalColor;
		private Color							selectedColor;
		private Color							hoverColor;
	}
}
