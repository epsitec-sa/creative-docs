//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// La classe AbstractLayoutEngine impl�mente les m�canismes de base pour le
	/// layout de widgets dans un panel, dont d�rivent toutes les impl�mentations
	/// r�elles.
	/// </summary>
	public abstract class AbstractLayoutEngine
	{
		public AbstractLayoutEngine()
		{
		}
		
		
		public void Validate()
		{
			//	Met � jour la structure interne, puis redispose les widgets en occupant
			//	au mieux la taille disponible.
			
			if (this.is_dirty_structure)
			{
				this.UpdateStructure ();
				
				this.is_dirty_structure = false;
				this.is_dirty_geometry  = true;
			}
			
			if (this.is_dirty_geometry)
			{
				this.UpdateGeometry ();
				
				this.is_dirty_geometry = false;
			}	
		}
		
		public void Invalidate()
		{
			if (this.panel != null)
			{
				this.is_dirty_structure = true;
				this.panel.Invalidate ();
			}
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
					}
				}
			}
		}
		
		
		protected virtual void AttachPanel(Panel panel)
		{
			if (panel != null)
			{
				panel.LayoutChanged   += new EventHandler (this.HandlePanelLayoutChanged);
				panel.ChildrenChanged += new EventHandler (this.HandlePanelChildrenChanged);
				panel.PreparePaint    += new EventHandler (this.HandlePanelPreparePaint);
				
				panel.SetPropagationModes (Widget.PropagationModes.UpChildrenChanged, true,
					/**/				   Widget.PropagationSetting.IncludeChildren);
				
				this.Invalidate ();
			}
		}
		
		protected virtual void DetachPanel(Panel panel)
		{
			if (panel != null)
			{
				panel.LayoutChanged   -= new EventHandler (this.HandlePanelLayoutChanged);
				panel.ChildrenChanged -= new EventHandler (this.HandlePanelChildrenChanged);
				panel.PreparePaint    -= new EventHandler (this.HandlePanelPreparePaint);
			}
		}
		
		
		protected abstract void UpdateStructure();
		protected abstract void UpdateGeometry();
		
		protected void HandlePanelLayoutChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.panel == sender);
			
			if (this.panel.Size != this.panel_size)
			{
				this.is_dirty_geometry = true;
				this.panel.Invalidate ();
			}
		}
		
		protected void HandlePanelChildrenChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.panel != null);
			
			this.is_dirty_structure = true;
			this.panel.Invalidate ();
		}
		
		protected void HandlePanelPreparePaint(object sender)
		{
			System.Diagnostics.Debug.Assert (this.panel == sender);
			
			//	Avant de donner l'occasion au panel de se peindre, on v�rifie que le layout
			//	est bien � jour.
			
			this.Validate ();
		}
		
		
		
		protected bool					is_dirty_structure;
		protected bool					is_dirty_geometry;
		
		protected Panel					panel;
		protected Drawing.Size			panel_size;
	}
}
