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
	public class DataViewOrchestrator : System.IDisposable, ICoreManualComponent, ICoreComponentHost<ViewControllerComponent>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataViewOrchestrator"/> class.
		/// </summary>
		/// <param name="host">The host.</param>
		/// <param name="data">The core data.</param>
		/// <param name="commandContext">The command context.</param>
		public DataViewOrchestrator(CoreApp host)
		{
			this.components         = new CoreComponentHostImplementation<ViewControllerComponent> ();
			this.host               = host;
			this.data               = this.host.FindComponent<CoreData> ();
			this.commandContext     = this.host.CommandContext;

			this.host.RegisterComponent (this);
			this.host.RegisterComponentAsDisposable (this);
			this.host.ActivateComponent (this);

			this.CreateNewBusinessContext ();

			Factories.ViewControllerComponentFactory.RegisterComponents (this);
			Factories.ViewControllerComponentFactory.SetupComponents (this.components.GetComponents ());

			this.mainViewController = this.GetComponent<MainViewController> ();
			this.mainWindowController = new MainWindowController (this);
			this.dataViewController = new DataViewController (this);
			this.navigator          = new NavigationOrchestrator (this);
			this.workflowController = new WorkflowController (this);

			this.workflowController.AttachBusinessContext (this.businessContext);
		}


		public CoreApp							Host
		{
			get
			{
				return this.host;
			}
		}

		public CoreData							Data
		{
			get
			{
				return this.data;
			}
		}


		public CommandContext					CommandContext
		{
			get
			{
				return this.commandContext;
			}
		}

		public MainWindowController				MainWindowController
		{
			get
			{
				return this.mainWindowController;
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


		public void RegisterApplicationComponent<T>(T component)
			where T : ICoreManualComponent
		{
			this.Host.RegisterComponent (component);
			this.Host.ActivateComponent (component);
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

			this.SetActiveBusinessContext (BusinessContext.Create (this.data));
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
				this.host.SetEnable (Library.Res.Commands.Edition.SaveRecord, true);
				this.host.SetEnable (Library.Res.Commands.Edition.DiscardRecord, true);
			}
			else
			{
				this.host.SetEnable (Library.Res.Commands.Edition.SaveRecord, false);
				this.host.SetEnable (Library.Res.Commands.Edition.DiscardRecord, false);
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

		#region ICoreComponentHost<MainWindowComponent> Members

		public T GetComponent<T>()
			where T : ViewControllerComponent
		{
			return this.components.GetComponent<T> ();
		}

		ViewControllerComponent ICoreComponentHost<ViewControllerComponent>.GetComponent(System.Type type)
		{
			return this.components.GetComponent (type);
		}

		public IEnumerable<ViewControllerComponent> GetComponents()
		{
			return this.components.GetComponents ();
		}

		public bool ContainsComponent<T>()
			where T : ViewControllerComponent
		{
			return this.components.ContainsComponent<T> ();
		}

		bool ICoreComponentHost<ViewControllerComponent>.ContainsComponent(System.Type type)
		{
			return this.components.ContainsComponent (type);
		}

		void ICoreComponentHost<ViewControllerComponent>.RegisterComponent<T>(T component)
		{
			this.components.RegisterComponent<T> (component);
		}

		void ICoreComponentHost<ViewControllerComponent>.RegisterComponent(System.Type type, ViewControllerComponent component)
		{
			this.components.RegisterComponent (type, component);
		}

		void ICoreComponentHost<ViewControllerComponent>.RegisterComponentAsDisposable(System.IDisposable component)
		{
			this.components.RegisterComponentAsDisposable (component);
		}

		#endregion


		public event EventHandler<ActiveEntityCancelEventArgs> SettingActiveEntity;

		private readonly CoreComponentHostImplementation<ViewControllerComponent> components;

		private readonly CoreApp				host;
		private readonly CoreData				data;
		private readonly CommandContext			commandContext;
		private readonly WorkflowController		workflowController;
		private readonly MainViewController		mainViewController;
		private readonly DataViewController		dataViewController;
		private readonly MainWindowController	mainWindowController;
		private readonly NavigationOrchestrator navigator;


		private BusinessContext					businessContext;
		private EntityKey?						activeEntityKey;
	}
}