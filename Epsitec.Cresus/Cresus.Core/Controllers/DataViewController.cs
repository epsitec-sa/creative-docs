//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Core.Widgets;

namespace Epsitec.Cresus.Core.Controllers
{
	public class DataViewController : CoreViewController
	{
		public DataViewController(string name)
			: base (name)
		{
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
			this.frame = new FrameBox ()
			{
				Parent = container,
				Dock = DockStyle.Fill,
			};

			this.viewLayoutController = new ViewLayoutController (this.Name + ".ViewLayout", this.frame);
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

			var column = this.viewLayoutController.CreateColumn ();
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

			var controller = this.viewControllers.Pop ();
			controller.Dispose ();
			
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
			System.Diagnostics.Debug.WriteLine ("Navigating: " + e.Direction);
		}

		private EntityViewController CreateCompactEntityViewController()
		{
			return EntityViewController.CreateEntityViewController ("ViewController", this.entity, ViewControllerMode.Summary, this.orchestrator);
		}

		private CoreViewController GetLeafController()
		{
			return this.viewControllers.Peek ();
		}
		

		private readonly Stack<CoreViewController> viewControllers;
		private readonly Orchestrators.DataViewOrchestrator orchestrator;
		
		private ViewLayoutController viewLayoutController;
		private FrameBox frame;
		
		private AbstractEntity entity;
	}
}
