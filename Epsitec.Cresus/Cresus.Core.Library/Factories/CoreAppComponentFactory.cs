//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.Core.Factories
{
	/// <summary>
	/// The <c>CoreAppComponentFactory</c> class provides methods to register and setup
	/// components implementing <see cref="CoreAppComponent"/>, used as components for
	/// <see cref="CoreApp"/>.
	/// </summary>
	public class CoreAppComponentFactory : CoreComponentFactory<CoreApp, ICoreAppComponentFactory, CoreAppComponent>
	{
	}
}
