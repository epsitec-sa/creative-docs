//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Behaviors
{
	/// <summary>
	/// Summary description for DropBehavior.
	/// </summary>
	public class DropBehavior
	{
		public DropBehavior(object host)
		{
			this.host                  = host;
			this.drop_adorner          = new HiliteWidgetAdorner ();
			this.drag_window_margins   = new Drawing.Margins (3, 3, 3, 3);
			this.widget_default_bounds = Drawing.Rectangle.Empty;
		}
		
		
		public Widget							Widget
		{
			get
			{
				return this.widget;
			}
			set
			{
				if (this.widget != value)
				{
					this.widget = value;
				}
			}
		}
		
		public Drawing.Rectangle				WidgetScreenBounds
		{
			get
			{
				if (this.widget == null)
				{
					return Drawing.Rectangle.Empty;
				}
				
				Drawing.Point pos;
				
				if (this.drag_window == null)
				{
					pos = this.widget.MapClientToScreen (new Drawing.Point (0, 0));
				}
				else
				{
					pos = this.drag_window.WindowLocation;
					
					pos.X += this.drag_window_margins.Left;
					pos.Y += this.drag_window_margins.Bottom;
				}
				
				return new Drawing.Rectangle (pos, this.widget.Size);
			}
		}
		
		public double							WidgetBaseLineOffset
		{
			get
			{
				Drawing.Point base_line = this.widget.BaseLine;
				
				if (base_line.IsEmpty)
				{
					return -1;
				}
				
				return base_line.Y;
			}
		}
		
		public Drawing.Rectangle				WidgetDefaultBounds
		{
			get
			{
				return this.widget_default_bounds;
			}
			set
			{
				this.widget_default_bounds = value;
			}
		}
		
		public Widget							WidgetDefaultTarget
		{
			get
			{
				return this.widget_default_target;
			}
			set
			{
				this.widget_default_target = value;
			}
		}
		
		
		public Drawing.Margins					Margins
		{
			get
			{
				return this.drag_window_margins;
			}
			set
			{
				this.drag_window_margins = value;
			}
		}
		
		
		public Widget							DropTarget
		{
			get
			{
				return this.drop_adorner.Widget;
			}
			set
			{
				if (this.drop_adorner.Widget != value)
				{
					this.drop_adorner.Widget     = value;
					this.drop_adorner.HiliteMode = this.DropTargetHiliteMode;
				}
				
				this.drop_adorner.Path.Clear ();
				this.drop_cx = null;
				this.drop_cy = null;
			}
		}
		
		public WidgetHiliteMode					DropTargetHiliteMode
		{
			get
			{
				return this.drop_target_hilite_mode;
			}
			set
			{
				if (this.drop_target_hilite_mode != value)
				{
					this.drop_target_hilite_mode = value;
					this.drop_adorner.HiliteMode = value;
				}
			}
		}
		
		public Drawing.Rectangle				DropTargetRelativeWidgetBounds
		{
			get
			{
				Drawing.Rectangle bounds = this.WidgetScreenBounds;
				Widget drop_target = this.DropTarget;
				
				if ((drop_target == null) ||
					(bounds.IsEmpty))
				{
					return Drawing.Rectangle.Empty;
				}
				
				return drop_target.MapScreenToClient (bounds);
			}
		}
		
		public Drawing.Point					DragWindowLocation
		{
			get
			{
				Drawing.Point offset = new Drawing.Point (this.drag_window_margins.Left, this.drag_window_margins.Bottom);
				return this.drag_window.WindowLocation + offset;
			}
			set
			{
				Drawing.Point offset = new Drawing.Point (this.drag_window_margins.Left, this.drag_window_margins.Bottom);
				this.drag_window.WindowLocation = value - offset;
			}
		}
		
		
		public void StartWidgetDragging(Drawing.Size initial_size, Drawing.Rectangle initial_bounds, Widget initial_target)
		{
			System.Diagnostics.Debug.Assert (this.widget != null);
			System.Diagnostics.Debug.Assert (this.drag_window == null);
			
			this.widget_default_bounds = initial_bounds;
			this.widget_default_target = initial_target;
			
			Drawing.Point offset = new Drawing.Point (this.widget.Width - initial_size.Width, this.widget.Height - initial_size.Height);
			Drawing.Point pos    = this.WidgetScreenBounds.Location;
			
			this.drag_window = new DragWindow ();
			this.drag_window.DefineWidget (this.widget, initial_size, this.drag_window_margins);
			this.DragWindowLocation = pos + (offset / 2);
			this.drag_window.Show ();
		}
		
		public void CancelWidgetDragging()
		{
			this.widget_default_bounds = Drawing.Rectangle.Empty;
			this.widget_default_target = null;
			
			if (this.widget == null)
			{
				return;
			}
			
			System.Diagnostics.Debug.Assert (this.drag_window != null);
			
			this.StopWidgetDragging ();
		}
		
		public bool ValidateWidgetDragging()
		{
			this.widget_default_bounds = Drawing.Rectangle.Empty;
			this.widget_default_target = null;
			
			Widget drop_target = this.DropTarget;
			
			if (drop_target == null)
			{
				//	Il n'y a pas de cible où déposer le widget... C'est donc comme si on avait annulé
				//	l'opération.
				
				this.CancelWidgetDragging ();
				return false;
			}
			
			if (this.widget == null)
			{
				return false;
			}
			
			System.Diagnostics.Debug.Assert (this.drag_window != null);
			
			Drawing.Point offset = new Drawing.Point (0, 0);
			AnchorStyles  anchor = AnchorStyles.None;
			
			//	Met à jour la position d'insertion en fonction des contraintes actuellement
			//	actives; calcule aussi la manière d'ancrer le widget par rapport à son parent :
			
			if (this.drop_cx.IsValid)
			{
				offset.X -= this.drop_cx.Distance;
				anchor   |= Behaviors.ConstraintBehavior.Hint.MergedAnchor (this.drop_cx.Hints);
			}
				
			if (this.drop_cy.IsValid)
			{
				offset.Y -= this.drop_cy.Distance;
				anchor   |= Behaviors.ConstraintBehavior.Hint.MergedAnchor (this.drop_cy.Hints);
			}
				
			if ((anchor & AnchorStyles.LeftAndRight) == 0)
			{
				anchor |= AnchorStyles.Left;
			}
				
			if ((anchor & AnchorStyles.TopAndBottom) == 0)
			{
				anchor |= AnchorStyles.Top;
			}
			
			
			Drawing.Point drop_location = this.DropTargetRelativeWidgetBounds.Location + offset;
			Drawing.Size  drop_size     = this.widget.Size;
			
			this.widget.Parent = null;
			
			this.widget.Anchor   = AnchorStyles.None;
			this.widget.Dock     = DockStyle.None;
			this.widget.Location = drop_location;
			this.widget.Size     = drop_size;
			
			Drawing.Rectangle drop_bounds = this.widget.Bounds;
			Drawing.Rectangle host_bounds = drop_target.Client.Bounds;
			
			this.widget.AnchorMargins = new Drawing.Margins (drop_bounds.Left - host_bounds.Left,
				/**/                                         host_bounds.Right - drop_bounds.Right,
				/**/                                         host_bounds.Top - drop_bounds.Top,
				/**/                                         drop_bounds.Bottom - host_bounds.Bottom);
			
			this.widget.Anchor = anchor;
			this.widget.Parent = drop_target;
			
			this.StopWidgetDragging ();
			
			return true;
		}
		
		
		public void ProcessDragging(Drawing.Point drag_cursor)
		{
			Window drop_window = DropBehavior.FindTargetWindow (drag_cursor);
			Widget drop_target = DropBehavior.FindTargetWidget (drop_window, drag_cursor);
			
			//	Définit la cible où le "drop" aurait lieu si l'utilisateur relâchait le bouton
			//	de la souris maintenant.
			
			this.DropTarget = drop_target;
			
			if (drop_target != null)
			{
				//	Détermine les contraintes selon [x] et selon [y] en fonction du widget courant et de sa position
				//	relative à la cible. Pour l'alignement selon [y], on utilise aussi la ligne de base.
				
				Behaviors.ConstraintBehavior cx = new Behaviors.ConstraintBehavior (5);
				Behaviors.ConstraintBehavior cy = new Behaviors.ConstraintBehavior (5);
				
				Behaviors.SmartGuideBehavior guide  = new Behaviors.SmartGuideBehavior (this.widget, Drawing.GripId.Body, drop_target);
				Drawing.Rectangle            bounds = this.DropTargetRelativeWidgetBounds;
				
				guide.DefaultBounds = this.widget_default_bounds;
				guide.DefaultTarget = this.widget_default_target;
				
				guide.Constrain (bounds, this.WidgetBaseLineOffset, cx, cy);
				
				//	Prend note des marques verticales (cx) et horizontales (cy) définissant visuellement les
				//	constraintes actives :
				
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
				
				//	Si les contraintes ont changé depuis la dernière fois, on met à jour nos copies locales.
				//	Cela permet d'éviter les repeintures inutiles lorsque aucune contrainte n'a bougé.
				
				if (! cx.Equals (this.drop_cx))
				{
					this.drop_cx = cx.Clone ();
					this.DropTarget.Invalidate ();
				}
				if (! cy.Equals (this.drop_cy))
				{
					this.drop_cy = cy.Clone ();
					this.DropTarget.Invalidate ();
				}
			}
		}
		
		
		public Drawing.Rectangle ConstrainWidgetBoundsRelative(Drawing.GripId grip, Drawing.Rectangle new_bounds, Behaviors.SmartGuideBehavior.Filter filter)
		{
			Widget widget = this.Widget;
			Widget parent = this.DropTarget;
			
			this.drop_adorner.Path.Clear ();
			
			Behaviors.ConstraintBehavior cx = new Behaviors.ConstraintBehavior (5);
			Behaviors.ConstraintBehavior cy = new Behaviors.ConstraintBehavior (5);
			
			Behaviors.SmartGuideBehavior guide  = new Behaviors.SmartGuideBehavior (widget, grip, parent, filter);
			Drawing.Rectangle            bounds = new_bounds;
			Drawing.Point                bline  = widget.BaseLine;
			
			guide.DefaultBounds = this.widget_default_bounds;
			guide.DefaultTarget = this.widget_default_target;
			
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
		
		public Drawing.Rectangle ConstrainWidgetBoundsMinMax(Drawing.GripId grip, Drawing.Rectangle new_bounds)
		{
			Widget widget = this.Widget;
			
			Drawing.Rectangle old_bounds = widget.Bounds;
			Drawing.Size      min_size   = widget.MinSize;
			Drawing.Size      max_size   = widget.MaxSize;
			
			return Widgets.GripsOverlay.ConstrainWidgetBoundsMinMax (grip, old_bounds, new_bounds, min_size, max_size);
		}
		
		
		public static Widget CloneWidget(Widget widget)
		{
			Support.ObjectBundler bundler = new Support.ObjectBundler ();
			widget = bundler.CopyObject (widget) as Widget;
			return widget;
		}
		
		public static Window FindTargetWindow(Drawing.Point pos)
		{
			Window[] windows = Epsitec.Common.Widgets.Platform.WindowList.GetVisibleWindows ();
			
			foreach (Window window in windows)
			{
				if (window is DragWindow)
				{
					continue;
				}
				
				if (window.WindowBounds.Contains (pos))
				{
					return window;
				}
			}
			
			return null;
		}
		
		public static Widget FindTargetWidget(Window window, Drawing.Point pos)
		{
			if (window == null)
			{
				return null;
			}
			
			pos = window.Root.MapScreenToClient (pos);
			
			Widget hit    = null;
			Widget widget = window.Root;
			Widget temp   = window.Root;
			
			do
			{
				widget = temp;
				pos    = widget.MapParentToClient (pos);
				temp   = widget.FindChild (pos, Widget.ChildFindMode.SkipDisabled | Widget.ChildFindMode.SkipHidden | Widget.ChildFindMode.SkipNonContainer);
				
				if ((widget.PossibleContainer) &&
					(widget.IsEditionEnabled))
				{
					hit = widget;
				}
			}
			while (temp != null);
			
			return hit;
		}
		
		
		protected void StopWidgetDragging()
		{
			if (this.drag_window != null)
			{
				this.drag_window.AnimateHide (Animation.FadeOut);
				this.drag_window.WindowAnimationEnded += new Epsitec.Common.Support.EventHandler(this.HandleDragWindowAnimationEnded);
				this.drag_window = null;
			}
			
			this.DropTarget = null;
		}
		
		
		private void HandleDragWindowAnimationEnded(object sender)
		{
			DragWindow drag_window = sender as DragWindow;
			
			drag_window.Hide ();
			drag_window.Dispose ();
		}
		
		
		private object							host;
		private Widget							widget;
		private Drawing.Rectangle				widget_default_bounds;
		private Widget							widget_default_target;
		private Drawing.Margins					drag_window_margins;
		private DragWindow						drag_window;
		
		private HiliteWidgetAdorner				drop_adorner;
		private Behaviors.ConstraintBehavior	drop_cx;
		private Behaviors.ConstraintBehavior	drop_cy;
		private WidgetHiliteMode				drop_target_hilite_mode;
	}
}
