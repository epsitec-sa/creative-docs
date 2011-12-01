//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Bouton pour une action, qui vient par-dessus les tuiles.
	/// </summary>
	public sealed class ActionButton : Button
	{
		public ActionButton()
		{
			this.textLayout = new TextLayout ();
			this.alpha      = 1.0;
			this.colored    = true;
		}


		public ActionClasses ActionClasses
		{
			get;
			set;
		}

		public Color ActionBackgroudColor
		{
			get;
			set;
		}

		public bool IsIcon
		{
			get;
			set;
		}

		public double Alpha
		{
			get
			{
				return this.alpha;
			}
			set
			{
				if (this.alpha != value)
				{
					this.alpha = value;
					this.Invalidate ();
				}
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine le bouton.
			if (this.alpha == 0.0)
			{
				return;
			}

			var adorner = Common.Widgets.Adorners.Factory.Active;

			var rect  = this.Client.Bounds;
			var state = Widget.ConstrainPaintState (this.GetPaintState ());
			var pos   = this.GetTextLayoutOffset ();

			//	Dessine le fond et le cadre du bouton.
			rect.Deflate (0.5);

			Color color;

			if ((state & WidgetPaintState.Entered) == 0)
			{
				color = this.BackgroundColor;
			}
			else
			{
				color = Tiles.TileColors.SurfaceHilitedSelectedColors.FirstOrDefault ();
			}

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (color);

			graphics.AddRectangle (rect);
			graphics.RenderSolid (Tiles.TileColors.BorderColors.FirstOrDefault ());

			//	Dessine le contenu du bouton.
			if (this.IsIcon)  // icône ?
			{
				var box = this.Client.Bounds;
				box.Inflate (1);

				this.textLayout.FormattedText = this.FormattedText;
				this.textLayout.Alignment = ContentAlignment.MiddleCenter;
				this.textLayout.LayoutSize = new Size (box.Width, box.Height);
				this.textLayout.Paint (new Point (box.Left, box.Bottom), graphics);
			}
			else  // texte ?
			{
				pos.Y += this.GetBaseLineVerticalOffset ();

				this.TextLayout.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;

				if (this.innerZoom != 1.0)
				{
					Transform transform = graphics.Transform;
					graphics.ScaleTransform (this.innerZoom, this.innerZoom, this.Client.Size.Width / 2, this.Client.Size.Height / 2);
					adorner.PaintButtonTextLayout (graphics, pos, this.TextLayout, state, this.ButtonStyle);
					graphics.Transform = transform;
				}
				else
				{
					adorner.PaintButtonTextLayout (graphics, pos, this.TextLayout, state, this.ButtonStyle);
				}
			}

			if (this.alpha < 1.0)
			{
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (Color.FromAlphaRgb (1.0-this.alpha, 1.0, 1.0, 1.0));  // dessine un voile blanc par dessus
			}
		}

		private Color BackgroundColor
		{
			get
			{
				if (this.colored)
				{
					return this.ActionBackgroudColor;
				}
				else
				{
					return Tiles.TileColors.SurfaceDefaultColors.FirstOrDefault ();
				}
			}
		}


		private readonly TextLayout		textLayout;
		private double					alpha;
		private bool					colored;
	}
}
