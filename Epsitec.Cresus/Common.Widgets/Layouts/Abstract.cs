//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Common.Widgets.Layouts
{
	/// <summary>
	/// La classe Abstract implémente les méthodes de base de toute classe
	/// de gestion de layout.
	/// </summary>
	public abstract class Abstract
	{
		public Abstract()
		{
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
						this.DetachPanel ();
					}
					
					this.panel = value;
					
					if (this.panel != null)
					{
						this.AttachPanel ();
					}
					
					this.Invalidate ();
				}
			}
		}
		
		
		public double					DesiredWidth
		{
			get
			{
				return this.desired_width;
			}
			
			set
			{
				if (this.desired_width != value)
				{
					this.desired_width = value;
					this.UpdateGeometry ();
				}
			}
		}
		
		public double					DesiredHeight
		{
			get
			{
				return this.desired_height;
			}
			
			set
			{
				if (this.desired_height != value)
				{
					this.desired_height = value;
					this.UpdateGeometry ();
				}
			}
		}
		
		public double					CurrentWidth
		{
			get
			{
				this.Update ();
				return this.current_width;
			}
		}
		
		public double					CurrentHeight
		{
			get
			{
				this.Update ();
				return this.current_height;
			}
		}
		
		
		public bool						EditionEnabled
		{
			get
			{
				return this.is_edition_enabled;
			}
			
			set
			{
				if (this.is_edition_enabled != value)
				{
					if (this.panel == null)
					{
						this.is_edition_enabled = value;
					}
					else
					{
						this.panel.Invalidate ();
						this.is_edition_enabled = value;
						this.panel.Invalidate ();
					}
				}
			}
		}
		
		
		public void Update()
		{
			if (this.is_dirty)
			{
				this.Rebuild ();
			}
		}
		
		public void Invalidate()
		{
			if (this.is_dirty == false)
			{
				this.is_dirty = true;
			}
		}
		
		
		protected abstract void Rebuild();
		protected abstract void UpdateGeometry();
		
		protected void AttachPanel()
		{
			this.panel.LayoutChanged   += new EventHandler (this.HandlePanelLayoutChanged);
			this.panel.ChildrenChanged += new EventHandler (this.HandlePanelChildrenChanged);
			this.panel.PreparePaint    += new EventHandler (this.HandlePanelPreparePaint);
			
			this.panel.SetPropagationModes (Widget.PropagationModes.UpChildrenChanged, true, Widget.PropagationSetting.IncludeChildren);
		}
		
		protected void DetachPanel()
		{
			this.panel.LayoutChanged   -= new EventHandler (this.HandlePanelLayoutChanged);
			this.panel.ChildrenChanged -= new EventHandler (this.HandlePanelChildrenChanged);
			this.panel.PreparePaint    -= new EventHandler (this.HandlePanelPreparePaint);
		}
		
		
		
		#region Panel Event Handlers
		private void HandlePanelLayoutChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.panel == sender);
			
			if ((this.panel.Width  != this.current_width) ||
				(this.panel.Height != this.current_height))
			{
				this.UpdateGeometry ();
			}
		}
		
		private void HandlePanelChildrenChanged(object sender)
		{
			this.Invalidate ();
		}
		
		private void HandlePanelPreparePaint(object sender)
		{
			System.Diagnostics.Debug.Assert (this.panel == sender);
			this.Update ();
		}
		#endregion		
		
		
		
		protected Panel					panel;
		
		protected bool					is_dirty;					//	Invalidate a été appelé
		
		protected double				current_width;
		protected double				current_height;
		protected double				desired_width;
		protected double				desired_height;
		
		protected bool					is_edition_enabled;			//	l'édition est possible
	}
}
