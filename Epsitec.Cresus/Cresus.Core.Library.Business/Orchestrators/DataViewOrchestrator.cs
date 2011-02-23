//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Orchestrators.Navigation;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Orchestrators
{
	/// <summary>
	/// The <c>DataViewOrchestrator</c> class is used by the various view controllers
	/// to change what is visible in the data view.
	/// </summary>
	public class DataViewOrchestrator : System.IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataViewOrchestrator"/> class.
		/// </summary>
		/// <param name="mainViewController">The main view controller.</param>
		public DataViewOrchestrator(CoreData data, CommandContext commandContext)
		{
			this.data               = data;
			this.commandContext     = commandContext;

			this.CreateNewBusinessContext ();

			this.mainViewController = new MainViewController (this, commandContext);
			this.dataViewController = new DataViewController (this);
			this.navigator          = new NavigationOrchestrator (this.mainViewController);
			this.workflowController = new WorkflowController (this);

			this.workflowController.AttachBusinessContext (this.businessContext);
		}


		public CoreData							Data
		{
			get
			{
				return this.data;
			}
		}

		public MainViewController				MainViewController
		{
			get
			{
				return this.mainViewController;
			}
		}

		public NavigationOrchestrator			Navigator
		{
			get
			{
				return this.navigator;
			}
		}

		public DataViewController				DataViewController
		{
			get
			{
				return this.dataViewController;
			}
		}

		public BusinessContext					DefaultBusinessContext
		{
			get
			{
				if (this.businessContext == null)
                {
					this.CreateNewBusinessContext ();
				}

				return this.businessContext;
			}
		}

		public DataContext						DefaultDataContext
		{
			get
			{
				return this.DefaultBusinessContext.DataContext;
			}
		}



		/// <summary>
		/// Clears the active entity. This implicitly calls <see cref="ClearBusinessContext"/>.
		/// </summary>
		public void ClearActiveEntity()
		{
			this.OnSettingActiveEntity (new ActiveEntityCancelEventArgs ());

			this.activeEntityKey = null;
			this.dataViewController.PopAllViewControllers ();
			this.ClearBusinessContext ();
		}

		/// <summary>
		/// Clears the business context.
		/// </summary>
		private void ClearBusinessContext()
		{
			System.Diagnostics.Debug.Assert (this.dataViewController.IsEmpty);
			
			if ((this.businessContext != null) &&
				(this.businessContext.IsEmpty == false))
			{
				this.businessContext.Dispose ();
				this.SetActiveBusinessContext (null);
			}
		}

		/// <summary>
		/// Sets the active entity.
		/// </summary>
		/// <param name="entityKey">The entity key.</param>
		/// <param name="navigationPathElement">The navigation path element.</param>
		public void SetActiveEntity(EntityKey? entityKey, NavigationPathElement navigationPathElement)
		{
			if ((this.CanSetActiveEntity (entityKey, navigationPathElement)) &&
				(this.activeEntityKey != entityKey))
			{
				this.ClearActiveEntity ();
				this.CreateNewBusinessContext ();

				this.businessContext.SetActiveEntity (entityKey, navigationPathElement);
				this.activeEntityKey = entityKey;

				var liveEntity = this.businessContext.ActiveEntity;
				var controller = EntityViewControllerFactory.Create ("Root", liveEntity, ViewControllerMode.Summary, this, navigationPathElement: navigationPathElement);

				this.dataViewController.PushViewController (controller);
			}
		}

		public void ResetActiveEntity(EntityKey? entityKey, NavigationPathElement navigationPathElement)
		{
			if ((this.activeEntityKey == entityKey) &&
				(this.businessContext.ActiveNavigationPathElement.Equals (navigationPathElement)))
			{
				if (this.CanSetActiveEntity (entityKey, navigationPathElement))
				{
					var liveEntity = this.businessContext.ActiveEntity;
					var controller = EntityViewControllerFactory.Create ("Root", liveEntity, ViewControllerMode.Summary, this, navigationPathElement: navigationPathElement);
					
					this.dataViewController.PopAllViewControllers ();
					this.dataViewController.PushViewController (controller);
				}
			}
			else
			{
				this.ClearActiveEntity ();
				this.SetActiveEntity (entityKey, navigationPathElement);
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

		/// <summary>
		/// Replaces the specified view controller with another view controller. This will
		/// simply close the old view controller and push the new one on the top of the
		/// stack.
		/// </summary>
		/// <param name="oldViewController">The old view controller.</param>
		/// <param name="newViewController">The new view controller.</param>
		public void ReplaceView(CoreViewController oldViewController, CoreViewController newViewController)
		{
			this.dataViewController.ReplaceViewController (oldViewController, newViewController);
		}

		/// <summary>
		/// Gets the leaf view controller (i.e. the currently rightmost view controller in the <see cref="DataViewController"/>).
		/// </summary>
		/// <returns>The leaf view controller.</returns>
		public CoreViewController GetLeafViewController()
		{
			return this.dataViewController.GetLeafViewController ();
		}


		#region IDisposable Members

		public void Dispose()
		{
			this.ClearActiveEntity ();
		}

		#endregion

		private bool CanSetActiveEntity(EntityKey? entityKey, NavigationPathElement navigationPathElement)
		{
			var e = new ActiveEntityCancelEventArgs (entityKey, navigationPathElement);

			this.OnSettingActiveEntity (e);

			if (e.Cancel)
			{
				return false;
			}

			if (entityKey.HasValue == false)
			{
				this.ClearActiveEntity ();
			}

			return true;
		}

		private void CreateNewBusinessContext()
		{
			if ((this.businessContext != null) &&
				(this.businessContext.IsEmpty))
			{
				return;
			}
			
			System.Diagnostics.Debug.Assert (this.businessContext == null);

			throw new System.NotImplementedException ();
#if false
			this.SetActiveBusinessContext (this.data.CreateBusinessContext ());
#endif
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
					this.workflowController.DetachBusinessContext (oldContext);
				}
				
				if (newContext != null)
				{
					newContext.ContainsChangesChanged += this.HandleBusinessContextContainsChangesChanged;

					if (this.workflowController == null)
					{
						//	Happens only while the DataViewOrchestrator is executing its constructor; we
						//	will manually attach the business context in the constructor.
					}
					else
					{
						this.workflowController.AttachBusinessContext (newContext);
					}
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
				(this.businessContext.ContainsChanges ()))
			{
				Dispatcher.SetCommandEnable (Res.Commands.Edition.SaveRecord, true);
				Dispatcher.SetCommandEnable (Res.Commands.Edition.DiscardRecord, true);
			}
			else
			{
				Dispatcher.SetCommandEnable (Res.Commands.Edition.SaveRecord, false);
				Dispatcher.SetCommandEnable (Res.Commands.Edition.DiscardRecord, false);
			}
		}


		private void OnSettingActiveEntity(ActiveEntityCancelEventArgs e)
		{
			var handler = this.SettingActiveEntity;

			if (handler != null)
			{
				handler (this, e);
			}
		}


		public event EventHandler<ActiveEntityCancelEventArgs> SettingActiveEntity;

		
		private readonly CoreData				data;
		private readonly CommandContext			commandContext;
		private readonly WorkflowController		workflowController;
		private readonly MainViewController		mainViewController;
		private readonly DataViewController		dataViewController;
		private readonly NavigationOrchestrator navigator;


		private BusinessContext					businessContext;
		private EntityKey?						activeEntityKey;
	}
}