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
			this.dragBehavior = new Behaviors.DragBehavior (this, false, false);
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

			Widget left, right;
			Widget bottom, top;
			
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

						if (left.Dock != DockStyle.Fill)
						{
							left.PreferredWidth = left.ActualWidth + dx;
						}
						if (right.Dock != DockStyle.Fill)
						{
							right.PreferredWidth = right.ActualWidth - dx;
						}

						this.lastOffset.X += dx;
					}
					else
					{
						throw new System.InvalidOperationException (string.Format ("Invalid splitter with DockStyle.{0}", this.Dock));
					}
					break;

				case DockStyle.Top:
				case DockStyle.Bottom:
					if ((this.IsHorizontal) &&
						(AbstractSplitter.GetBottomTopNeighbors (this, out bottom, out top)) &&
						(bottom != null) &&
						(top != null) &&
						(bottom.IsActualGeometryValid) &&
						(top.IsActualGeometryValid))
					{
						double dy = delta.Y;

						double h1 = System.Math.Max (bottom.MinHeight, System.Math.Min (bottom.MaxHeight, bottom.ActualHeight + dy));
						double h2 = System.Math.Max (top.MinHeight, System.Math.Min (top.MaxHeight, top.ActualHeight - dy));

						if (dy != h1 - bottom.ActualHeight)
						{
							dy = h1 - bottom.ActualHeight;
						}
						else if (dy != top.ActualHeight - h2)
						{
							dy = top.ActualHeight - h2;
						}

						if (bottom.Dock != DockStyle.Fill)
						{
							bottom.PreferredHeight = bottom.ActualHeight + dy;
						}
						if (top.Dock != DockStyle.Fill)
						{
							top.PreferredHeight = top.ActualHeight - dy;
						}

						this.lastOffset.Y += dy;
					}
					else
					{
						throw new System.InvalidOperationException (string.Format ("Invalid splitter with DockStyle.{0}", this.Dock));
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

						if (((left == null) || (left.Dock != DockStyle.Fill)) &&
							((right == null) || (right.Dock != DockStyle.Fill)))
						{
							//	Aucun des voisins directs n'utilise le mode Fill. S'il y a un
							//	widget avec Fill quelque part, on va lui donner la préférence
							//	pour répartir l'espace :

							for (int j = 0; j < siblings.Length; j++)
							{
								if (siblings[j].Dock == DockStyle.Fill)
								{
									if (j < i)
									{
										left = siblings[j];
									}
									else
									{
										right = siblings[j];
									}

									break;
								}
							}
						}

						return true;
					}
				}
			}

			left = null;
			right = null;

			return false;
		}

		public static bool GetBottomTopNeighbors(Widget widget, out Widget bottom, out Widget top)
		{
			Widget parent = widget.Parent;

			if (parent != null)
			{
				Widget[] siblings = AbstractSplitter.GetBottomTopDockedSiblings (parent);

				for (int i = 0; i < siblings.Length; i++)
				{
					if (siblings[i] == widget)
					{
						bottom = (i > 0) ? siblings[i-1] : null;
						top = (i+1 < siblings.Length) ? siblings[i+1] : null;

						if (((bottom == null) || (bottom.Dock != DockStyle.Fill)) &&
							((top == null) || (top.Dock != DockStyle.Fill)))
						{
							//	Aucun des voisins directs n'utilise le mode Fill. S'il y a un
							//	widget avec Fill quelque part, on va lui donner la préférence
							//	pour répartir l'espace :

							for (int j = 0; j < siblings.Length; j++)
							{
								if (siblings[j].Dock == DockStyle.Fill)
								{
									if (j < i)
									{
										bottom = siblings[j];
									}
									else
									{
										top = siblings[j];
									}

									break;
								}
							}
						}

						return true;
					}
				}
			}

			bottom = null;
			top = null;

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

		public static Widget[] GetBottomTopDockedSiblings(Widget parent)
		{
			List<Widget> bottomWidgets = new List<Widget> ();
			List<Widget> topWidgets = new List<Widget> ();
			List<Widget> fillWidgets = new List<Widget> ();

			foreach (Widget widget in parent.Children)
			{
				switch (widget.Dock)
				{
					case DockStyle.Bottom:
						bottomWidgets.Add (widget);
						break;
					case DockStyle.Top:
						topWidgets.Insert (0, widget);
						break;
					case DockStyle.Fill:
						fillWidgets.Add (widget);
						break;
				}
			}

			bottomWidgets.AddRange (fillWidgets);
			bottomWidgets.AddRange (topWidgets);

			return bottomWidgets.ToArray ();
		}

		private Behaviors.DragBehavior dragBehavior;
		private Drawing.Point lastOffset;

		#region IDragBehaviorHost Members

		Epsitec.Common.Drawing.Point Epsitec.Common.Widgets.Behaviors.IDragBehaviorHost.DragLocation
		{
			get
			{
				return Drawing.Point.Zero;
			}
		}

		bool Epsitec.Common.Widgets.Behaviors.IDragBehaviorHost.OnDragBegin(Epsitec.Common.Drawing.Point cursor)
		{
			this.lastOffset = Drawing.Point.Zero;
			return true;
		}

		void Epsitec.Common.Widgets.Behaviors.IDragBehaviorHost.OnDragging(DragEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("From:" + e.FromPoint + " To: " + e.ToPoint + " Offset: " + e.Offset + " Last: " + this.lastOffset);
			this.ProcessDragging (e.ToPoint - this.lastOffset);
		}

		void Epsitec.Common.Widgets.Behaviors.IDragBehaviorHost.OnDragEnd()
		{
		}

		#endregion
	}
}
