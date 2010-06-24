//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets.Layouts;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>DataViewController</c> class manages several <see cref="CoreViewController"/>
	/// instances which have a parent/child relationship. The <see cref="ViewLayoutController"/>
	/// is used for the layout.
	/// </summary>
	public class DataViewController : CoreViewController
	{
		public DataViewController(string name, CoreData data)
			: base (name)
		{
			this.data = data;

			this.viewControllers = new Stack<CoreViewController> ();
			this.Orchestrator = new Orchestrators.DataViewOrchestrator (this);
			
			this.frame = new FrameBox ();
			this.viewLayoutController = new ViewLayoutController (this.Name + ".ViewLayout", this.frame);
		}


		/// <summary>
		/// Gets the data context of the leaf sub view or the active one taken from the
		/// associated <see cref="CoreData"/>.
		/// </summary>
		/// <value>The data context.</value>
		public override DataContext DataContext
		{
			get
			{
				var leafController = this.GetLeafController ();

				if (leafController == null)
				{
					return this.data.DataContext;
				}
				else
				{
					return leafController.DataContext ?? this.data.DataContext;
				}
			}
			set
			{
				throw new System.InvalidOperationException ("Cannot set DataContext on DataViewController");
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
		/// Sets the active entity visible in the data view. This will collapse everything
		/// back to just one root view controller.
		/// </summary>
		/// <param name="entity">The entity.</param>
		public void SetActiveEntity(AbstractEntity entity)
		{
			this.ClearActiveEntity ();

			if (entity != null)
			{
				this.entity = entity;

				var context    = this.DataContext;
				var controller = this.CreateCompactEntityViewController ();

				controller.DataContext = context;

				this.PushViewController (controller);
			}
		}

		/// <summary>
		/// Clears the active entity and disposes any visible view controllers.
		/// </summary>
		public void ClearActiveEntity()
		{
			this.PopAllViewControllers ();
			this.entity = null;
		}


		/// <summary>
		/// Adds a new view controller to the data view. The default is to add a new column
		/// on the rightmost side of the data view.
		/// </summary>
		/// <param name="controller">The controller.</param>
		public void PushViewController(CoreViewController controller)
		{
			System.Diagnostics.Debug.Assert (controller != null);

			this.InheritLeafControllerDataContext (controller);

			var column = this.viewLayoutController.CreateColumn (controller);
			this.viewControllers.Push (controller);
			controller.CreateUI (column);

			this.AttachColumn (column);
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

			lastController.CloseUI (this.viewLayoutController.LastColumn);

			if (lastContext != null)
			{
				if ((leafController == null) ||
					(leafController.DataContext != lastController.DataContext))
				{
					this.data.SaveDataContext (lastContext);
					this.data.DisposeDataContext (lastContext);
				}
			}

			lastController.Dispose ();
			
			//	Remove the rightmost column in the layout:
			
			var column = this.viewLayoutController.DeleteColumn ();

			this.DetachColumn (column);
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
				System.Diagnostics.Debug.Assert (this.viewControllers.Contains (controller));

				while (this.GetLeafController () != controller)
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

		public void ReplaceViewController(CoreViewController oldViewController, CoreViewController newViewController)
		{
			this.PopViewControllersUntil (oldViewController);
			this.PopViewController ();
			this.PushViewController (newViewController);
		}

		private void InheritLeafControllerDataContext(CoreViewController controller)
		{
			if (controller.DataContext != null)
			{
				return;
			}
				
			var leafController = this.GetLeafController ();

			if (leafController == null)
            {
				return;
            }
			
			controller.DataContext = leafController.DataContext;
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

		private EntityViewController CreateCompactEntityViewController()
		{
			return EntityViewController.CreateEntityViewController ("ViewController", this.entity, ViewControllerMode.Summary, this.Orchestrator);
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


		private readonly CoreData data;
		private readonly Stack<CoreViewController> viewControllers;
		private readonly ViewLayoutController viewLayoutController;
		private readonly FrameBox frame;
		
		private Scrollable scrollable;
		private AbstractEntity entity;
	}
}
