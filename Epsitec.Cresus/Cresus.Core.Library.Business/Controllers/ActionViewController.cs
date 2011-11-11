//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public class ActionViewController : Library.ViewControllerComponent<ActionViewController>
	{
		public ActionViewController(Orchestrators.DataViewOrchestrator orchestrator)
			: base (orchestrator)
		{

		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}
	}
}
