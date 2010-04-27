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

		
		public void SetEntity(AbstractEntity entity)
		{
			this.ClearActiveEntity ();

			if (entity != null)
			{
				this.entity = entity;
				this.PushViewController (this.entity);
			}
		}

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


		public void PushViewController(AbstractEntity entity)
		{
			EntityViewController controller = EntityViewController.CreateViewController ("ViewController", entity, ViewControllerMode.Compact, this.orchestrator);
			controller.DataViewController = this;

			this.PushViewController (controller);
		}

		public void PushViewController(CoreViewController controller)
		{
			System.Diagnostics.Debug.Assert (controller != null);

			CoreViewController parent = null;

			if (this.viewControllers.Count != 0)
			{
				parent = this.viewControllers.Peek ();
			}

			var column = this.viewLayoutController.CreateColumn ();
			this.viewControllers.Push (controller);
			controller.CreateUI (column);

			if (parent != null)
			{
				this.SelectViewController (parent, controller);
			}
		}

		public void PopViewController()
		{
			System.Diagnostics.Debug.Assert (this.viewControllers.Count > 0);

			var controller = this.viewControllers.Pop ();
			controller.Dispose ();
			this.viewLayoutController.DeleteColumn ();
		}

		public void PopViewControllersUntil(CoreViewController controller)
		{
			System.Diagnostics.Debug.Assert (this.viewControllers.Contains (controller));

			while (this.viewControllers.Peek () != controller)
			{
				this.PopViewController ();
			}
		}

		public void RebuildViewController()
		{
			var controller = this.viewControllers.Peek ();
			var column = this.viewLayoutController.PeekedColumn;
			controller.CreateUI (column);
		}

		private void SelectViewController(CoreViewController parentController, CoreViewController selectedController)
		{
			var parent   = parentController   as EntityViewController;
			var selected = selectedController as EntityViewController;
			System.Diagnostics.Debug.Assert (parent != null && selected != null);

			foreach (var widget in parent.Container.Children)
			{
				if (widget is Widgets.AbstractTile)
				{
					var tileContainer = widget as Widgets.AbstractTile;
					tileContainer.SetSelected (tileContainer.Entity == selected.Entity);
				}
			}
		}


		private readonly Stack<CoreViewController> viewControllers;
		private readonly Orchestrators.DataViewOrchestrator orchestrator;
		
		private ViewLayoutController viewLayoutController;
		private FrameBox frame;
		
		private AbstractEntity entity;
	}
}
