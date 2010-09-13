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

		public DataContext DefaultDataContext
		{
			get
			{
				if (this.defaultDataContext == null)
				{
					this.defaultDataContext = this.data.CreateDataContext ("Default");
				}

				return this.defaultDataContext;
			}
		}

		

		public void ClearActiveEntity()
		{
			this.dataViewController.ClearActiveEntity ();

			if (this.defaultDataContext != null)
			{
				this.data.DisposeDataContext (this.defaultDataContext);
				this.defaultDataContext = null;
			}
		}

		public void SetActiveEntity(EntityKey? entityKey, NavigationPathElement navigationPathElement)
		{
			this.ClearActiveEntity ();

			var dataContext = this.DefaultDataContext;
			var realEntity  = dataContext.ResolveEntity (entityKey);

			this.dataViewController.SetActiveEntity (realEntity, navigationPathElement);
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
		private readonly MainViewController mainViewController;
		private readonly DataViewController dataViewController;
		private readonly NavigationOrchestrator navigator;

		private DataContext defaultDataContext;
	}
}
