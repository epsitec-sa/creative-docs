//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Common.Widgets.Design
{
	/// <summary>
	/// La classe GripsOverlay...
	/// </summary>
	public class GripsOverlay : Widget
	{
		public GripsOverlay()
		{
		}
		
		public Widget					TargetWidget
		{
			get
			{
				return this.target_widget;
			}
			
			set
			{
				if (this.target_widget != value)
				{
					if (this.target_widget != null)
					{
						this.DetachWidget ();
					}
					
					this.target_widget = value;
					
					if (this.target_widget != null)
					{
						this.AttachWidget ();
					}
					
					this.Invalidate ();
				}
			}
		}
		
		protected Drawing.Rectangle		TargetBounds
		{
			set
			{
				if (this.target_bounds != value)
				{
					this.target_bounds = value;
					this.Invalidate ();
				}
			}
		}
		
		protected Drawing.Rectangle		TargetClip
		{
			set
			{
				if (this.target_clip != value)
				{
					this.target_clip = value;
					this.Invalidate ();
				}
			}
		}
		
		
		
		protected virtual void AttachWidget()
		{
			this.target_widget.LayoutChanged += new EventHandler (this.HandleTargetLayoutChanged);
			this.target_widget.PreparePaint  += new EventHandler (this.HandleTargetPreparePaint);
			
			this.Parent = this.target_widget.Window.Root;
			this.Bounds = this.target_widget.Window.Root.Client.Bounds;
			this.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
			
			this.UpdateGeometry ();
		}
		
		protected virtual void DetachWidget()
		{
			this.target_widget.LayoutChanged -= new EventHandler (this.HandleTargetLayoutChanged);
			this.target_widget.PreparePaint  -= new EventHandler (this.HandleTargetPreparePaint);
			
			this.Parent = null;
		}
		
		protected void UpdateGeometry()
		{
			if (this.target_widget != null)
			{
				Widget parent = this.target_widget.Parent;
				
				this.TargetBounds = this.target_widget.MapClientToRoot (this.target_widget.Client.Bounds);
				this.TargetClip   = parent.MapClientToRoot (parent.GetClipStackBounds ());
			}
		}
		
		
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();
			this.UpdateGeometry ();
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
		{
			base.PaintBackgroundImplementation (graphics, clip_rect);
			
			graphics.SetClippingRectangle (this.target_clip);
			graphics.AddRectangle (Drawing.Rectangle.Inflate (this.target_bounds, -0.5, -0.5));
			graphics.RenderSolid (Drawing.Color.FromRGB (1.0, 0.0, 0.0));
//			graphics.AddFilledRectangle (this.target_clip);
//			graphics.RenderSolid (Drawing.Color.FromARGB (0.3, 1.0, 1.0, 0.0));
		}
		
		private void HandleTargetLayoutChanged(object sender)
		{
			this.UpdateGeometry ();
		}
		
		private void HandleTargetPreparePaint(object sender)
		{
			this.UpdateGeometry ();
		}
		
		
		protected Widget					target_widget;
		protected Drawing.Rectangle			target_bounds;
		protected Drawing.Rectangle			target_clip;
	}
}
