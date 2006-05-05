//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

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

			Widget left;
			Widget right;
			
			switch (this.Dock)
			{
				case DockStyle.Left:
				case DockStyle.Right:
					if ((this.IsVertical) &&
						(AbstractSplitter.GetLeftRightNeighbors (this, out left, out right)) &&
						(left != null) &&
						(right != null) &&
						(left.IsActualGeometryValid) &&
						(right.IsActualGeometryValid))
					{
						double dx = delta.X;

						double w1 = System.Math.Max (left.MinWidth, System.Math.Min (left.MaxWidth, left.ActualWidth + dx));
						double w2 = System.Math.Max (right.MinWidth, System.Math.Min (right.MaxWidth, right.ActualWidth - dx));

						if (dx != w1 - left.ActualWidth)
						{
							dx = w1 - left.ActualWidth;
						}
						else if (dx != right.ActualWidth - w2)
						{
							dx = right.ActualWidth - w2;
						}

						left.PreferredWidth = left.ActualWidth + dx;
						right.PreferredWidth = right.ActualWidth - dx;
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
					}
					else
					{
						throw new System.InvalidOperationException (string.Format ("Horizontal splitter with DockStyle.{0}", this.Dock));
					}
					break;

				default:
					throw new System.InvalidOperationException (string.Format ("Cannot drag splitter; DockStyle.{0}", this.Dock));
			}

			Layouts.LayoutContext.SyncArrange (this.Parent);
			this.Parent.Invalidate ();
		}

		private void ProcessEndDragging(Epsitec.Common.Drawing.Point pos)
		{
		}

		public static bool GetLeftRightNeighbors(Widget widget, out Widget left, out Widget right)
		{
			Widget parent = widget.Parent;

			if (parent != null)
			{
				Widget[] siblings = AbstractSplitter.GetLeftRightDockedSiblings (parent);

				for (int i = 0; i < siblings.Length; i++)
				{
					if (siblings[i] == widget)
					{
						left = (i > 0) ? siblings[i-1] : null;
						right = (i+1 < siblings.Length) ? siblings[i+1] : null;

						return true;
					}
				}
			}

			left = null;
			right = null;

			return false;
		}

		public static Widget[] GetLeftRightDockedSiblings(Widget parent)
		{
			List<Widget> leftWidgets = new List<Widget> ();
			List<Widget> rightWidgets = new List<Widget> ();
			List<Widget> fillWidgets = new List<Widget> ();

			foreach (Widget widget in parent.Children)
			{
				switch (widget.Dock)
				{
					case DockStyle.Left:
						leftWidgets.Add (widget);
						break;
					case DockStyle.Right:
						rightWidgets.Insert (0, widget);
						break;
					case DockStyle.Fill:
						fillWidgets.Add (widget);
						break;
				}
			}

			leftWidgets.AddRange (fillWidgets);
			leftWidgets.AddRange (rightWidgets);
			
			return leftWidgets.ToArray ();
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
