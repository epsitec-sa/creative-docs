//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			Rectangle rect   = Rectangle.Deflate (this.Client.Bounds, 1.5, 1.5);
			double    size   = System.Math.Min (rect.Width, rect.Height);
			double    radius = size / 12;

			double baseColorH, baseColorS, baseColorV;
			Color baseColor = this.GetCardColor ();
			baseColor.GetHsv (out baseColorH, out baseColorS, out baseColorV);

			using (Path round = new Path ())
			{
				WidgetPaintState paintState = this.PaintState;
				bool isSelected = (paintState & WidgetPaintState.Selected) != 0;

				double factor = 0.33;
				if ((paintState & WidgetPaintState.Entered) != 0)
				{
					factor = 0.75;
				}
				if (isSelected)
				{
					factor = 0.50;
				}

				round.AppendRoundedRectangle (rect, radius);
				graphics.Rasterizer.AddSurface (round);
				graphics.RenderSolid (Color.FromHsv (baseColorH, baseColorS * factor, 1.0));

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
						graphics.RenderSolid (Color.FromRgb (1.0, 0.5, 0.0));  // orange
						graphics.Color = Color.FromBrightness (1);
						graphics.PaintText (imageRect.X, imageRect.Y, imageRect.Width, imageRect.Height, "?", Font.GetFont ("Tahoma", "Bold"), imageRect.Height, ContentAlignment.MiddleCenter);
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

				if (isSelected)
				{
					using (Path inside = new Path())
					{
						rect.Deflate(1);
						inside.AppendRoundedRectangle (rect, radius-1);
						graphics.Rasterizer.AddOutline (inside, 3.0, CapStyle.Butt, JoinStyle.Round);
						graphics.RenderSolid (Color.FromHsv (baseColorH, 1.0, baseColorV * 0.9));
					}
				}

				graphics.Rasterizer.AddOutline (round, 1.0, CapStyle.Butt, JoinStyle.Round);
				graphics.RenderSolid (Color.FromHsv (baseColorH, 1.0, baseColorV * (isSelected ? 0.4 : 0.6)));
			}
		}

		private Color GetCardColor()
		{
			//	This is currently constant; in the future, we will map the card's
			//	owner role to a color here :

			return Color.FromRgb (1, 1, 0.5);
		}

		public static readonly DependencyProperty IdentityCardProperty = DependencyProperty.Register ("IdentityCard", typeof (IdentityCard), typeof (IdentityCard), new Epsitec.Common.Widgets.Helpers.VisualPropertyMetadata (Epsitec.Common.Widgets.Helpers.VisualPropertyMetadataOptions.AffectsDisplay));
	}
}
