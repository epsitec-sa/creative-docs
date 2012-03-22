//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList.Processors
{
	public class MouseDownProcessorPolicy : EventProcessorPolicy
	{
		public MouseDownProcessorPolicy()
		{
			this.AutoFollow = true;
		}

		public bool AutoFollow
		{
			get;
			set;
		}
	}
}
