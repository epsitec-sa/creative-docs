//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Orchestrators;

namespace Epsitec.Cresus.Core.Factories
{
	/// <summary>
	/// The <c>IViewControllerComponentFactory</c> interface must be implemented by all factories
	/// which provide <see cref="ViewControllerComponent"/> instances for <see cref="DataViewOrchestrator"/>.
	/// </summary>
	public interface IViewControllerComponentFactory : ICoreComponentFactory<DataViewOrchestrator, ViewControllerComponent>
	{
	}
}
