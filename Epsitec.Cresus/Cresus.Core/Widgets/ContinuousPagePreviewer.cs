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
			this.timer = new Timer ();
			this.timer.Delay = 1.0;
			this.timer.TimeElapsed += new EventHandler (this.HandleTimerElapsed);

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

				double w = this.documentPrinter.RequiredPageSize.Width;
				double h = Printers.AbstractDocumentPrinter.continuousHeight - this.documentPrinter.ContinuousVerticalMax;

				this.documentSize = new Size (w, h);
			}
		}

		public void CloseUI()
		{
			this.timer.Stop ();
			this.timer.Dispose ();
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
					this.UpdateBitmap (true);
				}
			}
		}

		public double TotalWidth
		{
			get
			{
				return this.documentSize.Width * this.DocumentToScreenScale;
			}
		}

		public double TotalHeight
		{
			get
			{
				return this.documentSize.Height * this.DocumentToScreenScale;
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
				this.UpdateBitmap (false);

				var bounds = this.Client.Bounds;
				var imageRect = new Rectangle (this.horizontalOffset, this.verticalOffset, bounds.Width, bounds.Height);
				imageRect.Scale(1.0/this.BitmapToScreenScale);
				graphics.PaintImage (this.bitmap, bounds, imageRect);

				//	Dessine le cadre de la page en dernier, pour recouvrir la page.
				bounds.Deflate (0.5);

				graphics.LineWidth = 1;
				graphics.AddRectangle (bounds);
				graphics.RenderSolid (Color.FromBrightness (0));
			}
		}


		private void HandleTimerElapsed(object sender)
		{
			//?System.Diagnostics.Debug.WriteLine ("Timber !!!");
			if (this.UpdateBitmap (true))
			{
				this.Invalidate ();
			}
		}

		private bool UpdateBitmap(bool force)
		{
			return this.UpdateBitmap (force, this.Client.Bounds.Width * this.zoom);
		}

		private bool UpdateBitmap(bool force, double width)
		{
			long now = System.DateTime.Now.Ticks;
			bool isLongTime = force || (now-this.bitmapTicks)/10000000 >= 1;  // écoulé plus d'une seconde ?
			bool dirty = false;

			if (this.bitmap == null || (this.lastBitmapWidth != width && isLongTime))
			{
				this.timer.Stop ();
				this.DisposeBitmap ();

				this.bitmap = this.CreateBitmap (width);
				this.lastBitmapWidth = width;
				dirty = true;
			}
			else
			{
				this.timer.Stop ();

				if (this.lastBitmapWidth != width)
				{
					this.timer.Start ();
				}
			}

			this.bitmapTicks = System.DateTime.Now.Ticks;
			return dirty;
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
			double scale = width / this.documentSize.Width;

			int w = (int) width;
			int h = (int) (this.documentSize.Height * scale);

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


		private double BitmapToScreenScale
		{
			get
			{
				return (this.Client.Bounds.Width / this.bitmap.Width) * this.zoom;
			}
		}

		private double DocumentToScreenScale
		{
			get
			{
				return (this.Client.Bounds.Width / this.documentSize.Width) * this.zoom;
			}
		}


		private readonly Timer						timer;

		private Printers.AbstractDocumentPrinter	documentPrinter;
		private Size								documentSize;		// taille du document en mm
		private double								zoom;				// zoom souhaité (1 ou 2)
		private double								lastBitmapWidth;
		private long								bitmapTicks;
		private Bitmap								bitmap;
		private double								verticalOffset;
		private double								horizontalOffset;
	}
}
