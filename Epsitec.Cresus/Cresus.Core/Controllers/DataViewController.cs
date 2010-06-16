//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets.Layouts;

namespace Epsitec.Cresus.Core.Controllers
{
	public class DataViewController : CoreViewController
	{
		public DataViewController(string name, CoreData data)
			: base (name)
		{
			this.data = data;

			this.viewControllers = new Stack<CoreViewController> ();
			this.orchestrator = new Orchestrators.DataViewOrchestrator (this);
		}

		public Orchestrators.DataViewOrchestrator Orchestrator
		{
			get
			{
				return this.orchestrator;
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

			this.frame = new FrameBox ()
			{
				Parent = this.scrollable.Viewport,
				Dock = DockStyle.Fill,
				DrawFrameState = FrameState.None,
				Padding = new Margins (3, 0, 3, 0),
			};

			this.viewLayoutController = new ViewLayoutController (this.Name + ".ViewLayout", this.frame);

			this.CreateViewLayoutHandler ();
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
				this.PushViewController (this.CreateCompactEntityViewController ());
			}
		}

		/// <summary>
		/// Clears the active entity and disposes any visible view controllers.
		/// </summary>
		public void ClearActiveEntity()
		{
			if (this.entity != null)
			{
				while (this.viewControllers.Count > 0)
				{
					this.PopViewController ();
				}

				this.entity = null;
			}
		}


		/// <summary>
		/// Adds a new view controller to the data view. The default is to add a new column
		/// on the rightmost side of the data view.
		/// </summary>
		/// <param name="controller">The controller.</param>
		public void PushViewController(CoreViewController controller)
		{
			System.Diagnostics.Debug.Assert (controller != null);

			if (controller.DataContext == null)
			{
				var leafController = this.GetLeafController ();

				if (leafController == null)
				{
					controller.DataContext = this.data.DataContext;
				}
				else
				{
					controller.DataContext = leafController.DataContext ?? this.data.DataContext;
				}
			}

			var column = this.viewLayoutController.CreateColumn (controller);
			this.viewControllers.Push (controller);
			controller.CreateUI (column);

			this.AttachColumn (column);
		}

		/// <summary>
		/// Disposes the leaf view controller. The default is to remove the rightmost
		/// column of the data view.
		/// </summary>
		public void PopViewController()
		{
			System.Diagnostics.Debug.Assert (this.viewControllers.Count > 0);

			var lastController = this.viewControllers.Pop ();
			var leafController = this.GetLeafController ();

			lastController.CloseUI (this.viewLayoutController.LastColumn);

			if ((leafController == null) ||
				(leafController.DataContext != lastController.DataContext))
			{
				this.data.SaveDataContext (lastController.DataContext);
				this.data.DisposeDataContext (lastController.DataContext);
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
		/// <param name="controller">The controller.</param>
		public void PopViewControllersUntil(CoreViewController controller)
		{
			System.Diagnostics.Debug.Assert (this.viewControllers.Contains (controller));

			while (this.GetLeafController () != controller)
			{
				this.PopViewController ();
			}
		}

		internal void RebuildLeafViewController()
		{
			if (this.viewControllers.Count > 0)
			{
				var controller = this.GetLeafController ();
				var column = this.viewLayoutController.LastColumn;

				//	TODO: remove this -- it can't work reliably, since we don't dispose the previous
				//	controller (and we cannot, by the way)

				column.Children.Clear ();
				controller.CreateUI (column);
			}
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
			return EntityViewController.CreateEntityViewController ("ViewController", this.entity, ViewControllerMode.Summary, this.orchestrator);
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
		private readonly Orchestrators.DataViewOrchestrator orchestrator;
		
		private ViewLayoutController viewLayoutController;
		private FrameBox frame;
		private Scrollable scrollable;
		
		private AbstractEntity entity;
	}
}
