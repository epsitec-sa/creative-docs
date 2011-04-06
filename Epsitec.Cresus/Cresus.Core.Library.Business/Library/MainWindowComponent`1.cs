//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.Core.Library
{
	/// <summary>
	/// The <c>MainWindowComponent</c> class is the base class used by all components, which are
	/// dynamically instanciated and attached to the <see cref="MainWindowController"/> host.
	/// </summary>
	public abstract class MainWindowComponent<T> : MainWindowComponent
		where T : MainWindowComponent<T>
	{
		protected MainWindowComponent(MainWindowController controller)
			: base (controller, typeof (T), controller.Host)
		{
		}
	}
}
