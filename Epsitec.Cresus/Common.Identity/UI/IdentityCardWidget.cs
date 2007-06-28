//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Identity;
using Epsitec.Common.Identity.UI;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (IdentityCardWidget))]

namespace Epsitec.Common.Identity.UI
{
	public class IdentityCardWidget : AbstractButton
	{
		public IdentityCardWidget()
		{
			this.AutoEngage = true;
		}

		public IdentityCardWidget(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		public IdentityCard IdentityCard
		{
			get
			{
				return this.GetValue (IdentityCardWidget.IdentityCardProperty) as IdentityCard;
			}
			set
			{
				if (value == null)
				{
					this.ClearValue (IdentityCardWidget.IdentityCardProperty);
				}
				else
				{
					this.SetValue (IdentityCardWidget.IdentityCardProperty, value);
				}
			}
		}

		protected override void PaintBackgroundImplementation(Epsitec.Common.Drawing.Graphics graphics, Epsitec.Common.Drawing.Rectangle clipRect)
		{
			Rectangle rect   = Rectangle.Deflate (this.Client.Bounds, 2, 2);
			double    size   = System.Math.Min (rect.Width, rect.Height);
			double    radius = size / 20;

			double baseColorH, baseColorS, baseColorV;
			Color baseColor = this.GetCardColor ();
			baseColor.GetHsv (out baseColorH, out baseColorS, out baseColorV);

			using (Path round = new Path ())
			{
				WidgetPaintState paintState = this.PaintState;

				round.AppendRoundedRectangle (rect, radius);
				graphics.Rasterizer.AddSurface (round);
				graphics.RenderSolid (Color.FromHsv (baseColorH, baseColorS * ((paintState & WidgetPaintState.Entered) == 0 ? 0.33 : 0.75), 1.0));

				double lineWidth = ((paintState & WidgetPaintState.Engaged) == 0) ? 1.0 : 2.0;
				
				if (size > 8)
				{
					Graphics mask = graphics.CreateAlphaMask ();
					
					mask.Rasterizer.AddSurface (round);
					mask.RenderSolid (Color.FromRgb (1, 0, 0));

					Image     image     = this.IdentityCard == null ? null : this.IdentityCard.GetImage ();
					Rectangle imageRect = new Rectangle (2, 2, size, size);

					if ((paintState & WidgetPaintState.Engaged) != 0)
					{
						double cx = imageRect.Center.X;
						double cy = imageRect.Center.Y;
						double dx = imageRect.Width * 1.05;
						double dy = imageRect.Height * 1.05;

						imageRect = new Rectangle (cx - dx/2, cy - dy/2, dx, dy);
					}
					
					if (image != null)
					{
						graphics.ImageRenderer.SetAlphaMask (mask.Pixmap, MaskComponent.R);
						graphics.PaintImage (image, imageRect);
						graphics.ImageRenderer.SetAlphaMask (null, MaskComponent.None);
					}
					else
					{
						graphics.SolidRenderer.SetAlphaMask (mask.Pixmap, MaskComponent.R);
						graphics.AddFilledRectangle (imageRect);
						graphics.RenderSolid (Color.FromHsv (0, 0, baseColorV * 0.8));
						graphics.SolidRenderer.SetAlphaMask (null, MaskComponent.None);
					}

					mask.Dispose ();

					double textWidth = rect.Width - size - 4;
					double textSize  = System.Math.Min (textWidth / 10, size / 4);
					double x = rect.Right - textWidth;
					double y = rect.Bottom;

					if ((textWidth > 20) &&
						(this.IdentityCard != null))
					{
						string textLine1 = this.IdentityCard.UserName;
						string textLine2 = this.IdentityCard.DeveloperId == -1 ? "" : string.Format ("Dev #{0}", this.IdentityCard.DeveloperId);

						graphics.Color = Color.FromName ("Black");
						graphics.PaintText (x, y + textSize/2, textWidth, size - textSize, textLine1, Font.DefaultFont, textSize * 1.0, ContentAlignment.TopLeft);
						graphics.PaintText (x, y - textSize/2, textWidth, size - textSize, textLine2, Font.DefaultFont, textSize * 0.8, ContentAlignment.TopLeft);
					}
				}

				graphics.Rasterizer.AddOutline (round, lineWidth, CapStyle.Butt, JoinStyle.Round);
				graphics.RenderSolid (Color.FromHsv (baseColorH, 1.0, baseColorV * 0.8));
			}
		}

		private Color GetCardColor()
		{
			//	This is currently constant; in the future, we will map the card's
			//	owner role to a color here :

			return Color.FromRgb (1, 1, 0.5);
		}

		public static readonly DependencyProperty IdentityCardProperty = DependencyProperty.Register ("IdentityCard", typeof (IdentityCard), typeof (IdentityCard), new DependencyPropertyMetadata ());
	}
}
