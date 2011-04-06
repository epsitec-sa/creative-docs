//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Orchestrators;

namespace Epsitec.Cresus.Core.Library
{
	/// <summary>
	/// The <c>ViewControllerComponent</c> class is the base class used by all components, which are
	/// dynamically instanciated and attached to the <see cref="MainWindowController"/> host.
	/// </summary>
	public abstract class ViewControllerComponent<T> : ViewControllerComponent
		where T : ViewControllerComponent<T>
	{
		protected ViewControllerComponent(DataViewOrchestrator orchestrator)
			: base (orchestrator, typeof (T), orchestrator.Host)
		{
		}
	}
}
