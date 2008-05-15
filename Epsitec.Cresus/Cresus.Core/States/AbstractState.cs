//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Xml.Linq;

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


		public abstract XElement Serialize(XElement element);

		public abstract AbstractState Deserialize(XElement element);
		
		private int zOrder;
	}
}
