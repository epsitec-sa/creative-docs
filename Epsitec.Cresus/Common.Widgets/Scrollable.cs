//	Copyright � 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 12/12/2003

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Scrollable permet de repr�senter un widget de taille
	/// quelconque dans une surface de taille d�termin�e, en ajoutant
	/// si besoin des ascenceurs.
	/// </summary>
	public class Scrollable : Widget
	{
		public Scrollable()
		{
			this.h_scroller = new HScroller (this);
			this.v_scroller = new VScroller (this);
			
			this.h_scroller.Parent = this;
			this.h_scroller.Minimum = 0;
			this.h_scroller.Maximum = 1;
			this.h_scroller.Display = 0;
			
			this.v_scroller.Parent = this;
			this.h_scroller.ValueChanged += new EventHandler (HandleHScrollerValueChanged);
			this.v_scroller.ValueChanged += new EventHandler(HandleVScrollerValueChanged);
			
			this.panel_container = new Widget (this);
			this.panel_container.Parent = this;
			
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
			panel.Parent = this.panel_container;
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
			
			double margin_x = 0;
			double margin_y = 0;
			
			double delta_dx;
			double delta_dy;
			
			for (;;)
			{
				delta_dx = this.panel.Width  - this.Client.Width  + margin_x;
				delta_dy = this.panel.Height - this.Client.Height + margin_y;
				
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
			
			double vis_dx = this.Client.Width - margin_x;
			double vis_dy = this.Client.Height - margin_y;
			double offset_x = 0;
			double offset_y = 0;
			
			if (delta_dx > 0)
			{
				offset_x = delta_dx * this.h_scroller.Value;
			}
			if (delta_dy > 0)
			{
				offset_y = delta_dy * this.v_scroller.Value;
			}
			
			this.panel_container.Bounds = new Drawing.Rectangle (0, margin_y, vis_dx, vis_dy);
			this.panel.Location = new Drawing.Point (offset_x, offset_y + this.Client.Height - this.panel.Height);
			
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
		protected Widget				panel_container;
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
