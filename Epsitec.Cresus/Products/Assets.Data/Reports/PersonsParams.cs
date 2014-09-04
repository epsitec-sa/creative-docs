//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data.Reports
{
	public class PersonsParams : AbstractReportParams
	{
		public override string					Title
		{
			get
			{
				return "Liste des contacts";
			}
		}

		public override bool					HasParams
		{
			get
			{
				return false;
			}
		}

	}
}
