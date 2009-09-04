//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// L'énumération ScrollableScrollerMode détermine comment Scrollable affiche les
	/// ascenceurs (automatiquement, en fonction de la place disponible, ne jamais les
	/// montrer ou toujours les montrer).
	/// </summary>
	public enum ScrollableScrollerMode
	{
		Auto,
		HideAlways,
		ShowAlways
	}
	
	/// <summary>
	/// La classe Scrollable permet de représenter un Viewport de taille
	/// quelconque dans une surface de taille déterminée, en ajoutant
	/// au besoin des ascenceurs.
	/// </summary>
	public class Scrollable : AbstractGroup
	{
		public Scrollable()
		{
			this.hScroller = new HScroller (this);
			this.vScroller = new VScroller (this);
			
			this.hScrollerMode = ScrollableScrollerMode.Auto;
			this.vScrollerMode = ScrollableScrollerMode.Auto;

			this.hScroller.Name = "HorizontalScroller";
			this.hScroller.MaxValue          = 0;
			this.hScroller.VisibleRangeRatio = 1;
			this.hScroller.IsInverted        = false;

			this.vScroller.Name = "VerticalScroller";
			this.vScroller.MaxValue          = 0;
			this.vScroller.VisibleRangeRatio = 1;
			this.vScroller.IsInverted        = true;
			
			this.hScroller.ValueChanged += this.HandleHScrollerValueChanged;
			this.vScroller.ValueChanged += this.HandleVScrollerValueChanged;

			this.Viewport = new Viewport ();
		}
		
		public Scrollable(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
		}
		
		
		public Viewport							Viewport
		{
			get
			{
				return (Viewport) this.GetValue (Scrollable.ViewportProperty);
			}
			private set
			{
				this.SetValue (Scrollable.ViewportProperty, value);
			}
		}

		public double ViewportOffsetX
		{
			get
			{
				return this.viewportOffset.X;
			}
			set
			{
				if (this.viewportOffset.X != value)
				{
					this.viewportOffset.X = value;
					this.UpdateViewportLocation ();
				}
			}
		}

		public double ViewportOffsetY
		{
			get
			{
				return this.viewportOffset.Y;
			}
			set
			{
				if (this.viewportOffset.Y != value)
				{
					this.viewportOffset.Y = value;
					this.UpdateViewportLocation ();
				}
			}
		}

		public ScrollableScrollerMode			HorizontalScrollerMode
		{
			get
			{
				return this.hScrollerMode;
			}
			set
			{
				if (this.hScrollerMode != value)
				{
					this.hScrollerMode = value;
					this.UpdateGeometry ();
				}
			}
		}
		
		public ScrollableScrollerMode			VerticalScrollerMode
		{
			get
			{
				return this.vScrollerMode;
			}
			set
			{
				if (this.vScrollerMode != value)
				{
					this.vScrollerMode = value;
					this.UpdateGeometry ();
				}
			}
		}
		
		public bool								PaintForegroundFrame
		{
			get { return this.paintForegroundFrame; }
			set { this.paintForegroundFrame = value; }
		}

		public Drawing.Margins					ForegroundFrameMargins
		{
			get { return this.foregroundFrameMargins; }
			set { this.foregroundFrameMargins = value; }
		}

		public HScroller						HorizontalScroller
		{
			get
			{
				return this.hScroller;
			}
		}

		public VScroller						VerticalScroller
		{
			get
			{
				return this.vScroller;
			}
		}

		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Viewport = null;
				
				this.hScroller.ValueChanged -= this.HandleHScrollerValueChanged;
				this.vScroller.ValueChanged -= this.HandleVScrollerValueChanged;
				
				this.hScroller.Dispose ();
				this.vScroller.Dispose ();
				
				this.hScroller = null;
				this.vScroller = null;
			}
			
			base.Dispose (disposing);
		}

		protected override void SetBoundsOverride(Drawing.Rectangle oldRect, Drawing.Rectangle newRect)
		{
			base.SetBoundsOverride(oldRect, newRect);
			this.UpdateGeometry ();
		}


		protected void AttachViewport(Viewport viewport)
		{
			if (viewport != null)
			{
				viewport.SetEmbedder (this);
				viewport.Aperture = Drawing.Rectangle.Empty;

				viewport.SurfaceSizeChanged += this.HandleViewportSurfaceSizeChanged;
			}
		}

		protected void DetachViewport(Viewport viewport)
		{
			if (viewport != null)
			{
				viewport.SurfaceSizeChanged -= this.HandleViewportSurfaceSizeChanged;

				viewport.SetEmbedder (null);
				viewport.Aperture = Drawing.Rectangle.MaxValue;
			}
		}
		
		
		protected void UpdateGeometry()
		{
			//	Met à jour la géométrie du viewport et des ascenceurs.
			
			if ((this.hScroller == null) ||
				(this.vScroller == null))
			{
				return;
			}

			//	Met à jour la position du viewport dans la surface disponible; ceci détermine aussi
			//	du même coup la visibilité des ascenceurs.

			this.UpdateViewportLocation ();
			this.UpdateScrollerLocation ();
		}

		protected virtual void UpdateScrollerLocation()
		{
			//	Place correctement les ascenceurs.

			double width  = (this.vScroller.Visibility) ? this.vScroller.PreferredWidth  : 0;
			double height = (this.hScroller.Visibility) ? this.hScroller.PreferredHeight : 0;

			double right = this.Client.Bounds.Right;
			double top   = this.Client.Bounds.Top;

			if (this.vScroller.Visibility)
			{
				Drawing.Rectangle bounds = new Drawing.Rectangle (right - width, height, width, top - height);
				this.UpdateVerticalScrollerBounds (bounds);
			}

			if (this.hScroller.Visibility)
			{
				Drawing.Rectangle bounds = new Drawing.Rectangle (0, 0, right - width, height);
				this.UpdateHorizontalScrollerBounds (bounds);
			}
		}

		protected virtual void UpdateVerticalScrollerBounds(Epsitec.Common.Drawing.Rectangle bounds)
		{
			this.vScroller.SetManualBounds (bounds);
		}

		protected virtual void UpdateHorizontalScrollerBounds(Drawing.Rectangle bounds)
		{
			this.hScroller.SetManualBounds (bounds);
		}

		protected virtual void UpdateViewportLocation()
		{
			Viewport viewport = this.Viewport;

			if (viewport == null)
			{
				this.hScroller.Hide ();
				this.vScroller.Hide ();
				
				return;
			}
			
			double totalDx = this.Client.Size.Width;
			double totalDy = this.Client.Size.Height;
			double viewportDx = viewport.SurfaceWidth;
			double viewportDy = viewport.SurfaceHeight;
			double marginX = (this.vScrollerMode == ScrollableScrollerMode.ShowAlways) ? this.vScroller.PreferredWidth : 0;
			double marginY = (this.hScrollerMode == ScrollableScrollerMode.ShowAlways) ? this.hScroller.PreferredHeight : 0;
			
			double deltaDx;
			double deltaDy;
			
			//	Procède itérativement pour savoir quels ascenceurs vont être utilisés
			//	et quelle place ils vont occuper.
			
			for (;;)
			{
				deltaDx = viewportDx - totalDx + marginX;
				deltaDy = viewportDy - totalDy + marginY;
				
				if ((deltaDx > 0) &&
					(this.hScrollerMode != ScrollableScrollerMode.HideAlways))
				{
					//	Il y a besoin d'un ascenceur horizontal.
					
					if (marginY == 0)
					{
						marginY = this.hScroller.PreferredHeight;
						continue;
					}
				}
				
				if ((deltaDy > 0) &&
					(this.vScrollerMode != ScrollableScrollerMode.HideAlways))
				{
					//	Il y a besoin d'un ascenceur vertical.
					
					if (marginX == 0)
					{
						marginX = this.vScroller.PreferredWidth;
						continue;
					}
				}
				
				break;
			}
			
			double visDx = totalDx - marginX;
			double visDy = totalDy - marginY;
			
			double offsetX = 0;
			double offsetY = 0;

			viewportDx = System.Math.Max (viewportDx, visDx);
			viewportDy = System.Math.Max (viewportDy, visDy);
			
			//	Détermine l'aspect des ascenceurs ainsi que les offsets [x] et [y] qui
			//	doivent s'appliquer à l'ouverture (aperture) qui permet de voir le viewport.

			if ((viewportDx > 0) &&
				(deltaDx > 0) &&
				(visDx > 0))
			{
				this.hScroller.MaxValue          = (decimal) (deltaDx);
				this.hScroller.VisibleRangeRatio = (decimal) (visDx / viewportDx);
				this.hScroller.SmallChange       = (decimal) (Scrollable.SmallScrollPixels);
				this.hScroller.LargeChange       = (decimal) (visDx * Scrollable.LargeScrollPercent / 100);

				offsetX = System.Math.Min (this.viewportOffset.X, deltaDx);
				
				this.hScrollerValue  = (decimal) offsetX;
				this.hScroller.Value = (decimal) offsetX;
			}
			else
			{
				viewportDx = visDx;
				
				this.hScrollerValue = 0;
				this.viewportOffset.X  = 0;
				
				this.hScroller.MaxValue          = 1.0M;
				this.hScroller.Value             = 0.0M;
				this.hScroller.VisibleRangeRatio = 1.0M;
			}

			if ((viewportDy > 0) &&
				(deltaDy > 0) &&
				(visDy > 0))
			{
				this.vScroller.MaxValue          = (decimal) (deltaDy);
				this.vScroller.VisibleRangeRatio = (decimal) (visDy / viewportDy);
				this.vScroller.SmallChange       = (decimal) (Scrollable.SmallScrollPixels);
				this.vScroller.LargeChange       = (decimal) (visDy * Scrollable.LargeScrollPercent / 100);

				offsetY = System.Math.Min (this.viewportOffset.Y, deltaDy);
				
				this.vScrollerValue = (decimal) offsetY;
				this.vScroller.Value = (decimal) offsetY;
			}
			else
			{
				viewportDy = visDy;
				
				this.vScrollerValue = 0;
				this.viewportOffset.Y  = 0;
				
				this.vScroller.MaxValue          = 1.0M;
				this.vScroller.Value             = 0.0M;
				this.vScroller.VisibleRangeRatio = 1.0M;
			}

			//	Met à jour l'ouverture (aperture) qui permet de voir le viewport et ajuste
			//	ce dernier pour que la partie qui intéresse l'utilisateur soit en face de
			//	l'ouverture.

			this.hScroller.Visibility = (marginY > 0);
			this.vScroller.Visibility = (marginX > 0);

			this.viewportAperture = new Drawing.Rectangle (0, marginY, visDx, visDy);
			viewport.SetManualBounds (new Drawing.Rectangle (-offsetX, totalDy - viewportDy + offsetY, viewportDx, viewportDy));
			viewport.Aperture = viewport.MapParentToClient (this.viewportAperture);
			
			this.Invalidate ();
		}
		
		
		private void HandleHScrollerValueChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.hScroller == sender);
			
			if (this.hScroller.Value != this.hScrollerValue)
			{
				this.viewportOffset.X = System.Math.Floor (this.hScroller.DoubleValue);
				this.UpdateViewportLocation ();
			}
		}
		
		private void HandleVScrollerValueChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.vScroller == sender);
			
			if (this.vScroller.Value != this.vScrollerValue)
			{
				this.viewportOffset.Y = System.Math.Floor (this.vScroller.DoubleValue);
				this.UpdateViewportLocation ();
			}
		}

		private void HandleViewportSurfaceSizeChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.Viewport == sender);
			
			this.UpdateGeometry ();
		}
		
		
		protected override void PaintForegroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			if (this.paintForegroundFrame == false)
			{
				return;
			}

			IAdorner adorner = Widgets.Adorners.Factory.Active;
			WidgetPaintState state = this.PaintState;
			
			Drawing.Rectangle rect  = this.Client.Bounds;
			double marginX = (this.vScroller.Visibility) ? this.vScroller.PreferredWidth  : 0;
			double marginY = (this.hScroller.Visibility) ? this.hScroller.PreferredHeight : 0;
			rect.Right -= marginX;
			rect.Bottom += marginY;
			rect.Deflate (this.foregroundFrameMargins);
			rect.Deflate (0.5);
			graphics.AddRectangle (rect);
			graphics.RenderSolid (adorner.ColorBorder);
		}

		private static void HandleViewportChanged(DependencyObject o, object oldValue, object newValue)
		{
			Scrollable that = (Scrollable) o;

			Viewport oldViewport = oldValue as Viewport;
			Viewport newViewport = newValue as Viewport;

			if (oldViewport != null)
			{
				that.DetachViewport (oldViewport);
			}
			if (newViewport != null)
			{
				that.AttachViewport (newViewport);
			}

			that.UpdateGeometry ();
		}

		private static object GetViewportOffsetXValue(DependencyObject o)
		{
			Scrollable that = (Scrollable) o;
			return that.ViewportOffsetX;
		}

		private static object GetViewportOffsetYValue(DependencyObject o)
		{
			Scrollable that = (Scrollable) o;
			return that.ViewportOffsetY;
		}

		private static void SetViewportOffsetXValue(DependencyObject o, object value)
		{
			Scrollable that = (Scrollable) o;
			that.ViewportOffsetX = (double) value;
		}

		private static void SetViewportOffsetYValue(DependencyObject o, object value)
		{
			Scrollable that = (Scrollable) o;
			that.ViewportOffsetX = (double) value;
		}

		public static readonly DependencyProperty ViewportProperty = DependencyProperty.Register ("Viewport", typeof (Viewport), typeof (Scrollable), new DependencyPropertyMetadata (null, Scrollable.HandleViewportChanged));
		public static readonly DependencyProperty ViewportOffsetXProperty = DependencyProperty.Register ("ViewportOffsetX", typeof (double), typeof (Scrollable), new DependencyPropertyMetadata (Scrollable.GetViewportOffsetXValue, Scrollable.SetViewportOffsetXValue));
		public static readonly DependencyProperty ViewportOffsetYProperty = DependencyProperty.Register ("ViewportOffsetY", typeof (double), typeof (Scrollable), new DependencyPropertyMetadata (Scrollable.GetViewportOffsetYValue, Scrollable.SetViewportOffsetYValue));

		private Drawing.Rectangle				viewportAperture;
		private Drawing.Point					viewportOffset;
		
		protected VScroller						vScroller;
		protected HScroller						hScroller;
		private decimal							vScrollerValue;
		private decimal							hScrollerValue;
		private ScrollableScrollerMode			vScrollerMode;
		private ScrollableScrollerMode			hScrollerMode;
		
		private bool							paintForegroundFrame;
		private Drawing.Margins					foregroundFrameMargins;

		protected const double					SmallScrollPixels  = 5;
		protected const double					LargeScrollPercent = 50;
	}
}
