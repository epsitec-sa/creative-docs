//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.Core.Helpers;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// IconButton éventuellement surchargé d'une partille rouge indiquant un
	/// nombre d'avertissements.
	/// </summary>
	public class ButtonWithRedDot : IconButton
	{
		public ButtonWithRedDot()
		{
			this.font = Font.GetFont (Font.DefaultFontFamily, "Bold");
		}


		public int								RedDotCount
		{
			get
			{
				return this.redDotCount;
			}
			set
			{
				if (this.redDotCount != value)
				{
					this.redDotCount = value;
					this.Invalidate ();
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
					this.Invalidate ();
				}
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			if (!this.HoverColor.IsEmpty && this.Enable)
			{
				//	Affiche le fond coloré lorsque le bouton est lié à un autre
				//	avec MainToolbar.AttachHover().
				var rect = new Rectangle (0, 0, this.ActualWidth, this.ActualHeight);
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (this.HoverColor);
			}

			base.PaintBackgroundImplementation (graphics, clipRect);

			if (this.redDotCount > 0)
			{
				var rect = new Rectangle (0, 0, this.ActualWidth, this.ActualHeight);
				this.DrawRedDotCount (graphics, rect, this.redDotCount);
			}
		}

		private void DrawRedDotCount(Graphics graphics, Rectangle rect, int count)
		{
			//	Dessine la pastille rouge avec le nombre d'avertissements.
			var radius = System.Math.Min (rect.Width*0.2, rect.Height*0.2);

			//	Rectangle en bas à droite.
			rect = new Rectangle (rect.Right-radius*2, rect.Bottom, radius*2, radius*2);

			//	Dessine la pastille.
			graphics.AddFilledCircle (rect.Center, radius);
			graphics.RenderSolid (ColorManager.RedDotBackgroundColor);

			//	Dessine le chiffre dans la pastille.
			var text = TypeConverters.IntToString (count);

			if (!string.IsNullOrEmpty (text))
			{
				//	Léger décalage vers le haut.
				rect.Offset (0, radius*0.1);

				if (text[0] == '1')
				{
					//	Léger décalage vers la gauche, si le nombre commence par '1'.
					rect.Offset (-radius*0.05, 0);
				}

				double fontSize;
				switch (text.Length)
				{
					//	Plus le nombre de digits est grand et plus la police doit être petite.
					case 1:
						fontSize = 1.6;
						break;

					case 2:
						fontSize = 1.2;
						break;

					case 3:
						fontSize = 1.0;
						break;

					case 4:
						fontSize = 0.7;
						break;

					default:
						fontSize = 1.0;
						break;
				}

				graphics.Color = ColorManager.RedDotTextColor;
				graphics.PaintText (rect, text, this.font, radius*fontSize, ContentAlignment.MiddleCenter);
			}
		}


		private readonly Font					font;
		private int								redDotCount;
		private Color							hoverColor;
	}
}
