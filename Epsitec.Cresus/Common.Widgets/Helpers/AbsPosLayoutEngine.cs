//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// La classe AbsPosLayoutEngine implémente un moteur de layout absolu; en clair,
	/// cela signifie que les widgets contenus dans le panel ne sont pas redimensionnés.
	/// </summary>
	public class AbsPosLayoutEngine : AbstractLayoutEngine
	{
		public AbsPosLayoutEngine()
		{
			this.anchor = AnchorStyles.Top | AnchorStyles.Left;
		}
		
		
		public AnchorStyles				Anchor
		{
			get
			{
				return this.anchor;
			}
			set
			{
				if (this.anchor != value)
				{
					switch (this.anchor & AnchorStyles.LeftAndRight)
					{
						case AnchorStyles.Left:
						case AnchorStyles.Right:
							break;
						default:
							throw new System.ArgumentException (string.Format ("Invalid anchor mode {0} specified.", (this.anchor & AnchorStyles.LeftAndRight)));
					}
					
					switch (this.anchor & AnchorStyles.TopAndBottom)
					{
						case AnchorStyles.Top:
						case AnchorStyles.Bottom:
							break;
						default:
							throw new System.ArgumentException (string.Format ("Invalid anchor mode {0} specified.", (this.anchor & AnchorStyles.TopAndBottom)));
					}
					
					this.anchor = value;
				}
			}
		}
		
		
		
		protected override void AttachPanel(Panel panel)
		{
			base.AttachPanel (panel);
			this.panel_size = panel.Size;
		}

		protected override void UpdateStructure()
		{
			//	Détermine la taille de la surface minimale désirée en fonction de
			//	la disposition des widgets.
			
			Drawing.Rectangle bounds = Drawing.Rectangle.Empty;
			
			foreach (Widget widget in this.panel.Children)
			{
				bounds = Drawing.Rectangle.Union (bounds, widget.Bounds);
			}
			
			System.Diagnostics.Debug.WriteLine("UpdateStructure: " + bounds.ToString ());
			
			//	TODO: gérer les transformation de coordonnées client
			//	TODO: gérer la marge du cadre du panel (FrameMargin)
			
			Drawing.Size size = this.panel.Size;
			
			double surface_dx = System.Math.Max (bounds.Right, size.Width);
			double surface_dy = System.Math.Max (bounds.Top, size.Height);
			
			switch (this.anchor & AnchorStyles.LeftAndRight)
			{
				case AnchorStyles.Left:		surface_dx  = bounds.Right;	break;
				case AnchorStyles.Right:	surface_dx -= bounds.Left;	break;
			}
			
			switch (this.anchor & AnchorStyles.TopAndBottom)
			{
				case AnchorStyles.Bottom:	surface_dy  = bounds.Top;	 break;
				case AnchorStyles.Top:		surface_dy -= bounds.Bottom; break;
			}
			
			this.panel.DesiredSize = new Drawing.Size (surface_dx, surface_dy);
		}
		
		protected override void UpdateGeometry()
		{
			Drawing.Size old_size = this.panel_size;
			Drawing.Size new_size = this.panel.Size;
			
			//	TODO: gérer les transformation de coordonnées client
			
			if (old_size == new_size)
			{
				return;
			}
			
			this.panel_size = new_size;
			
			double x_change = new_size.Width  - old_size.Width;
			double y_change = new_size.Height - old_size.Height;
			
			Drawing.Point offset = new Drawing.Point ();
			
			if (x_change != 0)
			{
				switch (this.anchor & AnchorStyles.LeftAndRight)
				{
					case AnchorStyles.Left:		offset.X = 0;			break;
					case AnchorStyles.Right:	offset.X = x_change;	break;
				}
			}
			
			if (y_change != 0)
			{
				switch (this.anchor & AnchorStyles.TopAndBottom)
				{
					case AnchorStyles.Bottom:	offset.Y = 0;			break;
					case AnchorStyles.Top:		offset.Y = y_change;	break;
				}
			}
			
			if ((offset.X != 0) ||
				(offset.Y != 0))
			{
				//	Déplace les widgets contenus dans le panel pour mieux occuper la surface
				//	disponible.
				
				foreach (Widget widget in this.panel.Children)
				{
					widget.Location += offset;
				}
			}
		}
		
		
		protected AnchorStyles			anchor;
		protected Drawing.Size			surface_size;
	}
}
