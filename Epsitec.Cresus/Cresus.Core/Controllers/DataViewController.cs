﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>DataViewController</c> class manages several <see cref="CoreViewController"/>
	/// instances which have a parent/child relationship. The <see cref="ViewLayoutController"/>
	/// is used for the layout.
	/// </summary>
	public class DataViewController : CoreViewController, IWidgetUpdater
	{
		public DataViewController(MainViewController mainViewController, DataViewOrchestrator orchestrator)
			: base ("Data", orchestrator)
		{
			this.mainViewController = mainViewController;
			this.data = this.mainViewController.Data;

			this.viewControllers = new Stack<CoreViewController> ();
			
			this.frame = new FrameBox ();
			this.viewLayoutController = new ViewLayoutController (this.Name + ".ViewLayout", this.frame);
		}


		public CoreData Data
		{
			get
			{
				return this.data;
			}
		}

		public MainViewController MainViewController
		{
			get
			{
				return this.mainViewController;
			}
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			return this.viewControllers;
		}

		public override void CreateUI(Widget container)
		{
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
			var lastContext    = lastController.DataContext;

			this.Navigator.Remove (leafController, lastController);

			lastController.CloseUI (this.viewLayoutController.LastColumn);
			lastController.Dispose ();
			
			//	Remove the rightmost column in the layout:
			
			var column = this.viewLayoutController.DeleteColumn ();

			this.DetachColumn (column);
		}

		public CoreViewController GetRootViewController()
		{
			return this.viewControllers.LastOrDefault ();
		}

		public CoreViewController GetLeafViewController()
		{
			return this.viewControllers.FirstOrDefault ();
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

		private bool ContainsViewController(CoreViewController controller)
		{
			return this.viewControllers.Any (x => x.Matches (controller));
		}


		#region IWidgetUpdater Members

		public void Update()
		{
			this.viewLayoutController.Update ();
		}

		#endregion
		

		protected override void AboutToDiscard()
		{
			//TODO: do we need to do this ?
//-			this.ClearActiveEntity ();

			base.AboutToDiscard ();
		}

		private void CreateViewLayoutHandler()
		{
			this.viewLayoutController.LayoutChanged +=
				delegate
				{
					LayoutContext.SyncArrange (this.scrollable.Viewport);

					var startValue = this.scrollable.ViewportOffsetX;
					var endValue   = this.scrollable.HorizontalScroller.MaxValue;

					this.scrollable.HorizontalScroller.Value = endValue;
				};
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

		private EntityViewController CreateRootSummaryViewController(NavigationPathElement navigationPathElement)
		{
			return EntityViewControllerFactory.Create ("ViewController", this.entity, ViewControllerMode.Summary, this.Orchestrator, navigationPathElement: navigationPathElement);
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


		private readonly MainViewController mainViewController;
		private readonly CoreData data;
		private readonly Stack<CoreViewController> viewControllers;
		private readonly ViewLayoutController viewLayoutController;
		private readonly FrameBox frame;
		
		private Scrollable scrollable;
		private AbstractEntity entity;
	}
}