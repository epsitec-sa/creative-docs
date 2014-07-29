//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractParams
	{
		public virtual bool StrictlyEquals(AbstractParams other)
		{
			return false;
		}

		public virtual AbstractParams ChangePeriod(int direction)
		{
			return null;
		}
	}
}
