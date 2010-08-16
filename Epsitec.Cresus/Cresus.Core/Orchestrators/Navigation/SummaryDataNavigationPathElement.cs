//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;

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
			return false;
		}

		public override string ToString()
		{
			return string.Concat ("<SummaryData:", this.name, ">");
		}

		
		private readonly string name;
	}
}