//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.BigList
{
    internal class IndexedStore<TValue> : Dictionary<int, TValue>
    {
        public IndexedStore(int capacity)
            : base(capacity) { }
    }
}
