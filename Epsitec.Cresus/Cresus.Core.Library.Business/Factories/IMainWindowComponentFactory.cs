//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.Core.Factories
{
	/// <summary>
	/// The <c>IMainWindowComponentFactory</c> interface must be implemented by all factories
	/// which provide <see cref="MainWindowComponent"/> instances for <see cref="MainWindowController"/>.
	/// </summary>
	public interface IMainWindowComponentFactory : ICoreComponentFactory<MainWindowController, MainWindowComponent>
	{
	}
}
