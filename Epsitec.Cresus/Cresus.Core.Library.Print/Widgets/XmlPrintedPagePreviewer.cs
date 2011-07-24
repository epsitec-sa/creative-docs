//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Printing;

using Epsitec.Cresus.Core.Print;
using Epsitec.Cresus.Core.Print.Serialization;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Ce widget montre le contenu d'une page imprimée désérialisée dans une zone rectangulaire.
	/// </summary>
	public class XmlPrintedPagePreviewer : Widget
	{
		public XmlPrintedPagePreviewer(IBusinessContext businessContext, bool showCheckButtons)
		{
			this.businessContext = businessContext;
			this.coreData = this.businessContext.Data;

			this.titleLayout = new TextLayout
			{
				DefaultFontSize = 11,
				Alignment = ContentAlignment.MiddleCenter,
				BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
			};

			if (showCheckButtons)
			{
				this.checkButton = new CheckButton
				{
					Parent = this,
					AutoToggle = false,
					Anchor = AnchorStyles.BottomLeft,
					Margins = new Margins (0, 0, 0, XmlPrintedPagePreviewer.titleHeight-1),
				};

				this.checkButton.Clicked += delegate
				{
					this.page.IsPrintable = !this.page.IsPrintable;
					this.checkButton.ActiveState = this.page.IsPrintable ? ActiveState.Yes : ActiveState.No;
				};

				ToolTip.Default.SetToolTip (this.checkButton, "Une coche indique que cette page sera imprimée");
			}
		}


		public DeserializedPage Page
		{
			//	Page affichée.
			get
			{
				return this.page;
			}
			set
			{
				if (this.page != value)
				{
					this.page = value;

					if (this.checkButton != null)
					{
						this.checkButton.ActiveState = this.page.IsPrintable ? ActiveState.Yes : ActiveState.No;
					}

					this.UpdateTitle ();
					this.bitmap = null;
					this.Invalidate ();
				}
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			this.UpdateBitmap ();

			double dx, dy;

			if (this.bitmap == null)  // erreur ?
			{
				double zoom, pageWidth, pageHeight;
				this.ComputeGeometry (out zoom, out pageWidth, out pageHeight);

				dx = System.Math.Floor (zoom*pageWidth);
				dy = System.Math.Floor (zoom*pageHeight);
			}
			else  // ok ?
			{
				dx = this.bitmap.Width;
				dy = this.bitmap.Height;
			}

			var rectTitle   = new Rectangle (0, 0, dx, XmlPrintedPagePreviewer.titleHeight+3);  // léger chevauchement avec rectPreview
			var rectPreview = new Rectangle (0, XmlPrintedPagePreviewer.titleHeight, dx, dy);

			//	Affiche le texte en bas.
			rectTitle.Deflate (7.5, 0.5);

			var path = new Path ();
			path.AppendRoundedRectangle (rectTitle.BottomLeft, new Size (rectTitle.Width, rectTitle.Height*2), rectTitle.Height*0.6);
			graphics.Rasterizer.AddSurface (path);
			var shadow = new Rectangle (rectTitle.Left, rectTitle.Top-8, rectTitle.Width, 8);
			graphics.DrawVerticalGradient (shadow, Color.FromBrightness (0.6), Color.FromBrightness (0.1));
			graphics.Rasterizer.AddOutline (path);
			graphics.RenderSolid (Color.FromBrightness (0));

			rectTitle.Deflate (8, 0);
			this.titleLayout.LayoutSize = rectTitle.Size;
			this.titleLayout.Paint (rectTitle.BottomLeft, graphics, Rectangle.MaxValue, Color.FromBrightness (1), GlyphPaintStyle.Normal);

			if (this.bitmap == null)
			{
				//	Affiche l'erreur.
				this.PaintError (graphics, rectPreview);
			}
			else
			{
				//	Affiche le bitmap.
				rectPreview.Deflate (1);
				graphics.PaintImage (this.bitmap, rectPreview);

				rectPreview.Inflate (0.5);
				graphics.AddRectangle (rectPreview);
				graphics.RenderSolid (Color.FromBrightness (0));
			}
		}

		private void UpdateTitle()
		{
			this.titleLayout.Text = this.page.ShortDescription;

			ToolTip.Default.SetToolTip (this, this.page.FullDescription);
		}

		private void UpdateBitmap()
		{
			double zoom, pageWidth, pageHeight;
			this.ComputeGeometry (out zoom, out pageWidth, out pageHeight);

			if (this.bitmap == null || this.lastZoom != zoom)
			{
				this.lastZoom = zoom;

				if (this.page.IsOK)
				{
					var port = new XmlPort (page.XRoot);
					this.bitmap = port.Deserialize (id => PrintEngine.GetImage (this.businessContext, id), new Size (pageWidth, pageHeight), zoom);
				}
			}
		}

		private void PaintError(Graphics graphics, Rectangle rectPreview)
		{
			//	Efface le fond.
			rectPreview.Deflate (1);
			graphics.AddFilledRectangle (rectPreview);
			graphics.RenderSolid (Color.FromBrightness (1));

			//	Dessine le texte.
			var textRect = rectPreview;
			textRect.Deflate (textRect.Width*0.1);

			var textLayout = new TextLayout ();
			textLayout.LayoutSize = textRect.Size;
			textLayout.Alignment = ContentAlignment.MiddleLeft;
			textLayout.Text = page.Error.ToString ();
			textLayout.Paint (textRect.BottomLeft, graphics);

			//	Dessine le cadre.
			rectPreview.Inflate (0.5);
			graphics.AddRectangle (rectPreview);
			graphics.RenderSolid (Color.FromBrightness (0));
		}

		private void ComputeGeometry(out double zoom, out double pageWidth, out double pageHeight)
		{
			//	Calcule la géométrie de la page.
			double widgetWidth  = this.Client.Bounds.Width;
			double widgetHeight = this.Client.Bounds.Height - XmlPrintedPagePreviewer.titleHeight;

			pageWidth  = this.page.ParentSectionPageSize.Width;
			pageHeight = this.page.ParentSectionPageSize.Height;

			double zoomX = widgetWidth  / pageWidth;
			double zoomY = widgetHeight / pageHeight;

			zoom = System.Math.Min (zoomX, zoomY);
		}


		public static readonly double		titleHeight = 18;

		private readonly IBusinessContext	businessContext;
		private readonly CoreData			coreData;
		private readonly CheckButton		checkButton;

		private DeserializedPage			page;
		private Bitmap						bitmap;
		private double						lastZoom;
		private TextLayout					titleLayout;
	}
}
