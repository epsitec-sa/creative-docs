//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Print;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Ce widget montre le contenu d'une page imprimée dans une zone rectangulaire.
	/// </summary>
	public sealed class ContinuousPagePreviewer : Widget, Common.Widgets.Behaviors.IDragBehaviorHost
	{
		public ContinuousPagePreviewer()
		{
			this.dragBehavior = this.CreateDragBehavior ();

			this.timer = new Timer ();
			this.timer.Delay = 1.0;
			this.timer.TimeElapsed += new EventHandler (this.HandleTimerElapsed);

			this.zoom = 1;
		}


		public AbstractPrinter DocumentPrinter
		{
			get
			{
				return this.documentPrinter;
			}
			set
			{
				this.documentPrinter = value;
				this.Update ();
			}
		}

		public void CloseUI()
		{
			this.timer.Stop ();
			this.timer.Dispose ();
		}

		public void Update()
		{
			if (this.documentPrinter == null)
			{
				return;
			}

			//	Met à jour le contenu de la page.
			double w = this.documentPrinter.RequiredPageSize.Width;
			double h = AbstractPrinter.ContinuousHeight - this.documentPrinter.ContinuousVerticalMax;
			var documentSize = new Size (w, h);
			var changed = false;

			if (this.documentSize != documentSize)
			{
				this.documentSize = documentSize;
				changed = true;
			}

			this.lastBitmapWidth = 0;
			this.UpdateBitmap (force: true);
			this.Invalidate ();

			if (changed)
			{
				this.OnCurrentValueChanged ();
			}
		}

		public double Zoom
		{
			//	Zoom servant à l'affichage de la page (1..n, 1 ou 2 en pratique).
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
					this.Invalidate ();
				}
			}
		}

		public Point MinValue
		{
			//	Valeur minimale en millimètres.
			get
			{
				var bounds = this.BoundsRectangle;

				return new Point (bounds.Width/2, bounds.Height/2);
			}
		}

		public Point MaxValue
		{
			//	Valeur maximale en millimètres.
			get
			{
				var bounds = this.BoundsRectangle;

				return new Point (this.documentSize.Width-bounds.Width/2, this.documentSize.Height-bounds.Height/2);
			}
		}

		public Size VisibleRangeRatio
		{
			//	Ratios pour les ascenseurs (0..1).
			get
			{
				var bounds = this.BoundsRectangle;

				double rx = System.Math.Min (bounds.Width  / this.documentSize.Width,  1);
				double ry = System.Math.Min (bounds.Height / this.documentSize.Height, 1);

				return new Size (rx, ry);
			}
		}

		public Point CurrentValue
		{
			//	Valeur courante en millimètres.
			get
			{
				return this.currentValue;
			}
			set
			{
				var min = this.MinValue;
				value.X = System.Math.Max (value.X, min.X);
				value.Y = System.Math.Max (value.Y, min.Y);

				var max = this.MaxValue;
				value.X = System.Math.Min (value.X, max.X);
				value.Y = System.Math.Min (value.Y, max.Y);

				if (this.currentValue != value)
				{
					this.currentValue = value;
					this.Invalidate ();
					this.OnCurrentValueChanged ();
				}
			}
		}

		public double ScreenToDocumentScale
		{
			//	Echelle pour convertir des valeurs de l'interface en millimètres.
			get
			{
				return 1.0 / this.DocumentToScreenScale;
			}
		}


		private Common.Widgets.Behaviors.DragBehavior CreateDragBehavior()
		{
			return new Common.Widgets.Behaviors.DragBehavior (this, true, true);
		}

		protected override void ProcessMessage(Message message, Point pos)
		{
			if (this.IsEnabled == false)
			{
				return;
			}

			if (message.IsMouseType)
			{
				if (message.MessageType == MessageType.MouseMove)
				{
					var ratio = this.VisibleRangeRatio;
					this.UseCursorHand (System.Math.Abs (ratio.Width -1) > 0.00001 ||
										System.Math.Abs (ratio.Height-1) > 0.00001);
				}
			}

			if (!this.dragBehavior.ProcessMessage (message, pos))
			{
				base.ProcessMessage (message, pos);
			}
		}

		#region IDragBehaviorHost Members

		public Point DragLocation
		{
			get
			{
				return Point.Zero;
			}
		}

		public bool OnDragBegin(Point cursor)
		{
			this.draggingPos = cursor;
			return true;
		}

		public void OnDragging(DragEventArgs e)
		{
			Point cursor = e.ToPoint;

			var scale = this.ScreenToDocumentScale;
			var offset = new Point ((this.draggingPos.X-cursor.X)*scale, (this.draggingPos.Y-cursor.Y)*scale);
			this.CurrentValue += offset;

			this.draggingPos = cursor;
		}

		public void OnDragEnd()
		{
		}

		#endregion

	
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			if (this.documentPrinter != null)
			{
				this.UpdateBitmap (false);

				var bounds = this.BoundsRectangle;
				bounds.Scale (this.DocumentToScreenScale);
				var imageRect = this.ImageRectangle;
				graphics.PaintImage (this.bitmap, bounds, imageRect);

				//	Dessine le cadre de la page en dernier, pour recouvrir la page.
				bounds.Deflate (0.5);

				graphics.LineWidth = 1;
				graphics.AddRectangle (bounds);
				graphics.RenderSolid (Color.FromBrightness (0));
			}
		}


		private Rectangle ImageRectangle
		{
			//	Retourne le rectangle de l'image, en points dans l'image.
			get
			{
				var min = this.MinValue;
				var imageRect = new Rectangle (this.currentValue.X-min.X, this.currentValue.Y-min.Y, min.X*2, min.Y*2);

				imageRect.Scale (this.DocumentToScreenScale);    // document -> screen
				imageRect.Scale (1.0/this.BitmapToScreenScale);  // screen -> bitmap

				return imageRect;
			}
		}

		private Rectangle BoundsRectangle
		{
			//	Retourne le rectangle de l'image, en millimètres.
			get
			{
				var bounds = this.Client.Bounds;

				if (bounds.IsEmpty)
				{
					return Rectangle.Zero;
				}
				else
				{
					bounds = Rectangle.Scale (bounds, this.ScreenToDocumentScale);

					double aspectRatio = this.documentSize.Height / this.documentSize.Width;
					double width  = bounds.Width;
					double height = System.Math.Min (bounds.Height, bounds.Width * aspectRatio);
					
					return new Rectangle (bounds.X, bounds.Y, width, height);
				}
			}
		}


		private void HandleTimerElapsed(object sender)
		{
			//?System.Diagnostics.Debug.WriteLine ("Timber !!!");
			if (this.UpdateBitmap (force: true))
			{
				this.Invalidate ();
			}
		}

		private bool UpdateBitmap(bool force)
		{
			//	Met à jour le bitmap. Retourne true s'il l'a été.
			if (this.Client.Bounds.Height <= 20)
			{
				//	Une hauteur <= 20 correspond à un widget non encore positionné par le système
				//	de layout. Il ne sert à rien de générer un bitmap si petit, qui s'afficherait
				//	complètement flou avant que le timer ne le régénère à la taille définitive !
				return false;
			}

			return this.UpdateBitmap (force, this.Client.Bounds.Width * this.zoom);
		}

		private bool UpdateBitmap(bool force, double width)
		{
			long now = System.DateTime.UtcNow.Ticks;
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

			this.bitmapTicks = System.DateTime.UtcNow.Ticks;
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

			double offsetY = h - AbstractPrinter.ContinuousHeight*scale;

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
			//	Echelle pour convertir des points dans le btmap en valeurs pour l'interface.
			get
			{
				return (this.Client.Bounds.Width / this.bitmap.Width) * this.zoom;
			}
		}

		private double DocumentToScreenScale
		{
			//	Echelle pour convertir des millimètres en valeurs pour l'interface.
			get
			{
				return (this.Client.Bounds.Width / this.documentSize.Width) * this.zoom;
			}
		}


		private void UseCursorHand(bool hand)
		{
			//	Utilise la main comme cursour souris dans ce widget.
			if (hand)
			{
				if (this.mouseCursorHand == null)
				{
					this.mouseCursorHand = MouseCursor.FromImage (Common.Support.ImageProvider.Default.GetImage ("manifest:Epsitec.Common.Widgets.Images.Cursor.Hand.icon", Common.Support.Resources.DefaultManager));
				}

				this.MouseCursor = this.mouseCursorHand;
			}
			else
			{
				this.MouseCursor = MouseCursor.AsArrow;
			}
		}


		#region Events handler
		private void OnCurrentValueChanged()
		{
			var handler = this.GetUserEventHandler (ContinuousPagePreviewer.CurrentValueChangedEvent);

			if (handler != null)
			{
				handler (this);
			}
		}

		public event EventHandler CurrentValueChanged
		{
			add
			{
				this.AddUserEventHandler (ContinuousPagePreviewer.CurrentValueChangedEvent, value);
			}
			remove
			{
				this.RemoveUserEventHandler (ContinuousPagePreviewer.CurrentValueChangedEvent, value);
			}
		}

		private const string CurrentValueChangedEvent = "CurrentValueChanged";
		#endregion


		private readonly Common.Widgets.Behaviors.DragBehavior	dragBehavior;
		private readonly Timer						timer;

		private AbstractPrinter						documentPrinter;
		private Size								documentSize;		// taille du document en mm
		private double								zoom;				// zoom souhaité (1 ou 2)
		private double								lastBitmapWidth;
		private long								bitmapTicks;
		private Bitmap								bitmap;
		private Point								currentValue;
		private MouseCursor							mouseCursorHand;
		private Point								draggingPos;
	}
}
