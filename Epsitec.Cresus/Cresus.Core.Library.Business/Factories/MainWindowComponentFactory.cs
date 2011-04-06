//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.Core.Factories
{
	/// <summary>
	/// The <c>CoreDataComponentFactory</c> class provides methods to register and setup
	/// components implementing <see cref="CoreDataComponent"/>, used as components for
	/// <see cref="CoreData"/>.
	/// </summary>
	public class MainWindowComponentFactory : CoreComponentFactory<MainWindowController, IMainWindowComponentFactory, MainWindowComponent>
	{
	}
}
