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
		
		
		static GripsOverlay()
		{
			GripsOverlay.grip_map = new GripMap[9];
			
			GripsOverlay.grip_map[0] = new GripMap (Drawing.GripId.VertexBottomLeft, GripType.Vertex, 0, 0);
			GripsOverlay.grip_map[1] = new GripMap (Drawing.GripId.VertexBottomRight, GripType.Vertex, -1, 0);
			GripsOverlay.grip_map[2] = new GripMap (Drawing.GripId.VertexTopLeft, GripType.Vertex, 0, -1);
			GripsOverlay.grip_map[3] = new GripMap (Drawing.GripId.VertexTopRight, GripType.Vertex, -1, -1);
			GripsOverlay.grip_map[4] = new GripMap (Drawing.GripId.Body, GripType.Center, 0, 0);
			GripsOverlay.grip_map[5] = new GripMap (Drawing.GripId.EdgeBottom, GripType.Edge, 0, 0);
			GripsOverlay.grip_map[6] = new GripMap (Drawing.GripId.EdgeRight, GripType.Edge, -1, 0);
			GripsOverlay.grip_map[7] = new GripMap (Drawing.GripId.EdgeTop, GripType.Edge, 0, -1);
			GripsOverlay.grip_map[8] = new GripMap (Drawing.GripId.EdgeLeft, GripType.Edge, 0, 0);
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
				int n = GripsOverlay.grip_map.Length;
				this.grips = new Grip[n];
				
				for (int i = 0; i < n; i++)
				{
					this.grips[i] = new Grip (this);
					this.grips[i].GripType  = GripsOverlay.grip_map[i].type;
					this.grips[i].Index     = i;
					this.grips[i].Dragging += new DragEventHandler (this.HandleGripsDragging);
				}
			}
		}
		
		public virtual Grip FindGrip(Drawing.GripId id)
		{
			if (this.grips != null)
			{
				System.Diagnostics.Debug.Assert (this.grips.Length == GripsOverlay.grip_map.Length);
				
				for (int i = 0; i < GripsOverlay.grip_map.Length; i++)
				{
					if (GripsOverlay.grip_map[i].id == id)
					{
						return this.grips[i];
					}
				}
			}
			
			return null;
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
				
				for (int i = 0; i < this.grips.Length; i++)
				{
					GripMap map  = GripsOverlay.grip_map[i];
					Grip    grip = this.grips[i];
					
					grip.GripLocation = this.target_bounds.GetGrip (map.id) + map.offset;
				}
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
			
			int     index = grip.Index;
			GripMap map   = GripsOverlay.grip_map[index];
			
			Drawing.Rectangle bounds = this.target_bounds;
			
			bounds.OffsetGrip (map.id, e.Offset);
			bounds = this.target_widget.MapRootToClient (bounds);
			bounds = this.target_widget.MapClientToParent (bounds);
			bounds = this.ConstrainWidgetBounds (map.id, bounds);
			
			this.target_widget.Bounds = bounds;
		}
		
		private Drawing.Rectangle ConstrainWidgetBounds(Drawing.GripId grip, Drawing.Rectangle new_bounds)
		{
			return GripsOverlay.ConstrainWidgetBounds (grip, this.target_widget.Bounds, new_bounds, this.target_widget.MinSize, this.target_widget.MaxSize);
		}
		
		
		public static Drawing.Rectangle ConstrainWidgetBounds(Drawing.GripId grip, Drawing.Rectangle old_bounds, Drawing.Rectangle new_bounds, Drawing.Size min, Drawing.Size max)
		{
			if (new_bounds.Width < min.Width)
			{
				switch (grip)
				{
					case Drawing.GripId.EdgeLeft:
					case Drawing.GripId.VertexBottomLeft:
					case Drawing.GripId.VertexTopLeft:
						new_bounds.Left = old_bounds.Right - min.Width;
						break;
					case Drawing.GripId.EdgeRight:
					case Drawing.GripId.VertexBottomRight:
					case Drawing.GripId.VertexTopRight:
						new_bounds.Right = old_bounds.Left + min.Width;
						break;
				}
			}
			if (new_bounds.Width > max.Width)
			{
				switch (grip)
				{
					case Drawing.GripId.EdgeLeft:
					case Drawing.GripId.VertexBottomLeft:
					case Drawing.GripId.VertexTopLeft:
						new_bounds.Left = old_bounds.Right - max.Width;
						break;
					case Drawing.GripId.EdgeRight:
					case Drawing.GripId.VertexBottomRight:
					case Drawing.GripId.VertexTopRight:
						new_bounds.Right = old_bounds.Left + max.Width;
						break;
				}
			}
			
			if (new_bounds.Height < min.Height)
			{
				switch (grip)
				{
					case Drawing.GripId.EdgeBottom:
					case Drawing.GripId.VertexBottomLeft:
					case Drawing.GripId.VertexBottomRight:
						new_bounds.Bottom = old_bounds.Top - min.Height;
						break;
					case Drawing.GripId.EdgeTop:
					case Drawing.GripId.VertexTopLeft:
					case Drawing.GripId.VertexTopRight:
						new_bounds.Top = old_bounds.Bottom + min.Height;
						break;
				}
			}
			if (new_bounds.Height > max.Height)
			{
				switch (grip)
				{
					case Drawing.GripId.EdgeBottom:
					case Drawing.GripId.VertexBottomLeft:
					case Drawing.GripId.VertexBottomRight:
						new_bounds.Bottom = old_bounds.Top - max.Height;
						break;
					case Drawing.GripId.EdgeTop:
					case Drawing.GripId.VertexTopLeft:
					case Drawing.GripId.VertexTopRight:
						new_bounds.Top = old_bounds.Bottom + max.Height;
						break;
				}
			}
			
			return new_bounds;
		}
		
		
		protected struct GripMap
		{
			public GripMap(Drawing.GripId id, GripType type, double x, double y)
			{
				this.id     = id;
				this.type   = type;
				this.offset = new Drawing.Point (x, y);
			}
			
			public Drawing.GripId			id;
			public GripType					type;
			public Drawing.Point			offset;
		}
		
		
		protected static GripMap[]			grip_map;
		
		protected Widget					target_widget;
		protected Drawing.Rectangle			target_bounds;
		protected Drawing.Rectangle			target_clip;
		
		protected Grip[]					grips;
	}
}
