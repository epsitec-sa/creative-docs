//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	public abstract class AbstractTile : TileContainer
	{
		public AbstractTile()
		{
			this.iconButton = new IconButton (this);
			this.iconButton.ButtonStyle = ButtonStyle.ToolItem;
			this.iconButton.AutoFocus = false;

			this.titleLayout = new TextLayout ();
			this.titleLayout.BreakMode = Common.Drawing.TextBreakMode.Ellipsis;
			this.titleLayout.Alignment = Common.Drawing.ContentAlignment.TopLeft;
			this.titleLayout.DefaultFontSize = AbstractTile.titleHeight*0.8;
		}

		public AbstractTile(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		static AbstractTile()
		{
			DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			metadataDy.DefineDefaultValue (AbstractTile.iconSize+AbstractTile.iconMargins*2);

			Common.Widgets.Visual.PreferredHeightProperty.OverrideMetadata (typeof (AbstractTile), metadataDy);
		}


		virtual public double ContentHeight
		{
			get
			{
				return this.PreferredHeight;
			}
		}

		/// <summary>
		/// Icône visible en haut à gauche de la tuile.
		/// </summary>
		/// <value>The icon URI.</value>
		public string IconUri
		{
			get
			{
				return this.icon;
			}
			set
			{
				this.icon = value;

				if (string.IsNullOrEmpty (this.icon) || this.icon.Length == 1)
				{
					this.iconButton.Visibility = false;
				}
				else
				{
					this.iconButton.IconUri = Misc.GetResourceIconUri (value);
					this.iconButton.Visibility = true;
				}
			}
		}

		/// <summary>
		/// Titre affiché en haut de la tuile.
		/// </summary>
		/// <value>The title.</value>
		public string Title
		{
			get
			{
				return this.titleLayout.Text;
			}
			set
			{
				if (this.titleLayout.Text != value)
				{
					this.titleLayout.Text = value;
					this.Invalidate ();
				}
			}
		}

		public object Data
		{
			get;
			set;
		}


		protected override void SetBoundsOverride(Rectangle oldRect, Rectangle newRect)
		{
			base.SetBoundsOverride (oldRect, newRect);
			this.UpdateGeometry ();
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			base.PaintBackgroundImplementation (graphics, clipRect);

			Rectangle iconRectangle  = this.IconRectangle;
			Rectangle titleRectangle = this.TitleRectangle;

			if (string.IsNullOrEmpty (this.icon) || this.icon.Length == 1)
			{
				Rectangle r = iconRectangle;
				r.Deflate (AbstractTile.iconMargins);

				//?graphics.AddRectangle (r);
				//?graphics.RenderSolid (adorner.ColorBorder);

				if (!string.IsNullOrEmpty (this.icon))
				{
					graphics.PaintText (r.Left, r.Bottom, r.Width, r.Height, this.icon, Font.DefaultFont, r.Height*0.8, Common.Drawing.ContentAlignment.MiddleCenter);
				}
			}

			this.titleLayout.LayoutSize = titleRectangle.Size;
			this.titleLayout.Paint (titleRectangle.BottomLeft, graphics, clipRect, adorner.ColorText (WidgetPaintState.Enabled), GlyphPaintStyle.Normal);
		}


		private void UpdateGeometry()
		{
			if (this.iconButton == null)
			{
				return;
			}

			Rectangle iconRectangle  = this.IconRectangle;
			iconRectangle.Deflate (AbstractTile.iconMargins-0.5);

			this.iconButton.SetManualBounds (iconRectangle);
		}


		protected Rectangle IconRectangle
		{
			get
			{
				Rectangle bounds = this.ContentBounds;
				double size = AbstractTile.iconSize+AbstractTile.iconMargins*2;
				return new Rectangle (bounds.Left, bounds.Top-size, size, size);
			}
		}

		protected Rectangle TitleRectangle
		{
			get
			{
				Rectangle bounds = this.ContentBounds;
				Rectangle iconRectangle = this.IconRectangle;
				return new Rectangle (iconRectangle.Right, bounds.Top-AbstractTile.titleHeight, bounds.Width-iconRectangle.Right, AbstractTile.titleHeight);
			}
		}

		protected Rectangle MainRectangle
		{
			get
			{
				Rectangle bounds = this.ContentBounds;
				Rectangle titleRectangle = this.TitleRectangle;
				return new Rectangle (titleRectangle.Left, bounds.Bottom, titleRectangle.Width, bounds.Height-AbstractTile.titleHeight);
			}
		}


		private static readonly double iconSize = 32;
		private static readonly double iconMargins = 5;
		private static readonly double titleHeight = 20;

		private string icon;
		private IconButton iconButton;
		private TextLayout titleLayout;
	}
}
