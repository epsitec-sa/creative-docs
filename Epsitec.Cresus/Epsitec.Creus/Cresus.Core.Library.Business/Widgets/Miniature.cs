//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Ce widget montre une miniature (ou pas !) d'une image dans la boîte du widget, en respectant les
	/// proportions de l'image. Si l'image utilise la transparence, elle est affichée par dessus le
	/// traditionnel damier blanc et gris, cher aux logiciels graphiques.
	/// </summary>
	public class Miniature : Widget
	{
		public Miniature()
		{
		}

		public Miniature(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		public Image Image
		{
			get
			{
				return this.image;
			}
			set
			{
				this.image = value;
				this.Invalidate ();
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			if (this.image != null)
			{
				double sx = this.Client.Bounds.Width  / this.image.Width;
				double sy = this.Client.Bounds.Height / this.image.Height;
				double scale = System.Math.Min (sx, sy);

				double dx = scale * this.image.Width;
				double dy = scale * this.image.Height;

				double ox = System.Math.Floor ((this.Client.Bounds.Width  - dx) / 2);
				double oy = System.Math.Floor ((this.Client.Bounds.Height - dy) / 2);

				Rectangle rect = new Rectangle (ox, oy, dx, dy);

				this.PaintCheckerboard (graphics, rect, 10);

				graphics.PaintImage (this.image, rect);

				rect.Deflate (0.5);
				graphics.AddRectangle (rect);
				graphics.RenderSolid (Color.FromBrightness (0.0));
			}
		}

		private void PaintCheckerboard(Graphics graphics, Rectangle bounds, double size)
		{
			//	Dessine le damier classique blanc et gris sur lequel les logiciels graphiques affichent
			//	les images avec transparence.
			graphics.AddFilledRectangle (bounds);
			graphics.RenderSolid (Color.FromBrightness (1.0));

			for (double y = 0; y < bounds.Height; y+=size)
			{
				for (double x = 0; x < bounds.Width; x+=size)
				{
					int ix = (int) (x/size);
					int iy = (int) (y/size);

					if ((ix+iy)%2 == 0)
					{
						Rectangle square = new Rectangle (bounds.Left+x, bounds.Bottom+y, size, size);
						square = Rectangle.Intersection (square, bounds);

						graphics.AddFilledRectangle (square);
					}
				}
			}

			graphics.RenderSolid (Color.FromBrightness (0.8));
		}


		private Image image;
	}
}
