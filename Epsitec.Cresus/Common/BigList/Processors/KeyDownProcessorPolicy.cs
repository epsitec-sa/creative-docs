//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList.Processors
{
	public class KeyDownProcessorPolicy : EventProcessorPolicy
	{
		public KeyDownProcessorPolicy()
		{
			this.ScrollMode = ScrollMode.MoveVisible;
		}

		
		public ScrollMode						ScrollMode
		{
			get;
			set;
		}
	}
}
