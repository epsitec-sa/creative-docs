//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Orchestrators;

namespace Epsitec.Cresus.Core.Factories
{
	/// <summary>
	/// The <c>ViewControllerComponentFactory</c> class provides methods to register and setup
	/// components implementing <see cref="ViewControllerComponent"/>, used as components for
	/// <see cref="DataViewOrchestrator"/>.
	/// </summary>
	public class ViewControllerComponentFactory : CoreComponentFactory<DataViewOrchestrator, IViewControllerComponentFactory, ViewControllerComponent>
	{
	}
}
