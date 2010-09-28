﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets;


using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Orchestrators.Navigation;
using Epsitec.Cresus.Core.BusinessLogic;
using Epsitec.Cresus.Core.Resolvers;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>CoreViewController</c> class is the base class for every view
	/// controller in the application.
	/// </summary>
	public abstract class CoreViewController : CoreController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CoreViewController"/> class.
		/// </summary>
		/// <param name="name">The name of the controller.</param>
		public CoreViewController(string name, DataViewOrchestrator orchestrator = null)
			: base (name)
		{
			if (EntityViewControllerFactory.Default == null)
			{
				this.orchestrator = orchestrator;
			}
			else
			{
				this.orchestrator          = EntityViewControllerFactory.Default.Orchestrator;
				this.viewControllerMode    = EntityViewControllerFactory.Default.Mode;
				this.navigationPathElement = EntityViewControllerFactory.Default.NavigationPathElement;
				this.parentController      = this.orchestrator.GetLeafViewController ();
			}

			this.businessContext = this.orchestrator.DefaultBusinessContext;
			
//-			System.Diagnostics.Debug.Assert (this.BusinessContext != null);
		}


		public CoreViewController				ParentController
		{
			get
			{
				return this.parentController;
			}
		}

		public DataViewOrchestrator				Orchestrator
		{
			get
			{
				return this.orchestrator;
			}
		}

		public NavigationOrchestrator			Navigator
		{
			get
			{
				return this.Orchestrator.Navigator;
			}
		}

		public BusinessLogic.BusinessContext	BusinessContext
		{
			get
			{
				return this.businessContext;
			}
		}

		public virtual CoreData					Data
		{
			get
			{
				return this.businessContext.Data;
			}
		}

		public DataContext						DataContext
		{
			get
			{
				return this.businessContext == null ? null : this.businessContext.DataContext;
			}
		}

		public ViewControllerMode				Mode
		{
			get
			{
				return this.viewControllerMode;
			}
		}

		public NavigationPathElement			NavigationPathElement
		{
			get
			{
				return this.navigationPathElement;
			}			
		}

	
		
		public System.Func<bool, bool>			ActivateNextSubView
		{
			get;
			set;
		}

		public System.Func<bool, bool>			ActivatePrevSubView
		{
			get;
			set;
		}



		public string GetNavigationPath()
		{
			return string.Join ("/", this.GetControllerChain ().Reverse ().Select (x => x.NavigationPathElement.ToString ()));
		}

		public int GetNavigationLevel()
		{
			return this.GetControllerChain ().Count () - 1;
		}

		public IEnumerable<CoreViewController> GetControllerChain()
		{
			var node = this;

			while (node != null)
			{
				yield return node;
				node = node.ParentController;
			}
		}
		

		public bool Matches(CoreViewController controller)
		{
			return (this == controller)
				|| (this.GetReplacementController () == controller);
		}

		/// <summary>
		/// Creates the UI managed by this controller.
		/// </summary>
		/// <param name="container">The container.</param>
		public abstract void CreateUI(Widget container);

		/// <summary>
		/// Closes the UI. The container will be disposed by the caller
		/// and is the same object that was provided to <c>CreateUI</c>.
		/// </summary>
		/// <param name="container">The container.</param>
		public void CloseUI(Widget container)
		{
			this.ReleaseUIFocus (container);
			this.AboutToCloseUI ();
		}

		public virtual CoreViewController GetReplacementController()
		{
			return this;
		}

		private IEnumerable<CoreViewController> GetCoreViewControllers()
		{
			return this.GetSubControllers ().Where (x => x is CoreViewController).Cast<CoreViewController> ();
		}

		private void ReleaseUIFocus(Widget container)
		{
			if (container.ContainsKeyboardFocus)
			{
				//	This will produce an automatic validation or cancellation of any pending
				//	TextFieldEx edition; without this, the last edition would not be validated
				//	correctly:

				container.Window.ClearFocusedWidget ();
			}
		}
		
		protected virtual void AboutToCloseUI()
		{
		}

		private readonly DataViewOrchestrator orchestrator;
		private readonly CoreViewController parentController;
		private readonly ViewControllerMode viewControllerMode;
		private readonly NavigationPathElement navigationPathElement;

		private readonly BusinessContext businessContext;
	}
}
