//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	using BundleAttribute  = Support.BundleAttribute;
	
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
			this.h_scroller = new HScroller (this);
			this.v_scroller = new VScroller (this);
			
			this.h_scroller_mode = ScrollableScrollerMode.Auto;
			this.v_scroller_mode = ScrollableScrollerMode.Auto;
			
			this.h_scroller.MaxValue          = 0;
			this.h_scroller.VisibleRangeRatio = 1;
			this.h_scroller.IsInverted        = false;
			
			this.v_scroller.MaxValue          = 0;
			this.v_scroller.VisibleRangeRatio = 1;
			this.v_scroller.IsInverted        = true;
			
			this.h_scroller.ValueChanged += new Support.EventHandler (this.HandleHScrollerValueChanged);
			this.v_scroller.ValueChanged += new Support.EventHandler (this.HandleVScrollerValueChanged);

			this.isForegroundFrame = false;
			this.foregroundFrameMargins = new Drawing.Margins(0,0,0,0);
			
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
				return this.panel;
			}
			
			set
			{
				if (this.panel != value)
				{
					if (this.panel != null)
					{
						this.DetachPanel (this.panel);
					}
					
					this.panel = value;
					
					if (this.panel != null)
					{
						this.AttachPanel (this.panel);
					}
					
					this.UpdateGeometry ();
				}
			}
		}
		
		
		[Bundle] public ScrollableScrollerMode	HorizontalScrollerMode
		{
			get
			{
				return this.h_scroller_mode;
			}
			set
			{
				if (this.h_scroller_mode != value)
				{
					this.h_scroller_mode = value;
					this.UpdateGeometry ();
				}
			}
		}
		
		[Bundle] public ScrollableScrollerMode	VerticalScrollerMode
		{
			get
			{
				return this.v_scroller_mode;
			}
			set
			{
				if (this.v_scroller_mode != value)
				{
					this.v_scroller_mode = value;
					this.UpdateGeometry ();
				}
			}
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.DetachPanel (this.panel);
				
				this.panel = null;
				
				this.h_scroller.ValueChanged -= new Support.EventHandler (this.HandleHScrollerValueChanged);
				this.v_scroller.ValueChanged -= new Support.EventHandler (this.HandleVScrollerValueChanged);
				
				this.h_scroller.Dispose ();
				this.v_scroller.Dispose ();
				
				this.h_scroller = null;
				this.v_scroller = null;
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
			
			if ((this.h_scroller == null) ||
				(this.v_scroller == null))
			{
				return;
			}
			
			//	Met à jour la position du panel dans la surface disponible; ceci détermine aussi
			//	du même coup la visibilité des ascenceurs.
			
			this.UpdatePanelLocation ();
			
			//	Place correctement les ascenceurs.

			double margin_x = (this.v_scroller.Visibility) ? this.v_scroller.ActualWidth  : 0;
			double margin_y = (this.h_scroller.Visibility) ? this.h_scroller.ActualHeight : 0;
			
			double total_dx = this.Client.Size.Width;
			double total_dy = this.Client.Size.Height;
			
			if (this.v_scroller.Visibility)
			{
				this.v_scroller.SetManualBounds(new Drawing.Rectangle(total_dx - margin_x, margin_y, margin_x, total_dy - margin_y));
			}

			if (this.h_scroller.Visibility)
			{
				this.h_scroller.SetManualBounds(new Drawing.Rectangle(0, 0, total_dx - margin_x, margin_y));
			}
		}
		
		protected void UpdatePanelLocation()
		{
			if (this.panel == null)
			{
				this.h_scroller.Hide ();
				this.v_scroller.Hide ();
				
				return;
			}
			
			double total_dx = this.Client.Size.Width;
			double total_dy = this.Client.Size.Height;
			double panel_dx = this.panel.SurfaceWidth;
			double panel_dy = this.panel.SurfaceHeight;
			double margin_x = (this.v_scroller_mode == ScrollableScrollerMode.ShowAlways) ? this.v_scroller.ActualWidth : 0;
			double margin_y = (this.h_scroller_mode == ScrollableScrollerMode.ShowAlways) ? this.h_scroller.ActualHeight : 0;
			
			double delta_dx;
			double delta_dy;
			
			//	Procède itérativement pour savoir quels ascenceurs vont être utilisés
			//	et quelle place ils vont occuper.
			
			for (;;)
			{
				delta_dx = panel_dx - total_dx + margin_x;
				delta_dy = panel_dy - total_dy + margin_y;
				
				if ((delta_dx > 0) &&
					(this.h_scroller_mode != ScrollableScrollerMode.HideAlways))
				{
					//	Il y a besoin d'un ascenceur horizontal.
					
					if (margin_y == 0)
					{
						margin_y = this.h_scroller.ActualHeight;
						continue;
					}
				}
				
				if ((delta_dy > 0) &&
					(this.v_scroller_mode != ScrollableScrollerMode.HideAlways))
				{
					//	Il y a besoin d'un ascenceur vertical.
					
					if (margin_x == 0)
					{
						margin_x = this.v_scroller.ActualWidth;
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
				this.h_scroller.MaxValue          = (decimal) (delta_dx);
				this.h_scroller.VisibleRangeRatio = (decimal) (vis_dx / panel_dx);
				this.h_scroller.SmallChange       = (decimal) (Scrollable.SmallScrollPixels);
				this.h_scroller.LargeChange       = (decimal) (vis_dx * Scrollable.LargeScrollPercent / 100);
			
				offset_x = System.Math.Min (this.panel_offset.X, delta_dx);
				
				this.h_scroller_value = (decimal) offset_x;
				this.h_scroller.Value = (decimal) offset_x;
			}
			else
			{
				panel_dx = vis_dx;
				
				this.h_scroller_value = 0;
				this.panel_offset.X   = 0;
				
				this.h_scroller.MaxValue          = 1.0M;
				this.h_scroller.Value             = 0.0M;
				this.h_scroller.VisibleRangeRatio = 1.0M;
			}
			
			if ((panel_dy > 0) &&
				(delta_dy > 0) &&
				(vis_dy > 0))
			{
				this.v_scroller.MaxValue          = (decimal) (delta_dy);
				this.v_scroller.VisibleRangeRatio = (decimal) (vis_dy / panel_dy);
				this.v_scroller.SmallChange       = (decimal) (Scrollable.SmallScrollPixels);
				this.v_scroller.LargeChange       = (decimal) (vis_dy * Scrollable.LargeScrollPercent / 100);
				
				offset_y = System.Math.Min (this.panel_offset.Y, delta_dy);
				
				this.v_scroller_value = (decimal) offset_y;
				this.v_scroller.Value = (decimal) offset_y;
			}
			else
			{
				panel_dy = vis_dy;
				
				this.v_scroller_value = 0;
				this.panel_offset.Y   = 0;
				
				this.v_scroller.MaxValue          = 1.0M;
				this.v_scroller.Value             = 0.0M;
				this.v_scroller.VisibleRangeRatio = 1.0M;
			}
			
			//	Met à jour l'ouverture (aperture) qui permet de voir le panel et ajuste
			//	ce dernier pour que la partie qui intéresse l'utilisateur soit en face de
			//	l'ouverture.
			
			this.panel_aperture = new Drawing.Rectangle (0, margin_y, vis_dx, vis_dy);
			this.panel.SetManualBounds(new Drawing.Rectangle(-offset_x, total_dy - panel_dy + offset_y, panel_dx, panel_dy));
			this.panel.Aperture = this.panel.MapParentToClient (this.panel_aperture);
			
			this.h_scroller.Visibility = (margin_y > 0);
			this.v_scroller.Visibility = (margin_x > 0);
			
			this.Invalidate ();
		}
		
		
		private void HandleHScrollerValueChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.h_scroller == sender);
			
			if (this.h_scroller.Value != this.h_scroller_value)
			{
				this.panel_offset.X = System.Math.Floor (this.h_scroller.DoubleValue);
				this.UpdatePanelLocation ();
			}
		}
		
		private void HandleVScrollerValueChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.v_scroller == sender);
			
			if (this.v_scroller.Value != this.v_scroller_value)
			{
				this.panel_offset.Y = System.Math.Floor (this.v_scroller.DoubleValue);
				this.UpdatePanelLocation ();
			}
		}
		
		private void HandlePanelSurfaceSizeChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.panel == sender);
			
			this.UpdateGeometry ();
		}
		
		
		public bool								IsForegroundFrame
		{
			get { return this.isForegroundFrame; }
			set { this.isForegroundFrame = value; }
		}

		public Drawing.Margins					ForegroundFrameMargins
		{
			get { return this.foregroundFrameMargins; }
			set { this.foregroundFrameMargins = value; }
		}

		protected override void PaintForegroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			if (this.isForegroundFrame == false)  return;

			IAdorner    adorner = Widgets.Adorners.Factory.Active;
			WidgetState state   = this.PaintState;
			
			Drawing.Rectangle rect  = this.Client.Bounds;
			double margin_x = (this.v_scroller.Visibility) ? this.v_scroller.ActualWidth  : 0;
			double margin_y = (this.h_scroller.Visibility) ? this.h_scroller.ActualHeight : 0;
			rect.Right -= margin_x;
			rect.Bottom += margin_y;
			rect.Deflate (this.foregroundFrameMargins);
			rect.Deflate (0.5);
			graphics.AddRectangle (rect);
			graphics.RenderSolid (adorner.ColorBorder);
		}

		
		protected Panel							panel;						//	panel utilisé pour le contenu
		protected Drawing.Rectangle				panel_aperture;				//	ouverture par laquelle on voit le panel
		protected Drawing.Point					panel_offset;				//	offset du panel, dérivé de la position des ascenceurs
		
		protected VScroller						v_scroller;
		protected HScroller						h_scroller;
		protected decimal						v_scroller_value;			//	dernière position de l'ascenceur vertical
		protected decimal						h_scroller_value;			//	dernière position de l'ascenceur horizontal
		protected ScrollableScrollerMode		v_scroller_mode;
		protected ScrollableScrollerMode		h_scroller_mode;
		
		protected bool							isForegroundFrame;
		protected Drawing.Margins				foregroundFrameMargins;

		protected const double					SmallScrollPixels  = 5;
		protected const double					LargeScrollPercent = 50;
	}
}
