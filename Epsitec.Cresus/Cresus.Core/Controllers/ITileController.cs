//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public interface ITileController : IGroupedItem
	{
		/// <summary>
		/// Creates a sub view controller for the item.
		/// </summary>
		/// <param name="orchestrator">The orchestrator.</param>
		/// <returns>The sub view controller.</returns>
		EntityViewController CreateSubViewController(Orchestrators.DataViewOrchestrator orchestrator);
	}
}