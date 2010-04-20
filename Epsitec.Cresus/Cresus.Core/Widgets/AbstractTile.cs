using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Widgets
{
	public abstract class AbstractTile : SubFrameBox
	{
		public AbstractTile()
		{
		}

		public AbstractTile(Widget embedder)
			: this()
		{
			this.SetEmbedder(embedder);
		}


		static AbstractTile()
		{
			DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			metadataDy.DefineDefaultValue(AbstractTile.iconSize+AbstractTile.iconMargins*2);

			Common.Widgets.Visual.PreferredHeightProperty.OverrideMetadata(typeof(AbstractTile), metadataDy);
		}


		/// <summary>
		/// Icône visible en haut à gauche de la tuile.
		/// </summary>
		/// <value>The icon.</value>
		public Image Icon
		{
			get
			{
				return this.icon;
			}
			set
			{
				this.icon = value;
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
				return this.title;
			}
			set
			{
				if (this.title != value)
				{
					this.title = value;
					this.Invalidate();
				}
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Rectangle bounds = this.Client.Bounds;

			base.PaintBackgroundImplementation(graphics, clipRect);

			Rectangle iconRectangle  = this.IconRectangle;
			Rectangle titleRectangle = this.TitleRectangle;
			Rectangle mainRectangle  = this.MainRectangle;

			if (this.icon == null)
			{
				Rectangle r = iconRectangle;
				r.Deflate(AbstractTile.iconMargins+0.5);

				graphics.AddRectangle(r);
				graphics.AddLine(r.TopLeft, r.BottomRight);
				graphics.AddLine(r.BottomLeft, r.TopRight);
				graphics.RenderSolid(adorner.ColorBorder);
			}
			else
			{
				Rectangle r = iconRectangle;
				r.Deflate(AbstractTile.iconMargins);

				graphics.PaintImage(this.icon, iconRectangle);
			}

			graphics.PaintText(titleRectangle.Left, titleRectangle.Bottom, titleRectangle.Width, titleRectangle.Height, this.title, Font.DefaultFont, titleRectangle.Height*0.8, Common.Drawing.ContentAlignment.MiddleLeft);

#if false
			graphics.AddLine(iconRectangle.Right, bounds.Bottom, iconRectangle.Right, bounds.Height);
			graphics.AddLine(titleRectangle.BottomLeft, titleRectangle.BottomRight);
			graphics.RenderSolid(adorner.ColorBorder);
#endif
		}


		private Rectangle IconRectangle
		{
			get
			{
				Rectangle bounds = this.Client.Bounds;
				double size = AbstractTile.iconSize+AbstractTile.iconMargins*2;
				return new Rectangle(bounds.Left, bounds.Top-size, size, size);
			}
		}

		private Rectangle TitleRectangle
		{
			get
			{
				Rectangle bounds = this.Client.Bounds;
				Rectangle iconRectangle = this.IconRectangle;
				return new Rectangle(iconRectangle.Right, bounds.Top-AbstractTile.titleHeight, bounds.Width-iconRectangle.Right, AbstractTile.titleHeight);
			}
		}

		private Rectangle MainRectangle
		{
			get
			{
				Rectangle bounds = this.Client.Bounds;
				Rectangle titleRectangle = this.TitleRectangle;
				return new Rectangle(titleRectangle.Left, bounds.Bottom, titleRectangle.Width, bounds.Height-AbstractTile.titleHeight);
			}
		}


		private static readonly double iconSize = 32;
		private static readonly double iconMargins = 5;
		private static readonly double titleHeight = 20;

		private Image icon;
		private string title;
	}
}
