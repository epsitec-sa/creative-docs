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
	public class StaticGlyph : Widget
	{
		public StaticGlyph()
		{
			this.glyphSize = Size.Zero;
		}

		public StaticGlyph(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		public GlyphShape GlyphShape
		{
			//	Forme représentée dans le bouton.
			get
			{
				return this.shape;
			}
			set
			{
				if (this.shape != value)
				{
					this.shape = value;
					this.Invalidate ();
				}
			}
		}

		public Size GlyphSize
		{
			//	Taille de la forme dans le bouton. Utile lorsque la forme est plus petite que le bouton,
			//	et qu'elle est éventuellement décentrée avec ContentAlignment.
			get
			{
				return this.glyphSize;
			}
			set
			{
				if (this.glyphSize != value)
				{
					this.glyphSize = value;
					this.Invalidate ();
				}
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Rectangle glyphBounds = this.GlyphBounds;
			//?glyphBounds.Inflate (System.Math.Min (this.Client.Bounds.Width, this.Client.Bounds.Height)*0.5);

			WidgetPaintState paintState = this.GetPaintState ();
			adorner.PaintGlyph (graphics, glyphBounds, paintState, this.shape, PaintTextStyle.Button);
		}

		protected Rectangle GlyphBounds
		{
			//	Retourne le rectangle à utiliser pour le glyph, en tenant compte de GlyphSize et ContentAlignment.
			get
			{
				Rectangle rect = this.Client.Bounds;

				if (!this.glyphSize.IsEmpty)
				{
					if (this.ContentAlignment == ContentAlignment.MiddleLeft ||
						this.ContentAlignment == ContentAlignment.BottomLeft ||
						this.ContentAlignment == ContentAlignment.TopLeft)
					{
						rect.Width = this.glyphSize.Width;
					}

					if (this.ContentAlignment == ContentAlignment.MiddleRight ||
						this.ContentAlignment == ContentAlignment.BottomRight ||
						this.ContentAlignment == ContentAlignment.TopRight)
					{
						rect.Left = rect.Right-this.glyphSize.Width;
					}

					if (this.ContentAlignment == ContentAlignment.BottomCenter ||
						this.ContentAlignment == ContentAlignment.BottomLeft   ||
						this.ContentAlignment == ContentAlignment.BottomRight)
					{
						rect.Height = this.glyphSize.Height;
					}

					if (this.ContentAlignment == ContentAlignment.TopCenter ||
						this.ContentAlignment == ContentAlignment.TopLeft   ||
						this.ContentAlignment == ContentAlignment.TopRight)
					{
						rect.Bottom = rect.Top-this.glyphSize.Height;
					}
				}

				return rect;
			}
		}

		private GlyphShape						shape;
		private Size							glyphSize;
	}
}
