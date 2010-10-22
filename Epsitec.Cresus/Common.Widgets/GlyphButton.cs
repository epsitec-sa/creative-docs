//	Copyright © 2004-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe GlyphButton dessine un bouton avec une icône simple.
	/// </summary>
	public class GlyphButton : Button
	{
		public GlyphButton()
		{
			this.ButtonStyle = ButtonStyle.Icon;
			this.AutoFocus = false;
			this.InternalState &= ~WidgetInternalState.Focusable;
			this.glyphSize = Size.Zero;
		}

		public GlyphButton(string command)
			: this (command, null)
		{
		}

		public GlyphButton(string command, string name)
			: this ()
		{
			this.CommandObject = Command.Get (command);
			this.Name = name;
		}

		public GlyphButton(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}

		public GlyphButton(Widget embedder, GlyphShape shape)
			: this (embedder)
		{
			this.GlyphShape  = shape;
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


		static GlyphButton()
		{
			Types.DependencyPropertyMetadata metadataDx = Visual.PreferredWidthProperty.DefaultMetadata.Clone ();
			Types.DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			metadataDx.DefineDefaultValue (17.0);
			metadataDy.DefineDefaultValue (17.0);

			Visual.PreferredWidthProperty.OverrideMetadata (typeof (GlyphButton), metadataDx);
			Visual.PreferredHeightProperty.OverrideMetadata (typeof (GlyphButton), metadataDy);
		}


		public override Margins GetShapeMargins()
		{
			return Widgets.Adorners.Factory.Active.GeometryToolShapeMargins;
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			Rectangle glyphBounds = this.GetGlyphBounds ();

			WidgetPaintState paintState = this.GetPaintState ();

			if (this.ButtonStyle != ButtonStyle.None)
			{
				Direction dir = this.GetGlyphDirection ();

				if (this.glyphSize.IsEmpty)
				{
					adorner.PaintButtonBackground (graphics, this.Client.Bounds, paintState, dir, this.ButtonStyle);
				}
				else
				{
					WidgetPaintState state = paintState;

					if ((state&WidgetPaintState.Entered) != 0)  // bouton survolé ?
					{
						state |=  WidgetPaintState.InheritedEnter;  // mode spécial pour le groupe d'un combo
					}

					adorner.PaintButtonBackground (graphics, this.Client.Bounds, state, dir, this.ButtonStyle);
					adorner.PaintButtonBackground (graphics, glyphBounds, paintState, dir, this.ButtonStyle);
				}
			}

			adorner.PaintGlyph (graphics, glyphBounds, paintState, this.shape, PaintTextStyle.Button);
		}

		private Direction GetGlyphDirection()
		{
			Direction dir = Direction.None;

			switch (this.shape)
			{
				case GlyphShape.ArrowUp:
					dir = Direction.Up;
					break;
				case GlyphShape.ArrowDown:
					dir = Direction.Down;
					break;
				case GlyphShape.ArrowLeft:
					dir = Direction.Left;
					break;
				case GlyphShape.ArrowRight:
					dir = Direction.Right;
					break;
			}

			return dir;
		}

		protected Rectangle GetGlyphBounds()
		{
			//	Retourne le rectangle à utiliser pour le glyph, en tenant compte de GlyphSize et ContentAlignment.
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


		private GlyphShape						shape;
		private Size							glyphSize;
	}
}
