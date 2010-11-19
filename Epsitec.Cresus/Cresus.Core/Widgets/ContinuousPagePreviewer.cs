//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Ce widget montre le contenu d'une page imprimée dans une zone rectangulaire.
	/// </summary>
	public class ContinuousPagePreviewer : Widget
	{
		public ContinuousPagePreviewer()
		{
			this.zoom = 1;
		}


		public Printers.AbstractDocumentPrinter DocumentPrinter
		{
			get
			{
				return this.documentPrinter;
			}
			set
			{
				this.documentPrinter = value;
			}
		}

		public double Zoom
		{
			get
			{
				return this.zoom;
			}
			set
			{
				if (this.zoom != value)
				{
					this.zoom = value;
					this.UpdateBitmap ();
				}
			}
		}

		public double TotalWidth
		{
			get
			{
				this.UpdateBitmap ();
				return this.bitmap.Width;
			}
		}

		public double TotalHeight
		{
			get
			{
				this.UpdateBitmap ();
				return this.bitmap.Height;
			}
		}

		public double HorizontalOffset
		{
			get
			{
				return this.horizontalOffset;
			}
			set
			{
				if (this.horizontalOffset != value)
				{
					this.horizontalOffset = value;
					this.Invalidate ();
				}
			}
		}

		public double VerticalOffset
		{
			get
			{
				return this.verticalOffset;
			}
			set
			{
				if (this.verticalOffset != value)
				{
					this.verticalOffset = value;
					this.Invalidate ();
				}
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			if (this.documentPrinter != null)
			{
				this.UpdateBitmap ();

				var bounds = this.Client.Bounds;
				Point origin = new Point (this.horizontalOffset, this.verticalOffset);
				graphics.PaintImage (this.bitmap, this.Client.Bounds, origin);

				//	Dessine le cadre de la page en dernier, pour recouvrir la page.
				bounds.Deflate (0.5);

				graphics.LineWidth = 1;
				graphics.AddRectangle (bounds);
				graphics.RenderSolid (Color.FromBrightness (0));
			}
		}

		private void UpdateBitmap()
		{
			this.UpdateBitmap (this.Client.Bounds.Width * this.zoom);
		}

		private void UpdateBitmap(double width)
		{
			if (this.bitmap == null || this.lastBitmapWidth != width)
			{
				this.DisposeBitmap ();

				this.lastBitmapWidth = width;
				this.bitmap = this.CreateBitmap (width);
			}
		}

		private void DisposeBitmap()
		{
			if (this.bitmap != null)
			{
				this.bitmap.Dispose ();
				this.bitmap = null;
			}
		}

		private Bitmap CreateBitmap(double width)
		{
			double scale = width / this.documentPrinter.RequiredPageSize.Width;

			int w = (int) width;
			int h = (int) (this.DocumentHeight * scale);

			double offsetY = h - Printers.AbstractDocumentPrinter.continuousHeight*scale;

			Graphics graphics = new Graphics ();
			graphics.SetPixmapSize (w, h);
			graphics.TranslateTransform (0, offsetY);
			graphics.ScaleTransform (scale, scale, 0, 0);

			this.documentPrinter.CurrentPage = 0;
			this.documentPrinter.PrintBackgroundCurrentPage (graphics);
			this.documentPrinter.PrintForegroundCurrentPage (graphics);

			var bitmap = Bitmap.FromPixmap (graphics.Pixmap) as Bitmap;
			bitmap.FlipY ();

			return bitmap;
		}

		private double DocumentHeight
		{
			get
			{
				return Printers.AbstractDocumentPrinter.continuousHeight - this.documentPrinter.ContinuousVerticalMax;
			}
		}


		private Printers.AbstractDocumentPrinter	documentPrinter;
		private double								zoom;
		private double								lastBitmapWidth;
		private Bitmap								bitmap;
		private double								verticalOffset;
		private double								horizontalOffset;
	}
}
