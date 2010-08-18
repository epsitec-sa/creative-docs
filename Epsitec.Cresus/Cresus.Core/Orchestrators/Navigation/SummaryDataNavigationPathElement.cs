//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Controllers;

namespace Epsitec.Cresus.Core.Orchestrators.Navigation
{
	public class SummaryDataNavigationPathElement : NavigationPathElement
	{
		public SummaryDataNavigationPathElement(SummaryData data)
		{
			this.name = data.Name;
		}


		public override bool Navigate(Orchestrators.NavigationOrchestrator navigator)
		{
			var tileContainerController = navigator.GetLeafClickSimulator ();

			System.Diagnostics.Debug.Assert (tileContainerController != null);

			return tileContainerController.SimulateClick (this.name);
		}

		public override string ToString()
		{
			return string.Concat ("<SummaryData:", this.name, ">");
		}

		
		private readonly string name;
	}
}