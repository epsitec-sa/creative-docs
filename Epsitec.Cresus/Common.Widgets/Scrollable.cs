//	Copyright © 2003-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>Scrollable</c> class manages a <see cref="Viewport"/> which can
	/// have an arbitrary surface size. Scrollers will be added as required.
	public class Scrollable : AbstractGroup, Behaviors.IDragBehaviorHost
	{
		public Scrollable()
		{
			this.dragBehavior = this.CreateDragBehavior ();

			this.hScroller = new HScroller (this)
			{
				Name = "HorizontalScroller",
				MaxValue = 0,
				VisibleRangeRatio = 1,
				IsInverted = false,
			};

			this.vScroller = new VScroller (this)
			{
				Name = "VerticalScroller",
				MaxValue = 0,
				VisibleRangeRatio = 1,
				IsInverted = true,
			};
			
			this.hScrollerMode = ScrollableScrollerMode.Auto;
			this.vScrollerMode = ScrollableScrollerMode.Auto;

			this.hScroller.ValueChanged += this.HandleHScrollerValueChanged;
			this.vScroller.ValueChanged += this.HandleVScrollerValueChanged;

			this.Viewport = new Viewport ();
		}

		public Scrollable(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}

		protected virtual Behaviors.DragBehavior CreateDragBehavior()
		{
			return new Behaviors.DragBehavior (this, true, false);
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


		public Point							ViewportOffset
		{
			get
			{
				return this.viewportOffset;
			}
			set
			{
				if (this.viewportOffset != value)
                {
					this.viewportOffset = value;
					this.UpdateViewportLocation ();
                }
			}
		}

		public double							ViewportOffsetX
		{
			get
			{
				return this.viewportOffset.X;
			}
			set
			{
				if (this.viewportOffset.X != value)
				{
					this.ViewportOffset = new Point (value, this.ViewportOffsetY);
				}
			}
		}

		public double							ViewportOffsetY
		{
			get
			{
				return this.viewportOffset.Y;
			}
			set
			{
				if (this.viewportOffset.Y != value)
				{
					this.ViewportOffset = new Point (this.ViewportOffsetX, value);
					
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

		public bool								PaintViewportFrame
		{
			get
			{
				return this.paintForegroundFrame;
			}
			set
			{
				if (this.paintForegroundFrame != value)
				{
					this.paintForegroundFrame = value;
					this.UpdateGeometry ();
				}
			}
		}

		public bool								ScrollWithHand
		{
			get
			{
				return this.scrollWithHand;
			}
			set
			{
				if (this.scrollWithHand != value)
				{
					this.scrollWithHand = value;
				}
			}
		}

		public Margins							ViewportFrameMargins
		{
			get
			{
				return this.viewportFrameMargins;
			}
			set
			{
				if (this.viewportFrameMargins != value)
                {
					this.viewportFrameMargins = value;
					this.UpdateGeometry ();
				}
			}
		}

		public Margins							ViewportPadding
		{
			get
			{
				return this.viewportPadding;
			}
			set
			{
				if (this.viewportPadding != value)
                {
					this.viewportPadding = value;
					this.UpdateGeometry ();
                }
			}
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


		public void Show(Widget child)
		{
			if (child == null)
            {
				return;
            }

			if (!child.IsActualGeometryValid)
            {
				this.Window.ForceLayout ();
            }

			var aperture = this.Viewport.Aperture;
			var bounds   = child.ActualBounds;

			//aperture.Deflate (this.ViewportPadding);

			if (aperture.Contains (bounds))
			{
				//	Nothing to do : the view is already completely visible in
				//	the current aperture.
			}
			else
			{
				double ox = 0;
				double oy = 0;

				if ((aperture.Right < bounds.Right) &&
					(aperture.Left < bounds.Left))
				{
					ox = System.Math.Max (aperture.Right - bounds.Right, aperture.Left - bounds.Left);
				}
				else if ((aperture.Right > bounds.Right) &&
					     (aperture.Left > bounds.Left))
				{
					ox = System.Math.Min (aperture.Right - bounds.Right, aperture.Left - bounds.Left);
				}

				if ((aperture.Top < bounds.Top) &&
					(aperture.Bottom < bounds.Bottom))
				{
					oy = System.Math.Max (aperture.Top - bounds.Top, aperture.Bottom - bounds.Bottom);
				}
				else if ((aperture.Top > bounds.Top) &&
					     (aperture.Bottom > bounds.Bottom))
				{
					oy = System.Math.Min (aperture.Top - bounds.Top, aperture.Bottom - bounds.Bottom);
				}

				var offset = this.ViewportOffset;

				offset = offset + new Point (ox, oy);

				this.ViewportOffset = offset;
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

		protected override void SetBoundsOverride(Rectangle oldRect, Rectangle newRect)
		{
			base.SetBoundsOverride(oldRect, newRect);

			//	Geometry becomes valid the first time the bounds get defined; before that, there is
			//	no need to try to update the geometry...
			
			this.isGeometryValid = true;
			this.UpdateGeometry ();
		}


		protected void AttachViewport(Viewport viewport)
		{
			if (viewport != null)
			{
				viewport.SetEmbedder (this);
				viewport.Aperture = Rectangle.Empty;

				viewport.SurfaceSizeChanged += this.HandleViewportSurfaceSizeChanged;
			}
		}

		protected void DetachViewport(Viewport viewport)
		{
			if (viewport != null)
			{
				viewport.SurfaceSizeChanged -= this.HandleViewportSurfaceSizeChanged;

				viewport.SetEmbedder (null);
				viewport.Aperture = Rectangle.MaxValue;
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
			
			if (this.isGeometryValid)
            {
				this.UpdateViewportLocation ();
				this.UpdateScrollerLocation ();
			}
		}

		protected virtual void UpdateScrollerLocation()
		{
			var rect = this.GetSurfaceRectangle ();

			if (this.vScroller.Visibility)
			{
				if (this.vScrollerMode == ScrollableScrollerMode.ShowAlwaysOppositeSide)
				{
					var bounds = new Rectangle (0, rect.Bottom, this.vScroller.PreferredWidth, rect.Height);
					this.UpdateVerticalScrollerBounds (bounds);
				}
				else
				{
					var bounds = new Rectangle (rect.Right, rect.Bottom, this.vScroller.PreferredWidth, rect.Height);
					this.UpdateVerticalScrollerBounds (bounds);
				}
			}

			if (this.hScroller.Visibility)
			{
				if (this.hScrollerMode == ScrollableScrollerMode.ShowAlwaysOppositeSide)
				{
					var bounds = new Rectangle (rect.Left, rect.Top, rect.Width, this.hScroller.PreferredHeight);
					this.UpdateHorizontalScrollerBounds (bounds);
				}
				else
				{
					var bounds = new Rectangle (rect.Left, 0, rect.Width, this.hScroller.PreferredHeight);
					this.UpdateHorizontalScrollerBounds (bounds);
				}
			}
		}

		private Rectangle GetSurfaceRectangle()
		{
			double width  = (this.vScroller.Visibility) ? this.vScroller.PreferredWidth  : 0;
			double height = (this.hScroller.Visibility) ? this.hScroller.PreferredHeight : 0;
			
			var rect = this.Client.Bounds;

			if (this.vScroller.Visibility)
			{
				if (this.vScrollerMode == ScrollableScrollerMode.ShowAlwaysOppositeSide)
				{
					rect = Rectangle.Deflate (rect, new Margins (width, 0, 0, 0));
				}
				else
				{
					rect = Rectangle.Deflate (rect, new Margins (0, width, 0, 0));
				}
			}

			if (this.hScroller.Visibility)
			{
				if (this.hScrollerMode == ScrollableScrollerMode.ShowAlwaysOppositeSide)
				{
					rect = Rectangle.Deflate (rect, new Margins (0, 0, height, 0));
				}
				else
				{
					rect = Rectangle.Deflate (rect, new Margins (0, 0, 0, height));
				}
			}

			return rect;
		}
		
		private Rectangle GetViewportRectangle()
		{
			var rect = this.GetSurfaceRectangle ();

			if (this.paintForegroundFrame)
			{
				rect = Rectangle.Deflate (rect, new Margins (1));
				rect = Rectangle.Deflate (rect, this.viewportFrameMargins + this.viewportPadding);
			}
			
			return rect;
		}


		protected virtual void UpdateVerticalScrollerBounds(Rectangle bounds)
		{
			this.vScroller.SetManualBounds (bounds);
		}

		protected virtual void UpdateHorizontalScrollerBounds(Rectangle bounds)
		{
			this.hScroller.SetManualBounds (bounds);
		}

		protected virtual void UpdateViewportLocation()
		{
			Viewport viewport = this.Viewport;

			this.somethingToScroll = false;

			if (viewport == null)
			{
				this.hScroller.Hide ();
				this.vScroller.Hide ();
				
				return;
			}
			
			double totalDx = this.Client.Size.Width;
			double totalDy = this.Client.Size.Height;

			if (this.paintForegroundFrame)
			{
				double mx = this.viewportFrameMargins.Width  + this.viewportPadding.Width  + 2;
				double my = this.viewportFrameMargins.Height + this.viewportPadding.Height + 2;

				totalDx -= mx;
				totalDy -= my;
			}

			double viewportDx = viewport.SurfaceWidth;
			double viewportDy = viewport.SurfaceHeight;
			double marginX = this.GetVerticalShowAlways   () ? this.vScroller.PreferredWidth  : 0;
			double marginY = this.GetHorizontalShowAlways () ? this.hScroller.PreferredHeight : 0;
			
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

				this.somethingToScroll = true;
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

				this.somethingToScroll = true;
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

			double ox = 0;
			double oy = marginY;

			if (this.hScrollerMode == ScrollableScrollerMode.ShowAlwaysOppositeSide)
			{
				oy = 0;
			}
			if (this.vScrollerMode == ScrollableScrollerMode.ShowAlwaysOppositeSide)
			{
				ox = marginX;
			}

			this.viewportAperture = this.GetViewportRectangle ();

			viewport.SetManualBounds (
				new Rectangle (
					this.viewportAperture.Left - offsetX, this.viewportAperture.Top - viewportDy + offsetY,
					viewportDx, viewportDy));

			viewport.Aperture = viewport.MapParentToClient (this.viewportAperture);
			
			this.Invalidate ();
		}


		private bool GetVerticalShowAlways()
		{
			return this.vScrollerMode == ScrollableScrollerMode.ShowAlways
				|| this.vScrollerMode == ScrollableScrollerMode.ShowAlwaysOppositeSide;
		}

		private bool GetHorizontalShowAlways()
		{
			return this.hScrollerMode == ScrollableScrollerMode.ShowAlways
				|| this.hScrollerMode == ScrollableScrollerMode.ShowAlwaysOppositeSide;
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
		
		
		protected override void PaintForegroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			if (this.paintForegroundFrame == false)
			{
				return;
			}

			IAdorner adorner = Widgets.Adorners.Factory.Active;
			WidgetPaintState state = this.GetPaintState ();

			var rect = this.GetSurfaceRectangle ();

			rect = Rectangle.Deflate (rect, this.viewportFrameMargins);
			rect = Rectangle.Deflate (rect, new Margins (0.5));
			
			graphics.AddRectangle (rect);
			graphics.RenderSolid (adorner.ColorBorder);
		}


		protected override void ProcessMessage(Message message, Point pos)
		{
			if (this.IsEnabled == false || this.ScrollWithHand == false)
			{
				return;
			}

			if (message.IsMouseType)
			{
				if (message.MessageType == MessageType.MouseMove)
				{
					var rect = this.GetViewportRectangle ();

					if (this.somethingToScroll && rect.Contains (pos))
					{
						if (this.mouseCursorHand == null)
						{
							this.mouseCursorHand = MouseCursor.FromImage (Support.ImageProvider.Default.GetImage ("manifest:Epsitec.Common.Widgets.Images.Cursor.Hand.icon", Support.Resources.DefaultManager));
						}

						this.MouseCursor = this.mouseCursorHand;
					}
					else
					{
						this.MouseCursor = MouseCursor.AsArrow;
					}
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
			this.isDragging = true;
			this.draggingInitialViewportOffset = this.ViewportOffset;

			return true;
		}

		public void OnDragging(DragEventArgs e)
		{
			Point cursor = e.ToPoint;

			double x = System.Math.Max (this.draggingInitialViewportOffset.X-cursor.X, 0);
			double y = System.Math.Max (this.draggingInitialViewportOffset.Y+cursor.Y, 0);

			this.ViewportOffset = new Point (x, y);
		}

		public void OnDragEnd()
		{
			this.isDragging = false;
		}

		#endregion


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

		public static readonly DependencyProperty ViewportProperty = DependencyProperty<Scrollable>.Register (x => x.Viewport, new DependencyPropertyMetadata (null, Scrollable.HandleViewportChanged));
		public static readonly DependencyProperty ViewportOffsetXProperty = DependencyProperty.Register ("ViewportOffsetX", typeof (double), typeof (Scrollable), new DependencyPropertyMetadata (Scrollable.GetViewportOffsetXValue, Scrollable.SetViewportOffsetXValue));
		public static readonly DependencyProperty ViewportOffsetYProperty = DependencyProperty.Register ("ViewportOffsetY", typeof (double), typeof (Scrollable), new DependencyPropertyMetadata (Scrollable.GetViewportOffsetYValue, Scrollable.SetViewportOffsetYValue));

		private readonly Behaviors.DragBehavior	dragBehavior;

		private bool							isGeometryValid;
		private Rectangle						viewportAperture;
		private Point							viewportOffset;
		
		protected VScroller						vScroller;
		protected HScroller						hScroller;
		private decimal							vScrollerValue;
		private decimal							hScrollerValue;
		private ScrollableScrollerMode			vScrollerMode;
		private ScrollableScrollerMode			hScrollerMode;
		
		private bool							paintForegroundFrame;
		private Margins							viewportFrameMargins;
		private Margins							viewportPadding;

		protected const double					SmallScrollPixels  = 5;
		protected const double					LargeScrollPercent = 50;

		private bool							scrollWithHand;
		private bool							isDragging;
		private Point							draggingInitialViewportOffset;

		private bool							somethingToScroll;
		private MouseCursor						mouseCursorHand;
	}
}
