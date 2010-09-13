//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Orchestrators.Navigation;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.Core.Orchestrators;

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
			if (EntityViewControllerResolver.Default == null)
			{
				this.orchestrator = orchestrator;
			}
			else
			{
				this.orchestrator          = EntityViewControllerResolver.Default.Orchestrator;
				this.viewControllerMode    = EntityViewControllerResolver.Default.Mode;
				this.navigationPathElement = EntityViewControllerResolver.Default.NavigationPathElement;
				this.parentController      = this.orchestrator.GetLeafViewController ();
			}

			System.Diagnostics.Debug.Assert (this.Orchestrator != null);
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
				var businessContext = this.GetLocalBusinessContext ();

				if ((businessContext !=  null) ||
					(this.parentController == null))
				{
					return businessContext;
				}
				else
				{
					return this.parentController.BusinessContext;
				}
			}
		}

		public virtual DataContext				DataContext
		{
			get
			{
				return this.dataContext;
			}
			set
			{
				if (this.dataContext != value)
                {
					this.dataContext = value;
				}
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

		public virtual BusinessLogic.BusinessContext GetLocalBusinessContext()
		{
			return null;
		}



		internal void NotifyAboutToSave()
		{
			this.GetCoreViewControllers ().ForEach (x => x.AboutToSave ());
			this.AboutToSave ();
		}

		internal void NotifyAboutToDiscard()
		{
			this.GetCoreViewControllers ().ForEach (x => x.AboutToDiscard ());
			this.AboutToDiscard ();
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

		protected virtual void AboutToSave()
		{
		}

		protected virtual void AboutToDiscard()
		{
		}
		
        protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.OnDisposing ();
				this.Disposing = null;
			}

			base.Dispose (disposing);
		}

		private void OnDisposing()
		{
			var handler = this.Disposing;

			if (handler != null)
			{
				handler (this);
			}
		}

		public event EventHandler Disposing;

		private readonly DataViewOrchestrator orchestrator;
		private readonly CoreViewController parentController;
		private readonly ViewControllerMode viewControllerMode;
		private readonly NavigationPathElement navigationPathElement;
		
		private DataContext dataContext;
	}
}
