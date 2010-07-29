//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets;

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
		public CoreViewController(string name)
			: base (name)
		{
		}

		
		public Orchestrators.DataViewOrchestrator Orchestrator
		{
			get;
			set;
		}

		public Orchestrators.NavigationOrchestrator Navigator
		{
			get
			{
				return this.Orchestrator == null ? null : this.Orchestrator.Navigator;
			}
		}

		public virtual Epsitec.Cresus.DataLayer.Context.DataContext DataContext
		{
			get;
			set;
		}

		public System.Func<bool, bool> ActivateNextSubView
		{
			get;
			set;
		}

		public System.Func<bool, bool> ActivatePrevSubView
		{
			get;
			set;
		}

		public ViewControllerMode Mode
		{
			get;
			protected set;
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


		internal void AboutToSave(DataLayer.Context.DataContext dataContext)
		{
			this.GetSubControllers ().Where (x => x is CoreViewController).Cast<CoreViewController> ().ForEach (x => x.AboutToSave (dataContext));

			if (this.DataContext == dataContext)
			{
				this.AboutToSave ();
			}
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
	}
}
