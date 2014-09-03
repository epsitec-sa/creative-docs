//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public abstract class AbstractReportParams
	{
		public virtual bool StrictlyEquals(AbstractReportParams other)
		{
			return false;
		}

		public virtual AbstractReportParams ChangePeriod(int direction)
		{
			return null;
		}
	}
}
