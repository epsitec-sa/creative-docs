//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Orchestrators.Navigation;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.PickerControllers
{
	public class ItemPickerController : CoreViewController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ItemPickerController"/> class.
		/// </summary>
		/// <param name="name">The name of the controller.</param>
		/// <param name="orchestrator">The orchestrator.</param>
		/// <param name="parentController">The parent controller.</param>
		/// <param name="navigationPathElement">The navigation path element.</param>
		public ItemPickerController(string name, DataViewOrchestrator orchestrator, CoreViewController parentController, NavigationPathElement navigationPathElement)
			: base (name, orchestrator, parentController, navigationPathElement)
		{
			
		}

		public override void CreateUI(Widget container)
		{
			base.CreateUI (container);
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}
	}
}
