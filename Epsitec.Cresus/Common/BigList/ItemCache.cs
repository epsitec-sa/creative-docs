//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public abstract class ItemCache
	{
		protected static readonly int DefaultExtraCapacity = 1000;
		protected static readonly int DefaultDataCapacity  = 1000;
	}
}
