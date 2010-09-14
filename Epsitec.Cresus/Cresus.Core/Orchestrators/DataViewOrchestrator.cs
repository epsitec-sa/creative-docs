//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Orchestrators;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Orchestrators.Navigation;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Core.BusinessLogic;

namespace Epsitec.Cresus.Core.Orchestrators
{
	/// <summary>
	/// The <c>DataViewOrchestrator</c> class is used by the various view controllers
	/// to change what is visible in the data view.
	/// </summary>
	public class DataViewOrchestrator
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataViewOrchestrator"/> class.
		/// </summary>
		/// <param name="mainViewController">The main view controller.</param>
		public DataViewOrchestrator(CoreData data, CommandContext commandContext)
		{
			this.data = data;
			this.commandContext = commandContext;
			this.mainViewController = new MainViewController (this.data, commandContext, this);
			this.dataViewController = new DataViewController (this.mainViewController, this);
			this.navigator = new NavigationOrchestrator (this.mainViewController);
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

		public NavigationOrchestrator Navigator
		{
			get
			{
				return this.navigator;
			}
		}

		public DataViewController Controller
		{
			get
			{
				return this.dataViewController;
			}
		}

		public BusinessContext BusinessContext
		{
			get
			{
				if (this.businessContext == null)
                {
					this.SetActiveBusinessContext (this.data.CreateBusinessContext ());
                }

				return this.businessContext;
			}
		}

		public BusinessContext CurrentBusinessContext
		{
			get
			{
				return this.businessContext;
			}
		}

		public DataContext DefaultDataContext
		{
			get
			{
				return this.BusinessContext.DataContext;
			}
		}

		

		public void ClearActiveEntity()
		{
			this.dataViewController.PopAllViewControllers ();
			this.ClearBusinessContext ();
		}

		public void ClearBusinessContext()
		{
			if ((this.businessContext != null) &&
				(this.businessContext.IsEmpty == false))
			{
				this.businessContext.Dispose ();
				this.SetActiveBusinessContext (null);
			}
		}
		
		public void SetActiveEntity(EntityKey? entityKey, NavigationPathElement navigationPathElement)
		{
			this.ClearActiveEntity ();

			if (entityKey.HasValue)
			{
				var businessContext = this.BusinessContext;

				businessContext.SetActiveEntity (entityKey, navigationPathElement);

				var liveEntity = businessContext.ActiveEntity;
				var controller = EntityViewControllerFactory.Create ("Root", liveEntity, ViewControllerMode.Summary, this, navigationPathElement: navigationPathElement);

				this.dataViewController.PushViewController (controller);
			}
		}


		private void SetActiveBusinessContext(BusinessContext context)
		{
			var oldContext = this.businessContext;
			var newContext = context;

			if (oldContext != newContext)
			{
				this.businessContext = context;

				if (oldContext != null)
				{
					oldContext.ContainsChangesChanged -= this.HandleBusinessContextContainsChangesChanged;
				}
				if (newContext != null)
				{
					newContext.ContainsChangesChanged += this.HandleBusinessContextContainsChangesChanged;
				}

				this.UpdateBusinessContextContainsChanges ();
			}
		}

		private void HandleBusinessContextContainsChangesChanged(object sender)
		{
			this.UpdateBusinessContextContainsChanges ();
		}

		private void UpdateBusinessContextContainsChanges()
		{
			if ((this.businessContext != null) &&
				(this.businessContext.ContainsChanges))
			{
				CoreProgram.Application.SetEnable (Res.Commands.Edition.SaveRecord, true);
				CoreProgram.Application.SetEnable (Res.Commands.Edition.DiscardRecord, true);
			}
			else
			{
				CoreProgram.Application.SetEnable (Res.Commands.Edition.SaveRecord, false);
				CoreProgram.Application.SetEnable (Res.Commands.Edition.DiscardRecord, false);
			}
		}



		/// <summary>
		/// Shows the specified sub view of a given controller. The view of the
		/// sub view controller will be created, if needed.
		/// </summary>
		/// <param name="viewController">The view controller (or <c>null</c>).</param>
		/// <param name="subViewController">The sub view controller.</param>
		public void ShowSubView(CoreViewController viewController, CoreViewController subViewController)
		{
			this.dataViewController.PopViewControllersUntil (viewController);
			this.dataViewController.PushViewController (subViewController);
		}

		/// <summary>
		/// Closes the sub views of the specified controller. The view of the
		/// controller becomes the top level view.
		/// </summary>
		/// <param name="viewController">The view controller (or <c>null</c> to close all views).</param>
		public void CloseSubViews(CoreViewController viewController = null)
		{
			this.dataViewController.PopViewControllersUntil (viewController);
		}

		/// <summary>
		/// Closes the view of the specified controller, including all sub views.
		/// </summary>
		/// <param name="viewController">The view controller.</param>
		public void CloseView(CoreViewController viewController)
		{
			this.dataViewController.PopViewControllersUntil (viewController);
			this.dataViewController.PopViewController ();
		}


		public void ReplaceView(CoreViewController oldViewController, CoreViewController newViewController)
		{
			this.dataViewController.ReplaceViewController (oldViewController, newViewController);
		}

		public CoreViewController GetLeafViewController()
		{
			return this.dataViewController.GetLeafViewController ();
		}


		private readonly CoreData data;
		private readonly CommandContext commandContext;
		private readonly MainViewController mainViewController;
		private readonly DataViewController dataViewController;
		private readonly NavigationOrchestrator navigator;

		private DataContext defaultDataContext;
		private BusinessContext businessContext;
	}
}
