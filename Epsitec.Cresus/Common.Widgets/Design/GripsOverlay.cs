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
	public class GripsOverlay : Widget, Helpers.IWidgetCollectionHost
	{
		public GripsOverlay()
		{
			this.drop_adorner = new Design.HiliteAdorner ();
		}
		
		public GripsOverlay(Widget embedder) : this()
		{
			this.SetEmbedder (embedder);
		}
		
		
		static GripsOverlay()
		{
			GripsOverlay.grip_map = new GripMap[9];
			
			GripsOverlay.grip_map[0] = new GripMap (Drawing.GripId.VertexBottomLeft, GripType.Vertex, 0, 0);
			GripsOverlay.grip_map[1] = new GripMap (Drawing.GripId.VertexBottomRight, GripType.Vertex, 0, 0);
			GripsOverlay.grip_map[2] = new GripMap (Drawing.GripId.VertexTopLeft, GripType.Vertex, 0, 0);
			GripsOverlay.grip_map[3] = new GripMap (Drawing.GripId.VertexTopRight, GripType.Vertex, 0, 0);
			GripsOverlay.grip_map[4] = new GripMap (Drawing.GripId.Body, GripType.Center, 0, 0);
			GripsOverlay.grip_map[5] = new GripMap (Drawing.GripId.EdgeBottom, GripType.Edge, 0, 0);
			GripsOverlay.grip_map[6] = new GripMap (Drawing.GripId.EdgeRight, GripType.Edge, 0, 0);
			GripsOverlay.grip_map[7] = new GripMap (Drawing.GripId.EdgeTop, GripType.Edge, 0, 0);
			GripsOverlay.grip_map[8] = new GripMap (Drawing.GripId.EdgeLeft, GripType.Edge, 0, 0);
		}
		
		
		public Grip						ActiveGrip
		{
			get
			{
				return this.active_grip;
			}
		}
		
		public Helpers.WidgetCollection	SelectedWidgets
		{
			get
			{
				if (this.selected_widgets == null)
				{
					this.selected_widgets = new Helpers.WidgetCollection (this);
				}
				
				return this.selected_widgets;
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
				if (this.selected_widgets != null)
				{
					this.selected_widgets.Clear ();
					this.selected_widgets.Dispose ();
					this.selected_widgets = null;
				}
			}
			
			base.Dispose (disposing);
		}

		
		protected virtual void AttachWidget(Widget widget)
		{
			widget.LayoutChanged += new EventHandler (this.HandleTargetLayoutChanged);
			widget.PreparePaint  += new EventHandler (this.HandleTargetPreparePaint);
			
			//	TODO: vérifier que l'on ne change pas de racine...
			
			this.CreateGrips (widget);
			
			if (this.Parent == null)
			{
				this.Parent = widget.Window.Root;
				this.Bounds = this.Parent.Client.Bounds;
				this.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
			}
			else
			{
				System.Diagnostics.Debug.Assert (this.Parent == widget.Window.Root);
			}
			
			this.UpdateGeometry ();
		}
		
		protected virtual void DetachWidget(Widget widget)
		{
			widget.LayoutChanged -= new EventHandler (this.HandleTargetLayoutChanged);
			widget.PreparePaint  -= new EventHandler (this.HandleTargetPreparePaint);
			
			int n   = GripsOverlay.grip_map.Length;
			int len = this.grips.Length;
			int j   = 0;
			
			Grip[] grips = new Grip[len - n];
			
			for (int i = 0; i < this.grips.Length; i++)
			{
				Grip grip = this.grips[i];
				
				if (grip.Widget == widget)
				{
					grip.Dragging  -= new DragEventHandler (this.HandleGripsDragging);
					grip.DragBegin -= new EventHandler (this.HandleGripsDragBegin);
					grip.DragEnd   -= new EventHandler (this.HandleGripsDragEnd);
					grip.Dispose ();
				}
				else
				{
					grips[j++] = grip;
				}
			}
			
			System.Diagnostics.Debug.Assert (j == grips.Length);
			
			this.grips = grips;
			
			if (this.grips.Length == 0)
			{
				this.Parent = null;
			}
		}
		
		protected virtual void CreateGrips(Widget widget)
		{
			int n   = GripsOverlay.grip_map.Length;
			int len = this.grips == null ? 0 : this.grips.Length;
			
			Grip[] grips = new Grip[len+n];
			
			if (this.grips != null)
			{
				this.grips.CopyTo (grips, 0);
			}
			
			for (int i = 0; i < n; i++)
			{
				Grip grip = new Grip (this);
				
				grip.GripType   = GripsOverlay.grip_map[i].type;
				grip.Index      = i;
				grip.Dragging  += new DragEventHandler (this.HandleGripsDragging);
				grip.DragBegin += new EventHandler (this.HandleGripsDragBegin);
				grip.DragEnd   += new EventHandler (this.HandleGripsDragEnd);
				grip.Widget     = widget;
				
				grips[len+i] = grip;
			}
			
			this.grips = grips;
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
			if (this.SelectedWidgets.Count > 0)
			{
				//	TODO: gérer la possibilité d'avoir plusieurs rectangles
				
				Widget target_parent = this.SelectedWidgets[0].Parent;
				
				this.TargetBounds = target_parent.MapClientToRoot (this.SelectedWidgets[0].Bounds);
				this.TargetClip   = target_parent.MapClientToRoot (target_parent.GetClipStackBounds ());
				
				for (int i = 0; i < this.grips.Length; i++)
				{
					Grip    grip   = this.grips[i];
					GripMap map    = GripsOverlay.grip_map[grip.Index];
					Widget  widget = grip.Widget;
					Widget  parent = widget.Parent;
					
					Drawing.Rectangle bounds = parent.MapClientToRoot (widget.Bounds);
					
					grip.GripLocation = bounds.GetGrip (map.id) + map.offset;
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
#if false			
			graphics.SetClippingRectangle (this.target_clip);
			graphics.AddRectangle (Drawing.Rectangle.Deflate (this.target_bounds, 0.5, 0.5));
			graphics.RenderSolid (HiliteAdorner.FrameColor);
#endif
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
			System.Diagnostics.Debug.Assert (this.active_grip == sender);
			
			Grip grip = sender as Grip;
			
			System.Diagnostics.Debug.Assert (grip != null);
			System.Diagnostics.Debug.Assert (this.grips[grip.Index] == grip);
			
			int     index = grip.Index;
			GripMap map   = GripsOverlay.grip_map[index];
			
			Drawing.Rectangle bounds = this.target_bounds;
			
			bounds.OffsetGrip (map.id, e.Offset);
			bounds = this.SelectedWidgets[0].MapRootToClient (bounds);
			bounds = this.SelectedWidgets[0].MapClientToParent (bounds);
			bounds = this.ConstrainWidgetBoundsRelative (map.id, bounds);
			bounds = this.ConstrainWidgetBoundsMinMax (map.id, bounds);
			
			this.SelectedWidgets[0].Bounds = bounds;
			
			if (this.Dragging != null)
			{
				this.Dragging (this, e);
			}
		}
		
		private void HandleGripsDragBegin(object sender)
		{
			System.Diagnostics.Debug.Assert (this.active_grip == null);
			
			Grip grip = sender as Grip;
			
			System.Diagnostics.Debug.Assert (grip != null);
			System.Diagnostics.Debug.Assert (this.grips[grip.Index] == grip);
			
			for (int i = 0; i < this.grips.Length; i++)
			{
				if (this.grips[i] != grip)
				{
					this.grips[i].SetVisible (false);
				}
			}
			
			this.active_grip = grip;
			
			if (this.DragBegin != null)
			{
				this.DragBegin (this);
			}
		}
		
		private void HandleGripsDragEnd(object sender)
		{
			System.Diagnostics.Debug.Assert (this.active_grip == sender);
			
			Grip grip = sender as Grip;
			
			System.Diagnostics.Debug.Assert (grip != null);
			System.Diagnostics.Debug.Assert (this.grips[grip.Index] == grip);
			
			for (int i = 0; i < this.grips.Length; i++)
			{
				if (this.grips[i] != grip)
				{
					this.grips[i].SetVisible (true);
				}
			}
			
			if (this.drop_adorner.Widget != null)
			{
				this.drop_adorner.Widget.InternalUpdateGeometry ();
			}
			
			this.drop_adorner.Widget = null;
			this.drop_cx = null;
			this.drop_cy = null;
			
			if (this.DragEnd != null)
			{
				this.DragEnd (this);
			}
			
			this.active_grip = null;
		}
		
		
		private Drawing.Rectangle ConstrainWidgetBoundsRelative(Drawing.GripId grip, Drawing.Rectangle new_bounds)
		{
			Design.Constraint cx = new Design.Constraint (5);
			Design.Constraint cy = new Design.Constraint (5);
			
			Widget widget = this.SelectedWidgets[0];
			Widget parent = widget.Parent;
			
			this.drop_adorner.Widget = parent;
			this.drop_adorner.HiliteMode = HiliteMode.DropCandidate;
			this.drop_adorner.Path.Clear ();
			
			Design.SmartGuide guide  = new Design.SmartGuide (widget, grip, parent);
			Drawing.Rectangle bounds = new_bounds;
			Drawing.Point     bline  = widget.BaseLine;
			
			if ((grip == Drawing.GripId.Body) &&
				(! bline.IsEmpty))
			{
				guide.Constrain (bounds, bline.Y, cx, cy);
			}
			else
			{
				guide.Constrain (bounds, cx, cy);
			}
			
			if (cx.Segments.Length > 0)
			{
				for (int i = 0; i < cx.Segments.Length; i++)
				{
					this.drop_adorner.Path.MoveTo (cx.Segments[i].P1);
					this.drop_adorner.Path.LineTo (cx.Segments[i].P2);
				}
			}
			
			if (cy.Segments.Length > 0)
			{
				for (int i = 0; i < cy.Segments.Length; i++)
				{
					this.drop_adorner.Path.MoveTo (cy.Segments[i].P1);
					this.drop_adorner.Path.LineTo (cy.Segments[i].P2);
				}
			}
			
			if (! cx.Equals (this.drop_cx))
			{
				this.drop_cx = cx.Clone ();
				parent.Invalidate ();
			}
			if (! cy.Equals (this.drop_cy))
			{
				this.drop_cy = cy.Clone ();
				parent.Invalidate ();
			}
			
			if (cx.IsValid)
			{
				bounds.OffsetGrip (grip, new Drawing.Point (- cx.Distance, 0));
			}
			if (cy.IsValid)
			{
				bounds.OffsetGrip (grip, new Drawing.Point (0, - cy.Distance));
			}
			
			return bounds;
		}
		
		private Drawing.Rectangle ConstrainWidgetBoundsMinMax(Drawing.GripId grip, Drawing.Rectangle new_bounds)
		{
			return GripsOverlay.ConstrainWidgetBoundsMinMax (grip, this.SelectedWidgets[0].Bounds, new_bounds, this.SelectedWidgets[0].MinSize, this.SelectedWidgets[0].MaxSize);
		}
		
		
		public static Drawing.Rectangle ConstrainWidgetBoundsMinMax(Drawing.GripId grip, Drawing.Rectangle old_bounds, Drawing.Rectangle new_bounds, Drawing.Size min, Drawing.Size max)
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
		
		#region IWidgetCollectionHost Members
		public void NotifyInsertion(Widget widget)
		{
			//	Un widget a été ajouté à la liste des widgets pour lesquels on doit afficher
			//	des poignées (les widgets sélectionnés). Il faut donc attacher ce widget à
			//	notre surface de travail :
			
			this.AttachWidget (widget);
			this.Invalidate ();
			
			if (this.SelectedTarget != null)
			{
				this.SelectedTarget (this, widget);
			}
		}
		
		public void NotifyRemoval(Widget widget)
		{
			if (this.DeselectingTarget != null)
			{
				this.DeselectingTarget (this, widget);
			}
			
			//	Un widget va être retiré de la liste des widgets sélectionnés. Il faut donc
			//	s'en détacher :
			
			this.DetachWidget (widget);
			this.Invalidate ();
		}
		
		public Epsitec.Common.Widgets.Helpers.WidgetCollection GetWidgetCollection()
		{
			return this.SelectedWidgets;
		}
		#endregion
		
		public class Selection : System.IDisposable
		{
			public Selection(GripsOverlay overlay, Widget widget)
			{
				this.overlay       = overlay;
				this.target_widget = widget;
				this.target_parent = widget.Parent;
			}
			
			public void Dispose()
			{
				this.DestroyGrips ();
			}
			
			public void CreateGrips()
			{
				int n   = GripsOverlay.grip_map.Length;
				
				this.grips = new Grip[n];
			
				for (int i = 0; i < n; i++)
				{
					Grip grip = new Grip (this.overlay);
					
					grip.GripType   = GripsOverlay.grip_map[i].type;
					grip.Index      = i;
					grip.Dragging  += new DragEventHandler (this.HandleGripsDragging);
					grip.DragBegin += new EventHandler (this.HandleGripsDragBegin);
					grip.DragEnd   += new EventHandler (this.HandleGripsDragEnd);
					grip.Widget     = this.target_widget;
				
					this.grips[i] = grip;
				}
			}
			
			public void DestroyGrips()
			{
				int n   = GripsOverlay.grip_map.Length;
				
				for (int i = 0; i < n; i++)
				{
					Grip grip = this.grips[i];
					
					System.Diagnostics.Debug.Assert (grip.Widget == this.target_widget);
					
					grip.Dragging  -= new DragEventHandler (this.HandleGripsDragging);
					grip.DragBegin -= new EventHandler (this.HandleGripsDragBegin);
					grip.DragEnd   -= new EventHandler (this.HandleGripsDragEnd);
					grip.Dispose ();
				}
			}
			
			
			private void HandleGripsDragging(object sender, DragEventArgs e)
			{
				System.Diagnostics.Debug.Assert (this.active_grip == sender);
				
				Grip grip = sender as Grip;
				
				System.Diagnostics.Debug.Assert (grip != null);
				System.Diagnostics.Debug.Assert (this.grips[grip.Index] == grip);
				
				int     index = grip.Index;
				GripMap map   = GripsOverlay.grip_map[index];
				
				Drawing.Rectangle bounds = this.target_bounds;
				
				bounds.OffsetGrip (map.id, e.Offset);
				bounds = this.SelectedWidgets[0].MapRootToClient (bounds);
				bounds = this.SelectedWidgets[0].MapClientToParent (bounds);
				bounds = this.ConstrainWidgetBoundsRelative (map.id, bounds);
				bounds = this.ConstrainWidgetBoundsMinMax (map.id, bounds);
				
				this.SelectedWidgets[0].Bounds = bounds;
				
				if (this.Dragging != null)
				{
					this.Dragging (this, e);
				}
			}
			
			private void HandleGripsDragBegin(object sender)
			{
				System.Diagnostics.Debug.Assert (this.active_grip == null);
				
				Grip grip = sender as Grip;
				
				System.Diagnostics.Debug.Assert (grip != null);
				System.Diagnostics.Debug.Assert (this.grips[grip.Index] == grip);
				
				for (int i = 0; i < this.grips.Length; i++)
				{
					if (this.grips[i] != grip)
					{
						this.grips[i].SetVisible (false);
					}
				}
				
				this.active_grip = grip;
				
				if (this.DragBegin != null)
				{
					this.DragBegin (this);
				}
			}
			
			private void HandleGripsDragEnd(object sender)
			{
				System.Diagnostics.Debug.Assert (this.active_grip == sender);
				
				Grip grip = sender as Grip;
				
				System.Diagnostics.Debug.Assert (grip != null);
				System.Diagnostics.Debug.Assert (this.grips[grip.Index] == grip);
				
				for (int i = 0; i < this.grips.Length; i++)
				{
					if (this.grips[i] != grip)
					{
						this.grips[i].SetVisible (true);
					}
				}
				
				if (this.drop_adorner.Widget != null)
				{
					this.drop_adorner.Widget.InternalUpdateGeometry ();
				}
				
				this.drop_adorner.Widget = null;
				this.drop_cx = null;
				this.drop_cy = null;
				
				if (this.DragEnd != null)
				{
					this.DragEnd (this);
				}
				
				this.active_grip = null;
			}
			
			
			protected GripsOverlay			overlay;
			protected Grip[]				grips;
			protected Widget				target_widget;
			protected Widget				target_parent;
			protected Drawing.Rectangle		target_bounds;
			protected Drawing.Rectangle		target_clip;
		}
		
		
		public event SelectionEventHandler	SelectedTarget;
		public event SelectionEventHandler	DeselectingTarget;
		
		public event EventHandler			DragBegin;
		public event EventHandler			DragEnd;
		public event DragEventHandler		Dragging;
		
		protected static GripMap[]			grip_map;
		
		protected Drawing.Rectangle			target_bounds;
		protected Drawing.Rectangle			target_clip;
		
		protected Grip[]					grips;
		protected Grip						active_grip;
		
		protected Design.HiliteAdorner		drop_adorner;
		protected Design.Constraint			drop_cx;
		protected Design.Constraint			drop_cy;
		
		protected Helpers.WidgetCollection	selected_widgets;
	}
}
