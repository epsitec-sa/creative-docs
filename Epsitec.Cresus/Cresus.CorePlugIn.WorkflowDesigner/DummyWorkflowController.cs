//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Orchestrators.Navigation;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.CorePlugIn.WorkflowDesigner
{
	/// <summary>
	/// The <c>DummyWorkflowController</c> is required in order to store the navigation
	/// events in the navigation history (see <see cref="NavigationOrchestrator"/>), since
	/// the workflow itself is not edited by a standard <see cref="CoreViewController"/>.
	/// </summary>
	public sealed class DummyWorkflowController : INavigationPathElementProvider, System.IDisposable
	{
		public DummyWorkflowController(NavigationOrchestrator navigator, NavigationPathElement path)
		{
			this.navigator = navigator;
			this.path = path;

			this.navigator.Add (null, this);
		}

		#region INavigationPathElementProvider Members

		public NavigationPathElement NavigationPathElement
		{
			get
			{
				return this.path;
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			this.navigator.Remove (null, this);
		}

		#endregion

		private readonly NavigationOrchestrator	navigator;
		private readonly NavigationPathElement	path;
	}
}
