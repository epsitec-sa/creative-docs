//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Common.Widgets.Design
{
	/// <summary>
	/// La classe GripsOverlay crée une surface qui a la même taille que
	/// la fenêtre sous-jacente; cette surface va abriter des poignées
	/// (widgets Grip) et un rectangle correspondant au Bounds du widget
	/// cible.
	/// </summary>
	public class GripsOverlay : Widget
	{
		public GripsOverlay()
		{
		}
		
		public GripsOverlay(Widget embedder) : this()
		{
			this.SetEmbedder (embedder);
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
		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.TargetWidget = null;
			}
			
			base.Dispose (disposing);
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
			
			if (this.grips != null)
			{
				for (int i = 0; i < this.grips.Length; i++)
				{
					this.grips[i].Dragging -= new DragEventHandler (this.HandleGripsDragging);
					this.grips[i].Dispose ();
					this.grips[i] = null;
				}
				
				this.grips = null;
			}
		}
		
		protected virtual void CreateGrips()
		{
			if (this.grips == null)
			{
				this.grips = new Grip[GripsOverlay.GripCount];
				
				for (int i = 0; i < this.grips.Length; i++)
				{
					this.grips[i] = new Grip (this);
					
					switch (i)
					{
						case GripsOverlay.GripBottomLeft:
						case GripsOverlay.GripBottomRight:
						case GripsOverlay.GripTopLeft:
						case GripsOverlay.GripTopRight:
							this.grips[i].GripType = GripType.Vertex;
							break;
						
						case GripsOverlay.GripCenter:
							this.grips[i].GripType = GripType.Center;
							break;
					}
					
					this.grips[i].Index = i;
					this.grips[i].Dragging += new DragEventHandler (this.HandleGripsDragging);
				}
			}
		}
		
		
		protected virtual void UpdateGeometry()
		{
			if ((this.target_widget != null) &&
				(this.target_widget.Parent != null))
			{
				if (this.grips == null)
				{
					this.CreateGrips ();
				}
				
				Widget parent = this.target_widget.Parent;
				
				this.TargetBounds = parent.MapClientToRoot (this.target_widget.Bounds);
				this.TargetClip   = parent.MapClientToRoot (parent.GetClipStackBounds ());
				
				this.grips[GripsOverlay.GripBottomLeft].GripLocation  = this.target_bounds.BottomLeft;
				this.grips[GripsOverlay.GripBottomRight].GripLocation = this.target_bounds.BottomRight - new Drawing.Point (1, 0);
				this.grips[GripsOverlay.GripTopRight].GripLocation    = this.target_bounds.TopRight    - new Drawing.Point (1, 1);
				this.grips[GripsOverlay.GripTopLeft].GripLocation     = this.target_bounds.TopLeft     - new Drawing.Point (0, 1);
				this.grips[GripsOverlay.GripCenter].GripLocation      = this.target_bounds.Center;
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
		
		private void HandleGripsDragging(object sender, DragEventArgs e)
		{
			Grip grip = sender as Grip;
			
			System.Diagnostics.Debug.Assert (grip != null);
			System.Diagnostics.Debug.Assert (this.grips[grip.Index] == grip);
			
			int index = grip.Index;
			
			Drawing.Rectangle bounds = this.target_bounds;
			
			if (index < GripsOverlay.GripCenter)
			{
				Drawing.VertexIndex vertex = (Drawing.VertexIndex) index;
				
				bounds.OffsetVertex (vertex, e.Offset);
				
				bounds = this.target_widget.MapRootToClient (bounds);
				bounds = this.target_widget.MapClientToParent (bounds);
				bounds = this.ConstrainWidgetBounds (vertex, bounds);
			}
			else if (index == GripsOverlay.GripCenter)
			{
				bounds.Offset (e.Offset);
			}
			
			this.target_widget.Bounds = bounds;
		}
		
		protected virtual Drawing.Rectangle ConstrainWidgetBounds(Drawing.VertexIndex vertex, Drawing.Rectangle new_bounds)
		{
			Drawing.Rectangle old_bounds = this.target_widget.Bounds;
			
			if (new_bounds.Width < this.target_widget.MinSize.Width)
			{
				switch (vertex)
				{
					case Drawing.VertexIndex.BottomLeft:
					case Drawing.VertexIndex.TopLeft:
						new_bounds.Left = old_bounds.Right - this.target_widget.MinSize.Width;
						break;
					case Drawing.VertexIndex.BottomRight:
					case Drawing.VertexIndex.TopRight:
						new_bounds.Right = old_bounds.Left + this.target_widget.MinSize.Width;
						break;
				}
			}
			if (new_bounds.Width > this.target_widget.MaxSize.Width)
			{
				switch (vertex)
				{
					case Drawing.VertexIndex.BottomLeft:
					case Drawing.VertexIndex.TopLeft:
						new_bounds.Left = old_bounds.Right - this.target_widget.MaxSize.Width;
						break;
					case Drawing.VertexIndex.BottomRight:
					case Drawing.VertexIndex.TopRight:
						new_bounds.Right = old_bounds.Left + this.target_widget.MaxSize.Width;
						break;
				}
			}
			
			if (new_bounds.Height < this.target_widget.MinSize.Height)
			{
				switch (vertex)
				{
					case Drawing.VertexIndex.BottomLeft:
					case Drawing.VertexIndex.BottomRight:
						new_bounds.Bottom = old_bounds.Top - this.target_widget.MinSize.Height;
						break;
					case Drawing.VertexIndex.TopLeft:
					case Drawing.VertexIndex.TopRight:
						new_bounds.Top = old_bounds.Bottom + this.target_widget.MinSize.Height;
						break;
				}
			}
			if (new_bounds.Height > this.target_widget.MaxSize.Height)
			{
				switch (vertex)
				{
					case Drawing.VertexIndex.BottomLeft:
					case Drawing.VertexIndex.BottomRight:
						new_bounds.Bottom = old_bounds.Top - this.target_widget.MaxSize.Height;
						break;
					case Drawing.VertexIndex.TopLeft:
					case Drawing.VertexIndex.TopRight:
						new_bounds.Top = old_bounds.Bottom + this.target_widget.MaxSize.Height;
						break;
				}
			}
			
			return new_bounds;
		}
		
		
		protected const int					GripBottomLeft	= (int) Drawing.VertexIndex.BottomLeft;
		protected const int					GripBottomRight	= (int) Drawing.VertexIndex.BottomRight;
		protected const int					GripTopRight	= (int) Drawing.VertexIndex.TopRight;
		protected const int					GripTopLeft		= (int) Drawing.VertexIndex.TopLeft;
		protected const int					GripCenter		= 4;
		protected const int					GripCount		= 5;
		
		protected Widget					target_widget;
		protected Drawing.Rectangle			target_bounds;
		protected Drawing.Rectangle			target_clip;
		
		protected Grip[]					grips;
	}
}
