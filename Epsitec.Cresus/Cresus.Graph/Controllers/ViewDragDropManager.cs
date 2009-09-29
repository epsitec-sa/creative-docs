//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph;
using Epsitec.Common.Graph.Data;
using Epsitec.Common.Graph.Renderers;
using Epsitec.Common.Graph.Styles;
using Epsitec.Common.Graph.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Graph.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Controllers
{
	class ViewDragDropManager
	{
		public ViewDragDropManager(WorkspaceController workspace, GraphDataSeries item, MiniChartView view, Point mouse)
		{
			this.item = item;
			this.view = view;
			this.workspace = workspace;
			this.viewMouseCursor = this.view.MouseCursor;
			
			this.workspace.HideBalloonTip ();
			
			this.viewOrigin  = view.MapClientToScreen (new Point (0, 0));
			this.mouseOrigin = mouse;

			this.groupIndex = -1;
			this.outputIndex = -1;
			
			this.outputInsertionMark = new Separator ()
			{
				Parent = this.workspace.Outputs.Container,
				Visibility = false,
				Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Left,
				PreferredWidth = 2,
			};

			this.groupInsertionMark = new Separator ()
			{
				Parent = this.workspace.Groups.Container,
				Visibility = false,
				Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top,
				PreferredHeight = 2,
			};
		}

		
		public bool							LockX
		{
			get;
			set;
		}

		public bool							LockY
		{
			get;
			set;
		}

		
		public bool ProcessMouseMove(Point mouse, System.Action dragStartAction)
		{
			mouse = this.view.MapClientToScreen (mouse);
			
			if (this.dragWindow == null)
			{
				if (Point.Distance (mouse, this.mouseOrigin) > 2.0)
				{
					this.CreateDragWindow ();
				}

				dragStartAction ();
			}

			if (this.dragWindow != null)
			{
				double x = this.viewOrigin.X + (this.LockX ? 0 : mouse.X - this.mouseOrigin.X);
				double y = this.viewOrigin.Y + (this.LockY ? 0 : mouse.Y - this.mouseOrigin.Y);

				this.dragWindow.WindowLocation = new Point (x, y);
				
				if ((this.DetectOutput (mouse)) ||
					(this.DetectGroup (mouse)))
				{
					return true;
				}

				this.workspace.RefreshHints ();
			}
			
			return false;
		}

		public bool ProcessDragEnd()
		{
			bool ok = this.dragWindow != null;

			if (this.dragWindow != null)
			{
				this.dragWindow.Dispose ();
				this.dragWindow = null;
			}

			if (this.outputInsertionMark != null)
			{
				this.outputInsertionMark.Dispose ();
				this.outputInsertionMark = null;
			}

			if (this.groupInsertionMark != null)
			{
				this.groupInsertionMark.Dispose ();
				this.groupInsertionMark = null;
			}

			if (this.outputIndex >= 0)
			{
				if (!this.workspace.Document.SetOutputIndex (this.item, this.outputIndex))
				{
					this.workspace.AddOutputToDocument (this.item);
					this.workspace.Document.SetOutputIndex (this.item, this.outputIndex);
				}

				this.workspace.Refresh ();
			}
			else if (this.groupIndex >= 0)
			{
				var groups = this.workspace.Document.Groups;

				if ((this.groupIndex < groups.Count) &&
					(this.groupUpdate))
				{
					this.workspace.Groups.UpdateGroup (groups[this.groupIndex], Collection.Single (this.item));
				}
				else
				{
					this.workspace.Groups.CreateGroup (Collection.Single (this.item));
				}
				
				this.workspace.Refresh ();
			}

			this.view.Enable = true;
			this.view.MouseCursor = this.viewMouseCursor;
			this.view.ClearUserEventHandlers (Widget.EventNames.MouseMoveEvent);

			return ok;
		}

		
		private bool DetectOutput(Point screenMouse)
		{
			var container = this.workspace.Outputs.Container;
			var mouse     = container.MapScreenToClient (screenMouse);

			if (container.Client.Bounds.Contains (mouse))
			{
				//	If the target is empty, accept to drop independently of the position :

				if (this.workspace.Outputs.Count == 0)
				{
					this.SetOutputDropTarget (2, 0);
					return true;
				}

				//	Otherwise, find the best possible match, but discard dropping on the item
				//	itself, or between it and its immediate neighbours :

				var dist = this.workspace.Outputs.Select (x => new
				{
					Distance = mouse.X - x.ActualBounds.Center.X,
					View = x
				});

				System.Diagnostics.Debug.WriteLine (string.Join (", ", dist.Select (x => string.Format ("{0}:{1}", x.View.Index, x.Distance)).ToArray ()));

				var best = dist.OrderBy (x => System.Math.Abs (x.Distance)).FirstOrDefault ();

				if ((best != null) &&
					(best.View != this.view))
				{
					if ((best.Distance < 0) &&
						((best.View.Index != this.view.Index+1) || !this.workspace.Document.OutputSeries.Contains (this.item)))
					{
						this.SetOutputDropTarget (best.View.ActualBounds.Left - this.outputInsertionMark.PreferredWidth + 3, best.View.Index);
						return true;
					}
					if ((best.Distance >= 0) &&
					    ((best.View.Index != this.view.Index-1) || !this.workspace.Document.OutputSeries.Contains (this.item)))
					{
						this.SetOutputDropTarget (best.View.ActualBounds.Right - 3, best.View.Index+1);
						return true;
					}
				}
			}
			else if (container.Parent.Parent.Client.Bounds.Contains (container.Parent.MapScreenToParent (screenMouse)))
			{
				int n = this.workspace.Outputs.Count;

				this.SetOutputDropTarget (n > 0 ? this.workspace.Outputs.Last ().ActualBounds.Right - 3 : 2, n);
				
				return true;
			}
			
			this.outputInsertionMark.Visibility = false;
			this.outputIndex = -1;
			
			return false;
		}

		private bool DetectGroup(Point screenMouse)
		{
			var container = this.workspace.Groups.Container;
			var mouse     = container.MapScreenToClient (screenMouse);

			if (container.Client.Bounds.Contains (mouse))
			{
				int  index = 0;
				bool update = false;
				double y = 0;
				double height = 0;

				this.workspace.Groups.DetectGroup (mouse,
					(setIndex, setUpdate) =>
					{
						index = setIndex;
						update = setUpdate;
					},
					(setY, setHeight) =>
					{
						y = setY;
						height = setHeight;
					});

				this.SetGroupDropTarget (index, update, y, height);
				return true;
			}
			else
			{
				this.SetGroupDropTarget (-1, false, 0, 0);
				return false;
			}
		}

		
		private void SetOutputDropTarget(double x, int index)
		{
			this.outputInsertionMark.Margins = new Margins (x, 0, 0, 0);
			this.outputInsertionMark.Visibility = true;
			
			this.outputIndex = index;

			this.workspace.HideOutputItemsHint ();
		}

		private void SetGroupDropTarget(int index, bool update, double y, double dy)
		{
			this.groupInsertionMark.Margins = new Margins (0, 0, y, 0);
			this.groupInsertionMark.Visibility = dy > 0;
			this.groupInsertionMark.PreferredHeight = dy;
			this.groupInsertionMark.ZOrder = this.groupInsertionMark.Parent.Children.Count - 1;
			
			this.groupIndex = index;
			this.groupUpdate = update;

			this.workspace.HideGroupItemsHint ();
		}

		
		private void CreateDragWindow()
		{
			this.dragWindow = new DragWindow ()
			{
				Alpha = 0.6,
				WindowLocation = this.viewOrigin,
				Owner = this.view.Window,
			};

			this.dragWindow.DefineWidget (this.workspace.CreateView (this.item), this.view.PreferredSize, Margins.Zero);
			this.dragWindow.Show ();
		}

		
		private readonly GraphDataSeries	item;
		private readonly MiniChartView		view;
		private readonly WorkspaceController workspace;
		private readonly Point				viewOrigin;
		private readonly Point				mouseOrigin;
		private readonly MouseCursor		viewMouseCursor;
		
		private Separator					outputInsertionMark;
		private Separator					groupInsertionMark;
		private DragWindow					dragWindow;

		private int							outputIndex;
		private int							groupIndex;
		private bool						groupUpdate;
	}
}
