//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Orchestrators.Navigation;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>CoreViewController</c> class is the base class for every view
	/// controller in the application.
	/// </summary>
	public abstract class CoreViewController : CoreController, INavigationPathElementProvider, ICoreManualComponent
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

			//	Make sure we create the default business context :
			var businessContext = this.BusinessContext;
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

		public BusinessContext					BusinessContext
		{
			get
			{
				return this.orchestrator.DefaultBusinessContext;
			}
		}

		public virtual CoreData					Data
		{
			get
			{
				return this.BusinessContext.Data;
			}
		}

		public DataContext						DataContext
		{
			get
			{
				var businessContext = this.BusinessContext;
				return businessContext == null ? null : businessContext.DataContext;
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

		public virtual double GetPreferredWidth(int columnIndex, int columnCount)
		{
			return 300;
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
		public virtual void CreateUI(Widget container)
		{
			this.AboutToCreateUI ();
		}

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

		/// <summary>
		/// Opens a linked sub-view (if any). This gets called by the <see cref="DataViewController"/>
		/// after this controller's view was successfully recorded in a data view column.
		/// </summary>
		public virtual void OpenLinkedSubView()
		{
		}

		private IEnumerable<CoreViewController> GetCoreViewControllers()
		{
			return this.GetSubControllers ().Where (x => x is CoreViewController).Cast<CoreViewController> ();
		}

		private void ReleaseUIFocus(Widget container)
		{
			if ((container.ContainsKeyboardFocus) &&
				(container.Window != null))
			{
				//	This will produce an automatic validation or cancellation of any pending
				//	TextFieldEx edition; without this, the last edition would not be validated
				//	correctly:

				container.Window.ClearFocusedWidget ();
			}
		}

		protected virtual void AboutToCreateUI()
		{
			this.GetSubControllers ().OfType<CoreViewController> ().ForEach (x => x.AboutToCreateUI ());
		}
		
		protected virtual void AboutToCloseUI()
		{
			this.GetSubControllers ().OfType<CoreViewController> ().ForEach (x => x.AboutToCloseUI ());
		}

		private readonly DataViewOrchestrator	orchestrator;
		private readonly CoreViewController		parentController;
		private readonly ViewControllerMode		viewControllerMode;
		private readonly NavigationPathElement	navigationPathElement;
	}
}
