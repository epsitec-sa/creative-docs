//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Widgets.Behaviors
{
	public abstract class SlimFieldBehavior
	{
		protected SlimFieldBehavior(SlimField host)
		{
			this.host = host;
			this.host.UpdatePreferredSize ();
		}

		protected readonly SlimField			host;
	}
}
