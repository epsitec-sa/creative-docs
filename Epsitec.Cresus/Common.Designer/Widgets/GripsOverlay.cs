//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.Widgets
{
	using IWidgetCollectionHost = Epsitec.Common.Widgets.Helpers.IWidgetCollectionHost;
	using WidgetCollection      = Epsitec.Common.Widgets.Helpers.WidgetCollection;
	
	/// <summary>
	/// La classe GripsOverlay crée une surface qui a la même taille que
	/// la fenêtre sous-jacente; cette surface va abriter des poignées
	/// (widgets Grip) et un rectangle correspondant au Bounds du widget
	/// cible.
	/// </summary>
	
	[Support.SuppressBundleSupport]
	
	public class GripsOverlay : Widget, IWidgetCollectionHost
	{
		public GripsOverlay(Support.ICommandDispatcherHost host)
		{
			this.drop_behavior = new Behaviors.DropBehavior (this);
			this.selections    = new System.Collections.ArrayList ();
			this.Name          = string.Format ("GripsOverlay_{0}", GripsOverlay.overlay_id++);
			this.host          = host;
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
		
		
		public WidgetCollection					SelectedWidgets
		{
			get
			{
				if (this.selected_widgets == null)
				{
					this.selected_widgets = new WidgetCollection (this);
				}
				
				return this.selected_widgets;
			}
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
			Selection sel = new Selection (this, widget);
			
			widget.SetProperty (string.Concat (GripsOverlay.prop_selected_by, this.Name), sel);
			
			this.Parent = null;
			
			this.SetEmbedder (widget.Window.Root);
			this.Anchor        = AnchorStyles.All;
			this.AnchorMargins = new Drawing.Margins (0, 0, 0, 0);
			
			this.selections.Add (sel);
			this.UpdateGeometry ();
			
			if (this.selections.Count > 1)
			{
				foreach (Selection iter in this.selections)
				{
					iter.DisableSizing ();
				}
			}
		}
		
		protected virtual void DetachWidget(Widget widget)
		{
			for (int i = 0; i < this.selections.Count; i++)
			{
				Selection sel = this.selections[i] as Selection;
				
				if (sel.Widget == widget)
				{
					System.Diagnostics.Debug.Assert (widget.GetProperty (string.Concat (GripsOverlay.prop_selected_by, this.Name)) == sel);
					widget.ClearProperty (string.Concat (GripsOverlay.prop_selected_by, this.Name));
					this.selections.RemoveAt (i);
					sel.Dispose ();
					break;
				}
			}
			
			if (this.selections.Count == 1)
			{
				foreach (Selection iter in this.selections)
				{
					iter.EnableSizing ();
				}
			}
			
			if (this.selections.Count == 0)
			{
				this.Parent = null;
			}
		}
		
		
		protected virtual bool WidgetAlignFilter(Widget widget)
		{
			return widget.IsPropertyDefined (string.Concat (GripsOverlay.prop_selected_by, this.Name));
		}
		
		
		protected virtual  void UpdateGeometry()
		{
			if ((this.selections != null) &&
				(this.selections.Count > 0))
			{
				foreach (Selection sel in this.selections)
				{
					sel.UpdateGeometry ();
				}
			}
		}
		
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();
			this.UpdateGeometry ();
		}

		
		#region GripMap Structure
		protected struct GripMap
		{
			public GripMap(Drawing.GripId id, GripType type, double x, double y)
			{
				this.id     = id;
				this.type   = type;
				this.offset = new Drawing.Point (x, y);
			}
			
			
			public Drawing.GripId				id;
			public GripType						type;
			public Drawing.Point				offset;
		}
		#endregion
		
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
		
		public void NotifyPostRemoval(Widget widget)
		{
			if (this.DeselectedTarget != null)
			{
				this.DeselectedTarget (this, widget);
			}
		}
		
		public WidgetCollection GetWidgetCollection()
		{
			return this.SelectedWidgets;
		}
		#endregion
		
		#region Selection Class
		public class Selection : System.IDisposable
		{
			public Selection(GripsOverlay overlay, Widget widget)
			{
				this.overlay       = overlay;
				this.target_widget = widget;
				this.target_parent = widget.Parent;
				this.filter        = new Behaviors.SmartGuideBehavior.Filter (this.overlay.WidgetAlignFilter);
				
				this.CreateGrips ();
				
				this.target_widget.PreparePaint  += new Support.EventHandler (this.HandleTargetPreparePaint);
				this.target_widget.LayoutChanged += new Support.EventHandler (this.HandleTargetLayoutChanged);
			}
			
			
			public void Dispose()
			{
				this.DestroyGrips ();
				
				this.target_widget.LayoutChanged -= new Support.EventHandler (this.HandleTargetLayoutChanged);
				this.target_widget.PreparePaint  -= new Support.EventHandler (this.HandleTargetPreparePaint);
			}
			
			
			public void UpdateGeometry()
			{
				System.Diagnostics.Debug.Assert (this.target_widget != null);
				System.Diagnostics.Debug.Assert (this.target_parent != null);
				
				this.target_bounds = this.target_parent.MapClientToRoot (this.target_widget.Bounds);
				this.target_clip   = this.target_parent.MapClientToRoot (this.target_parent.GetClipStackBounds ());
				
				for (int i = 0; i < this.grips.Length; i++)
				{
					Grip    grip   = this.grips[i];
					GripMap map    = GripsOverlay.grip_map[grip.Index];
					
					grip.GripLocation = this.target_bounds.GetGrip (map.id) + map.offset;
				}
			}
			
			
			public void ShowGrips()
			{
				for (int i = 0; i < this.grips.Length; i++)
				{
					this.grips[i].SetVisible (true);
				}
			}
			
			public void HideGrips()
			{
				for (int i = 0; i < this.grips.Length; i++)
				{
					this.grips[i].SetVisible (false);
				}
			}
			
			public void EnableSizing()
			{
				for (int i = 0; i < this.grips.Length; i++)
				{
					if (GripsOverlay.grip_map[this.grips[i].Index].id != Drawing.GripId.Body)
					{
						this.grips[i].SetEnabled (true);
					}
				}
			}
			
			public void DisableSizing()
			{
				for (int i = 0; i < this.grips.Length; i++)
				{
					if (GripsOverlay.grip_map[this.grips[i].Index].id != Drawing.GripId.Body)
					{
						this.grips[i].SetEnabled (false);
					}
				}
			}
			
			
			private void CreateGrips()
			{
				int n = GripsOverlay.grip_map.Length;
				
				this.grips = new Grip[n];
			
				for (int i = 0; i < n; i++)
				{
					Grip grip = new Grip (this.overlay);
					
					grip.GripType   = GripsOverlay.grip_map[i].type;
					grip.Index      = i;
					grip.Dragging  += new DragEventHandler (this.HandleGripsDragging);
					grip.DragBegin += new Support.EventHandler (this.HandleGripsDragBegin);
					grip.DragEnd   += new Support.EventHandler (this.HandleGripsDragEnd);
					grip.Widget     = this.target_widget;
				
					this.grips[i] = grip;
				}
			}
			
			private void DestroyGrips()
			{
				int n = GripsOverlay.grip_map.Length;
				
				for (int i = 0; i < n; i++)
				{
					Grip grip = this.grips[i];
					
					System.Diagnostics.Debug.Assert (grip.Widget == this.target_widget);
					
					grip.Dragging  -= new DragEventHandler (this.HandleGripsDragging);
					grip.DragBegin -= new Support.EventHandler (this.HandleGripsDragBegin);
					grip.DragEnd   -= new Support.EventHandler (this.HandleGripsDragEnd);
					grip.Dispose ();
				}
			}
			
			
			private void HandleGripsDragging(object sender, DragEventArgs e)
			{
				System.Diagnostics.Debug.Assert (this.active_grip == sender);
				
				if (this.drag_drop_active)
				{
					Drawing.Point drag_cursor = this.overlay.Window.MapWindowToScreen (Message.State.LastPosition) - new Drawing.Point (1, 1);
					
					this.overlay.drop_behavior.DragWindowLocation = this.drag_window_origin + e.Offset;
					this.overlay.drop_behavior.ProcessDragging (drag_cursor);
					
					return;
				}
				
				Grip grip = sender as Grip;
				
				System.Diagnostics.Debug.Assert (grip != null);
				System.Diagnostics.Debug.Assert (grip.Index >= 0);
				System.Diagnostics.Debug.Assert (grip.Index < this.grips.Length);
				System.Diagnostics.Debug.Assert (this.grips[grip.Index] == grip);
				
				int     index = grip.Index;
				GripMap map   = GripsOverlay.grip_map[index];
				
				Drawing.Rectangle bounds  = this.target_bounds;
				Drawing.Point     old_pos = this.target_parent.MapRootToClient (bounds.GetGrip (map.id));
				
				bounds.OffsetGrip (map.id, e.Offset);
				bounds = this.target_parent.MapRootToClient (bounds);
				bounds = this.overlay.drop_behavior.ConstrainWidgetBoundsRelative (map.id, bounds, this.filter);
				bounds = this.overlay.drop_behavior.ConstrainWidgetBoundsMinMax (map.id, bounds);
				
				Drawing.Point     new_pos = bounds.GetGrip (map.id);
				
				this.target_widget.Bounds = bounds;
				this.overlay.HandleGripDragging (this, map.id, old_pos, new_pos);
			}
			
			private void HandleGripsDragBegin(object sender)
			{
				System.Diagnostics.Debug.Assert (this.active_grip == null);
				
				Grip grip = sender as Grip;
				
				System.Diagnostics.Debug.Assert (grip != null);
				System.Diagnostics.Debug.Assert (grip.Index >= 0);
				System.Diagnostics.Debug.Assert (grip.Index < this.grips.Length);
				System.Diagnostics.Debug.Assert (this.grips[grip.Index] == grip);
				
				//	Cache toutes les poignées, sauf celle qui est active :
				
				for (int i = 0; i < this.grips.Length; i++)
				{
					if (this.grips[i] != grip)
					{
						this.grips[i].SetVisible (false);
					}
				}
				
				this.active_grip = grip;
				this.overlay.HandleGripDragBegin (this);
				
				if ((GripsOverlay.grip_map[this.active_grip.Index].id == Drawing.GripId.Body) &&
					(this.overlay.SelectedWidgets.Count == 1))
				{
					Drawing.Rectangle bounds = this.target_widget.Bounds;
					
					this.target_z_order  = this.target_widget.ZOrder;
					this.target_z_parent = this.target_widget.Parent;
					
					this.target_widget.Dock   = DockStyle.None;
					this.target_widget.Anchor = AnchorStyles.None;
					this.target_widget.Bounds = bounds;
					
					this.overlay.drop_behavior.Widget               = this.target_widget;
					this.overlay.drop_behavior.DropTarget           = null;
					this.overlay.drop_behavior.DropTargetHiliteMode = WidgetHiliteMode.DropCandidate;
					
					this.overlay.drop_behavior.StartWidgetDragging ();
					
					this.drag_window_origin = this.overlay.drop_behavior.DragWindowLocation;
					this.drag_drop_active   = true;
					
					grip.Parent = this.overlay.drop_behavior.Widget.Parent;
				}
				else
				{
					this.overlay.drop_behavior.Widget               = this.target_widget;
					this.overlay.drop_behavior.DropTarget           = this.target_parent;
					this.overlay.drop_behavior.DropTargetHiliteMode = WidgetHiliteMode.None;
					
					this.drag_window_origin = Drawing.Point.Empty;
					this.drag_drop_active   = false;
				}
			}
			
			private void HandleGripsDragEnd(object sender)
			{
				System.Diagnostics.Debug.Assert (this.active_grip == sender);
				
				Grip grip = sender as Grip;
				
				if (this.drag_drop_active)
				{
					grip.Parent = this.overlay;
					
					if (this.overlay.drop_behavior.ValidateWidgetDragging ())
					{
						if (this.target_parent.Window != this.target_widget.Window)
						{
							//	Le widget a été déposé dans une autre fenêtre.
							
							this.target_parent = null;
							this.overlay.host.CommandDispatcher.Dispatch ("ReselectActiveSelection", this.overlay);
						}
						else
						{
							this.target_parent = this.target_widget.Parent;
						}
						
						if (this.target_parent == this.target_z_parent)
						{
							this.target_widget.ZOrder = this.target_z_order;
						}
					}
					else
					{
						//	Le widget a été déposé hors de la fenêtre et il faut donc le mettre à la
						//	poubelle...
						
						this.overlay.host.CommandDispatcher.Dispatch ("DeleteActiveSelection", this.overlay);
					}
					
					this.drag_drop_active = false;
				}
				
				System.Diagnostics.Debug.Assert (grip != null);
				System.Diagnostics.Debug.Assert (grip.Index >= 0);
				System.Diagnostics.Debug.Assert (grip.Index < this.grips.Length);
				System.Diagnostics.Debug.Assert (this.grips[grip.Index] == grip);
				
				//	Montre toutes les poignées, sauf celle qui était déjà visible :
				
				for (int i = 0; i < this.grips.Length; i++)
				{
					if (this.grips[i] != grip)
					{
						this.grips[i].SetVisible (true);
					}
				}
				
				this.overlay.HandleGripDragEnd (this);
				this.overlay.drop_behavior.DropTarget = null;
				this.active_grip = null;
			}
			
			private void HandleTargetLayoutChanged(object sender)
			{
				this.UpdateGeometry ();
			}
			
			private void HandleTargetPreparePaint(object sender)
			{
				this.UpdateGeometry ();
			}
			
			
			public Widget						Widget
			{
				get
				{
					return this.target_widget;
				}
			}
			
			
			GripsOverlay						overlay;
			Grip[]								grips;
			Grip								active_grip;
			Widget								target_widget;
			Widget								target_parent;
			Drawing.Rectangle					target_bounds;
			Drawing.Rectangle					target_clip;
			int									target_z_order;
			Widget								target_z_parent;
			Behaviors.SmartGuideBehavior.Filter	filter;
			
			Drawing.Point						drag_window_origin;
			bool								drag_drop_active;
		}
		#endregion
		
		protected void HandleGripDragBegin(Selection selection)
		{
			foreach (Selection sel in this.selections)
			{
				if (sel != selection)
				{
					sel.HideGrips ();
				}
			}
			
			if (this.DragBegin != null)
			{
				this.DragBegin (this);
			}
		}
		
		protected void HandleGripDragging(Selection selection, Drawing.GripId id, Drawing.Point p1, Drawing.Point p2)
		{
			Drawing.Point offset = p2 - p1;
			
			foreach (Selection sel in this.selections)
			{
				if (sel != selection)
				{
					Drawing.Rectangle bounds = sel.Widget.Bounds;
					bounds.OffsetGrip (id, offset);
					sel.Widget.Bounds = bounds;
				}
			}
			
			if (this.Dragging != null)
			{
				DragEventArgs e = new DragEventArgs (p1, p2);
				this.Dragging (this, e);
			}
		}
		
		protected void HandleGripDragEnd(Selection selection)
		{
			foreach (Selection sel in this.selections)
			{
				if (sel != selection)
				{
					sel.ShowGrips ();
				}
			}
			
			this.UpdateGeometry ();
			
			if (this.DragEnd != null)
			{
				this.DragEnd (this);
			}
		}
		
		
		public event SelectionEventHandler		SelectedTarget;
		public event SelectionEventHandler		DeselectingTarget;
		public event SelectionEventHandler		DeselectedTarget;
		
		public event Support.EventHandler		DragBegin;
		public event Support.EventHandler		DragEnd;
		public event DragEventHandler			Dragging;
		
		
		private Behaviors.DropBehavior			drop_behavior;
		
		private WidgetCollection				selected_widgets;
		private System.Collections.ArrayList	selections;
		
		private static long						overlay_id;
		
		private static GripMap[]				grip_map;
		private const string					prop_selected_by = "$grips overlay$selected by$";
		private Support.ICommandDispatcherHost	host;
	}
}
