//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	public abstract class AbstractSplitter : Widget, Behaviors.IDragBehaviorHost
	{
		protected AbstractSplitter()
		{
			this.AutoFocus = false;
			
			this.InternalState &= ~InternalState.PossibleContainer;
			this.InternalState &= ~InternalState.Engageable;
			this.InternalState &= ~InternalState.Focusable;

			this.MouseCursor = this.IsVertical ? MouseCursor.AsVSplit : MouseCursor.AsHSplit;
			this.dragBehavior = new Behaviors.DragBehavior (this, false, true);
		}

		public abstract bool IsVertical
		{
			get;
		}

		public bool IsHorizontal
		{
			get
			{
				return this.IsVertical == false;
			}
		}

		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			if (!this.dragBehavior.ProcessMessage (message, pos))
			{
				base.ProcessMessage (message, pos);
			}
		}

		private void ProcessDragging(Drawing.Point delta)
		{
			if (this.IsVertical)
			{
				delta.Y = 0;
			}
			else
			{
				delta.X = 0;
			}

			if (delta == Drawing.Point.Zero)
			{
				return;
			}

			Widget target = null;

			switch (this.Dock)
			{
				case DockStyle.Left:
				case DockStyle.Right:
					if (this.IsVertical)
					{
						target = this.FindNeighbor (this.Dock);
					}
					else
					{
						throw new System.InvalidOperationException (string.Format ("Vertical splitter with DockStyle.{0}", this.Dock));
					}
					break;

				case DockStyle.Top:
				case DockStyle.Bottom:
					if (this.IsHorizontal)
					{
						target = this.FindNeighbor (this.Dock);
					}
					else
					{
						throw new System.InvalidOperationException (string.Format ("Horizontal splitter with DockStyle.{0}", this.Dock));
					}
					break;

				default:
					throw new System.InvalidOperationException (string.Format ("Cannot drag splitter; DockStyle.{0}", this.Dock));
			}

			switch (this.Dock)
			{
				case DockStyle.Right:
					delta.X = -delta.X;
					break;
				
				case DockStyle.Top:
					delta.Y = -delta.Y;
					break;
			}

			System.Diagnostics.Debug.WriteLine (delta);
			
			if (target != null)
			{
				if (delta.X != 0)
				{
					target.PreferredWidth += delta.X;
				}
				if (delta.Y != 0)
				{
					target.PreferredHeight += delta.Y;
				}

				Layouts.LayoutContext.SyncArrange (this.Parent);
				this.Parent.Invalidate ();
			}
		}

		private Widget FindNeighbor(DockStyle dockStyle)
		{
			//	Trouve le voisin (suivant/précédent en fonction du docking) qui a
			//	le mode de docking spécifié.
			
			Widget parent = this.Parent;

			if (parent == null)
			{
				return null;
			}

			Widget found = null;
			
			switch (dockStyle)
			{
				case DockStyle.Left:
				case DockStyle.Top:
				case DockStyle.Right:
				case DockStyle.Bottom:
					foreach (Widget sibling in parent.Children)
					{
						if (sibling.Dock == dockStyle)
						{
							if (sibling == this)
							{
								break;
							}

							found = sibling;
						}
					}
					break;
			}

			return found;
		}

		private void ProcessEndDragging(Epsitec.Common.Drawing.Point pos)
		{
		}


		private Behaviors.DragBehavior dragBehavior;
		private Drawing.Point lastOrigin;
		private Drawing.Point lastOffset;

		#region IDragBehaviorHost Members

		Epsitec.Common.Drawing.Point Epsitec.Common.Widgets.Behaviors.IDragBehaviorHost.DragLocation
		{
			get
			{
				return this.lastOrigin;
			}
		}

		bool Epsitec.Common.Widgets.Behaviors.IDragBehaviorHost.OnDragBegin(Epsitec.Common.Drawing.Point cursor)
		{
			this.lastOrigin = cursor;
			this.lastOffset = Drawing.Point.Zero;
			return true;
		}

		void Epsitec.Common.Widgets.Behaviors.IDragBehaviorHost.OnDragging(DragEventArgs e)
		{
			this.ProcessDragging (e.Offset - this.lastOffset);
			this.lastOffset = e.Offset;
		}

		void Epsitec.Common.Widgets.Behaviors.IDragBehaviorHost.OnDragEnd()
		{
		}

		#endregion
	}
}
