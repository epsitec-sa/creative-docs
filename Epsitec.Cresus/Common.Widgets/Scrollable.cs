//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 16.01.2004

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Scrollable permet de représenter un panel de taille
	/// quelconque dans une surface de taille déterminée, en ajoutant
	/// au besoin des ascenceurs.
	/// </summary>
	public class Scrollable : Widget
	{
		public Scrollable()
		{
			this.h_scroller = new HScroller (this);
			this.v_scroller = new VScroller (this);
			
			this.h_scroller.Parent            = this;
			this.h_scroller.Range             = 0;
			this.h_scroller.VisibleRangeRatio = 1;
			
			this.v_scroller.Parent            = this;
			this.v_scroller.Range             = 0;
			this.v_scroller.VisibleRangeRatio = 1;
			this.v_scroller.IsInverted        = true;
			
			this.h_scroller.ValueChanged += new EventHandler (this.HandleHScrollerValueChanged);
			this.v_scroller.ValueChanged += new EventHandler (this.HandleVScrollerValueChanged);
			
			this.UpdateGeometry ();
		}
		
		public Scrollable(Widget embedder) : this()
		{
			this.SetEmbedder (embedder);
		}
		
		
		public Panel					Panel
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
						this.UpdateGeometry ();
					}
				}
			}
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.DetachPanel (this.panel);
				
				this.panel = null;
				
				this.h_scroller.ValueChanged -= new EventHandler (this.HandleHScrollerValueChanged);
				this.v_scroller.ValueChanged -= new EventHandler (this.HandleVScrollerValueChanged);
				
				this.h_scroller.Dispose ();
				this.v_scroller.Dispose ();
				
				this.h_scroller = null;
				this.v_scroller = null;
			}
			
			base.Dispose (disposing);
		}

		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			this.UpdateGeometry ();
		}
		
		
		protected void AttachPanel(Panel panel)
		{
			if (panel != null)
			{
				panel.Parent   = this;
				panel.Aperture = Drawing.Rectangle.Empty;
				
				panel.LayoutChanged += new EventHandler (this.HandlePanelLayoutChanged);
				panel.SurfaceSizeChanged += new EventHandler (this.HandlePanelSurfaceSizeChanged);
			}
		}
		
		protected void DetachPanel(Panel panel)
		{
			if (panel != null)
			{
				panel.LayoutChanged -= new EventHandler (this.HandlePanelLayoutChanged);
				panel.SurfaceSizeChanged -= new EventHandler (this.HandlePanelSurfaceSizeChanged);
				
				panel.Parent   = null;
				panel.Aperture = Drawing.Rectangle.Infinite;
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
			
			double margin_x = (this.v_scroller.IsVisible) ? this.v_scroller.Width  : 0;
			double margin_y = (this.h_scroller.IsVisible) ? this.h_scroller.Height : 0;
			
			double total_dx = this.Client.Width;
			double total_dy = this.Client.Height;
			
			if (this.v_scroller.IsVisible)
			{
				this.v_scroller.Bounds = new Drawing.Rectangle (total_dx - margin_x, margin_y, margin_x, total_dy - margin_y);
			}
			
			if (this.h_scroller.IsVisible)
			{
				this.h_scroller.Bounds = new Drawing.Rectangle (0, 0, total_dx - margin_x, margin_y);
			}
		}
		
		protected void UpdatePanelLocation()
		{
			if (this.panel == null)
			{
				this.h_scroller.SetVisible (false);
				this.v_scroller.SetVisible (false);
				
				return;
			}
			
			double total_dx = this.Client.Width;
			double total_dy = this.Client.Height;
			double panel_dx = this.panel.SurfaceWidth;
			double panel_dy = this.panel.SurfaceHeight;
			double margin_x = 0;
			double margin_y = 0;
			
			double delta_dx;
			double delta_dy;
			
			//	Procède itérativement pour savoir quels ascenceurs vont être utilisés
			//	et quelle place ils vont occuper.
			
			for (;;)
			{
				delta_dx = panel_dx - total_dx + margin_x;
				delta_dy = panel_dy - total_dy + margin_y;
				
				if (delta_dx > 0)
				{
					//	Il y a besoin d'un ascenceur horizontal.
					
					if (margin_y == 0)
					{
						margin_y = this.h_scroller.Height;
						continue;
					}
				}
				
				if (delta_dy > 0)
				{
					//	Il y a besoin d'un ascenceur vertical.
					
					if (margin_x == 0)
					{
						margin_x = this.v_scroller.Width;
						continue;
					}
				}
				
				break;
			}
			
			double vis_dx = total_dx - margin_x;
			double vis_dy = total_dy - margin_y;
			
			double offset_x = 0;
			double offset_y = 0;
			
			//	Détermine l'aspect des ascenceurs ainsi que les offsets [x] et [y] qui
			//	doivent s'appliquer à l'ouverture (aperture) qui permet de voir le panel.
			
			if ((panel_dx > 0) &&
				(delta_dx > 0) &&
				(vis_dx > 0))
			{
				this.h_scroller.Range             = delta_dx;
				this.h_scroller.VisibleRangeRatio = vis_dx / panel_dx;
				this.h_scroller.SmallChange       = Scrollable.SmallScrollPixels;
				this.h_scroller.LargeChange       = vis_dx * Scrollable.LargeScrollPercent / 100;
			
				offset_x = System.Math.Min (this.panel_offset.X, delta_dx);
				
				this.h_scroller_value = offset_x;
				this.h_scroller.Value = offset_x;
			}
			else
			{
				panel_dx = vis_dx;
				
				this.h_scroller_value = 0;
				this.panel_offset.X   = 0;
				
				this.h_scroller.Range             = 0.0;
				this.h_scroller.Value             = 0.0;
				this.h_scroller.VisibleRangeRatio = 1.0;
			}
			
			if ((panel_dy > 0) &&
				(delta_dy > 0) &&
				(vis_dy > 0))
			{
				this.v_scroller.Range             = delta_dy;
				this.v_scroller.VisibleRangeRatio = vis_dy / panel_dy;
				this.v_scroller.SmallChange       = Scrollable.SmallScrollPixels;
				this.v_scroller.LargeChange       = vis_dy * Scrollable.LargeScrollPercent / 100;
				
				offset_y = System.Math.Min (this.panel_offset.Y, delta_dy);
				
				this.v_scroller_value = offset_y;
				this.v_scroller.Value = offset_y;
			}
			else
			{
				panel_dy = vis_dy;
				
				this.v_scroller_value = 0;
				this.panel_offset.Y   = 0;
				
				this.v_scroller.Range             = 0.0;
				this.v_scroller.Value             = 0.0;
				this.v_scroller.VisibleRangeRatio = 1.0;
			}
			
			//	Met à jour l'ouverture (aperture) qui permet de voir le panel et ajuste
			//	ce dernier pour que la partie qui intéresse l'utilisateur soit en face de
			//	l'ouverture.
			
			this.panel_aperture = new Drawing.Rectangle (0, margin_y, vis_dx, vis_dy);
			this.panel_size     = new Drawing.Size (panel_dx, panel_dy);
			
			this.panel.Bounds   = new Drawing.Rectangle (-offset_x, total_dy - panel_dy + offset_y, panel_dx, panel_dy);
			this.panel.Aperture = this.panel.MapParentToClient (this.panel_aperture);
			
			this.h_scroller.SetVisible (margin_y > 0);
			this.v_scroller.SetVisible (margin_x > 0);
			
			this.Invalidate ();
		}
		
		
		private void HandleHScrollerValueChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.h_scroller == sender);
			
			if (this.h_scroller.Value != this.h_scroller_value)
			{
				this.panel_offset.X = this.h_scroller.Value;
				this.UpdatePanelLocation ();
			}
		}
		
		private void HandleVScrollerValueChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.v_scroller == sender);
			
			if (this.v_scroller.Value != this.v_scroller_value)
			{
				this.panel_offset.Y = this.v_scroller.Value;
				this.UpdatePanelLocation ();
			}
		}
		
		private void HandlePanelLayoutChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.panel == sender);
			
			if (this.panel_size != this.panel.Size)
			{
				this.panel_size = this.panel.Size;
				this.UpdatePanelLocation ();
			}
		}
		
		private void HandlePanelSurfaceSizeChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.panel == sender);
			
			this.UpdatePanelLocation ();
		}
		
		
		
		protected Panel					panel;
		protected Drawing.Rectangle		panel_aperture;				//	ouverture par laquelle on voit le panel
		protected Drawing.Size			panel_size;					//	taille du panel
		protected Drawing.Point			panel_offset;				//	offset du panel, dérivé de la position des ascenceurs
		
		protected VScroller				v_scroller;
		protected double				v_scroller_value;			//	dernière position de l'ascenceur
		protected HScroller				h_scroller;
		protected double				h_scroller_value;			//	dernière position de l'ascenceur
		
		protected const double			SmallScrollPixels  = 5;
		protected const double			LargeScrollPercent = 50;
	}
}
