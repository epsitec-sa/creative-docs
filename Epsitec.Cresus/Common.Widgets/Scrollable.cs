//	Copyright © 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
	/// La classe Scrollable permet de représenter un Panel de taille
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
			
			this.hScroller.ValueChanged += new Support.EventHandler (this.HandleHScrollerValueChanged);
			this.vScroller.ValueChanged += new Support.EventHandler (this.HandleVScrollerValueChanged);

			this.Panel = new Panel ();
		}
		
		public Scrollable(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
		}
		
		
		public Panel							Panel
		{
			get
			{
				return (Panel) this.GetValue (Scrollable.PanelProperty);
			}
			set
			{
				this.SetValue (Scrollable.PanelProperty, value);
			}
		}

		public double							PanelOffsetX
		{
			get
			{
				return this.panelOffset.X;
			}
			set
			{
				if (this.panelOffset.X != value)
				{
					this.panelOffset.X = value;
					this.UpdatePanelLocation ();
				}
			}
		}

		public double							PanelOffsetY
		{
			get
			{
				return this.panelOffset.Y;
			}
			set
			{
				if (this.panelOffset.Y != value)
				{
					this.panelOffset.Y = value;
					this.UpdatePanelLocation ();
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
				this.Panel = null;
				
				this.hScroller.ValueChanged -= new Support.EventHandler (this.HandleHScrollerValueChanged);
				this.vScroller.ValueChanged -= new Support.EventHandler (this.HandleVScrollerValueChanged);
				
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
		
		
		protected void AttachPanel(Panel panel)
		{
			if (panel != null)
			{
				panel.SetEmbedder (this);
				panel.Aperture = Drawing.Rectangle.Empty;
				
				panel.SurfaceSizeChanged += new Support.EventHandler (this.HandlePanelSurfaceSizeChanged);
			}
		}
		
		protected void DetachPanel(Panel panel)
		{
			if (panel != null)
			{
				panel.SurfaceSizeChanged -= new Support.EventHandler (this.HandlePanelSurfaceSizeChanged);
				
				panel.SetEmbedder (null);
				panel.Aperture = Drawing.Rectangle.MaxValue;
			}
		}
		
		
		protected void UpdateGeometry()
		{
			//	Met à jour la géométrie du panel et des ascenceurs.
			
			if ((this.hScroller == null) ||
				(this.vScroller == null))
			{
				return;
			}
			
			//	Met à jour la position du panel dans la surface disponible; ceci détermine aussi
			//	du même coup la visibilité des ascenceurs.
			
			this.UpdatePanelLocation ();
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

		protected virtual void UpdatePanelLocation()
		{
			Panel panel = this.Panel;
			
			if (panel == null)
			{
				this.hScroller.Hide ();
				this.vScroller.Hide ();
				
				return;
			}
			
			double total_dx = this.Client.Size.Width;
			double total_dy = this.Client.Size.Height;
			double panel_dx = panel.SurfaceWidth;
			double panel_dy = panel.SurfaceHeight;
			double margin_x = (this.vScrollerMode == ScrollableScrollerMode.ShowAlways) ? this.vScroller.PreferredWidth : 0;
			double margin_y = (this.hScrollerMode == ScrollableScrollerMode.ShowAlways) ? this.hScroller.PreferredHeight : 0;
			
			double delta_dx;
			double delta_dy;
			
			//	Procède itérativement pour savoir quels ascenceurs vont être utilisés
			//	et quelle place ils vont occuper.
			
			for (;;)
			{
				delta_dx = panel_dx - total_dx + margin_x;
				delta_dy = panel_dy - total_dy + margin_y;
				
				if ((delta_dx > 0) &&
					(this.hScrollerMode != ScrollableScrollerMode.HideAlways))
				{
					//	Il y a besoin d'un ascenceur horizontal.
					
					if (margin_y == 0)
					{
						margin_y = this.hScroller.PreferredHeight;
						continue;
					}
				}
				
				if ((delta_dy > 0) &&
					(this.vScrollerMode != ScrollableScrollerMode.HideAlways))
				{
					//	Il y a besoin d'un ascenceur vertical.
					
					if (margin_x == 0)
					{
						margin_x = this.vScroller.PreferredWidth;
						continue;
					}
				}
				
				break;
			}
			
			double vis_dx = total_dx - margin_x;
			double vis_dy = total_dy - margin_y;
			
			double offset_x = 0;
			double offset_y = 0;
			
			panel_dx = System.Math.Max (panel_dx, vis_dx);
			panel_dy = System.Math.Max (panel_dy, vis_dy);
			
			//	Détermine l'aspect des ascenceurs ainsi que les offsets [x] et [y] qui
			//	doivent s'appliquer à l'ouverture (aperture) qui permet de voir le panel.
			
			if ((panel_dx > 0) &&
				(delta_dx > 0) &&
				(vis_dx > 0))
			{
				this.hScroller.MaxValue          = (decimal) (delta_dx);
				this.hScroller.VisibleRangeRatio = (decimal) (vis_dx / panel_dx);
				this.hScroller.SmallChange       = (decimal) (Scrollable.SmallScrollPixels);
				this.hScroller.LargeChange       = (decimal) (vis_dx * Scrollable.LargeScrollPercent / 100);
			
				offset_x = System.Math.Min (this.panelOffset.X, delta_dx);
				
				this.hScrollerValue  = (decimal) offset_x;
				this.hScroller.Value = (decimal) offset_x;
			}
			else
			{
				panel_dx = vis_dx;
				
				this.hScrollerValue = 0;
				this.panelOffset.X  = 0;
				
				this.hScroller.MaxValue          = 1.0M;
				this.hScroller.Value             = 0.0M;
				this.hScroller.VisibleRangeRatio = 1.0M;
			}
			
			if ((panel_dy > 0) &&
				(delta_dy > 0) &&
				(vis_dy > 0))
			{
				this.vScroller.MaxValue          = (decimal) (delta_dy);
				this.vScroller.VisibleRangeRatio = (decimal) (vis_dy / panel_dy);
				this.vScroller.SmallChange       = (decimal) (Scrollable.SmallScrollPixels);
				this.vScroller.LargeChange       = (decimal) (vis_dy * Scrollable.LargeScrollPercent / 100);
				
				offset_y = System.Math.Min (this.panelOffset.Y, delta_dy);
				
				this.vScrollerValue = (decimal) offset_y;
				this.vScroller.Value = (decimal) offset_y;
			}
			else
			{
				panel_dy = vis_dy;
				
				this.vScrollerValue = 0;
				this.panelOffset.Y  = 0;
				
				this.vScroller.MaxValue          = 1.0M;
				this.vScroller.Value             = 0.0M;
				this.vScroller.VisibleRangeRatio = 1.0M;
			}
			
			//	Met à jour l'ouverture (aperture) qui permet de voir le panel et ajuste
			//	ce dernier pour que la partie qui intéresse l'utilisateur soit en face de
			//	l'ouverture.

			this.hScroller.Visibility = (margin_y > 0);
			this.vScroller.Visibility = (margin_x > 0);

			this.panelAperture = new Drawing.Rectangle (0, margin_y, vis_dx, vis_dy);
			panel.SetManualBounds (new Drawing.Rectangle (-offset_x, total_dy - panel_dy + offset_y, panel_dx, panel_dy));
			panel.Aperture = panel.MapParentToClient (this.panelAperture);
			
			this.Invalidate ();
		}
		
		
		private void HandleHScrollerValueChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.hScroller == sender);
			
			if (this.hScroller.Value != this.hScrollerValue)
			{
				this.panelOffset.X = System.Math.Floor (this.hScroller.DoubleValue);
				this.UpdatePanelLocation ();
			}
		}
		
		private void HandleVScrollerValueChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.vScroller == sender);
			
			if (this.vScroller.Value != this.vScrollerValue)
			{
				this.panelOffset.Y = System.Math.Floor (this.vScroller.DoubleValue);
				this.UpdatePanelLocation ();
			}
		}
		
		private void HandlePanelSurfaceSizeChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.Panel == sender);
			
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
			double margin_x = (this.vScroller.Visibility) ? this.vScroller.PreferredWidth  : 0;
			double margin_y = (this.hScroller.Visibility) ? this.hScroller.PreferredHeight : 0;
			rect.Right -= margin_x;
			rect.Bottom += margin_y;
			rect.Deflate (this.foregroundFrameMargins);
			rect.Deflate (0.5);
			graphics.AddRectangle (rect);
			graphics.RenderSolid (adorner.ColorBorder);
		}

		private static void HandlePanelChanged(DependencyObject o, object oldValue, object newValue)
		{
			Scrollable that = (Scrollable) o;

			Panel oldPanel = oldValue as Panel;
			Panel newPanel = newValue as Panel;
			
			if (oldPanel != null)
			{
				that.DetachPanel (oldPanel);
			}
			if (newPanel != null)
			{
				that.AttachPanel (newPanel);
			}

			that.UpdateGeometry ();
		}

		private static object GetPanelOffsetXValue(DependencyObject o)
		{
			Scrollable that = (Scrollable) o;
			return that.PanelOffsetX;
		}

		private static object GetPanelOffsetYValue(DependencyObject o)
		{
			Scrollable that = (Scrollable) o;
			return that.PanelOffsetY;
		}

		private static void SetPanelOffsetXValue(DependencyObject o, object value)
		{
			Scrollable that = (Scrollable) o;
			that.PanelOffsetX = (double) value;
		}

		private static void SetPanelOffsetYValue(DependencyObject o, object value)
		{
			Scrollable that = (Scrollable) o;
			that.PanelOffsetX = (double) value;
		}

		public static readonly DependencyProperty PanelProperty = DependencyProperty.Register ("Panel", typeof (Panel), typeof (Scrollable), new DependencyPropertyMetadata (null, Scrollable.HandlePanelChanged));
		public static readonly DependencyProperty PanelOffsetXProperty = DependencyProperty.Register ("PanelOffsetX", typeof (double), typeof (Scrollable), new DependencyPropertyMetadata (Scrollable.GetPanelOffsetXValue, Scrollable.SetPanelOffsetXValue));
		public static readonly DependencyProperty PanelOffsetYProperty = DependencyProperty.Register ("PanelOffsetY", typeof (double), typeof (Scrollable), new DependencyPropertyMetadata (Scrollable.GetPanelOffsetYValue, Scrollable.SetPanelOffsetYValue));

		private Drawing.Rectangle				panelAperture;
		private Drawing.Point					panelOffset;
		
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
