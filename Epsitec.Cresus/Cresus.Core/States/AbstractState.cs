//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.Core.States
{
	public abstract class AbstractState
	{
		protected AbstractState()
		{
		}


		public int ZOrder
		{
			get
			{
				return this.zOrder;
			}
			internal set
			{
				this.zOrder = value;
			}
		}

		private int zOrder;
	}
}
