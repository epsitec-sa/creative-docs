//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types;

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
			this.ProcessDragging (e.ToPoint - this.lastOffset);
		}

		void Epsitec.Common.Widgets.Behaviors.IDragBehaviorHost.OnDragEnd()
		{
		}

		#endregion

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
						(left.IsActualGeometryValid || !left.Visibility) &&
						(right.IsActualGeometryValid || !right.Visibility))
					{
						double dx = delta.X;
						
						double s1 = AbstractSplitter.GetWidth (left);
						double w1 = AbstractSplitter.GetUpdatedWidth (left, dx);

						if (w1 == 0)
						{
							if (left.Visibility)
							{
								left.Visibility = false;
							}
						}
						else
						{
							if (left.Visibility == false)
							{
								left.Visibility = true;
							}
						}

						dx = w1 - s1;
						
						double s2 = AbstractSplitter.GetWidth (right);
						double w2 = AbstractSplitter.GetUpdatedWidth (right, -dx);
						
						if (w2 == 0)
						{
							if (right.Visibility)
							{
								right.Visibility = false;
							}
						}
						else
						{
							if (right.Visibility == false)
							{
								right.Visibility = true;
							}
						}

						dx = s2 - w2;
						
						this.lastOffset.X += dx;

						if (left.Dock != DockStyle.Fill)
						{
							left.PreferredWidth = s1+dx;
						}
						if (right.Dock != DockStyle.Fill)
						{
							right.PreferredWidth = s2-dx;
						}
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

						double s1 = AbstractSplitter.GetHeight (bottom);
						double w1 = AbstractSplitter.GetUpdatedHeight (bottom, dy);

						if (w1 == 0)
						{
							if (bottom.Visibility)
							{
								bottom.Visibility = false;
							}
						}
						else
						{
							if (bottom.Visibility == false)
							{
								bottom.Visibility = true;
							}
						}

						dy = w1 - s1;

						double s2 = AbstractSplitter.GetHeight (top);
						double w2 = AbstractSplitter.GetUpdatedHeight (top, -dy);

						if (w2 == 0)
						{
							if (top.Visibility)
							{
								top.Visibility = false;
							}
						}
						else
						{
							if (top.Visibility == false)
							{
								top.Visibility = true;
							}
						}

						dy = s2 - w2;

						this.lastOffset.Y += dy;

						if (bottom.Dock != DockStyle.Fill)
						{
							bottom.PreferredHeight = s1+dy;
						}
						if (top.Dock != DockStyle.Fill)
						{
							top.PreferredHeight = s2-dy;
						}
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

		private static bool CheckCollapseWidth(Widget widget, double s1, double dx)
		{
			if (dx < 0)
			{
				s1 = System.Math.Max (s1, widget.MinWidth);
			}
			
			s1 += dx;
			
			if (widget.Dock == DockStyle.Fill)
			{
				return false;
			}

			if ((s1 <= 0) ||
				(s1 < widget.MinWidth-4))
			{
				//	TODO: ajouter test pour propriété "collapsable"
				
				return true;
			}
			else
			{
				return false;
			}
		}

		private static double GetWidth(Widget widget)
		{
			if ((widget.IsVisible) &&
				(widget.IsActualGeometryValid))
			{
				return widget.ActualWidth;
			}
			else
			{
				return 0;
			}
		}

		private static double GetHeight(Widget widget)
		{
			if ((widget.IsVisible) &&
				(widget.IsActualGeometryValid))
			{
				return widget.ActualHeight;
			}
			else
			{
				return 0;
			}
		}

		private static double GetUpdatedWidth(Widget widget, double dx)
		{
			if ((widget.IsVisible) &&
				(widget.IsActualGeometryValid))
			{
				double w;

				w = widget.ActualWidth + dx;
				w = System.Math.Min (widget.MaxWidth, w);

				if (w < 5)
				{
					w = 0;
				}
				else
				{
					w = System.Math.Max (widget.MinWidth, w);
				}

				return w;
			}
			else
			{
				if (dx > 5)
				{
					return widget.MinWidth;
				}
				else
				{
					return 0;
				}
			}
		}

		private static double GetUpdatedHeight(Widget widget, double dy)
		{
			if ((widget.IsVisible) &&
				(widget.IsActualGeometryValid))
			{
				double h;

				h = widget.ActualHeight + dy;
				h = System.Math.Min (widget.MaxHeight, h);

				if (h < 5)
				{
					h = 0;
				}
				else
				{
					h = System.Math.Max (widget.MinHeight, h);
				}

				return h;
			}
			else
			{
				if (dy > 5)
				{
					return widget.MinHeight;
				}
				else
				{
					return 0;
				}
			}
		}
		
		
		public static bool GetAutoCollapseEnable(DependencyObject o)
		{
			return (bool) o.GetValue (AbstractSplitter.AutoCollapseEnableProperty);
		}

		public static void SetAutoCollapseEnable(DependencyObject o, bool value)
		{
			o.SetValue (AbstractSplitter.AutoCollapseEnableProperty, value);
		}

		public static readonly DependencyProperty AutoCollapseEnableProperty = DependencyProperty.RegisterAttached ("AutoCollapseEnable", typeof (bool), typeof (AbstractSplitter), new DependencyPropertyMetadata (false));
		
		private Behaviors.DragBehavior dragBehavior;
		private Drawing.Point lastOffset;
	}
}
