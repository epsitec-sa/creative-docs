//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 12/12/2003

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Scrollable permet de représenter un widget de taille
	/// quelconque dans une surface de taille déterminée, en ajoutant
	/// si besoin des ascenceurs.
	/// </summary>
	public class Scrollable : Widget
	{
		public Scrollable()
		{
			this.h_scroller = new HScroller (this);
			this.v_scroller = new VScroller (this);
			
			this.h_scroller.Parent = this;
			this.h_scroller.Range  = 0;
			this.h_scroller.VisibleRangeRatio = 1;
			
			this.v_scroller.Parent = this;
			this.v_scroller.Range  = 0;
			this.v_scroller.VisibleRangeRatio = 1;
			this.v_scroller.IsInverted = true;
			
			this.h_scroller.ValueChanged += new EventHandler (HandleHScrollerValueChanged);
			this.v_scroller.ValueChanged += new EventHandler(HandleVScrollerValueChanged);
			
			this.aperture = new Widget (this);
			this.aperture.Parent = this;
			
			this.UpdateGeometry ();
		}
		
		public Panel					Panel
		{
			get { return this.panel; }
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
		
		
		protected void AttachPanel(Panel panel)
		{
			panel.LayoutChanged += new EventHandler (HandlePanelLayoutChanged);
			panel.Parent = this.aperture;
		}
		
		protected void DetachPanel(Panel panel)
		{
			panel.LayoutChanged -= new EventHandler (HandlePanelLayoutChanged);
			panel.Parent = null;
		}
		
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			this.UpdateGeometry ();
		}
		
		protected void UpdateGeometry()
		{
			if ((this.h_scroller == null) ||
				(this.v_scroller == null))
			{
				return;
			}
			
			this.UpdatePanelLocation ();
			
			double margin_x = (this.v_scroller.IsVisible) ? this.v_scroller.Width : 0;
			double margin_y = (this.h_scroller.IsVisible) ? this.h_scroller.Height : 0;
			
			if (this.v_scroller.IsVisible)
			{
				this.v_scroller.Bounds = new Drawing.Rectangle (this.Client.Width - margin_x, margin_y, margin_x, this.Client.Height - margin_y);
			}
			
			if (this.h_scroller.IsVisible)
			{
				this.h_scroller.Bounds = new Drawing.Rectangle (0, 0, this.Client.Width - margin_x, margin_y);
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
			
			//	TODO: gérer la rotation/le changement d'échelle du panel qui doit aussi
			//	être reflété sur ses marges...
			
			Drawing.Margins frame = this.panel.FrameMargins;
			
			double margin_x = 0;
			double margin_y = 0;
			
			double delta_dx;
			double delta_dy;
			
			for (;;)
			{
				delta_dx = this.panel.Width  + frame.Width  - this.Client.Width  + margin_x;
				delta_dy = this.panel.Height + frame.Height - this.Client.Height + margin_y;
				
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
			
			double vis_dx = this.Client.Width  - margin_x - frame.Width;
			double vis_dy = this.Client.Height - margin_y - frame.Height;
			
			double offset_x = 0;
			double offset_y = 0;
			
			//	Détermine l'aspect des ascenceurs ainsi que les offsets [x] et [y] qui
			//	doivent s'appliquer à l'ouverture qui permet de voir le panel.
			
			if ((this.panel.Width > 0) &&
				(delta_dx > 0) &&
				(vis_dx > 0))
			{
				this.h_scroller.VisibleRangeRatio = vis_dx / this.panel.Width;
				this.h_scroller.Range = delta_dx;
				this.h_scroller.SmallChange = 5;
				this.h_scroller.LargeChange = vis_dx / 2;
			
				offset_x = this.h_scroller.Value;
			}
			else
			{
				this.h_scroller.Range = 0.0;
				this.h_scroller.VisibleRangeRatio = 1.0;
			}
			
			if ((this.panel.Height > 0) &&
				(delta_dy > 0) &&
				(vis_dy > 0))
			{
				this.v_scroller.VisibleRangeRatio = vis_dy / this.panel.Height;
				this.v_scroller.Range = delta_dy;
				this.v_scroller.SmallChange = 5;
				this.v_scroller.LargeChange = vis_dy / 2;
				
				offset_y = this.v_scroller.Value;
			}
			else
			{
				this.v_scroller.Range = 0.0;
				this.v_scroller.VisibleRangeRatio = 1.0;
			}
			
			//	Met à jour l'ouverture qui permet de voir le panel (aperture) et déplace
			//	le panel pour que la partie qui intéresse l'utilisateur soit en face de
			//	l'ouverture.
			
			this.aperture.Bounds = new Drawing.Rectangle (frame.Left, frame.Bottom + margin_y, vis_dx, vis_dy);
			this.panel.Location  = new Drawing.Point (-offset_x, this.Client.Height - this.panel.Height - margin_y - frame.Height + offset_y);
			this.panel.Aperture  = this.panel.MapParentToClient (this.aperture.Client.Bounds);
			
			this.Invalidate ();
			
			this.h_scroller.SetVisible (margin_y > 0);
			this.v_scroller.SetVisible (margin_x > 0);
		}
		
		
		private void HandleHScrollerValueChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.h_scroller == sender);
			this.UpdatePanelLocation ();
		}
		
		private void HandleVScrollerValueChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.v_scroller == sender);
			this.UpdatePanelLocation ();
		}
		
		
		
		protected Panel					panel;
		protected Widget				aperture;
		protected Drawing.Size			panel_size;
		protected VScroller				v_scroller;
		protected HScroller				h_scroller;
		
		private void HandlePanelLayoutChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.panel == sender);
			
			if (this.panel_size != panel.Size)
			{
				this.panel_size = panel.Size;
				this.UpdatePanelLocation ();
			}
		}
	}
}
