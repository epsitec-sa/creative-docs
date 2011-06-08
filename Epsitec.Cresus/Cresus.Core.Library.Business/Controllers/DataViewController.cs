//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Layouts;

using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Orchestrators.Navigation;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>DataViewController</c> class manages several <see cref="CoreViewController"/>
	/// instances which have a parent/child relationship. The <see cref="ViewLayoutController"/>
	/// is used for the layout.
	/// </summary>
	public sealed class DataViewController : ViewControllerComponent<DataViewController>, IWidgetUpdater
	{
		private DataViewController(DataViewOrchestrator orchestrator)
			: base (orchestrator)
		{
			this.viewControllers = new Stack<CoreViewController> ();
			this.pushSafeRecursion = new SafeCounter ();
			this.frame = new FrameBox ();
			this.viewLayoutController = new ViewLayoutController (this.Name + ".ViewLayout", this.frame);
		}



		public bool IsEmpty
		{
			get
			{
				return this.viewLayoutController.ColumnCount == 0;
			}
		}

		
		public override IEnumerable<CoreController> GetSubControllers()
		{
			return this.viewControllers;
		}

		public override void CreateUI(Widget container)
		{
			base.CreateUI (container);
			this.scrollable = new Scrollable ()
			{
				Parent = container,
				Dock = DockStyle.Fill,
				HorizontalScrollerMode = ScrollableScrollerMode.ShowAlways,
				VerticalScrollerMode = ScrollableScrollerMode.HideAlways,
			};

			this.scrollable.Viewport.IsAutoFitting = true;

			this.frame.Parent = this.scrollable.Viewport;
			this.frame.Dock = DockStyle.Fill;
			this.frame.DrawFrameState = FrameState.None;
			this.frame.Padding = new Margins (3, 0, 3, 0);

			this.CreateViewLayoutHandler ();
		}


		/// <summary>
		/// Sets a custom UI in the data view. This is only possible if no
		/// view controllers are currently active.
		/// </summary>
		/// <param name="customUI">The custom UI.</param>
		public void SetCustomUI(Widget customUI)
		{
			if (this.viewControllers.Count > 0)
			{
				throw new System.InvalidOperationException ("Cannot set custom UI while view controllers are active");
			}

			this.frame.Children.Clear ();
			this.frame.Children.Add (customUI);
		}


		/// <summary>
		/// Adds a new view controller to the data view. The default is to add a new column
		/// on the rightmost side of the data view.
		/// </summary>
		/// <param name="controller">The controller.</param>
		public void PushViewController(CoreViewController controller)
		{
			if (controller == null)
			{
				return;
			}

			var leaf   = this.GetLeafController ();
			var column = this.viewLayoutController.CreateColumn (controller);
			this.viewControllers.Push (controller);

			controller.CreateUI (column);

			this.AttachColumn (column);

			this.Navigator.Add (leaf, controller);
			this.Host.NotifyPushViewController (controller);
		}

		/// <summary>
		/// Disposes the leaf view controller. The default is to remove the rightmost
		/// column of the data view. In the case of a <see cref="DataContext"/> change
		/// between two controllers, automatically save and dispose the popped context.
		/// </summary>
		public void PopViewController()
		{
			System.Diagnostics.Debug.Assert (this.viewControllers.Count > 0);

			var lastController = this.viewControllers.Pop ();
			var leafController = this.GetLeafController ();

			this.Navigator.Remove (leafController, lastController);
			this.Host.NotifyPopViewController (lastController);

			lastController.CloseUI (this.viewLayoutController.LastColumn);
			lastController.Dispose ();
			
			//	Remove the rightmost column in the layout:
			
			var column = this.viewLayoutController.DeleteColumn ();

			this.DetachColumn (column);
		}

		/// <summary>
		/// Gets the leaf view controller (the last one on the stack, i.e. the most
		/// recently pushed controller).
		/// </summary>
		/// <returns>The leaf <see cref="CoreViewController"/> or <c>null</c>.</returns>
		public CoreViewController GetLeafViewController()
		{
			return this.viewControllers.FirstOrDefault ();
		}

		/// <summary>
		/// Saves the focus (based on the column and widget tab index).
		/// </summary>
		/// <returns>The <c>FocusInformation</c>.</returns>
		public FocusInformation SaveFocus()
		{
			var focusedColumn = this.viewLayoutController.GetColumns ().FirstOrDefault (x => x.ContainsKeyboardFocus);

			if ((focusedColumn == null) ||
				(focusedColumn.Window == null) ||
				(focusedColumn.Window.FocusedWidget == null))
			{
				return FocusInformation.Empty;
			}

			int columnIndex = this.viewLayoutController.GetColumnIndex (focusedColumn);
			int widgetIndex = focusedColumn.Window.FocusedWidget.TabIndex;

			return new FocusInformation (columnIndex, widgetIndex);
		}

		/// <summary>
		/// Restores the focus back to what it was when it was saved.
		/// </summary>
		/// <param name="focus">The <c>FocusInformation</c> returned by <see cref="SaveFocus"/>.</param>
		public void RestoreFocus(FocusInformation focus)
		{
			if (focus.IsEmpty)
			{
				return;
			}

			int columnIndex = focus.ColumnIndex;
			int widgetIndex = focus.WidgetIndex;

			var focusedColumn = this.viewLayoutController.GetColumn (columnIndex);
			var focusedWidget = focusedColumn == null ? null : focusedColumn.FindAllChildren (x => x.TabIndex == widgetIndex).FirstOrDefault ();

			if (focusedWidget != null)
			{
				focusedWidget.SetFocusOnTabWidget ();
			}
		}


		/// <summary>
		/// Disposes the leaf views until we reach the specified controller, which will
		/// be left untouched. This disposes all sub-views of the specified controller.
		/// </summary>
		/// <param name="controller">The view controller (or <c>null</c> to close all views).</param>
		public void PopViewControllersUntil(CoreViewController controller)
		{
			if (controller == null)
			{
				this.PopAllViewControllers ();
			}
			else
			{
				System.Diagnostics.Debug.Assert (this.ContainsViewController (controller));

				while (this.GetLeafController ().Matches (controller) == false)
				{
					this.PopViewController ();
				}
			}
		}

		/// <summary>
		/// Disposes all leaf views, until all are closed.
		/// </summary>
		public void PopAllViewControllers()
		{
			while (this.viewControllers.Count > 0)
			{
				this.PopViewController ();
			}
		}

		/// <summary>
		/// Replaces a specific view controller with another one.
		/// </summary>
		/// <param name="oldViewController">The old view controller.</param>
		/// <param name="newViewController">The new view controller.</param>
		public void ReplaceViewController(CoreViewController oldViewController, CoreViewController newViewController)
		{
			this.PopViewControllersUntil (oldViewController);
			this.PopViewController ();
			this.PushViewController (newViewController);
		}

		#region FocusInformation Structure

		public struct FocusInformation
		{
			public FocusInformation(int columnIndex, int widgetIndex)
			{
				this.columnIndex = columnIndex;
				this.widgetIndex = widgetIndex;
			}

			public int ColumnIndex
			{
				get
				{
					return this.columnIndex;
				}
			}

			public int WidgetIndex
			{
				get
				{
					return this.widgetIndex;
				}
			}

			public bool IsEmpty
			{
				get
				{
					return this.columnIndex == -1;
				}
			}

			public static readonly FocusInformation Empty = new FocusInformation (-1, -1);


			private readonly int columnIndex;
			private readonly int widgetIndex;
		}

		#endregion

		#region IWidgetUpdater Members

		public void Update()
		{
			this.viewLayoutController.Update ();
		}

		#endregion

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.viewLayoutController.LayoutChanged -= this.HandleViewLayoutControllerLayoutChanged;
			}

			base.Dispose (disposing);
		}

		private bool ContainsViewController(CoreViewController controller)
		{
			return this.viewControllers.Any (x => x.Matches (controller));
		}
		
		private void CreateViewLayoutHandler()
		{
			this.viewLayoutController.LayoutChanged += this.HandleViewLayoutControllerLayoutChanged;
		}

		private void HandleViewLayoutControllerLayoutChanged(object sender)
		{
			if (this.scrollable.IsDisposed)
			{
				return;
			}

			LayoutContext.SyncArrange (this.scrollable.Viewport);

			var startValue = this.scrollable.ViewportOffsetX;
			var endValue   = this.scrollable.HorizontalScroller.MaxValue;

			this.scrollable.HorizontalScroller.Value = endValue;
		}
		
		private void AttachColumn(TileContainer column)
		{
			if (column != null)
            {
				column.TabNavigating += this.HandleColumnTabNavigating;
            }
		}

		private void DetachColumn(TileContainer column)
		{
			if (column != null)
			{
				column.TabNavigating -= this.HandleColumnTabNavigating;
			}
		}

		private void HandleColumnTabNavigating(object sender, TabNavigateEventArgs e)
		{
			int depth = 0;
			var functions = new Stack<System.Func<bool>> ();

			while (this.viewControllers.Count > ++depth)
			{
				//	Find the (grand-) parent controller and try to activate the next (or previous)
				//	sub view; if this succeeds, cancel the tab navigation - the focus will already
				//	have been set by the implied call to DataViewOrchestrator.OpenSubView.

				var parentController = this.GetParentController (depth);
				var activateSubView  = e.Direction == TabNavigationDir.Forwards ? parentController.ActivateNextSubView : parentController.ActivatePrevSubView;

				if (activateSubView != null)
				{
					if (activateSubView (false))
					{
						e.Cancel = true;
						break;
					}

					//	If non-cyclic activation failed, keep track of the cyclic activation
					//	function, so that we can try it later on:

					functions.Push (() => activateSubView (true));
				}
			}

			//	We were not able to activate the next (or previous) sub-view of any parent
			//	controller; maybe we have hit the end (or beginning) of the sub-view list.
			
			while (functions.Count > 0)
			{
				//	Try to activate the next possible sub-views by cycling past the end
				//	of the list:

				var activateSubView = functions.Pop ();

				if (activateSubView ())
				{
					e.Cancel = true;
					break;
				}
			}
		}

		private CoreViewController GetParentController(int depth)
		{
			return this.viewControllers.Skip (depth).FirstOrDefault ();
		}

		private CoreViewController GetLeafController()
		{
			if (this.viewControllers.Count == 0)
			{
				return null;
			}
			else
			{
				return this.viewControllers.Peek ();
			}
		}


		#region Factory Class

		private sealed class Factory : Epsitec.Cresus.Core.Factories.DefaultViewControllerComponentFactory<DataViewController>
		{
		}

		#endregion


		private readonly Stack<CoreViewController>	viewControllers;
		private readonly ViewLayoutController		viewLayoutController;
		private readonly FrameBox					frame;
		private readonly SafeCounter				pushSafeRecursion;
		
		private Scrollable							scrollable;
	}
}
